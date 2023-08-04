using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LibSugar;

public static partial class Sugar
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> Seq<T>(this T v) => v.Repeat(1);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> Seq<T>(this T v, params T[] args) => args.Prepend(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> Repeat<T>(this T v, int count) => Enumerable.Repeat(v, count);


    public static string JoinStr<T>(this IEnumerable<T> iter) => string.Join(", ", iter);
    public static string JoinStr<T>(this IEnumerable<T> iter, char separator) => string.Join(separator, iter);
    public static string JoinStr<T>(this IEnumerable<T> iter, string separator) => string.Join(separator, iter);
    public static string JoinStr(this object[] arr) => string.Join(", ", arr);
    public static string JoinStr(this object[] arr, char separator) => string.Join(separator, arr);
    public static string JoinStr(this object[] arr, string separator) => string.Join(separator, arr);
    public static string JoinStr(this string[] arr) => string.Join(", ", arr);
    public static string JoinStr(this string[] arr, char separator) => string.Join(separator, arr);
    public static string JoinStr(this string[] arr, string separator) => string.Join(separator, arr);


    public static IEnumerable<(T a, int i)> Indexed<T>(this IEnumerable<T> iter) => iter.Select((a, b) => (a, b));

    public static ParallelQuery<(T a, int i)> Indexed<T>(this ParallelQuery<T> iter) => iter.Select((a, b) => (a, b));


    public static IEnumerable<R> WhereSelect<T, R>(this IEnumerable<T> iter, Func<T, Option<R>> f)
        => iter.Select(f).Where(a => a.Has).Select(a => a.Value);

    public static ParallelQuery<R> WhereSelect<T, R>(this ParallelQuery<T> iter, Func<T, Option<R>> f)
        => iter.Select(f).Where(a => a.Has).Select(a => a.Value);

    public static IEnumerable<R> WhereCast<T, R>(this IEnumerable<T> iter)
        => iter.Where(a => a is R).Cast<R>();

    public static IEnumerable<R> WhereCast<R>(this IEnumerable iter)
    {
        foreach (var item in iter)
        {
            if (item is R r) yield return r;
        }
    }

    public static ParallelQuery<R> WhereCast<T, R>(this ParallelQuery<T> iter)
        => iter.Where(a => a is R).Cast<R>();


    /// <summary>ForEach for <see cref="IEnumerable&lt;T&gt;"/></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForEach<T>(this IEnumerable<T> iter, Action<T> f)
    {
        foreach (var item in iter)
        {
            f(item);
        }
    }

    /// <summary>
    /// ForEach for <see cref="IEnumerable{T}"/>
    /// <para>Param <c>f</c> return <c>true</c> to continue, <c>false</c> to break</para>
    /// </summary>
    /// <param name="iter"></param>
    /// <param name="f">Return <c>true</c> to continue, <c>false</c> to break</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForEach<T>(this IEnumerable<T> iter, Func<T, bool> f)
    {
        foreach (var item in iter)
        {
            if (!f(item)) break;
        }
    }
    /// <summary>
    /// ForEach for <see cref="IEnumerable{T}"/>
    /// <para>Param <c>f</c> return <see cref="Option{R}.None"/> to continue, <see cref="Option{R}.Some"/> to break and return a <c>R</c></para>
    /// </summary>
    /// <param name="iter"></param>
    /// <param name="f">Return <see cref="Option{R}.None"/> to continue, <see cref="Option{R}.Some"/> to break and return a <c>R</c></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<R> ForEach<T, R>(this IEnumerable<T> iter, Func<T, Option<R>> f)
    {
        foreach (var item in iter)
        {
            var r = f(item);
            if (r.IsSome) return r;
        }
        return new();
    }

    #region IEnumerable ByX

#if !NET6_0_OR_GREATER

    public static IEnumerable<T> DistinctBy<T, D>(this IEnumerable<T> iter, Func<T, D> f)
        => iter.Distinct(EqBy(f));

    public static IEnumerable<T> DistinctBy<T, D>(this IEnumerable<T> iter, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.Distinct(EqBy(f, equality));

    public static IEnumerable<T> DistinctBy<T, D>(this IEnumerable<T> iter, Func<T, D> f, IEqualityComparer<D> equality)
        => iter.Distinct(EqBy(f, equality));


    public static IEnumerable<T> UnionBy<T, D>(this IEnumerable<T> iter, IEnumerable<T> other, Func<T, D> f)
        => iter.Union(other, EqBy(f));

    public static IEnumerable<T> UnionBy<T, D>(this IEnumerable<T> iter, IEnumerable<T> other, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.Union(other, EqBy(f, equality));

    public static IEnumerable<T> UnionBy<T, D>(this IEnumerable<T> iter, IEnumerable<T> other, Func<T, D> f, IEqualityComparer<D> equality)
        => iter.Union(other, EqBy(f, equality));

#else

    public static IEnumerable<T> DistinctBy<T, D>(this IEnumerable<T> iter, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.DistinctBy(f, equality);


    public static IEnumerable<T> ExceptBy<T, D>(this IEnumerable<T> iter, IEnumerable<D> other, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.ExceptBy(other, f, equality);


    public static IEnumerable<T> IntersectBy<T, D>(this IEnumerable<T> iter, IEnumerable<D> other, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.IntersectBy(other, f, equality);


    public static IEnumerable<T> UnionBy<T, D>(this IEnumerable<T> iter, IEnumerable<T> other, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.UnionBy(other, f, equality);

#endif

    public static IEnumerable<T> ExceptBy<T, D>(this IEnumerable<T> iter, IEnumerable<T> other, Func<T, D> f)
        => iter.Except(other, EqBy(f));

    public static IEnumerable<T> ExceptBy<T, D>(this IEnumerable<T> iter, IEnumerable<T> other, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.Except(other, EqBy(f, equality));

    public static IEnumerable<T> ExceptBy<T, D>(this IEnumerable<T> iter, IEnumerable<T> other, Func<T, D> f, IEqualityComparer<D> equality)
        => iter.Except(other, EqBy(f, equality));


    public static IEnumerable<T> IntersectBy<T, D>(this IEnumerable<T> iter, IEnumerable<T> other, Func<T, D> f)
        => iter.Intersect(other, EqBy(f));

    public static IEnumerable<T> IntersectBy<T, D>(this IEnumerable<T> iter, IEnumerable<T> other, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.Intersect(other, EqBy(f, equality));

    public static IEnumerable<T> IntersectBy<T, D>(this IEnumerable<T> iter, IEnumerable<T> other, Func<T, D> f, IEqualityComparer<D> equality)
        => iter.Intersect(other, EqBy(f, equality));

    #endregion


    #region ParallelQuery ByX

    public static ParallelQuery<T> DistinctBy<T, D>(this ParallelQuery<T> iter, Func<T, D> f)
        => iter.Distinct(EqBy(f));

    public static ParallelQuery<T> DistinctBy<T, D>(this ParallelQuery<T> iter, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.Distinct(EqBy(f, equality));

    public static ParallelQuery<T> DistinctBy<T, D>(this ParallelQuery<T> iter, Func<T, D> f, IEqualityComparer<D> equality)
        => iter.Distinct(EqBy(f, equality));


    public static ParallelQuery<T> ExceptBy<T, D>(this ParallelQuery<T> iter, ParallelQuery<T> other, Func<T, D> f)
        => iter.Except(other, EqBy(f));

    public static ParallelQuery<T> ExceptBy<T, D>(this ParallelQuery<T> iter, ParallelQuery<T> other, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.Except(other, EqBy(f, equality));

    public static ParallelQuery<T> ExceptBy<T, D>(this ParallelQuery<T> iter, ParallelQuery<T> other, Func<T, D> f, IEqualityComparer<D> equality)
        => iter.Except(other, EqBy(f, equality));


    public static ParallelQuery<T> IntersectBy<T, D>(this ParallelQuery<T> iter, ParallelQuery<T> other, Func<T, D> f)
        => iter.Intersect(other, EqBy(f));

    public static ParallelQuery<T> IntersectBy<T, D>(this ParallelQuery<T> iter, ParallelQuery<T> other, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.Intersect(other, EqBy(f, equality));

    public static ParallelQuery<T> IntersectBy<T, D>(this ParallelQuery<T> iter, ParallelQuery<T> other, Func<T, D> f, IEqualityComparer<D> equality)
        => iter.Intersect(other, EqBy(f, equality));


    public static ParallelQuery<T> UnionBy<T, D>(this ParallelQuery<T> iter, ParallelQuery<T> other, Func<T, D> f)
        => iter.Union(other, EqBy(f));

    public static ParallelQuery<T> UnionBy<T, D>(this ParallelQuery<T> iter, ParallelQuery<T> other, IEqualityComparer<D> equality, Func<T, D> f)
        => iter.Union(other, EqBy(f, equality));

    public static ParallelQuery<T> UnionBy<T, D>(this ParallelQuery<T> iter, ParallelQuery<T> other, Func<T, D> f, IEqualityComparer<D> equality)
        => iter.Union(other, EqBy(f, equality));
    
    #endregion

}
