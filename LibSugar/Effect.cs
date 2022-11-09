using System;
using System.Runtime.CompilerServices;

namespace LibSugar;

public static partial class Sugar
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Run(this Action f) => f();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static U Run<U>(this Func<U> f) => f();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static U Let<T, U>(this T v, Func<T, U> f) => f(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LetIf<T>(this T v, bool cond, Func<T, T> f) => cond ? f(v) : v;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Also<T>(this T v, Action<T> f)
    {
        f(v);
        return v;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AlsoIf<T>(this T v, bool cond, Action<T> f)
    {
        if (cond) f(v);
        return v;
    }
}
