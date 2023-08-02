using System;

namespace LibSugar;

/// <summary>Mark type is struct</summary>
[AttributeUsage(AttributeTargets.All)]
public class StructAttribute : Attribute
{
}

/// <summary>Mark type is class</summary>
[AttributeUsage(AttributeTargets.All)]
public class ClassAttribute : Attribute
{
}

/// <summary>Mark type is record</summary>
[AttributeUsage(AttributeTargets.All)]
public class RecordAttribute : Attribute
{
}

/// <summary>Mark type is interface</summary>
[AttributeUsage(AttributeTargets.All)]
public class InterfaceAttribute : Attribute
{
}

/// <summary>Mark type is interface</summary>
[AttributeUsage(AttributeTargets.All)]
public class TemplateAttribute : Attribute
{
}

/// <summary>Mark type generic params; Means <c>forall ∀</c></summary>
[AttributeUsage(AttributeTargets.All)]
public class ForAttribute : Attribute
{
    public string[]? Names { get; }

    public ForAttribute() { }
    public ForAttribute(params string[] names) => Names = names;
}

/// <summary>Mark item type</summary>
[AttributeUsage(AttributeTargets.All)]
public class OfAttribute : Attribute
{
    public Type? Type { get; }
    public string? TypeName { get; }

    public OfAttribute() { }

    public OfAttribute(string type) => TypeName = type;
    public OfAttribute(Type type) => Type = type;
}

/// <summary>Mark item type</summary>
[AttributeUsage(AttributeTargets.All)]
public class OfAttribute<T> : OfAttribute
{
    public OfAttribute() : base(typeof(T)) { }
}

/// <summary>Mark item name</summary>
[AttributeUsage(AttributeTargets.All)]
public class NameAttribute : Attribute
{
    public string Name { get; }

    public NameAttribute(string name) => Name = name;
}
