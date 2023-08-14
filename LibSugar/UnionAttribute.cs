using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
    public string? Name { get; }

    public UnionAttribute() { }

    public UnionAttribute(string name) => Name = name;
}

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Class | AttributeTargets.Struct)]
public class UnionJsonAttribute : Attribute
{
#if NETSTANDARD
    /// <summary>
    /// Generate System.Text.Json.Serialization.JsonConverter
    /// </summary>
#else
    /// <summary>
    /// Generate <see cref="System.Text.Json.Serialization.JsonConverter{T}"/>
    /// </summary>
#endif
    public bool WithSystemText { get; set; } = true;

    /// <summary>
    /// Converter class name for System.Text.Json
    /// </summary>
    public string? SystemTextClassName { get; set; }

    /// <summary>
    /// How to serialize and deserialize json
    /// </summary>
    public UnionJsonMode JsonMode { get; set; } = UnionJsonMode.External;

    /// <summary>
    /// Tag name when <see cref="JsonMode"/> is <see cref="UnionJsonMode.Adjacent"/>
    /// </summary>
    public string Tag { get; set; } = "t";
    /// <summary>
    /// Content name when <see cref="JsonMode"/> is <see cref="UnionJsonMode.Adjacent"/>
    /// </summary>
    public string Content { get; set; } = "c";

    /// <summary>
    /// Use enum value as tag
    /// </summary>
    public bool NumberTag { get; set; } = false;
}

/// <summary>
/// How to serialize and deserialize json
/// </summary>
public enum UnionJsonMode
{
    /// <summary>
    /// <code>
    /// { "Tag": { "a": 1 } }
    /// </code>
    /// </summary>
    External,
    /// <summary>
    /// <code>
    /// ["Tag", { "a": 1 }]
    /// </code>
    /// </summary>
    Tuple,
    /// <summary>
    /// <code>
    /// { "t": "Tag", "c": { "a": 1 } }
    /// </code>
    /// <para>
    /// For System.Text.Json: <br/>
    /// This object needs to be ordered, "t" must come before "c"
    /// </para>
    /// </summary>
    Adjacent,
}

public record struct UnionMeta<K> where K : Enum
{
    public UnionMeta(Dictionary<string, K> nameToKind, Dictionary<K, string> kindToName, Dictionary<K, Type> variantTypes, Dictionary<string, K> jsonNameToKind)
    {
        NameToKind = new(nameToKind);
        KindToName = new(kindToName);
        VariantTypes = new(variantTypes);
        JsonNameToKind = new(jsonNameToKind);
    }
    public ReadOnlyDictionary<string, K> NameToKind { get; }
    public ReadOnlyDictionary<K, string> KindToName { get; }
    public ReadOnlyDictionary<K, Type> VariantTypes { get; }
    public ReadOnlyDictionary<string, K> JsonNameToKind { get; }
}

[AttributeUsage(AttributeTargets.Field)]
public class UnionJsonNameAttribute : Attribute
{
    public string Name { get; }

    public UnionJsonNameAttribute(string name)
    {
        Name = name;
    }
}
