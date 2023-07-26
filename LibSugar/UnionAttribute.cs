using System;

namespace LibSugar;

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Class | AttributeTargets.Struct)]
public class UnionAttribute : Attribute
{
    /// <summary>
    /// Default name assoc
    /// <code>
    /// UnionName   :   Foo
    /// EnumName    :   FooKind
    /// </code>
    /// </summary>
    public string? Name { get; set; }

    public UnionAttribute() { }

    public UnionAttribute(string name) => Name = name;
}
