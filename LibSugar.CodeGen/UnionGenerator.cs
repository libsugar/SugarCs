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

                var has_generic = generics.Length > 0;
                var type_generic = has_generic ? $"<{string.Join(", ", generics)}>" : string.Empty;
                var type_name = has_generic ? $"{name}{type_generic}" : name;
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

                var generic_parameters = partial_part is { IsGenericType: true, TypeParameters: var tps }
                    ? tps
                    : ImmutableArray<ITypeParameterSymbol>.Empty;

                var constraints = string.Join(" ", generic_parameters.AsParallel().AsOrdered().GetConstraint().ToArray());

                var usings_base = new HashSet<string> { "using System;", "using System.Numerics;", "using System.Collections.Generic;", "using System.Runtime.InteropServices;", "using LibSugar;" };
                Utils.GetUsings(syn, usings_base);

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
                    var attr_m_json_name = m_attrs.AsParallel()
                        .Where(a => a.name == "LibSugar.UnionJsonNameAttribute")
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

                    string json_name = member_name;
                    if (attr_m_json_name is { ConstructorArguments: { Length: > 0 } cargs_json })
                    {
                        var first_arg = cargs_json.FirstOrDefault();
                        json_name = $"{first_arg.Value}";
                    }

                    return (raw_name, member_name, member_type, is_unmanaged, is_reference, json_name);
                })
                .ToImmutableArray();

                var attr_json = attrs.AsParallel().AsOrdered()
                    .Where(a => a.name == "LibSugar.UnionJsonAttribute")
                    .Select(a => a.a)
                    .FirstOrDefault();
                var json = GetJsonInfo(attr_json);

                if (json?.WithSystemText ?? false)
                {
                    usings_base.Add("using System.Text.Json;");
                    usings_base.Add("using System.Text.Json.Serialization;");
                }

                var usings = usings_base.ToImmutableHashSet();

                return (sym, raw_full_name, name, generics, is_struct, accessibility, has_generic, type_generic, constraints, type_name, usings, members, json);
            });

        context.RegisterSourceOutput(enums, (ctx, input) =>
        {
            var (sym, raw_full_name, name, generics, is_struct, accessibility, has_generic, type_generic, constraints, type_name, usings, members, json) = input;

            var member_readonly = is_struct ? " readonly" : string.Empty;

            var union_is = new string[members.Length];
            var union_get = new string[members.Length];
            var union_try_get = new string[members.Length];
            var union_make = new string[members.Length];

            var gen_system_text_json = json?.WithSystemText ?? false;
            var gen_json = gen_system_text_json;
            var json_system_text_name = json?.SystemTextClassName ?? (gen_json ? $"{name}JsonConverter" : string.Empty);
            var json_attr = gen_json && !has_generic ? $", JsonConverter(typeof({json_system_text_name}))" : string.Empty;

            //////////////////////////////////////////////////

            GenCommon();

            var gened = is_struct ? GenStruct() : GenClass();

            var source_text = SourceText.From($@"// <auto-generated/>

#nullable enable

{string.Join("\n", usings.OrderBy(u => u.Length))}

{sym.WrapNameSpace(sym.WrapNestedType(gened))}
", Encoding.UTF8);
            var raw_source_file_name = $"{raw_full_name.Replace('<', '[').Replace('>', ']')}";
            var source_file_name = $"{raw_source_file_name}.union.gen.cs";
            ctx.AddSource(source_file_name, source_text);

            //////////////////////////////////////////////////

            if (json.HasValue && gen_json && gen_system_text_json)
            {
                var json_gened = GenSystemTextJson(json.Value);
                var json_source_text = SourceText.From($@"// <auto-generated/>

#nullable enable

{string.Join("\n", usings.OrderBy(u => u.Length))}

{sym.WrapNameSpace(sym.WrapNestedType(json_gened))}
", Encoding.UTF8);
                var json_source_file_name = $"{raw_source_file_name}.union.json.gen.cs";
                ctx.AddSource(json_source_file_name, json_source_text);
            }

            //////////////////////////////////////////////////

            void GenCommon()
            {
                members.AsParallel().AsOrdered().Indexed().ForAll(m =>
                {
                    var ((raw_name, member_name, _, _, _, _), i) = m;
                    union_is[i] = $"public{member_readonly} bool Is{member_name} => Kind is {raw_full_name}.{raw_name};";
                });
            }

            //////////////////////////////////////////////////

            string GenClass()
            {
                var union_tag_class = new string[members.Length];

                var meta_name_to_kind = new string[members.Length];
                var meta_kind_to_name = new string[members.Length];
                var meta_variant_types = new string[members.Length];
                var meta_json_name_to_kind = new string[members.Length];

                members.AsParallel().AsOrdered().Indexed().ForAll(m =>
                {
                    var ((member_raw_name, member_name, member_type, is_unmanaged, is_reference, json_name), i) = m;

                    if (member_type != null)
                    {
                        union_get[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{member_raw_name}""/>
    public virtual {member_type} Get{member_name} => default!;";
                        union_try_get[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{member_raw_name}""/>
    public virtual {member_type}? TryGet{member_name} => default;";

                        meta_variant_types[i] = $@"{{ {raw_full_name}.{member_raw_name}, typeof({member_type})  }}";
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

                    meta_name_to_kind[i] = $@"{{ ""{member_name}"" , {raw_full_name}.{member_raw_name} }}";
                    meta_kind_to_name[i] = $@"{{ {raw_full_name}.{member_raw_name}, ""{member_name}""  }}";
                    meta_json_name_to_kind[i] = $@"{{ ""{json_name}"" , {raw_full_name}.{member_raw_name} }}, {{ $""{{{raw_full_name}.{member_raw_name}:D}}"" , {raw_full_name}.{member_raw_name} }}";

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
[Union{json_attr}]
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

    public static UnionMeta<{raw_full_name}> UnionMeta {{ get; }} = new(
        new() {{ {string.Join(", ", meta_name_to_kind)} }},
        new() {{ {string.Join(", ", meta_kind_to_name)} }},
        new() {{ {string.Join(", ", meta_variant_types.AsParallel().AsOrdered().Where(a => a != null).ToArray())} }},
        new() {{ {string.Join(", ", meta_json_name_to_kind)} }}
    );
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

                var meta_name_to_kind = new string[members.Length];
                var meta_kind_to_name = new string[members.Length];
                var meta_variant_types = new string[members.Length];
                var meta_json_name_to_kind = new string[members.Length];

                members.AsParallel().AsOrdered().Indexed().ForAll(m =>
                {
                    var ((raw_name, member_name, member_type, is_unmanaged, is_reference, json_name), i) = m;

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

                        meta_variant_types[i] = $@"{{ {raw_full_name}.{raw_name}, typeof({member_type})  }}";
                    }
                    else
                    {
                        union_make[i] = $@"/// <inheritdoc cref=""{raw_full_name}.{raw_name}""/>
    public static {type_name} Make{member_name}() => new() {{ Kind = {raw_full_name}.{raw_name} }};";
                        to_strs[i] = $@"{raw_full_name}.{raw_name} => $""{name}.{member_name}"",";
                    }

                    meta_name_to_kind[i] = $@"{{ ""{member_name}"" , {raw_full_name}.{raw_name} }}";
                    meta_kind_to_name[i] = $@"{{ {raw_full_name}.{raw_name}, ""{member_name}""  }}";
                    meta_json_name_to_kind[i] = $@"{{ ""{json_name}"" , {raw_full_name}.{raw_name} }}, {{ $""{{{raw_full_name}.{raw_name}:D}}"" , {raw_full_name}.{raw_name} }}";
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
[Union{json_attr}]
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
        _ => ""{name}"",
    }};

    public static UnionMeta<{raw_full_name}> UnionMeta {{ get; }} = new(
        new() {{ {string.Join(", ", meta_name_to_kind)} }},
        new() {{ {string.Join(", ", meta_kind_to_name)} }},
        new() {{ {string.Join(", ", meta_variant_types.AsParallel().AsOrdered().Where(a => a != null).ToArray())} }},
        new() {{ {string.Join(", ", meta_json_name_to_kind)} }}
    );
}}
";
            }

            //////////////////////////////////////////////////

            string GenSystemTextJson(JsonInfo info)
            {
                var can_convert_types = is_struct ? Array.Empty<string>() : new string[members.Length];
                var reads = new string[members.Length];
                var writes = new string[members.Length];

                var t = info.Tag;
                var c = info.Content;

                var get = is_struct ? string.Empty : "Get";

                members.AsParallel().AsOrdered().Indexed().ForAll(mi =>
                {
                    var ((raw_name, member_name, member_type, is_unmanaged, is_reference, json_name), i) = mi;

                    var tag = info.NumberTag ? $@"$""{{{raw_full_name}.{raw_name}:D}}""" : $@"""{json_name}""";

                    if (!is_struct)
                    {
                        can_convert_types[i] = $"typeof({type_name}.{member_name})";
                    }

                    switch (info.JsonMode)
                    {
                        case UnionJsonMode.Tuple:
                            {
                                if (member_type == null)
                                {
                                    reads[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                result = {type_name}.Make{member_name}();
                break;
            }}";
                                    writes[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                writer.WriteStringValue({tag});
                writer.WriteNullValue();
                break;
            }}";
                                }
                                else
                                {
                                    reads[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                var type = typeof({member_type});
                var conv = (JsonConverter<{member_type}>)options.GetConverter(type);
                var v = conv.Read(ref reader, type, options);
                result = {type_name}.Make{member_name}(v!);
                break;
            }}";
                                    writes[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                writer.WriteStringValue({tag});
                var conv = (JsonConverter<{member_type}>)options.GetConverter(typeof({member_type}));
                conv.Write(writer, value.{get}{member_name}, options);
                break;
            }}";
                                }
                                break;
                            }
                        case UnionJsonMode.Adjacent:
                            {
                                if (member_type == null)
                                {
                                    reads[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                result = {type_name}.Make{member_name}();
                break;
            }}";
                                    writes[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                writer.WriteString(""{t}"", {tag});
                writer.WriteNull(""{c}"");
                break;
            }}";
                                }
                                else
                                {
                                    reads[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                var type = typeof({member_type});
                var conv = (JsonConverter<{member_type}>)options.GetConverter(type);
                var v = conv.Read(ref reader, type, options);
                result = {type_name}.Make{member_name}(v!);
                break;
            }}";
                                    writes[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                writer.WriteString(""{t}"", {tag});
                writer.WritePropertyName(""{c}"");
                var conv = (JsonConverter<{member_type}>)options.GetConverter(typeof({member_type}));
                conv.Write(writer, value.{get}{member_name}, options);
                break;
            }}";
                                }
                                break;
                            }
                        default:
                            {
                                if (member_type == null)
                                {
                                    reads[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                result = {type_name}.Make{member_name}();
                break;
            }}";
                                    writes[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                writer.WriteNull({tag});
                break;
            }}";
                                }
                                else
                                {
                                    reads[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                var type = typeof({member_type});
                var conv = (JsonConverter<{member_type}>)options.GetConverter(type);
                var v = conv.Read(ref reader, type, options);
                result = {type_name}.Make{member_name}(v!);
                break;
            }}";
                                    writes[i] = $@"
            case {raw_full_name}.{raw_name}: 
            {{
                writer.WritePropertyName({tag});
                var conv = (JsonConverter<{member_type}>)options.GetConverter(typeof({member_type}));
                conv.Write(writer, value.{get}{member_name}, options);
                break;
            }}";
                                }
                                break;
                            }
                    }
                });

                string read;
                string write;
                switch (info.JsonMode)
                {
                    case UnionJsonMode.Tuple:
                        {
                            read = $@"if (reader.TokenType is not JsonTokenType.StartArray) throw new JsonException();
        reader.Read();
        var meta = {type_name}.UnionMeta;
        if (!meta.JsonNameToKind.TryGetValue(reader.GetString()!, out var kind)) throw new JsonException();
        reader.Read();
        {type_name} result;
        switch (kind)
        {{{string.Join("", reads.Where(a => !string.IsNullOrEmpty(a)))}
            default: throw new ArgumentOutOfRangeException();
        }}
        reader.Read();
        if (reader.TokenType is not JsonTokenType.EndArray) throw new JsonException();
        return result;
        ";
                            write = $@"writer.WriteStartArray();
        switch (value.Kind)
        {{{string.Join("", writes.Where(a => !string.IsNullOrEmpty(a)))}
            default: throw new ArgumentOutOfRangeException();
        }}
        writer.WriteEndArray();";
                            break;
                        }
                    case UnionJsonMode.Adjacent:
                        {
                            read = $@"if (reader.TokenType is not JsonTokenType.StartObject) throw new JsonException();
        reader.Read();
        if (reader.TokenType is not JsonTokenType.PropertyName) throw new JsonException();
        if (reader.GetString() != ""{t}"") throw new JsonException();
        reader.Read();
        var meta = {type_name}.UnionMeta;
        if (!meta.JsonNameToKind.TryGetValue(reader.GetString()!, out var kind)) throw new JsonException();
        reader.Read();
        if (reader.TokenType is not JsonTokenType.PropertyName) throw new JsonException();
        if (reader.GetString() != ""{c}"") throw new JsonException();
        reader.Read();
        {type_name} result;
        switch (kind)
        {{{string.Join("", reads.Where(a => !string.IsNullOrEmpty(a)))}
            default: throw new ArgumentOutOfRangeException();
        }}
        reader.Read();
        if (reader.TokenType is not JsonTokenType.EndObject) throw new JsonException();
        return result;
        ";
                            write = $@"writer.WriteStartObject();
        switch (value.Kind)
        {{{string.Join("", writes.Where(a => !string.IsNullOrEmpty(a)))}
            default: throw new ArgumentOutOfRangeException();
        }}
        writer.WriteEndObject();";
                            break;
                        }
                    default:
                        {
                            read = $@"if (reader.TokenType is not JsonTokenType.StartObject) throw new JsonException();
        reader.Read();
        if (reader.TokenType is not JsonTokenType.PropertyName) throw new JsonException();
        var meta = {type_name}.UnionMeta;
        if (!meta.JsonNameToKind.TryGetValue(reader.GetString()!, out var kind)) throw new JsonException();
        reader.Read();
        {type_name} result;
        switch (kind)
        {{{string.Join("", reads.Where(a => !string.IsNullOrEmpty(a)))}
            default: throw new ArgumentOutOfRangeException();
        }}
        reader.Read();
        if (reader.TokenType is not JsonTokenType.EndObject) throw new JsonException();
        return result;
        ";
                            write = $@"writer.WriteStartObject();
        switch (value.Kind)
        {{{string.Join("", writes.Where(a => !string.IsNullOrEmpty(a)))}
            default: throw new ArgumentOutOfRangeException();
        }}
        writer.WriteEndObject();";
                            break;
                        }
                }

                var can_convert = is_struct ? string.Empty : $@"
    private static readonly HashSet<Type> CanConvertTypes = new() {{ typeof({type_name}), {string.Join(", ", can_convert_types)} }};

    public override bool CanConvert(Type typeToConvert) => CanConvertTypes.Contains(typeToConvert);
