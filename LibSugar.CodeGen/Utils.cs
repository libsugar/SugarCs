﻿using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace LibSugar.CodeGen;

internal static class Utils
{
    public static string GetNamespace(SyntaxNode? node)
    {
        var ns = "";

        for (; ; )
        {
            if (node == null) break;
            if (node is NamespaceDeclarationSyntax nds)
            {
                ns = nds.Name.ToString();
                break;
            }
            if (node is CompilationUnitSyntax cus)
            {
                foreach (var member in cus.Members)
                {
                    if (member is FileScopedNamespaceDeclarationSyntax fsnds)
                    {
                        ns = fsnds.Name.ToString();
                        goto end_ns_loop;
                    }
                }
            }
            node = node.Parent;
        }
    end_ns_loop:

        return ns;
    }

    public static string GetFullName(SyntaxNode? node)
    {
        var names = new List<string>();

        for (; ; )
        {
            if (node == null) break;
            else if (node is TypeDeclarationSyntax tds)
            {
                var generics = string.Join(",", tds.TypeParameterList?.Parameters.Select(a => a.Identifier.Text) ?? Array.Empty<string>());
                names.Add($"{tds.Identifier.Text}{(string.IsNullOrEmpty(generics) ? "" : $"<{generics}>")}");
            }
            else if (node is BaseTypeDeclarationSyntax btds)
            {
                names.Add(btds.Identifier.Text);
            }
            else if (node is NamespaceDeclarationSyntax nds)
            {
                names.Add(nds.Name.ToString());
                break;
            }
            else if (node is CompilationUnitSyntax cus)
            {
                foreach (var member in cus.Members)
                {
                    if (member is FileScopedNamespaceDeclarationSyntax fsnds)
                    {
                        names.Add(fsnds.Name.ToString());
                        goto end_ns_loop;
                    }
                }
            }
            node = node.Parent;
        }
    end_ns_loop:

        names.Reverse();
        return string.Join(".", names);
    }

    public static void GetUsings(SyntaxNode? node, HashSet<string> usings)
    {
        for (; ; )
        {
            if (node == null) break;
            if (node is CompilationUnitSyntax cus)
            {
                foreach (var use in cus.Usings)
                {
                    usings.Add(use.ToString());
                }
                return;
            }
            node = node.Parent;
        }
    }

    public static bool CheckAttrName(this NameSyntax ns, string name)
    {
        return ns switch
        {
            GenericNameSyntax gns => name == gns.Identifier.Text,
            SimpleNameSyntax sns => name == sns.Identifier.Text,
            QualifiedNameSyntax qns => CheckAttrName(qns.Right, name),
            _ => false
        };
    }

    public static IEnumerable<AttributeData> QueryAttr(this IEnumerable<AttributeData> iter, string name)
        => iter.Select(static a => (a, n: a.AttributeClass?.ToDisplayString()))
            .Where(a => a.n?.StartsWith(name) ?? false)
            .Select(static a => a.a);

    public static IEnumerable<AttributeData> QueryAttrEq(this IEnumerable<AttributeData> iter, string name)
        => iter.Select(static a => (a, n: a.AttributeClass?.ToDisplayString()))
            .Where(a => a.n == name)
            .Select(static a => a.a);

    public static IEnumerable<S> QuerySymbolByName<S>(this IEnumerable<S> iter, string name) where S : ISymbol
        => iter.Select(static a => (a, n: a.ToDisplayString()))
            .Where(a => a.n.StartsWith(name))
            .Select(static a => a.a);

    public static IEnumerable<S> QuerySymbolByNameEq<S>(this IEnumerable<S> iter, string name) where S : ISymbol
        => iter.Select(static a => (a, n: a.ToDisplayString()))
            .Where(a => a.n == name)
            .Select(static a => a.a);

    public static IEnumerable<TypedConstant> FlatAll(this IEnumerable<TypedConstant> iter)
        => iter.SelectMany(a => a.Kind is TypedConstantKind.Array ? FlatAll(a.Values) : Enumerable.Repeat(a, 1));

