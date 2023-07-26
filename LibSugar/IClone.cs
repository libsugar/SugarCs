using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LibSugar;

public interface IClone<out T>
{
    public T Clone();
}

public static partial class Sugar
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> SeqCloned<T>(this IEnumerable<T> self) where T : IClone<T>
        => self.Select(a => a.Clone());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<R> SeqCloned<T, R>(this IEnumerable<T> self) where T : IClone<R>
        => self.Select(a => a.Clone());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Cloned<T>(this T[] self) where T : IClone<T>
        => self.Select(a => a.Clone()).ToArray();
}

