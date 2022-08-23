using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSugar;

public static partial class Sugar
{
    public static IEnumerable<T> Seq<T>(this T v) => v.Repeat(1);
    public static IEnumerable<T> Seq<T>(this T v, params T[] args) => args.Prepend(v);
    public static IEnumerable<T> Repeat<T>(this T v, int count) => Enumerable.Repeat(v, count);

    public static string JoinStr<T>(this IEnumerable<T> iter) => string.Join(", ", iter);
    public static string JoinStr<T>(this IEnumerable<T> iter, char separator) => string.Join(separator, iter);
    public static string JoinStr<T>(this IEnumerable<T> iter, string separator) => string.Join(separator, iter);
    public static string JoinStr(this object[] arr) => string.Join(", ", arr);
    public static string JoinStr(this object[] arr, char separator) => string.Join(separator, arr);
    public static string JoinStr(this object[] arr, string separator) => string.Join(separator, arr);
    public static string JoinStr(this string[] arr) => string.Join(", ", arr);
    public static string JoinStr(this string[] arr, char separator) => string.Join(separator, arr);
    public static string JoinStr(this string[] arr, string separator) => string.Join(separator, arr);
}
