using System;

namespace LibSugar;

[AttributeUsage(AttributeTargets.All)]
public class StructAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.All)]
public class ClassAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.All)]
public class RecordAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.All)]
public class InterfaceAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.All)]
public class ForAttribute : Attribute
{
    public string[]? Names { get; set; }
    public ForAttribute() { }

    public ForAttribute(params string[] names) => Names = names;
}

[AttributeUsage(AttributeTargets.All)]
public class WhereAttribute : Attribute
{
    public string[]? Wheres { get; set; }
    public WhereAttribute() { }

    public WhereAttribute(params string[] wheres) => Wheres = wheres;
}

[AttributeUsage(AttributeTargets.All)]
public class OfAttribute : Attribute
{
    public Type? Type { get; set; }
    public string? TypeName { get; set; }

    public OfAttribute() { }

    public OfAttribute(string type) => TypeName = type;
    public OfAttribute(Type type) => Type = type;
}

[AttributeUsage(AttributeTargets.All)]
public class OfAttribute<T> : OfAttribute
{
    public OfAttribute() : base(typeof(T)) { }
}

[AttributeUsage(AttributeTargets.All)]
public class NameAttribute : Attribute
{
    public string Name { get; set; }

    public NameAttribute(string name) => Name = name;
}
