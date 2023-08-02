using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace LibSugar.CodeGen;

[Generator]
public class UnionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var source = context.SyntaxProvider.ForAttributeWithMetadataName("LibSugar.UnionAttribute",
            (node, _) => node is EnumDeclarationSyntax,
            (ctx, _) =>
            {
                var syn = (EnumDeclarationSyntax)ctx.TargetNode;
                var sym = (INamedTypeSymbol)ctx.TargetSymbol;

                var union_attr = ctx.Attributes.First();

                return (syn, sym, union_attr);
            });
        var enums = source
            .Combine(context.CompilationProvider)
            .Select((input, _) =>
            {
                var ((syn, sym, union_attr), compilation) = input;

                var attrs = sym.GetAttributes().AsParallel().AsOrdered()
                    .Select(a => (name: a.AttributeClass?.GetFullMetaName(), a))
                    .ToArray();

                var attr_name = attrs.AsParallel().AsOrdered()
                    .Where(a => a.name == "LibSugar.NameAttribute")
                    .Select(a => a.a)
                    .FirstOrDefault();

                var raw_full_name = sym.ToDisplayString();

                var name = sym.Name;
                if (union_attr is { ConstructorArguments: { Length: > 0 } args } && args.First() is { Value: string v })
                    name = v;
                else if (attr_name is { ConstructorArguments: { Length: > 0 } arg2s } && arg2s.First() is { Value: string v2 })
                    name = v2;
                else if (name.EndsWith("kind", StringComparison.OrdinalIgnoreCase))
                    name = name.Substring(0, name.Length - 4);
                else name = $"{name}Union";

                var attr_for = attrs.AsParallel().AsOrdered()
                    .Where(a => a.name == "LibSugar.ForAttribute")
                    .Select(a => a.a)
                    .FirstOrDefault();

                var generics = ImmutableArray<string>.Empty;
                if (attr_for is { ConstructorArguments.Length: > 0 })
                {
                    generics = attr_for.ConstructorArguments.AsParallel().AsOrdered().FlatAll().Select(a => $"{a.Value}").ToImmutableArray();
                }

                var attr_class = attrs.AsParallel().AsOrdered()
                    .Where(a => a.name == "LibSugar.ClassAttribute")
                    .Select(a => a.a)
                    .FirstOrDefault();
                var is_struct = attr_class == null;

                var accessibility = sym.DeclaredAccessibility;

                var type_generic = generics.Length == 0 ? string.Empty : $"<{string.Join(", ", generics)}>";
                var type_name = generics.Length == 0 ? name : $"{name}{type_generic}";
                var type_full_name = sym.ReplaceName(type_name);

                var partial_part = compilation
                    .GetSymbolsWithName(name, SymbolFilter.Type)
                    .AsParallel().AsOrdered()
                    .WhereCast<ISymbol, INamedTypeSymbol>()
                    .QuerySymbolByNameEq(type_full_name)
                    .FirstOrDefault();

                var generic_symbols = partial_part is { IsGenericType: true, TypeArguments: var tas }
                    ? tas.ToDictionary(a => a.Name)
                    : new();
                
                var usings_base = new HashSet<string> { "using System;", "using System.Numerics;", "using System.Collections.Generic;", "using System.Runtime.InteropServices;", "using LibSugar;" };
                Utils.GetUsings(syn, usings_base);
                var usings = usings_base.ToImmutableHashSet();

                var members = sym.GetMembers().AsParallel().AsOrdered()
                .Where(m => m.Kind is SymbolKind.Field && m.IsStatic)
                .Select(m =>
                {
                    var member_name = m.Name;
                    var m_attrs = m.GetAttributes().AsParallel().AsOrdered()
                        .Select(a => (name: a.AttributeClass?.GetFullMetaName(), a))
                        .ToArray();
                    var attr_m_name = m_attrs.AsParallel()
                        .Where(a => a.name == "LibSugar.NameAttribute")
                        .Select(a => a.a)
                        .FirstOrDefault();
                    var attr_m_of = m_attrs.AsParallel()
                        .Where(a => a.name?.StartsWith("LibSugar.OfAttribute") ?? false)
                        .Select(a => a.a)
                        .FirstOrDefault();

                    if (attr_m_name is { ConstructorArguments: { Length: > 0 } cargs })
                    {
                        var first_arg = cargs.FirstOrDefault();
                        member_name = $"{first_arg.Value}";
                    }

                    ITypeSymbol? member_type_sym = null;
                    string? member_type = null;
                    if (attr_m_of is { AttributeClass: { IsGenericType: true, TypeArguments: var targs } })
                    {
                        member_type_sym = targs.FirstOrDefault();
                        member_type = member_type_sym?.ToDisplayString();
                    }
                    else if (attr_m_of is { ConstructorArguments: { Length: > 0 } cargs1 })
                    {
                        var first_arg = cargs1.FirstOrDefault();
                        if (first_arg.Kind is TypedConstantKind.Type)
                        {
                            member_type_sym = (ITypeSymbol)first_arg.Value!;
                            member_type = member_type_sym?.ToDisplayString();
                        }
                        else if (first_arg.Kind is TypedConstantKind.Primitive)
                        {
                            member_type = $"{first_arg.Value}";
                            generic_symbols.TryGetValue(member_type, out member_type_sym);
                        }
                    }

                    var is_unmanaged = member_type_sym?.IsUnmanagedType ?? false;

                    var is_reference = member_type_sym?.IsReferenceType ?? false;

                    var raw_name = m.Name;

                    return (raw_name, member_name, member_type, is_unmanaged, is_reference);
                })
                .ToImmutableArray();

                return (sym, raw_full_name, name, generics, is_struct, accessibility, type_name, usings, members);
            });

        context.RegisterSourceOutput(enums, (ctx, input) =>
        {
            var (sym, raw_full_name, name, generics, is_struct, accessibility, type_name, usings, members) = input;

            var member_readonly = is_struct ? " readonly" : string.Empty;

            var union_is = new string[members.Length];
            var union_get = new string[members.Length];
            var union_try_get = new string[members.Length];
            var union_make = new string[members.Length];

            GenCommon();

            var gened = is_struct ? GenStruct() : GenClass();

            var source_text = SourceText.From($@"// <auto-generated/>

#nullable enable

{string.Join("\n", usings.OrderBy(u => u.Length))}

{sym.WrapNameSpace(sym.WrapNestedType(gened))}
", Encoding.UTF8);
            var source_file_name = $"{raw_full_name.Replace('<', '[').Replace('>', ']')}.union.gen.cs";
            ctx.AddSource(source_file_name, source_text);

            //////////////////////////////////////////////////

            void GenCommon()
            {
                members.AsParallel().AsOrdered().Indexed().ForAll(m =>
                {
                    var ((raw_name, member_name, _, _, _), i) = m;
                    union_is[i] = $"public{member_readonly} bool Is{member_name} => Kind is {raw_full_name}.{raw_name};";
                });
            }

            //////////////////////////////////////////////////

            string GenClass()
            {
                var union_tag_class = new string[members.Length];

                members.AsParallel().AsOrdered().Indexed().ForAll(m =>
                {
                    var ((member_raw_name, member_name, member_type, is_unmanaged, is_reference), i) = m;

                    if (member_type != null)
                    {
                        union_get[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{member_raw_name}""/>
    public virtual {member_type} Get{member_name} => default!;";
                        union_try_get[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{member_raw_name}""/>
    public virtual {member_type}? TryGet{member_name} => default;";
                    }

                    var member_type_value = member_type == null ? string.Empty : $@"
        public {member_type} Value {{ get; }}

        public override {member_type} Get{member_name} => Value;

        public override {member_type}? TryGet{member_name} => Value;
";
                    var ctor_arg = member_type == null ? string.Empty : $"{member_type} value";
                    var ctor_body = member_type == null ? " " : $"this.Value = value; ";
                    var ctor_value = member_type == null ? string.Empty : $"value";

                    var i_box = member_type == null ? string.Empty : $", IBox<{member_type}>";

                    var eq = member_type == null ? string.Empty : $" && EqualityComparer<{member_type}>.Default.Equals(this.Value, other.Value)";
                    var hash = member_type == null ? string.Empty : $", this.Value";
                    var to_str = member_type == null ? string.Empty : $" {{{{ {{this.Value}} }}}}";

                    union_tag_class[i] = $@"
    /// <inheritdoc cref=""{raw_full_name}.{member_raw_name}""/>
    public partial class {member_name} : {type_name}, IEquatable<{member_name}>{i_box}
    {{
        public {member_name}({ctor_arg}) {{{ctor_body}}}

        public override {raw_full_name} Kind => {raw_full_name}.{member_raw_name};
        {member_type_value}
        public bool Equals({member_name}? other) => other != null{eq};

        public override bool Equals({type_name}? obj) => obj is {member_name} other && this.Equals(other);

        public override bool Equals(object? obj) => obj is {member_name} other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(this.Kind{hash});

        public override string ToString() => $""{name}.{member_name}{to_str}"";
    }}";

                    union_make[i] = $"public static {member_name} Make{member_name}({ctor_arg}) => new({ctor_value});";
                });

                return $@"
/// <inheritdoc cref=""{raw_full_name}""/>
[Union]
{accessibility.GetAccessStr()} abstract partial class {type_name} : 
    IEquatable<{type_name}>
#if NET7_0_OR_GREATER
    , IEqualityOperators<{type_name}, {type_name}, bool>
#endif
{{

    public abstract {raw_full_name} Kind {{ get; }}

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

                var unmanaged_count = members.AsParallel().Select(a => a.is_unmanaged ? 1 : 0).Sum();
                var reference_count = members.AsParallel().Select(a => a.is_reference ? 1 : 0).Sum();

                members.AsParallel().AsOrdered().Indexed().ForAll(m =>
                {
                    var ((raw_name, member_name, member_type, is_unmanaged, is_reference), i) = m;

                    if (member_type != null)
                    {
                        if (is_unmanaged && unmanaged_count > 1)
                        {
                            union_get[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{raw_name}""/>
    public readonly {member_type} {member_name} => _union._{member_name};";
                            union_make[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{raw_name}""/>
    public static {type_name} Make{member_name}({member_type} v) => new() {{ Kind = {raw_full_name}.{raw_name}, _union = new() {{ _{member_name} = v }} }};";
                            union_fields[i] = $@"[FieldOffset(0)]
        public {member_type} _{member_name};";
                        }
                        else if (is_reference && reference_count > 1)
                        {
                            union_get[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{raw_name}""/>
    public readonly {member_type} {member_name} => ({member_type})_ref_type;";
                            union_make[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{raw_name}""/>
    public static {type_name} Make{member_name}({member_type} v) => new() {{ Kind = {raw_full_name}.{raw_name}, _ref_type = v }};";
                        }
                        else
                        {
                            union_get[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{raw_name}""/>
    public readonly {member_type} {member_name} {{ get; init; }}";
                            union_make[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{raw_name}""/>
    public static {type_name} Make{member_name}({member_type} v) => new() {{ Kind = {raw_full_name}.{raw_name}, {member_name} = v }};";
                        }

                        union_try_get[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{raw_name}""/>
    public readonly {member_type}? TryGet{member_name} => Is{member_name} ? this.{member_name} : default;";
                        eqs[i] = $"{raw_full_name}.{raw_name} => EqualityComparer<{member_type}>.Default.Equals(this.{member_name}, other.{member_name}),";
                        hashs[i] = $"{raw_full_name}.{raw_name} => HashCode.Combine(this.Kind, this.{member_name}),";
                        to_strs[i] = $@"{raw_full_name}.{raw_name} => $""{name}.{member_name} {{{{ {{this.{member_name}}} }}}}"",";
                    }
                    else
                    {
                        union_make[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{raw_name}""/>
    public static {type_name} Make{member_name}() => new() {{ Kind = {raw_full_name}.{raw_name} }};";
                        to_strs[i] = $@"{raw_full_name}.{raw_name} => $""{name}.{member_name}"",";
                    }
                });

                var union_struct = unmanaged_count < 2 ? string.Empty : $@"
    [StructLayout(LayoutKind.Explicit)]
    private struct _union_
    {{
        {string.Join("\n        ", union_fields.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
    }}
";
                var union_field = unmanaged_count < 2 ? string.Empty : $@"
    private readonly _union_ _union {{ get; init; }}
";
                var ref_type_field = reference_count < 2 ? string.Empty : $@"private readonly object _ref_type {{ get; init; }}
";

                return $@"
/// <inheritdoc cref=""{raw_full_name}""/>
[Union]
{accessibility.GetAccessStr()} readonly partial struct {type_name} : 
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
    public readonly {raw_full_name} Kind {{ get; init; }}
    {union_struct}
    {string.Join("\n    ", union_make)}

    public readonly bool Equals({type_name} other) => this.Kind != other.Kind ? false : this.Kind switch
    {{
        {string.Join("\n        ", eqs.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
        _ => true,
    }};

    public readonly override bool Equals(object? obj) => obj is {type_name} other && Equals(other);

    public readonly override int GetHashCode() => this.Kind switch
    {{
        {string.Join("\n        ", hashs.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
        _ => HashCode.Combine(this.Kind),
    }};

    public static bool operator ==({type_name} left, {type_name} right) => Equals(left, right);

    public static bool operator !=({type_name} left, {type_name} right) => Equals(left, right);

    public readonly override string ToString() => this.Kind switch
    {{
        {string.Join("\n        ", to_strs.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
        _ => ""{type_name}"",
    }};
}}
";
            }

        });
    }
}
