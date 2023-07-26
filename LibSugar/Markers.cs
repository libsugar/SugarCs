using System;
using System.Runtime.CompilerServices;

namespace LibSugar;

public record struct Borrow<T>(T Value) : IBox<T>;

public record struct Owner<T>(T Value) : IBox<T>
{
    public static implicit operator Borrow<T>(Owner<T> s) => new(s.Value);
}

public static partial class Sugar
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Borrow<T> Borrow<T>(this T self) => new(self);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Owner<T> Owner<T>(this T self) => new(self);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Owner<T> Owner<T>(this Movable<T> self) where T : IDisposable => new(self);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Owner<T> Clone<T>(this Borrow<T> self) where T : IClone<T> => new(self.Value.Clone());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Owner<T> Clone<T>(this Owner<T> self) where T : IClone<T> => new(self.Value.Clone());


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Owner<T> Move<T>(this Owner<T> self) where T : IMovable<T> => new(self.Value.Move());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Owner<Movable<T>> Move<T>(this Owner<Movable<T>> self) where T : IDisposable =>
        new(self.Value.Move());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Owner<T> Unwrap<T>(this Owner<Movable<T>> s) where T : IDisposable => new(s.Value);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Dispose<T>(this Borrow<T> self) where T : IDisposable => self.Value.Dispose();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Dispose<T>(this Owner<T> self) where T : IDisposable => self.Value.Dispose();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Borrow<R> Map<T, R>(this Owner<T> self, Func<T, R> map) => new(map(self.Value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Borrow<R> Map<T, R>(this Borrow<T> self, Func<T, R> map) => new(map(self.Value));
}
