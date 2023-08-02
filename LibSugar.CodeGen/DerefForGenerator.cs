using Microsoft.CodeAnalysis;
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
public class DerefForGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new DerefForReceiver());
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
        var receiver = (DerefForReceiver)context.SyntaxReceiver!;

        var defs = context.CollectDeclSymbol(receiver.Defs);

        defs.AsParallel().ForAll(def =>
        {
            var (syn, _, sym, semantic) = def.Value;
            var attr_derefs = sym.GetAttributes().AsParallel().AsOrdered()
                .QueryAttr("LibSugar.DerefForAttribute")
                .ToArray();
            // not LibSugar's deref attr
            if (attr_derefs.Length == 0) return;

            attr_derefs.AsParallel().Indexed().ForAll(ai =>
            {
                var (attr_deref, index) = ai;
                INamedTypeSymbol? deref_source = null;
                if (attr_deref is { AttributeClass: { IsGenericType: true, TypeArguments: var type_args } })
                {
                    var first_type = type_args.FirstOrDefault();
                    if (first_type is INamedTypeSymbol nts)
                    {
                        deref_source = nts;
                    }
                }
                else if (attr_deref is { ConstructorArguments.Length: > 0 })
                {
                    var first_arg = attr_deref.ConstructorArguments.FirstOrDefault();
                    if (first_arg.Value is INamedTypeSymbol nts)
                    {
                        deref_source = nts;
                    }
                }

                if (deref_source is null)
                {
                    var loc = attr_deref.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? syn.Identifier.GetLocation();
                    context.LogError($"Unable to resolve deref target type", loc);
                    return;
                }
                else if (deref_source.IsUnboundGenericType)
                {
                    var loc = attr_deref.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? syn.Identifier.GetLocation();
                    context.LogError($"Deref target cannot be an unbound generic type", loc);
                    return;
                }

                var deref_expr = ".Deref";
                if (attr_deref is { ConstructorArguments.Length: > 1 })
                {
                    var first_arg = attr_deref.ConstructorArguments.Skip(1).FirstOrDefault();
                    deref_expr = $"{first_arg.Value}";
                }
                else if (attr_deref is { ConstructorArguments.Length: > 0, AttributeClass.IsGenericType: true })
                {
                    var first_arg = attr_deref.ConstructorArguments.FirstOrDefault();
                    deref_expr = $"{first_arg.Value}";
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

                var extension_name = attr_deref.NamedArguments
                    .Where(a => a.Key == "ExtensionName")
                    .Select(a => a.Value)
                    .Where(a => a.Kind is TypedConstantKind.Primitive)
                    .Select(a => a.Value is string s ? s : null)
                    .FirstOrDefault() ?? $"{syn.Identifier}Deref";

                var access = sym.DeclaredAccessibility.GetAccessStr();

                var usings = new HashSet<string> { "using System.Runtime.CompilerServices;", "using LibSugar;" };
                Utils.GetUsings(syn, usings);

                var results = new List<string>();
                var method_table = new ConcurrentDictionary<string, ConcurrentBag<IMethodSymbol>>();
                GenStep(sym, 0);

                var source_text = SourceText.From($@"// <auto-generated/>

#nullable enable
#pragma warning disable CS0693

{string.Join("\n", usings.OrderBy(u => u.Length))}

{sym.WrapNameSpace(string.Join("\n", results))}
#pragma warning restore CS0693
", Encoding.UTF8);
                var source_file_name = $"{sym.ToDisplayString().Replace('<', '[').Replace('>', ']')}.deref.for.{index}.gen.cs";
                context.AddSource(source_file_name, source_text);

                //////////////////////////////////////////////////

                void GenStep(INamedTypeSymbol deref_target, int level)
                {
                    var deref_target_name = deref_target.ToDisplayString();
                    var deref_target_typeof = deref_target.GetTypeOf();

                    var methods = deref_target.GetMembers().AsParallel().AsOrdered()
                        .Where(m => m is { Kind: SymbolKind.Method, IsStatic: false, CanBeReferencedByName: true })
                        .WhereCast<IMethodSymbol>()
                        .Where(m => !m.GetAttributes().AsParallel().QueryAttrEq("LibSugar.DoNotDerefAttribute").Any())
                        .ToArray();

                    var gened = Gen();
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

                    string Gen()
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

                            var self_type = deref_source.CollectPlaceholder(method_generic_names, out var placeholder_generic_with_constraint);

                            var has_generic = !(placeholder_generic_with_constraint.IsEmpty && m.TypeParameters.IsEmpty);
                            var merge_generic = placeholder_generic_with_constraint.Keys.AsParallel().AsOrdered()
                                .Concat(m.TypeParameters.AsParallel().AsOrdered().Select(a => a.Name)).ToArray();
                            var generic = has_generic ? $"<{string.Join(", ", merge_generic)}>" : string.Empty;

                            var m_generic = m.TypeParameters.IsEmpty ? string.Empty : $"<{string.Join(", ", m.TypeParameters.Select(a => a.Name))}>";

                            var merge_constraint = placeholder_generic_with_constraint.Values.AsParallel().AsOrdered()
                                .Concat(m.TypeParameters.AsParallel().GetConstraint()).Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                            var constraint = merge_constraint.Length == 0 ? string.Empty : $" {string.Join(" ", merge_constraint)}";

                            var @params = $"{string.Join(", ", m.Parameters)}";
                            if (!string.IsNullOrEmpty(@params)) @params = $", {@params}";

                            var doc_params = m.Parameters.IsEmpty ? string.Empty : $"({string.Join(", ", m.Parameters.ParamToDocArg())})";
                            var doc_m_generic = m.IsGenericMethod ? $"<{string.Join(", ", m.TypeParameters)}>" : string.Empty;
                            var doc_cref = $"{deref_target_name}.{m.Name}{doc_m_generic}{doc_params}".Replace("<", "{").Replace(">", "}");

                            method_table.GetOrAdd(m.Name, _ => new()).Add(m);
                            methods_gen[i] = $@"    /// <inheritdoc cref=""{doc_cref}""/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), CompilerGenerated, LibSugar.DerefFrom(typeof({deref_target_typeof}), ""{m.Name}"")]
    {acc} static {ref_mod}{ret_type} {m.Name}{generic}(this {self_type} self{@params}){constraint} => {ref_oper}self{deref_expr}.{m.Name}{m_generic}({string.Join(", ", m.Parameters.ParamToArg())});
";
                        });

                        return $@"
[LibSugar.DerefFrom(typeof({deref_target_typeof}))]
{access} static partial class {extension_name}
{{
{string.Join("\n", methods_gen.AsParallel().AsOrdered().Where(a => a != null).ToArray())}
}}
";
                    }
                }
            });
        });
    }

    class DerefForReceiver : ISyntaxReceiver
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
                            attribute.Name.CheckAttrName("DerefFor") ||
                            attribute.Name.CheckAttrName("DerefForAttribute")
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