";

                return $@"
{accessibility.GetAccessStr()} partial class {json_system_text_name}{type_generic} : JsonConverter<{type_name}> {constraints}
{{{can_convert}
    public override {type_name} Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {{
        {read}
    }}

    public override void Write(Utf8JsonWriter writer, {type_name} value, JsonSerializerOptions options)
    {{
        {write}
    }}
}}
";
            }
        });
    }

    public JsonInfo? GetJsonInfo(AttributeData? attr_json)
    {
        if (attr_json == null) return null;
        var args = attr_json.NamedArguments.AsParallel()
            .ToDictionary(a => a.Key, a => a.Value);
        var WithSystemText = args.TryGet("WithSystemText")?.Value is not false;
        var SystemTextClassName = args.TryGet("SystemTextClassName")?.Value as string;
        var JsonMode = args.TryGet("JsonMode")?.Value is int i ? (UnionJsonMode)i : 0;
        var Tag = args.TryGet("Tag")?.Value as string ?? "t";
        var Content = args.TryGet("Content")?.Value as string ?? "c";
        var NumberTag = args.TryGet("NumberTag")?.Value is true;
        return new JsonInfo
        {
            WithSystemText = WithSystemText,
            SystemTextClassName = SystemTextClassName,
            JsonMode = JsonMode,
            Tag = Tag,
            Content = Content,
            NumberTag = NumberTag,
        };
    }

    public record struct JsonInfo
    {
        public bool WithSystemText { get; set; }
        public string? SystemTextClassName { get; set; }
        public UnionJsonMode JsonMode { get; set; }
        public string Tag { get; set; }
        public string Content { get; set; }
        public bool NumberTag { get; set; }
    }

    public enum UnionJsonMode
    {
        External,
        Tuple,
        Adjacent,
    }

}