    public static ParallelQuery<TypedConstant> FlatAll(this ParallelQuery<TypedConstant> iter)
        => iter.SelectMany(a => a.Kind is TypedConstantKind.Array ? FlatAll(a.Values.AsParallel().AsOrdered()) : ParallelEnumerable.Repeat(a, 1));

    public static string WrapNameSpace(this INamedTypeSymbol symbol, string code)
    {
        var ns = symbol.ContainingNamespace;
        if (ns == null || ns.IsGlobalNamespace) return code;
        return $@"
namespace {ns}
{{
{code}
}}
";
    }

    public static string WrapNestedType(this INamedTypeSymbol symbol, string code)
    {
        var parent = symbol.ContainingType;
        if (parent == null) return code;
        var access = parent.DeclaredAccessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.Private => "private",
            _ => "",
        };
        var type_decl = parent switch
        {
            { IsValueType: true, IsRecord: true, IsReadOnly: false } => "partial record struct",
            { IsValueType: true, IsRecord: true, IsReadOnly: true } => "readonly partial record struct",
            { IsValueType: true, IsRecord: false, IsReadOnly: true, IsRefLikeType: false } => "readonly partial struct",
            { IsValueType: true, IsRecord: false, IsReadOnly: false, IsRefLikeType: true } => "ref partial struct",
            { IsValueType: true, IsRecord: false, IsReadOnly: true, IsRefLikeType: true } => "readonly ref partial struct",
            { IsValueType: true, IsRecord: false, IsReadOnly: false, IsRefLikeType: false } => "partial struct",
            { IsValueType: false, IsRecord: true, IsAbstract: false } => "partial record",
            { IsValueType: false, IsRecord: true, IsAbstract: true } => "abstract partial record",
            { IsValueType: false, IsStatic: true } => "static partial class",
            { IsValueType: false, IsAbstract: true, } => "abstract partial record",
            _ => "partial class",
        };
        var generic = string.Empty;
        if (parent.IsGenericType)
        {
            var ps = new List<string>();
            foreach (var tp in parent.TypeParameters)
            {
                var variance = tp.Variance switch
                {
                    VarianceKind.Out => "out ",
                    VarianceKind.In => "in ",
                    _ => "",
                };
                ps.Add($"{variance}{tp.ToDisplayString()}");
            }
            generic = $"<{string.Join(", ", ps)}>";
        }
        return WrapNestedType(parent, $@"
{access} {type_decl} {parent.Name}{generic}
{{
{code}
}}
");
    }

    public static string GetParentName(this INamedTypeSymbol symbol)
    {
        var parent = symbol.ContainingSymbol;
        if (parent == null || parent is INamespaceSymbol { IsGlobalNamespace: true }) return string.Empty;
        return parent.ToDisplayString();
    }

    public static string ReplaceName(this INamedTypeSymbol symbol, string name)
    {
        var parent = symbol.GetParentName();
        if (string.IsNullOrWhiteSpace(parent)) return name;
        return $"{parent}.{name}";
    }

    public static IEnumerable<R> WhereCast<T, R>(this IEnumerable<T> iter)
    {
        foreach (var item in iter)
        {
            if (item is R r) yield return r;
        }
    }

    public static IEnumerable<R> WhereCast<R>(this IEnumerable iter)
    {
        foreach (var item in iter)
        {
            if (item is R r) yield return r;
        }
    }

    public static IEnumerable<(T a, int i)> Indexed<T>(this IEnumerable<T> iter) => iter.Select((a, b) => (a, b));

    public static ParallelQuery<(T a, int i)> Indexed<T>(this ParallelQuery<T> iter) => iter.Select((a, b) => (a, b));

    public static void LogDebug(this GeneratorExecutionContext context, string msg, Location loc)
    {
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("DEBUG", "", msg, "", DiagnosticSeverity.Warning, true), loc, DiagnosticSeverity.Warning));
    }

    public static void LogError(this GeneratorExecutionContext context, string msg, Location loc)
    {
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("Sugar", "", msg, "", DiagnosticSeverity.Error, true), loc, DiagnosticSeverity.Error));
    }

    public static void LogWarning(this GeneratorExecutionContext context, string msg, Location loc)
    {
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("Sugar", "", msg, "", DiagnosticSeverity.Warning, true), loc, DiagnosticSeverity.Warning));
    }
}