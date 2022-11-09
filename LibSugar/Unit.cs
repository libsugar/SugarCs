#if NET7_0_OR_GREATER
using System.Numerics;
#endif
using System;
using System.Runtime.CompilerServices;

namespace LibSugar;

public readonly record struct Unit
#if NET7_0_OR_GREATER
    : IEqualityOperators<Unit, Unit, bool>
#endif
{
    public static readonly Unit Instance = default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => "Unit";
}
