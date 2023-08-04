using System;
using System.Collections.Generic;
using System.Text;

namespace LibSugar;

public static partial class Sugar
{
    public static EqByEqualityComparer<T, D> EqBy<T, D>(this Func<T, D> f) => new(f);
    public static EqByEqualityComparer<T, D> EqBy<T, D>(this Func<T, D> f, IEqualityComparer<D> equality) => new(f, equality);
    public static EqByEqualityComparer<T, D> EqBy<T, D>(this IEqualityComparer<D> equality, Func<T, D> f) => new(f, equality);
}

public record EqByEqualityComparer<T, D>(Func<T, D> F, IEqualityComparer<D> Equality) : IEqualityComparer<T>
{
    public EqByEqualityComparer(Func<T, D> f) : this(f, EqualityComparer<D>.Default) { }

    public bool Equals(T? x, T? y) => Equality.Equals(F(x!), F(y!));

    public int GetHashCode(T obj) => Equality.GetHashCode(F(obj)!);
}
