using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LibSugar;

//public static class UResult
//{
//    public static UResult<T, E> MakeErr<T, E>(E v) where T : unmanaged where E : unmanaged
//        => UResult<T, E>.MakeErr(v);

//    public static UResult<T, E> MakeOk<T, E>(T v) where T : unmanaged where E : unmanaged
//        => UResult<T, E>.MakeOk(v);
//}

//public static class UResult<A> where A : unmanaged
//{
//    public static UResult<A, E> MakeErr<E>(E v) where E : unmanaged
//        => UResult<A, E>.MakeErr(v);

//    public static UResult<T, A> MakeOk<T>(T v) where T : unmanaged
//        => UResult<T, A>.MakeOk(v);
//}

//[Union, For("T", "E")]
//public enum UResultKind : byte
//{
//    [Of("E")]
//    Err,
//    [Of("T")]
//    Ok,
//}

//public readonly partial struct UResult<T, E>
//#if NET7_0_OR_GREATER
//    : IBitwiseOperators<UResult<T, E>, UResult<T, E>, UResult<T, E>>
//#endif
//    where T : unmanaged where E : unmanaged
//{
//    public UResult<U, E> MapOk<U>(Func<T, U> f) where U : unmanaged
//        => IsOk ? UResult<U, E>.MakeOk(f(Ok)) : UResult<U, E>.MakeErr(Err);

//    public UResult<T, U> MapErr<U>(Func<E, U> f) where U : unmanaged
//        => IsErr ? UResult<T, U>.MakeErr(f(Err)) : UResult<T, U>.MakeOk(Ok);

//    public UResult<U, E> And<U>(UResult<U, E> o) where U : unmanaged
//        => IsErr ? UResult<U, E>.MakeErr(Err) : o;
//    public UResult<U, E> And<U>(Func<UResult<U, E>> o) where U : unmanaged
//        => IsErr ? UResult<U, E>.MakeErr(Err) : o();

//    public UResult<T, E> Or(UResult<T, E> o) => IsErr ? o : this;
//    public UResult<T, E> Or(Func<UResult<T, E>> o) => IsErr ? o() : this;

//    public static UResult<T, E> operator |(UResult<T, E> left, UResult<T, E> right) => left.Or(right);
//    public static UResult<T, E> operator &(UResult<T, E> left, UResult<T, E> right) => left.And(right);

//#if NET7_0_OR_GREATER
//    static UResult<T, E> IBitwiseOperators<UResult<T, E>, UResult<T, E>, UResult<T, E>>.operator ^(UResult<T, E> left, UResult<T, E> right) => throw new NotSupportedException();
//    static UResult<T, E> IBitwiseOperators<UResult<T, E>, UResult<T, E>, UResult<T, E>>.operator ~(UResult<T, E> _) => throw new NotSupportedException();
//#endif
//}

//public static partial class Sugar
//{
//    public static UResult<T, E> Flatten<T, E>(this UResult<UResult<T, E>, E> r) where T : unmanaged where E : unmanaged
//        => r.IsErr ? UResult<T, E>.MakeErr(r.Err) : r.Ok;

//    public static Option<UResult<T, E>> Transpose<T, E>(this UResult<Option<T>, E> r) where T : unmanaged where E : unmanaged
//    {
//        switch (r)
//        {
//            case { IsOk: true, Ok: { has: true, val: var val } }: return new(UResult<T, E>.MakeOk(val));
//            case { IsOk: true, Ok.has: false }: return new();
//            default: return new(UResult<T, E>.MakeErr(r.Err));
//        }
//    }
//}
