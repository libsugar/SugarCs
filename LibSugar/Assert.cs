using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

namespace LibSugar;

public static partial class Sugar
{
    public static bool AssertTrue(this bool v) => v ? v : throw new AssertException($"Expect true but actually is {v}");
    public static bool AssertFalse(this bool v) => v ? v : throw new AssertException($"Expect false but actually is {v}");

    public static T AssertEq<T>(this T self, T other) => EqualityComparer<T>.Default.Equals(self, other) ? self : throw new AssertException($"Expect {other} but actually is {self}");
    public static T AssertNe<T>(this T self, T other) => !EqualityComparer<T>.Default.Equals(self, other) ? self : throw new AssertException($"Expect not {other} but actually is {self}");

    public static T AssertSame<T>(this T self, T other) where T : class => ReferenceEquals(self, other) ? self : throw new AssertException($"Expect {other} but actually is {self}");
    public static T AssertNotSame<T>(this T self, T other) where T : class => !ReferenceEquals(self, other) ? self : throw new AssertException($"Expect not {other} but actually is {self}");

#if NET7_0_OR_GREATER
    public static T AssertZero<T>(this T v) where T : INumberBase<T> => v == T.Zero ? v : throw new AssertException($"Expect 0 but actually is {v}");
#endif

    public static byte AssertZero(this byte v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static ushort AssertZero(this ushort v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static uint AssertZero(this uint v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static ulong AssertZero(this ulong v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static nuint AssertZero(this nuint v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static sbyte AssertZero(this sbyte v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static short AssertZero(this short v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static int AssertZero(this int v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static long AssertZero(this long v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static nint AssertZero(this nint v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
#if !UNITY
    public static Half AssertZero(this Half v) => v == (Half)0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
#endif
    public static float AssertZero(this float v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static double AssertZero(this double v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static decimal AssertZero(this decimal v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");
    public static BigInteger AssertZero(this BigInteger v) => v == 0 ? v : throw new AssertException($"Expect 0 but actually is {v}");

#if NET7_0_OR_GREATER
    public static T AssertNotNaN<T>(this T v) where T : INumberBase<T> => !T.IsNaN(v) ? v : throw new AssertException($"Expect not NaN but actually is {v}");
#endif
#if !UNITY
    public static Half AssertNotNaN(this Half v) => !Half.IsNaN(v) ? v : throw new AssertException($"Expect not NaN but actually is {v}");
#endif
    public static float AssertNotNaN(this float v) => !float.IsNaN(v) ? v : throw new AssertException($"Expect not NaN but actually is {v}");
    public static double AssertNotNaN(this double v) => !double.IsNaN(v) ? v : throw new AssertException($"Expect not NaN but actually is {v}");

#if NET7_0_OR_GREATER
    public static T AssertNaN<T>(this T v) where T : INumberBase<T> => T.IsNaN(v) ? v : throw new AssertException($"Expect NaN but actually is {v}");
#endif
#if !UNITY
    public static Half AssertNaN(this Half v) => Half.IsNaN(v) ? v : throw new AssertException($"Expect NaN but actually is {v}");
#endif
    public static float AssertNaN(this float v) => float.IsNaN(v) ? v : throw new AssertException($"Expect NaN but actually is {v}");
    public static double AssertNaN(this double v) => double.IsNaN(v) ? v : throw new AssertException($"Expect NaN but actually is {v}");

}

public static partial class SugarClass
{
    public static T? AssertNull<T>(this T? v) where T : class => v == null ? v : throw new AssertException($"Expect null but actually is {v}");
    public static T AssertNonNull<T>(this T? v) where T : class => v ?? throw new AssertException($"Expect non null but actually is {v}");
}

public static partial class SugarStruct
{
    public static T? AssertNull<T>(this T? v) where T : struct => !v.HasValue ? v : throw new AssertException($"Expect null but actually is {v}");
    public static T AssertNonNull<T>(this T? v) where T : struct => v ?? throw new AssertException($"Expect non null but actually is {v}");
}

[Serializable]
public class AssertException : Exception
{
    public AssertException() { }
    public AssertException(string message) : base(message) { }
    public AssertException(string message, Exception inner) : base(message, inner) { }
    protected AssertException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
