using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LibSugar;

public struct AtomicBool32 : IEquatable<AtomicBool32>
{
    private volatile uint v;

    public AtomicBool32(bool v)
    {
        this.v = v ? 1u : 0u;
    }

    public bool Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
        get => v != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Exchange(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(AtomicBool32 self) => self.Value;

#if NETSTANDARD
#pragma warning disable CS0420
    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public bool Exchange(bool value)
        => Interlocked.Exchange(ref Unsafe.As<uint, int>(ref v), value ? 1 : 0) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public bool CompareExchange(bool value, bool comparand)
        => Interlocked.CompareExchange(ref Unsafe.As<uint, int>(ref v), value ? 1 : 0, comparand ? 1 : 0) != 0;
#pragma warning restore CS0420
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public bool Exchange(bool value)
        => Interlocked.Exchange(ref v, value ? 1u : 0u) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public bool CompareExchange(bool value, bool comparand)
        => Interlocked.CompareExchange(ref v, value ? 1u : 0u, comparand ? 1u : 0u) != 0;
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(AtomicBool32 other) => Value == other.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is AtomicBool32 other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(AtomicBool32 left, AtomicBool32 right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(AtomicBool32 left, AtomicBool32 right) => !left.Equals(right);
}
