using System;
using System.Collections.Generic;
using System.Text;

namespace LibSugar;

public static partial class Sugar
{
    public static ReadOnlySpan<T> ToReadOnly<T>(this Span<T> self) => self;

    public static ReadOnlyMemory<T> ToReadOnly<T>(this Memory<T> self) => self;
}
