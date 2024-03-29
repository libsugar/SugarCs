﻿using System;

namespace LibSugar;

public interface IDeref<out T>
{
    public T Deref { get; }
}

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
public class DerefAttribute : Attribute
{
    /// <summary>Deref getter name</summary>
    public string Deref { get; } = "Deref";
    /// <summary>
    /// Generated in the extension class or generated in the current class,
    /// only the current class supports deref property
    /// </summary>
    public bool UseExtension { get; set; } = false;
    /// <summary>
    /// By default, only the deref type itself does not contain the base class of the type,
    /// -1 indicates to deref all base classes, until <see cref="object"/> or <see cref="EndBaseClass"/>
    /// </summary>
    public int InheritLevels { get; set; } = 0;
    /// <summary>End base class</summary>
    public Type EndBaseClass { get; set; } = typeof(object);
    /// <summary>
    /// Extension class name when <see cref="UseExtension"/> = <c>true</c>
    /// </summary>
    public string? ExtensionName { get; set; }

    public DerefAttribute() { }
    public DerefAttribute(string deref) => Deref = deref;
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
public class DerefForAttribute : Attribute
{
    /// <summary>Deref expr</summary>
    public string Deref { get; } = ".Deref";
    public Type? For { get; }
    /// <summary>
    /// By default, only the deref type itself does not contain the base class of the type,
    /// -1 indicates to deref all base classes, until <see cref="object"/> or <see cref="EndBaseClass"/>
    /// </summary>
    public int InheritLevels { get; set; } = 0;
    /// <summary>End base class</summary>
    public Type EndBaseClass { get; set; } = typeof(object);
    /// <summary>Extension class name</summary>
    public string? Name { get; set; }
    public DerefForAttribute(Type @for) { For = @for; }

    public DerefForAttribute(Type @for, string deref)
    {
        For = @for;
        Deref = deref;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
public class DerefForAttribute<T> : DerefForAttribute
{
    public DerefForAttribute() : base(typeof(T)) { }

    public DerefForAttribute(string deref) : base(typeof(T), deref) { }
}

/// <summary>Generated meta info mark</summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class DerefFromAttribute : Attribute
{
    public Type Source { get; set; }
    public string? Name { get; set; }

    public DerefFromAttribute(Type source, string name)
    {
        Source = source;
        Name = name;
    }

    public DerefFromAttribute(Type source)
    {
        Source = source;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class DoNotDerefAttribute : Attribute { }
