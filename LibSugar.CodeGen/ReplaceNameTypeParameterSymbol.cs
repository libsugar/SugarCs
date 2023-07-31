using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Threading;

namespace LibSugar.CodeGen;

public record ReplaceNameTypeParameterSymbol(string Name, ITypeParameterSymbol Base)
{
    public override string ToString()
    {
        return Base.ToString();
    }

    public virtual bool Equals(ISymbol? other)
    {
        return Base.Equals(other);
    }

    public ImmutableArray<AttributeData> GetAttributes()
    {
        return Base.GetAttributes();
    }

    public void Accept(SymbolVisitor visitor)
    {
        Base.Accept(visitor);
    }

    public TResult? Accept<TResult>(SymbolVisitor<TResult> visitor)
    {
        return Base.Accept(visitor);
    }

    public TResult Accept<TArgument, TResult>(SymbolVisitor<TArgument, TResult> visitor, TArgument argument)
    {
        return Base.Accept(visitor, argument);
    }

    public string? GetDocumentationCommentId()
    {
        return Base.GetDocumentationCommentId();
    }

    public string? GetDocumentationCommentXml(CultureInfo? preferredCulture = null, bool expandIncludes = false,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Base.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
    }

    public string ToDisplayString(SymbolDisplayFormat? format = null)
    {
        return Base.ToDisplayString(format);
    }

    public ImmutableArray<SymbolDisplayPart> ToDisplayParts(SymbolDisplayFormat? format = null)
    {
        return Base.ToDisplayParts(format);
    }

    public string ToMinimalDisplayString(SemanticModel semanticModel, int position, SymbolDisplayFormat? format = null)
    {
        return Base.ToMinimalDisplayString(semanticModel, position, format);
    }

    public ImmutableArray<SymbolDisplayPart> ToMinimalDisplayParts(SemanticModel semanticModel, int position, SymbolDisplayFormat? format = null)
    {
        return Base.ToMinimalDisplayParts(semanticModel, position, format);
    }

    public bool Equals(ISymbol? other, SymbolEqualityComparer equalityComparer)
    {
        return Base.Equals(other, equalityComparer);
    }

    public SymbolKind Kind => Base.Kind;

    public string Language => Base.Language;

    public string MetadataName => Base.MetadataName;

    public int MetadataToken => Base.MetadataToken;

    public ISymbol ContainingSymbol => Base.ContainingSymbol;

    public IAssemblySymbol ContainingAssembly => Base.ContainingAssembly;

    public IModuleSymbol ContainingModule => Base.ContainingModule;

    public INamedTypeSymbol ContainingType => Base.ContainingType;

    public INamespaceSymbol ContainingNamespace => Base.ContainingNamespace;

    public bool IsDefinition => Base.IsDefinition;

    public bool IsStatic => Base.IsStatic;

    public bool IsVirtual => Base.IsVirtual;

    public bool IsOverride => Base.IsOverride;

    public bool IsAbstract => Base.IsAbstract;

    public bool IsSealed => Base.IsSealed;

    public bool IsExtern => Base.IsExtern;

    public bool IsImplicitlyDeclared => Base.IsImplicitlyDeclared;

    public bool CanBeReferencedByName => Base.CanBeReferencedByName;

    public ImmutableArray<Location> Locations => Base.Locations;

    public ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Base.DeclaringSyntaxReferences;

    public Accessibility DeclaredAccessibility => Base.DeclaredAccessibility;

    public ITypeParameterSymbol OriginalDefinition => Base.OriginalDefinition;

    public ITypeParameterSymbol? ReducedFrom => Base.ReducedFrom;

    public int Ordinal => Base.Ordinal;

    public VarianceKind Variance => Base.Variance;

    public TypeParameterKind TypeParameterKind => Base.TypeParameterKind;

    public IMethodSymbol? DeclaringMethod => Base.DeclaringMethod;

    public INamedTypeSymbol? DeclaringType => Base.DeclaringType;

