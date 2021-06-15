﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSugar
{
    public static partial class Sugar
    {
        public static IEnumerable<T> Seq<T>(this T v)
        {
            yield return v;
        }
        public static IEnumerable<T> Repeat<T>(this T v, int count) => Enumerable.Repeat(v, count);
    }
}
