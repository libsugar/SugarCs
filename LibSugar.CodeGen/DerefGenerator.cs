﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace LibSugar.CodeGen;

[Generator]
public class DerefGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new DerefReceiver());
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
        var receiver = (DerefReceiver)context.SyntaxReceiver!;

        var defs = context.CollectDeclSymbol(receiver.Defs);

        defs.AsParallel().ForAll(def =>
        {
            var (syn, syn_attr, sym, semantic) = def.Value;

            var attr_deref = sym.GetAttributes().AsParallel().AsOrdered()
                .QueryAttr("LibSugar.DerefAttribute")
                .FirstOrDefault();
            // not LibSugar's deref attr
            if (attr_deref == null) return;

            var deref_method_name = "Deref";
            if (attr_deref is { ConstructorArguments.Length: > 0 })
            {
                var first_arg = attr_deref.ConstructorArguments.FirstOrDefault();
                deref_method_name = $"{first_arg.Value}";
            }

            var deref_method = sym.GetMembers(deref_method_name).AsParallel().AsOrdered()
                .Where(m => m is { Kind: SymbolKind.Property, IsStatic: false })
                .WhereCast<IPropertySymbol>()
                .Select(m => m.GetMethod)
                .FirstOrDefault();

            if (deref_method == null)
            {
                if (sym.TypeKind is TypeKind.Interface)
                {
                    deref_method = sym.AllInterfaces.AsParallel().AsOrdered()
                        .SelectMany(a => a.GetMembers(deref_method_name).AsParallel().AsOrdered())
                        .Where(m => m is { Kind: SymbolKind.Property, IsStatic: false })
                        .WhereCast<IPropertySymbol>()
                        .Select(m => m.GetMethod)
                        .FirstOrDefault();
                }
                else if (sym.TypeKind is TypeKind.Class)
                {
                    var base_type = sym.BaseType;
                    while (base_type is { SpecialType: not SpecialType.System_Object })
                    {
                        deref_method = base_type.GetMembers(deref_method_name).AsParallel().AsOrdered()
                            .Where(m => m is { Kind: SymbolKind.Property, IsStatic: false, DeclaredAccessibility: not Accessibility.Private })
                            .WhereCast<IPropertySymbol>()
                            .Select(m => m.GetMethod)
                            .FirstOrDefault();
                        if (deref_method != null) break;
                        base_type = base_type.BaseType;
                    }
                }
            }
            if (deref_method == null)
            {
                context.LogError($"Could not find deref method {sym.ToDisplayString()}.{deref_method_name} {{ get; }}", syn_attr.GetLocation());
                return;
            }

            var deref_root_type = deref_method.ReturnType as INamedTypeSymbol;
            if (deref_root_type == null)
            {
                context.LogError($"Deref target type can not be array, pointer, type parameter", syn_attr.GetLocation());
                return;
            }

            var inherit_levels = attr_deref.NamedArguments
                .Where(a => a.Key == "InheritLevels")
                .Select(a => a.Value)
                .Where(a => a.Kind is TypedConstantKind.Primitive)
                .Select(a => a.Value is int v ? v : 0)
                .FirstOrDefault();
            if (inherit_levels == -1) inherit_levels = int.MaxValue;

            var end_base_class = attr_deref.NamedArguments
                .Where(a => a.Key == "EndBaseClass")
                .Select(a => a.Value)
                .Where(a => a.Kind is TypedConstantKind.Type)
                .Select(a => a.Value as INamedTypeSymbol)
                .FirstOrDefault();

            var use_extension = attr_deref.NamedArguments
                .Where(a => a.Key == "UseExtension")
                .Select(a => a.Value)
                .Where(a => a.Kind is TypedConstantKind.Primitive)
                .Select(a => a.Value is true ? (bool?)true : null)
                .FirstOrDefault() ?? sym.TypeKind is TypeKind.Interface;

            var extension_name = attr_deref.NamedArguments
                .Where(a => a.Key == "ExtensionName")
                .Select(a => a.Value)
                .Where(a => a.Kind is TypedConstantKind.Primitive)
                .Select(a => a.Value is string s ? s : null)
                .FirstOrDefault() ?? $"{syn.Identifier}Deref";

            var extension_access = sym.DeclaredAccessibility.GetAccessStr();

            var usings = new HashSet<string> { "using System.Runtime.CompilerServices;", "using LibSugar;" };
            Utils.GetUsings(syn, usings);

            var results = new List<string>();
            var indexer_table = new ConcurrentBag<IPropertySymbol>();
            var property_table = new ConcurrentBag<string>();
            var method_table = new ConcurrentDictionary<string, ConcurrentBag<IMethodSymbol>>();
            GenStep(deref_root_type, 0);

            var wrapped = use_extension
                ? $"{sym.WrapNameSpace(string.Join("\n", results))}"
                : $"{sym.WrapNameSpace(sym.WrapNestedType(string.Join("\n", results)))}";

            var source_text = SourceText.From($@"// <auto-generated/>

#nullable enable
#pragma warning disable CS0693

{string.Join("\n", usings.OrderBy(u => u.Length))}

{wrapped}
#pragma warning restore CS0693
", Encoding.UTF8);
            var source_file_name = $"{sym.ToDisplayString().Replace('<', '[').Replace('>', ']')}.deref.gen.cs";
            context.AddSource(source_file_name, source_text);

            //////////////////////////////////////////////////

            void GenStep(INamedTypeSymbol deref_target, int level)
            {
                var deref_target_name = deref_target.ToDisplayString();
                var deref_target_typeof = deref_target.GetTypeOf();

                var properties = deref_target.GetMembers().AsParallel().AsOrdered()
                    .Where(m => m is { Kind: SymbolKind.Property, IsStatic: false })
                    .WhereCast<IPropertySymbol>()
                    .Where(p => p is { IsIndexer: true } or { IsIndexer: false, CanBeReferencedByName: true })
                    .Where(p => !p.GetAttributes().AsParallel().QueryAttrEq("LibSugar.DoNotDerefAttribute").Any())
                    .ToArray();
                var methods = deref_target.GetMembers().AsParallel().AsOrdered()
                    .Where(m => m is { Kind: SymbolKind.Method, IsStatic: false, CanBeReferencedByName: true })
                    .WhereCast<IMethodSymbol>()
                    .Where(m => !m.GetAttributes().AsParallel().QueryAttrEq("LibSugar.DoNotDerefAttribute").Any())
                    .ToArray();

                var gened = use_extension ? GenExtension() : GenInner();

                results.Add(gened);

                if (level < inherit_levels)
                {
                    var base_type = deref_target.BaseType;
                    if (base_type == null || base_type.SpecialType is SpecialType.System_Object) return;
                    if (end_base_class != null && base_type.Equals(end_base_class, SymbolEqualityComparer.Default)) return;

                    GenStep(base_type, level + 1);
                    return;
                }

                //////////////////////////////////////////////////

                string GenInner()
                {
                    var properties_gen = new string[properties.Length];
                    var methods_gen = new string[methods.Length];

                    properties.AsParallel().AsOrdered().Indexed().ForAll(pi =>
                    {
                        var (p, i) = pi;

                        if (p.IsIndexer)
                        {
                            #region Distinct

                            if (indexer_table.AsParallel().Any(a => a.IndexerEq(p))) return;
                            var self_indexs = sym.GetMembers(p.Name);
                            if (self_indexs.AsParallel().WhereCast<ISymbol, IPropertySymbol>()
                                .Where(a => a.IsIndexer).Any(a => a.IndexerEq(p))) return;

                            #endregion

                            var args = p.Parameters;

                            var items = new List<string>();

                            var acc = p.DeclaredAccessibility.GetAccessStr();
                            var type = p.Type.ToDisplayString();

                            var doc_name = $"this[{string.Join(", ", p.Parameters.Select(a => a.Type.ToDisplayString()))}]";

                            var is_ref = p.ReturnsByRef || p.ReturnsByRefReadonly;

                            var ref_mod = p.ReturnsByRefReadonly ? "ref readonly " : p.ReturnsByRef ? "ref " : string.Empty;
                            var ref_oper = is_ref ? "ref " : string.Empty;

                            var getter = p.GetMethod;
                            if (getter != null)
                            {
                                var can_acc = semantic.IsAccessible(syn.SpanStart, getter);
                                if (can_acc)
                                {
                                    items.Add($@"        [MethodImpl(MethodImplOptions.AggressiveInlining), CompilerGenerated, LibSugar.DerefFrom(typeof({deref_target_typeof}), ""{p.Name}"")]
        get => {ref_oper}{deref_method_name}[{string.Join(", ", args.ParamToArg())}];");
                                }
                            }

                            var setter = p.SetMethod;
                            if (setter != null && !is_ref)
                            {
                                var can_acc = semantic.IsAccessible(syn.SpanStart, setter);
                                if (can_acc)
                                {
                                    items.Add($@"        [MethodImpl(MethodImplOptions.AggressiveInlining), CompilerGenerated, LibSugar.DerefFrom(typeof({deref_target_typeof}), ""{p.Name}"")]
        set => {deref_method_name}[{string.Join(", ", args.ParamToArg())}] = value;");
                                }
                            }

                            if (items.Count > 0)
                            {
                                indexer_table.Add(p);
                                var doc_cref = $"{deref_target_name}.{doc_name}".Replace("<", "{").Replace(">", "}");
                                properties_gen[i] = $@"    /// <inheritdoc cref=""{doc_cref}""/>
    {acc} {ref_mod}{type} this[{string.Join(", ", args)}]
    {{
{string.Join("\n", items)}
    }}
";
                            }
                        }
                        else
                        {
                            #region Distinct

                            if (property_table.AsParallel().Any(a => a == p.Name)) return;
                            if (!sym.GetMembers(p.Name).IsEmpty) return;

                            #endregion

                            var items = new List<string>();

                            var acc = p.DeclaredAccessibility.GetAccessStr();
                            var type = p.Type.ToDisplayString();

                            var is_ref = p.ReturnsByRef || p.ReturnsByRefReadonly;

                            var ref_mod = p.ReturnsByRefReadonly ? "ref readonly " : p.ReturnsByRef ? "ref " : string.Empty;
                            var ref_oper = is_ref ? "ref " : string.Empty;

                            var getter = p.GetMethod;
                            if (getter != null)
                            {
                                var can_acc = semantic.IsAccessible(syn.SpanStart, getter);
                                if (can_acc)
                                {
                                    items.Add($@"        [MethodImpl(MethodImplOptions.AggressiveInlining), CompilerGenerated, LibSugar.DerefFrom(typeof({deref_target_typeof}), ""{p.Name}"")]
        get => {ref_oper}{deref_method_name}.{p.Name};");
                                }
                            }

                            var setter = p.SetMethod;
                            if (setter != null && !is_ref)
                            {
                                var can_acc = semantic.IsAccessible(syn.SpanStart, setter);
                                if (can_acc)
                                {
                                    items.Add($@"        [MethodImpl(MethodImplOptions.AggressiveInlining), CompilerGenerated, LibSugar.DerefFrom(typeof({deref_target_typeof}), ""{p.Name}"")]
        set => {deref_method_name}.{p.Name} = value;");
                                }
                            }

                            if (items.Count > 0)
                            {
                                property_table.Add(p.Name);
                                var doc_cref = $"{deref_target_name}.{p.Name}".Replace("<", "{").Replace(">", "}");
                                properties_gen[i] = $@"    /// <inheritdoc cref=""{doc_cref}""/>
    [CompilerGenerated, LibSugar.DerefFrom(typeof({deref_target_typeof}), ""{p.Name}"")]
    {acc} {ref_mod}{type} {p.Name}
    {{
{string.Join("\n", items)}
    }}
";
                            }
                        }
                    });
                    methods.AsParallel().AsOrdered().Indexed().ForAll(mi =>
                    {
                        var (m, i) = mi;

                        #region Distinct

                        if (method_table.GetOrAdd(m.Name, _ => new())
                            .AsParallel().Any(a => a.MethodEq(m))) return;
                        var self_methods = sym.GetMembers(m.Name);
                        if (self_methods.AsParallel().WhereCast<ISymbol, IMethodSymbol>().Any(a => a.MethodEq(m))) return;

                        #endregion

                        var acc = m.DeclaredAccessibility.GetAccessStr();
                        var ret_type = m.ReturnType.ToDisplayString();

                        var can_acc = semantic.IsAccessible(syn.SpanStart, m);
                        if (!can_acc) return;

                        var is_ref = m.ReturnsByRef || m.ReturnsByRefReadonly;

                        var ref_mod = m.ReturnsByRefReadonly ? "ref readonly " : m.ReturnsByRef ? "ref " : string.Empty;
                        var ref_oper = is_ref ? "ref " : string.Empty;

                        var generic = m.IsGenericMethod ? $"<{string.Join(", ", m.TypeParameters)}>" : string.Empty;
                        var constraint = string.Join(" ", m.TypeParameters.GetConstraint());
                        if (!string.IsNullOrWhiteSpace(constraint)) constraint = $" {constraint}";

                        var doc_params = m.Parameters.IsEmpty ? string.Empty : $"({string.Join(", ", m.Parameters.ParamToDocArg())})";
                        var doc_cref = $"{deref_target_name}.{m.Name}{generic}{doc_params}".Replace("<", "{").Replace(">", "}");

                        method_table.GetOrAdd(m.Name, _ => new()).Add(m);
                        methods_gen[i] = $@"    /// <inheritdoc cref=""{doc_cref}""/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CompilerGenerated, LibSugar.DerefFrom(typeof({deref_target_typeof}), ""{m.Name}"")]
    {acc} {ref_mod}{ret_type} {m.Name}{generic}({string.Join(", ", m.Parameters)}){constraint} => {ref_oper}{deref_method_name}.{m.Name}{generic}({string.Join(", ", m.Parameters.ParamToArg())});
";
                    });

                    return $@"
[LibSugar.DerefFrom(typeof({deref_target_typeof}))]
{syn.Modifiers} {syn.Keyword} {syn.Identifier}{syn.TypeParameterList}
{{
{string.Join("\n", properties_gen.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
{string.Join("\n", methods_gen.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
}}
";
                }

                //////////////////////////////////////////////////

                string GenExtension()
                {

                    var methods_gen = new string[methods.Length];
                    methods.AsParallel().AsOrdered().Indexed().ForAll(mi =>
                    {
                        var (m, i) = mi;

                        #region Distinct

                        if (method_table.GetOrAdd(m.Name, _ => new())
                            .AsParallel().Any(a => a.MethodEq(m))) return;

                        #endregion

                        var acc = m.DeclaredAccessibility.GetAccessStr();
                        var ret_type = m.ReturnType.ToDisplayString();

                        var can_acc = m.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal;
                        if (!can_acc) return;

                        var is_ref = m.ReturnsByRef || m.ReturnsByRefReadonly;

                        var ref_mod = m.ReturnsByRefReadonly ? "ref readonly " : m.ReturnsByRef ? "ref " : string.Empty;
                        var ref_oper = is_ref ? "ref " : string.Empty;

                        var method_generic_names =
                            m.TypeParameters.AsParallel().Select(a => a.Name).ToImmutableHashSet();

                        var has_generic = !(sym.TypeArguments.IsEmpty && m.TypeParameters.IsEmpty);
                        var class_generic = sym.TypeParameters.AsParallel().AsOrdered()
                            .Select(a => new ReplaceNameTypeParameterSymbol(method_generic_names.GetUniqueName(a.Name), a)).ToArray();
                        var method_generic = m.TypeParameters.AsParallel().AsOrdered()
                            .Select(a => new ReplaceNameTypeParameterSymbol(a.Name, a)).ToArray();
                        var merge_generic = class_generic.AsParallel().AsOrdered()
                            .Concat(method_generic.AsParallel().AsOrdered()).ToArray();

                        var c_generic = sym.TypeParameters.IsEmpty ? string.Empty : $"<{string.Join(", ", class_generic.Select(a => a.Name))}>";
                        var m_generic = m.TypeParameters.IsEmpty ? string.Empty : $"<{string.Join(", ", method_generic.Select(a => a.Name))}>";
                        var generic = has_generic ? $"<{string.Join(", ", merge_generic.Select(a => a.Name))}>" : string.Empty;

                        var constraint = string.Join(" ", merge_generic.GetConstraint().Where(a => !string.IsNullOrWhiteSpace(a)));
                        if (!string.IsNullOrWhiteSpace(constraint)) constraint = $" {constraint}";

                        var @params = $"{string.Join(", ", m.Parameters)}";
                        if (!string.IsNullOrEmpty(@params)) @params = $", {@params}";

                        var doc_params = m.Parameters.IsEmpty ? string.Empty : $"({string.Join(", ", m.Parameters.ParamToDocArg())})";
                        var doc_m_generic = m.IsGenericMethod ? $"<{string.Join(", ", m.TypeParameters)}>" : string.Empty;
                        var doc_cref = $"{deref_target_name}.{m.Name}{doc_m_generic}{doc_params}".Replace("<", "{").Replace(">", "}");

                        method_table.GetOrAdd(m.Name, _ => new()).Add(m);
                        methods_gen[i] = $@"    /// <inheritdoc cref=""{doc_cref}""/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CompilerGenerated, LibSugar.DerefFrom(typeof({deref_target_typeof}), ""{m.Name}"")]
    {acc} static {ref_mod}{ret_type} {m.Name}{generic}(this {syn.Identifier}{c_generic} self{@params}){constraint} => {ref_oper}self.{deref_method_name}.{m.Name}{m_generic}({string.Join(", ", m.Parameters.ParamToArg())});
";
                    });

                    return $@"
[LibSugar.DerefFrom(typeof({deref_target_typeof}))]
{extension_access} static partial class {extension_name}
{{
{string.Join("\n", methods_gen.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
}}
";
                }

            }

        });
    }

    class DerefReceiver : ISyntaxReceiver
    {
        public readonly List<(TypeDeclarationSyntax, AttributeSyntax)> Defs = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax tds)
            {
                foreach (var attributeList in tds.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        if (
                            attribute.Name.CheckAttrName("Deref") ||
                            attribute.Name.CheckAttrName("DerefAttribute")
                        )
                        {
                            Defs.Add((tds, attribute));
                            return;
                        }
                    }
                }
            }
        }
    }

}