    public bool HasReferenceTypeConstraint => Base.HasReferenceTypeConstraint;

    public NullableAnnotation ReferenceTypeConstraintNullableAnnotation => Base.ReferenceTypeConstraintNullableAnnotation;

    public bool HasValueTypeConstraint => Base.HasValueTypeConstraint;

    public bool HasUnmanagedTypeConstraint => Base.HasUnmanagedTypeConstraint;

    public bool HasNotNullConstraint => Base.HasNotNullConstraint;

    public bool HasConstructorConstraint => Base.HasConstructorConstraint;

    public ImmutableArray<ITypeSymbol> ConstraintTypes => Base.ConstraintTypes;

    public ImmutableArray<NullableAnnotation> ConstraintNullableAnnotations => Base.ConstraintNullableAnnotations;
    
    public SpecialType SpecialType => Base.SpecialType;

    public bool IsRefLikeType => Base.IsRefLikeType;

    public bool IsUnmanagedType => Base.IsUnmanagedType;

    public bool IsReadOnly => Base.IsReadOnly;

    public bool IsRecord => Base.IsRecord;

    public NullableAnnotation NullableAnnotation => Base.NullableAnnotation;

    public ISymbol? FindImplementationForInterfaceMember(ISymbol interfaceMember)
    {
        return Base.FindImplementationForInterfaceMember(interfaceMember);
    }

    public string ToDisplayString(NullableFlowState topLevelNullability, SymbolDisplayFormat? format = null)
    {
        return Base.ToDisplayString(topLevelNullability, format);
    }

    public ImmutableArray<SymbolDisplayPart> ToDisplayParts(NullableFlowState topLevelNullability, SymbolDisplayFormat? format = null)
    {
        return Base.ToDisplayParts(topLevelNullability, format);
    }

    public string ToMinimalDisplayString(SemanticModel semanticModel, NullableFlowState topLevelNullability, int position,
        SymbolDisplayFormat? format = null)
    {
        return Base.ToMinimalDisplayString(semanticModel, topLevelNullability, position, format);
    }

    public ImmutableArray<SymbolDisplayPart> ToMinimalDisplayParts(SemanticModel semanticModel, NullableFlowState topLevelNullability, int position,
        SymbolDisplayFormat? format = null)
    {
        return Base.ToMinimalDisplayParts(semanticModel, topLevelNullability, position, format);
    }

    public ITypeSymbol WithNullableAnnotation(NullableAnnotation nullableAnnotation)
    {
        return Base.WithNullableAnnotation(nullableAnnotation);
    }

    public TypeKind TypeKind => Base.TypeKind;

    public INamedTypeSymbol? BaseType => Base.BaseType;

    public ImmutableArray<INamedTypeSymbol> Interfaces => Base.Interfaces;

    public ImmutableArray<INamedTypeSymbol> AllInterfaces => Base.AllInterfaces;

    public bool IsReferenceType => Base.IsReferenceType;

    public bool IsValueType => Base.IsValueType;

    public bool IsAnonymousType => Base.IsAnonymousType;

    public bool IsTupleType => Base.IsTupleType;

    public bool IsNativeIntegerType => Base.IsNativeIntegerType;

    public bool HasUnsupportedMetadata => Base.HasUnsupportedMetadata;

    public ImmutableArray<ISymbol> GetMembers()
    {
        return Base.GetMembers();
    }

    public ImmutableArray<ISymbol> GetMembers(string name)
    {
        return Base.GetMembers(name);
    }

    public ImmutableArray<INamedTypeSymbol> GetTypeMembers()
    {
        return Base.GetTypeMembers();
    }

    public ImmutableArray<INamedTypeSymbol> GetTypeMembers(string name)
    {
        return Base.GetTypeMembers(name);
    }

    public ImmutableArray<INamedTypeSymbol> GetTypeMembers(string name, int arity)
    {
        return Base.GetTypeMembers(name, arity);
    }

    public bool IsNamespace => Base.IsNamespace;

    public bool IsType => Base.IsType;
}
