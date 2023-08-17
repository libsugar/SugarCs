using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LibSugar;

/// <summary>Movable ownership</summary>
public interface IMovable<out T>
{
    /// <summary>Has been moved</summary>
    public bool IsMoved { get; }

    /// <summary>Move, take ownership, original value will skip destruction</summary>
    public T Move();
}

public struct Moved : IMovable<bool>
{
    private AtomicBool32 isMoved;
    public bool IsMoved
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => isMoved;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Moved(bool isMoved)
    {
        this.isMoved = new(isMoved);
    }

    /// <returns>Return was moved</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Move() => isMoved.Exchange(true);
}

public struct Movable<T> : IMovable<Movable<T>>, IDisposable where T : IDisposable
{
    public Movable(T value)
    {
        Value = value;
    }

    internal Movable(T value, Moved moved)
    {
        Value = value;
        this.moved = moved;
    }

    public T Value;
    private Moved moved;

    public bool IsMoved => moved.IsMoved;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public Movable<T> Move()
    {
        if (moved.Move()) return new Movable<T>(default!, new Moved(true));
        return new Movable<T>(Value, new Moved(false));
    }

    public void Dispose()
    {
        if (moved.Move()) return;
        Value.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static implicit operator T(Movable<T> self) => self.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static implicit operator Movable<T>(T value) => new(value);
}

public static partial class Sugar
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Movable<T> Movable<T>(this T value) where T : IDisposable => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Owner<Movable<T>> Movable<T>(this Owner<T> value) where T : IDisposable => new(new(value.Value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MovableBy<T> MovableBy<T>(this T value, MovableBy<T>.Drop drop) => new(value, drop);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MovableByUnmanaged<T> MovableByUnmanaged<T>(this T value, MovableByUnmanaged<T>.Drop drop)
        where T : unmanaged => new(value, drop);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> Move<T>(this IEnumerable<T> self) where T : IMovable<T>
        => self.Select(a => a.Move());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<R> Move<T, R>(this IEnumerable<T> self) where T : IMovable<R>
        => self.Select(a => a.Move());
}

public static partial class Sugar
{
    /// <summary>Exchange</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static T Swap<T>(ref T self, T newValue)
    {
        if (!typeof(T).IsValueType)
        {
            var a = Interlocked.Exchange(ref Unsafe.As<T, object?>(ref self), newValue);
            return Unsafe.As<object?, T>(ref a);
        }
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            var a = Interlocked.Exchange(ref Unsafe.As<T, int>(ref self), Unsafe.As<T, int>(ref newValue));
            return Unsafe.As<int, T>(ref a);
        }
        else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
        {
            var a = Interlocked.Exchange(ref Unsafe.As<T, long>(ref self), Unsafe.As<T, long>(ref newValue));
            return Unsafe.As<long, T>(ref a);
        }
        else if (typeof(T) == typeof(float))
        {
            var a = Interlocked.Exchange(ref Unsafe.As<T, float>(ref self), Unsafe.As<T, float>(ref newValue));
            return Unsafe.As<float, T>(ref a);
        }
        else if (typeof(T) == typeof(double))
        {
            var a = Interlocked.Exchange(ref Unsafe.As<T, double>(ref self), Unsafe.As<T, double>(ref newValue));
            return Unsafe.As<double, T>(ref a);
        }
        else if (typeof(T) == typeof(IntPtr) || typeof(T) == typeof(UIntPtr))
        {
            var a = Interlocked.Exchange(ref Unsafe.As<T, IntPtr>(ref self), Unsafe.As<T, IntPtr>(ref newValue));
            return Unsafe.As<IntPtr, T>(ref a);
        }
        else
        {
            var tmp = self;
            self = newValue;
            return tmp;
        }
    }
}

public static partial class SugarStruct
{
    public static T Swap<T>(this ref T self, T newValue) where T : struct
    {
        var tmp = self;
        self = newValue;
        return tmp;
    }

    public static T Swap<T>(this ref T self, in T newValue) where T : struct
    {
        var tmp = self;
        self = newValue;
        return tmp;
    }
}

public struct MovableBy<T> : IMovable<MovableBy<T>>, IDisposable
{
    public delegate void Drop(in T val);

    private readonly Drop drop;

    public MovableBy(T value, Drop drop)
    {
        Value = value;
        this.drop = drop;
    }

    internal MovableBy(T value, Moved moved, Drop drop)
    {
        Value = value;
        this.moved = moved;
        this.drop = drop;
    }

    public T Value;
    private Moved moved;

    public bool IsMoved => moved.IsMoved;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public MovableBy<T> Move()
    {
        if (moved.Move()) return new MovableBy<T>(default!, new Moved(true), drop);
        return new MovableBy<T>(Value, new Moved(false), drop);
    }

    public void Dispose()
    {
        if (moved.Move()) return;
        drop(in Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static implicit operator T(MovableBy<T> self) => self.Value;
}

public unsafe struct MovableByUnmanaged<T> : IMovable<MovableByUnmanaged<T>>, IDisposable where T : unmanaged
{
    public delegate void Drop(T* val);

    private readonly Drop drop;

    public MovableByUnmanaged(T value, Drop drop)
    {
        Value = value;
        this.drop = drop;
    }

    internal MovableByUnmanaged(T value, Moved moved, Drop drop)
    {
        Value = value;
        this.moved = moved;
        this.drop = drop;
    }

    public T Value;
    private Moved moved;

    public bool IsMoved => moved.IsMoved;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public MovableByUnmanaged<T> Move()
    {
        if (moved.Move()) return new MovableByUnmanaged<T>(default!, new Moved(true), drop);
        return new MovableByUnmanaged<T>(Value, new Moved(false), drop);
    }

    public void Dispose()
    {
        if (moved.Move()) return;
        fixed (T* ptr = &Value)
        {
            drop(ptr);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static implicit operator T(MovableByUnmanaged<T> self) => self.Value;
}

public class MovedException : Exception
{
    public MovedException() { }
    public MovedException(string message) : base(message) { }
    public MovedException(string message, Exception inner) : base(message, inner) { }
}
