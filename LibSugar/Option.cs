﻿using System;
using System.Collections.Generic;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace LibSugar;

public struct Option<T> : IBox<T>, IEquatable<Option<T>>
#if NET7_0_OR_GREATER
    , IEqualityOperators<Option<T>, Option<T>, bool>, IBitwiseOperators<Option<T>, Option<T>, Option<T>>
#endif
{
    internal T val;
    internal bool has;

    public Option(T val)
    {
        has = true;
        this.val = val;
    }

    public static Option<T> None => new();
    public static Option<T> Some(T val) => new(val);

    public bool Has => has;
    public T Value => val;

    public bool IsNone => !has;
    public bool IsSome => has;

    public Option<U> Map<U>(Func<T, U> f) => has ? new(f(val)) : new();

    public Option<U> Then<U>(Func<T, Option<U>> f) => has ? f(val) : new();

    public Option<U> And<U>(Option<U> o) => has ? o : new();

    public Option<T> Or(Option<T> o) => has ? this : o;
    public Option<T> Or(Func<Option<T>> o) => has ? this : o();

    public Option<T> Xor(Option<T> o) => has && !o.has ? this : !has && o.has ? o : new();

    public Option<T> Take()
    {
        if (has)
        {
            has = false;
            return new(val);
        }
        else return new();
    }

    public Option<T> Replace(Option<T> o)
    {
        var r = this;
        this = o;
        return r;
    }

    public override bool Equals(object? obj) => obj is Option<T> option && Equals(option);

    public bool Equals(Option<T> other) =>
        has == other.has && (!has || EqualityComparer<T>.Default.Equals(val, other.val));

    public override int GetHashCode() => HashCode.Combine(has, val);

    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

    public static bool operator !=(Option<T> left, Option<T> right) => !(left == right);

    public static Option<T> operator |(Option<T> left, Option<T> right) => left.Or(right);
    public static Option<T> operator &(Option<T> left, Option<T> right) => left.And(right);
    public static Option<T> operator ^(Option<T> left, Option<T> right) => left.Xor(right);
#if NET7_0_OR_GREATER
    static Option<T> IBitwiseOperators<Option<T>, Option<T>, Option<T>>.operator ~(Option<T> _) => throw new NotImplementedException();
#endif
}

public static partial class Sugar
{
    public static Option<T> Flatten<T>(this Option<Option<T>> o) => o.has ? o.val : new();

    public static Result<Option<T>, E> Transpose<T, E>(this Option<Result<T, E>> o)
    {
        switch (o)
        {
            case { has: true, val: { IsOk: true, res: var x } }: return new() { res = new(x) };
            case { has: true, val: { IsErr: true, err: var e } }: return new() { err = new(e) };
            default: return new() { res = new() };
        }
    }

    public static Option<T> Cloned<T>(this Option<T> o) where T : IClone<T> => o.has ? new(o.val.Clone()) : new();
}

public static partial class SugarClass
{
    public static T? TryGet<T>(this Option<T> self) where T : class => self.Has ? self.Value : null;
}

public static partial class SugarStruct
{
    public static T? TryGet<T>(this Option<T> self) where T : struct => self.Has ? self.Value : null;
}
