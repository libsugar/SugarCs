using System;
using System.Collections.Generic;

namespace LibSugar;

public struct Result<T, E> : IEquatable<Result<T, E>>
{
    public static Result<T, E> Ok(T val) => new() { res = val };
    public static Result<T, E> Err(E err) => new() { err = new(err) };

    internal T res;
    internal Box<E> err;
    public bool IsOk => err == null;
    public bool IsErr => err != null;

    public T Value => res;
    public E Error => err;

    public Result<U, E> MapOk<U>(Func<T, U> f) => IsOk ? new() { res = f(res) } : new() { err = err };

    public Result<T, U> MapErr<U>(Func<T, U> f) => IsErr ? new() { err = new(f(res)) } : new() { res = res };

    public Result<U, E> And<U>(Result<U, E> o) => IsErr ? new() { err = err } : o;
    public Result<U, E> And<U>(Func<Result<U, E>> o) => IsErr ? new() { err = err } : o();

    public Result<T, E> Or(Result<T, E> o) => IsErr ? o : this;
    public Result<T, E> Or(Func<Result<T, E>> o) => IsErr ? o() : this;

    public override bool Equals(object obj) => obj is Result<T, E> result && Equals(result);

    public bool Equals(Result<T, E> other) => err == null && other.err == null ? EqualityComparer<T>.Default.Equals(res, other.res) : err != null && other.err != null ? EqualityComparer<Box<E>>.Default.Equals(err, other.err) : false;

    public override int GetHashCode() => HashCode.Combine(res, err);

    public static bool operator ==(Result<T, E> left, Result<T, E> right) => left.Equals(right);

    public static bool operator !=(Result<T, E> left, Result<T, E> right) => !(left == right);

    public static Result<T, E> operator |(Result<T, E> left, Result<T, E> right) => left.Or(right);
    public static Result<T, E> operator &(Result<T, E> left, Result<T, E> right) => left.And(right);
}

public static partial class Sugar
{
    public static Result<T, E> Flatten<T, E>(this Result<Result<T, E>, E> r) => r.IsErr ? new() { err = r.err } : r.res;

    public static Option<Result<T, E>> Transpose<T, E>(this Result<Option<T>, E> r)
    {
        switch (r)
        {
            case { IsOk: true, res: { has: true, val: var x } }: return new(new() { res = x });
            case { IsOk: true, res: { has: false } }: return new();
            default: return new(new() { err = r.err });
        }
    }
}
