using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace LibSugar.CodeGen;

[Generator]
public class UnionGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new UnionReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            Execute1(context);
        }
        catch (Exception e)
        {
            context.LogError(e.ToString().Replace("\n", "    \\n    "), Location.None);
        }
    }

    public void Execute1(GeneratorExecutionContext context)
    {
        var receiver = (UnionReceiver)context.SyntaxReceiver!;

        var enums = CollectDeclSymbol(receiver.Enums, context);

        foreach (var enum_kv in enums)
        {
            var enum_full_name = enum_kv.Key;
            var (enum_decl, _, enum_symbol) = enum_kv.Value;
            var enum_name = enum_symbol.Name;

            var name = enum_decl.Identifier.Text;
            var attr_union = enum_symbol.GetAttributes().AsParallel().AsOrdered()
                .QueryAttr("LibSugar.UnionAttribute")
                .FirstOrDefault();

            if (attr_union is { ConstructorArguments.Length: > 0 })
            {
                var first_arg = attr_union.ConstructorArguments.FirstOrDefault();
                name = $"{first_arg.Value}";
            }
            else if (name.ToLower().EndsWith("kind")) name = name.Substring(0, name.Length - 4);
            else name = $"{name}Union";

            var attr_for = enum_symbol.GetAttributes().AsParallel().AsOrdered()
                .QueryAttr("LibSugar.ForAttribute")
                .FirstOrDefault();

            var generics = Array.Empty<string>();
            if (attr_for is { ConstructorArguments.Length: > 0 })
            {
                generics = attr_for.ConstructorArguments.AsParallel().AsOrdered().FlatAll().Select(a => $"{a.Value}").ToArray();
            }

            var attr_class = enum_symbol.GetAttributes().AsParallel().AsOrdered()
                .QueryAttr("LibSugar.ClassAttribute")
                .FirstOrDefault();

            var modifiers = enum_decl.Modifiers.ToString();
            var usings = new HashSet<string> { "using System;", "using System.Numerics;", "using System.Collections.Generic;", "using System.Runtime.InteropServices;", "using LibSugar;" };
            Utils.GetUsings(enum_decl, usings);

            var type_generic = generics.Length == 0 ? string.Empty : $"<{string.Join(", ", generics)}>";
            var type_name = $"{name}{type_generic}";
            var type_full_name = enum_symbol.ReplaceName(type_name);

            var partial_part = context.Compilation
                .GetSymbolsWithName(name, SymbolFilter.Type)
                .WhereCast<ISymbol, INamedTypeSymbol>()
                .QuerySymbolByNameEq(type_full_name)
                .FirstOrDefault();

            var generic_symbols = partial_part is { IsGenericType: true, TypeArguments: var tas } ? tas.ToDictionary(a => a.Name) : new();

            var syntax_members = enum_decl.Members.AsParallel()
                .ToDictionary(m => m.Identifier.Text);

            var members = enum_symbol.GetMembers().AsParallel().AsOrdered()
                .Where(m => m.Kind is SymbolKind.Field && m.IsStatic)
                .Select(m =>
                {
                    var member_name = m.Name;
                    syntax_members.TryGetValue(m.Name, out var syntax);
                    var attrs = m.GetAttributes();
                    var attr_name = attrs.AsParallel()
                        .QueryAttr("LibSugar.NameAttribute")
                        .FirstOrDefault();
                    var attr_of = attrs.AsParallel()
                        .QueryAttr("LibSugar.OfAttribute")
                        .FirstOrDefault();

                    if (attr_name is { ConstructorArguments.Length: > 0 })
                    {
                        var first_arg = attr_name.ConstructorArguments.FirstOrDefault();
                        member_name = $"{first_arg.Value}";
                    }

                    ITypeSymbol? member_type = null;
                    string? member_type_name = null;
                    if (attr_of is { AttributeClass: not null })
                    {
                        var attr_class = attr_of.AttributeClass;
                        if (attr_class.IsGenericType)
                        {
                            member_type = attr_class.TypeArguments.FirstOrDefault();
                        }
                        else
                        {
                            var first_arg = attr_of.ConstructorArguments.FirstOrDefault();
                            if (first_arg.Kind is TypedConstantKind.Type)
                            {
                                member_type = (ITypeSymbol)first_arg.Value!;
                            }
                            else if (first_arg.Kind is TypedConstantKind.Primitive)
                            {
                                member_type_name = $"{first_arg.Value}";
                                generic_symbols.TryGetValue(member_type_name, out member_type);
                            }
                        }
                    }

                    return (m, syntax, member_name, member_type, member_type_name);
                })
                .ToArray();

            var union_is = new string[members.Length];
            var union_get = new string[members.Length];
            var union_try_get = new string[members.Length];
            var union_make = new string[members.Length];

            var isStruct = attr_class == null;
            var member_readonly = isStruct ? " readonly" : string.Empty;

            GenCommon();

            var gened = isStruct ? GenStruct() : GenClass();

            var source_text = SourceText.From($@"// <auto-generated/>

#nullable enable

{string.Join("\n", usings.OrderBy(u => u.Length))}

{enum_symbol.WrapNameSpace(enum_symbol.WrapNestedType(gened))}
", Encoding.UTF8);
            var source_file_name = $"{enum_full_name.Replace('<', '[').Replace('>', ']')}.union.gen.cs";
            context.AddSource(source_file_name, source_text);

            //////////////////////////////////////////////////

            void GenCommon()
            {
                members.AsParallel().AsOrdered().Indexed().ForAll(m =>
                {
                    var ((member, _, member_name, _, _), i) = m;
                    union_is[i] = $"public{member_readonly} bool Is{member_name} => Kind is {enum_name}.{member.Name};";
                });
            }

            //////////////////////////////////////////////////

            string GenClass()
            {
                var union_tag_class = new string[members.Length];

                members.AsParallel().AsOrdered().Indexed().ForAll(m =>
                {
                    var ((member, _, member_name, member_type, member_type_name), i) = m;
                    var member_type_str = member_type?.ToDisplayString() ?? member_type_name;

                    if (member_type_str != null)
                    {
                        union_get[i] = $"public virtual {member_type_str} Get{member_name} => default!;";
                        union_try_get[i] = $"public virtual {member_type_str}? TryGet{member_name} => default;";
                    }

                    var member_type_value = member_type_str == null ? string.Empty : $@"
        public {member_type_str} Value {{ get; }}

        public override {member_type_str} Get{member_name} => Value;

        public override {member_type_str}? TryGet{member_name} => Value;
";
                    var ctor_arg = member_type_str == null ? string.Empty : $"{member_type_str} value";
                    var ctor_body = member_type_str == null ? " " : $" Value = value; ";
                    var ctor_value = member_type_str == null ? string.Empty : $"value";

                    var i_box = member_type_str == null ? string.Empty : $", IBox<{member_type_str}>";

                    var eq = member_type_str == null ? string.Empty : $" && EqualityComparer<{member_type_str}>.Default.Equals(Value, other.Value)";
                    var hash = member_type_str == null ? string.Empty : $", Value";
                    var to_str = member_type_str == null ? string.Empty : $" {{{{ {{Value}} }}}}";

                    union_tag_class[i] = $@"
    public partial class {member_name} : {type_name}, IEquatable<{member_name}>{i_box}
    {{
        public {member_name}({ctor_arg}) {{{ctor_body}}}

        public override {enum_name} Kind => {enum_name}.{member.Name};
        {member_type_value}
        public bool Equals({member_name}? other) => other != null{eq};

        public override bool Equals({type_name}? obj) => obj is {member_name} other && Equals(other);

        public override bool Equals(object? obj) => obj is {member_name} other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Kind{hash});

        public override string ToString() => $""{name}.{member_name}{to_str}"";
    }}";

                    union_make[i] = $"public static {member_name} Make{member_name}({ctor_arg}) => new({ctor_value});";
                });

                return $@"
[Union]
{modifiers} abstract partial class {type_name} : 
    IEquatable<{type_name}>
#if NET7_0_OR_GREATER
    , IEqualityOperators<{type_name}, {type_name}, bool>
#endif
{{

    public abstract {enum_name} Kind {{ get; }}

    {string.Join("\n    ", union_get.AsParallel().AsOrdered().Where(a => a != null).ToArray())}

    {string.Join("\n    ", union_try_get.AsParallel().AsOrdered().Where(a => a != null).ToArray())}

    {string.Join("\n    ", union_is)}

    {string.Join("\n    ", union_make)}
    
    public abstract bool Equals({type_name}? other);

    public abstract override bool Equals(object? obj);

    public abstract override int GetHashCode();

    public static bool operator ==({type_name}? left, {type_name}? right) => Equals(left, right);

    public static bool operator !=({type_name}? left, {type_name}? right) => Equals(left, right);

    public abstract override string ToString();
    {string.Join("\n    ", union_tag_class)}
}}
";
            }

            //////////////////////////////////////////////////

            string GenStruct()
            {
                var eqs = new string[members.Length];
                var hashs = new string[members.Length];
                var to_strs = new string[members.Length];

                var union_fields = new string[members.Length];

                var unmanaged_members = members.AsParallel().AsOrdered().Indexed().Where(m =>
                {
                    var ((_, _, _, member_type, _), _) = m;
                    return member_type?.IsUnmanagedType ?? false;
                }).Select(m => m.i).ToImmutableHashSet();

                var ref_type_members = members.AsParallel().AsOrdered().Indexed().Where(m =>
                {
                    var ((_, _, _, member_type, _), _) = m;
                    return member_type?.IsReferenceType ?? false;
                }).Select(m => m.i).ToImmutableHashSet();

                members.AsParallel().AsOrdered().Indexed().ForAll(m =>
                {
                    var ((member, _, member_name, member_type, member_type_name), i) = m;
                    var member_type_str = member_type?.ToDisplayString() ?? member_type_name;

                    if (member_type_str != null)
                    {
                        var isUnmanaged = unmanaged_members.Count > 1 && unmanaged_members.TryGetValue(i, out _);
                        var isRefType = ref_type_members.Count > 1 && ref_type_members.TryGetValue(i, out _);

                        if (isUnmanaged)
                        {
                            union_get[i] = $"public readonly {member_type_str} {member_name} => _union._{member_name};";
                            union_make[i] = $"public static {type_name} Make{member_name}({member_type_str} v) => new() {{ Kind = {enum_name}.{member.Name}, _union = new() {{ _{member_name} = v }} }};";
                            union_fields[i] = $"[FieldOffset(0)]\n        public {member_type_str} _{member_name};";
                        }
                        else if (isRefType)
                        {
                            union_get[i] = $"public readonly {member_type_str} {member_name} => ({member_type_str})_ref_type;";
                            union_make[i] = $"public static {type_name} Make{member_name}({member_type_str} v) => new() {{ Kind = {enum_name}.{member.Name}, _ref_type = v }};";
                        }
                        else
                        {
                            union_get[i] = $"public readonly {member_type_str} {member_name} {{ get; init; }}";
                            union_make[i] = $"public static {type_name} Make{member_name}({member_type_str} v) => new() {{ Kind = {enum_name}.{member.Name}, {member_name} = v }};";
                        }

                        union_try_get[i] = $"public readonly {member_type_str}? TryGet{member_name} => Is{member_name} ? {member_name} : default;";
                        eqs[i] = $"{enum_name}.{member.Name} => EqualityComparer<{member_type_str}>.Default.Equals({member_name}, other.{member_name}),";
                        hashs[i] = $"{enum_name}.{member.Name} => HashCode.Combine(Kind, {member_name}),";
                        to_strs[i] = $@"{enum_name}.{member.Name} => $""{name}.{member_name} {{{{ {{{member_name}}} }}}}"",";
                    }
                    else
                    {
                        union_make[i] = $"public static {type_name} Make{member_name}() => new() {{ Kind = {enum_name}.{member.Name} }};";
                        to_strs[i] = $@"{enum_name}.{member.Name} => $""{name}.{member_name}"",";
                    }
                });

                var union_struct = unmanaged_members.Count < 2 ? string.Empty : $@"
    [StructLayout(LayoutKind.Explicit)]
    private struct _union_
    {{
        {string.Join("\n        ", union_fields.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
    }}
";
                var union_field = unmanaged_members.Count < 2 ? string.Empty : $@"
    private readonly _union_ _union {{ get; init; }}
";
                var ref_type_field = ref_type_members.Count < 2 ? string.Empty : $@"private readonly object _ref_type {{ get; init; }}
";

                return $@"
[Union]
{modifiers} readonly partial struct {type_name} : 
    IEquatable<{type_name}>
#if NET7_0_OR_GREATER
    , IEqualityOperators<{type_name}, {type_name}, bool>
#endif
{{
    {string.Join("\n    ", union_get.AsParallel().AsOrdered().Where(a => a != null).ToArray())}

    {string.Join("\n    ", union_try_get.AsParallel().AsOrdered().Where(a => a != null).ToArray())}

    {string.Join("\n    ", union_is)}
    {union_field}
    {ref_type_field}
    public readonly {enum_name} Kind {{ get; init; }}
    {union_struct}
    {string.Join("\n    ", union_make)}

    public readonly bool Equals({type_name} other) => Kind != other.Kind ? false : Kind switch
    {{
        {string.Join("\n        ", eqs.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
        _ => true,
    }};

    public readonly override bool Equals(object? obj) => obj is {type_name} other && Equals(other);

    public readonly override int GetHashCode() => Kind switch
    {{
        {string.Join("\n        ", hashs.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
        _ => HashCode.Combine(Kind),
    }};

    public static bool operator ==({type_name} left, {type_name} right) => Equals(left, right);

    public static bool operator !=({type_name} left, {type_name} right) => Equals(left, right);

    public readonly override string ToString() => Kind switch
    {{
        {string.Join("\n        ", to_strs.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
        _ => ""{type_name}"",
    }};
}}
";
            }
        }

    }

    static Dictionary<string, (T, AttributeSyntax, INamedTypeSymbol)> CollectDeclSymbol<T>(List<(T, AttributeSyntax)> items, GeneratorExecutionContext context) where T : BaseTypeDeclarationSyntax
    {
        return items
            .AsParallel()
            .Select(sa =>
            {
                var (@enum, enum_attr) = sa;
                var name = Utils.GetFullName(@enum);
                var symbol = context.Compilation.GetSymbolsWithName(@enum.Identifier.Text, SymbolFilter.Type)
                    .AsParallel()
                    .Select(static a => (a, name: a.ToDisplayString()))
                    .Where(a => a.name == name)
                    .Select(static a => (INamedTypeSymbol)a.a)
                    .FirstOrDefault();
                if (symbol != null) return new { name, @enum, enum_attr, symbol };
                else
                {
                    context.LogError($"Cannot find symbol {name}, please submit an issues", @enum.Identifier.GetLocation());
                    return null;
                }
            })
            .Where(static a => a != null)
            .ToDictionary(static a => a!.name, static a => (a!.@enum, a.enum_attr, a.symbol));
    }

    class UnionReceiver : ISyntaxReceiver
    {
        public readonly List<(EnumDeclarationSyntax, AttributeSyntax)> Enums = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is EnumDeclarationSyntax eds)
            {
                foreach (var attributeList in eds.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        if (attribute.Name.CheckAttrName("Union") || attribute.Name.CheckAttrName("UnionAttribute"))
                        {
                            Enums.Add((eds, attribute));
                        }
                    }
                }
            }
        }
    }
}
