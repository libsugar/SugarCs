#if NETSTANDARD && !UNITY
using NInt.MinMaxValue;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibSugar;

/// <summary>
/// Math Ex
/// </summary>
public static partial class LibMath
{
    #region Consts

    /// <summary>
    /// <c>e</c>
    /// </summary>
    public const double E = 2.7182818284590452353602874713526624977572470936999595749669676277;
    /// <summary>
    /// <c>π</c>
    /// </summary>
    public const double PI = 3.1415926535897932384626433832795028841971693993751058209749445923;
    /// <summary>
    /// <c>2π</c>
    /// </summary>
    public const double PI2 = 6.2831853071795864769252867665590057683943387987502116419498891846;
    /// <summary>
    /// <c>180 ÷ π</c>
    /// </summary>
    public const double DEG_PER_RAD = 57.295779513082320876798154814105170332405472466564321549160243861;
    /// <summary>
    /// <c>π ÷ 180</c>
    /// </summary>
    public const double RAD_PER_DEG = 0.0174532925199432957692369076848861271344287188854172545609719144;
    /// <summary>
    /// <c>√1</c>
    /// </summary>
    public const double Sqrt1 = 1;
    /// <summary>
    /// <c>√2</c>
    /// </summary>
    public const double Sqrt2 = 1.4142135623730950488016887242096980785696718753769480731766797379;
    /// <summary>
    /// <c>√3</c>
    /// </summary>
    public const double Sqrt3 = 1.7320508075688772935274463415058723669428052538103806280558069794;
    /// <summary>
    /// <c>√4</c>
    /// </summary>
    public const double Sqrt4 = 2;
    /// <summary>
    /// <c>√5</c>
    /// </summary>
    public const double Sqrt5 = 2.2360679774997896964091736687312762354406183596115257242708972454;
    /// <summary>
    /// <c>√6</c>
    /// </summary>
    public const double Sqrt6 = 2.4494897427831780981972840747058913919659474806566701284326925672;
    /// <summary>
    /// <c>√7</c>
    /// </summary>
    public const double Sqrt7 = 2.6457513110645905905016157536392604257102591830824501803683344592;
    /// <summary>
    /// <c>√8</c>
    /// </summary>
    public const double Sqrt8 = 2.8284271247461900976033774484193961571393437507538961463533594759;
    /// <summary>
    /// <c>√9</c>
    /// </summary>
    public const double Sqrt9 = 3;
    /// <summary>
    /// <c>√10</c>
    /// </summary>
    public const double Sqrt10 = 3.1622776601683793319988935444327185337195551393252168268575048527;
    /// <summary>
    /// <c>ϕ | φ</c>
    /// </summary>
    public const double GoldenRatio = 1.6180339887498948482045868343656381177203091798057628621354486227;
    /// <summary>
    /// <c>K</c>
    /// </summary>
    public const double Khinchin = 2.6854520010653064453097148354817956938203822939944629530511523455;
    /// <summary>
    /// <c>λ</c>
    /// </summary>
    public const double Conway = 1.3035772690342963912570991121525518907307025046594048757548613906;
    /// <summary>
    /// <c>C10</c>
    /// </summary>
    public const double Champernowne = 0.1234567891011121314151617181920212223242526272829303132333435363;
    /// <summary>
    /// <c>γ</c>
    /// </summary>
    public const double Euler = 0.5772156649015328606065120900824024310421593359399235988057672348;

    #endregion

    #region Float Consts

    /// <summary>
    /// <c>e</c>
    /// </summary>
    public const float E_F = 2.7182818284590452353602874713526624977572470936999595749669676277f;
    /// <summary>
    /// <c>π</c>
    /// </summary>
    public const float PI_F = 3.1415926535897932384626433832795028841971693993751058209749445923f;
    /// <summary>
    /// <c>2π</c>
    /// </summary>
    public const float PI2_F = 6.2831853071795864769252867665590057683943387987502116419498891846f;
    /// <summary>
    /// <c>180 ÷ π</c>
    /// </summary>
    public const float DEG_PER_RAD_F = 57.295779513082320876798154814105170332405472466564321549160243861f;
    /// <summary>
    /// <c>π ÷ 180</c>
    /// </summary>
    public const float RAD_PER_DEG_F = 0.0174532925199432957692369076848861271344287188854172545609719144f;
    /// <summary>
    /// <c>√1</c>
    /// </summary>
    public const float Sqrt1_F = 1;
    /// <summary>
    /// <c>√2</c>
    /// </summary>
    public const float Sqrt2_F = 1.4142135623730950488016887242096980785696718753769480731766797379f;
    /// <summary>
    /// <c>√3</c>
    /// </summary>
    public const float Sqrt3_F = 1.7320508075688772935274463415058723669428052538103806280558069794f;
    /// <summary>
    /// <c>√4</c>
    /// </summary>
    public const float Sqrt4_F = 2;
    /// <summary>
    /// <c>√5</c>
    /// </summary>
    public const float Sqrt5_F = 2.2360679774997896964091736687312762354406183596115257242708972454f;
    /// <summary>
    /// <c>√6</c>
    /// </summary>
    public const float Sqrt6_F = 2.4494897427831780981972840747058913919659474806566701284326925672f;
    /// <summary>
    /// <c>√7</c>
    /// </summary>
    public const float Sqrt7_F = 2.6457513110645905905016157536392604257102591830824501803683344592f;
    /// <summary>
    /// <c>√8</c>
    /// </summary>
    public const float Sqrt8_F = 2.8284271247461900976033774484193961571393437507538961463533594759f;
    /// <summary>
    /// <c>√9</c>
    /// </summary>
    public const float Sqrt9_F = 3;
    /// <summary>
    /// <c>√10</c>
    /// </summary>
    public const float Sqrt10_F = 3.1622776601683793319988935444327185337195551393252168268575048527f;
    /// <summary>
    /// <c>ϕ | φ</c>
    /// </summary>
    public const float GoldenRatio_F = 1.6180339887498948482045868343656381177203091798057628621354486227f;
    /// <summary>
    /// <c>K</c>
    /// </summary>
    public const float Khinchin_F = 2.6854520010653064453097148354817956938203822939944629530511523455f;
    /// <summary>
    /// <c>λ</c>
    /// </summary>
    public const float Conway_F = 1.3035772690342963912570991121525518907307025046594048757548613906f;
    /// <summary>
    /// <c>C10</c>
    /// </summary>
    public const float Champernowne_F = 0.1234567891011121314151617181920212223242526272829303132333435363f;
    /// <summary>
    /// <c>γ</c>
    /// </summary>
    public const float Euler_F = 0.5772156649015328606065120900824024310421593359399235988057672348f;

    #endregion

    #region Decimal Consts

    /// <summary>
    /// <c>e</c>
    /// </summary>
    public const decimal E_M = 2.7182818284590452353602874713526624977572470936999595749669676277m;
    /// <summary>
    /// <c>π</c>
    /// </summary>
    public const decimal PI_M = 3.1415926535897932384626433832795028841971693993751058209749445923m;
    /// <summary>
    /// <c>2π</c>
    /// </summary>
    public const decimal PI2_M = 6.2831853071795864769252867665590057683943387987502116419498891846m;
    /// <summary>
    /// <c>180 ÷ π</c>
    /// </summary>
    public const decimal DEG_PER_RAD_M = 57.295779513082320876798154814105170332405472466564321549160243861m;
    /// <summary>
    /// <c>π ÷ 180</c>
    /// </summary>
    public const decimal RAD_PER_DEG_M = 0.0174532925199432957692369076848861271344287188854172545609719144m;
    /// <summary>
    /// <c>√1</c>
    /// </summary>
    public const decimal Sqrt1_M = 1;
    /// <summary>
    /// <c>√2</c>
    /// </summary>
    public const decimal Sqrt2_M = 1.4142135623730950488016887242096980785696718753769480731766797379m;
    /// <summary>
    /// <c>√3</c>
    /// </summary>
    public const decimal Sqrt3_M = 1.7320508075688772935274463415058723669428052538103806280558069794m;
    /// <summary>
    /// <c>√4</c>
    /// </summary>
    public const decimal Sqrt4_M = 2;
    /// <summary>
    /// <c>√5</c>
    /// </summary>
    public const decimal Sqrt5_M = 2.2360679774997896964091736687312762354406183596115257242708972454m;
    /// <summary>
    /// <c>√6</c>
    /// </summary>
    public const decimal Sqrt6_M = 2.4494897427831780981972840747058913919659474806566701284326925672m;
    /// <summary>
    /// <c>√7</c>
    /// </summary>
    public const decimal Sqrt7_M = 2.6457513110645905905016157536392604257102591830824501803683344592m;
    /// <summary>
    /// <c>√8</c>
    /// </summary>
    public const decimal Sqrt8_M = 2.8284271247461900976033774484193961571393437507538961463533594759m;
    /// <summary>
    /// <c>√9</c>
    /// </summary>
    public const decimal Sqrt9_M = 3;
    /// <summary>
    /// <c>√10</c>
    /// </summary>
    public const decimal Sqrt10_M = 3.1622776601683793319988935444327185337195551393252168268575048527m;
    /// <summary>
    /// <c>ϕ | φ</c>
    /// </summary>
    public const decimal GoldenRatio_M = 1.6180339887498948482045868343656381177203091798057628621354486227m;
    /// <summary>
    /// <c>K</c>
    /// </summary>
    public const decimal Khinchin_M = 2.6854520010653064453097148354817956938203822939944629530511523455m;
    /// <summary>
    /// <c>λ</c>
    /// </summary>
    public const decimal Conway_M = 1.3035772690342963912570991121525518907307025046594048757548613906m;
    /// <summary>
    /// <c>C10</c>
    /// </summary>
    public const decimal Champernowne_M = 0.1234567891011121314151617181920212223242526272829303132333435363m;
    /// <summary>
    /// <c>γ</c>
    /// </summary>
    public const decimal Euler_M = 0.5772156649015328606065120900824024310421593359399235988057672348m;

    #endregion

    #region MinMax

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this T a, T b) where T : INumber<T> => T.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this T a, T b) where T : INumber<T> => T.Max(a, b);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Min(this sbyte a, sbyte b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Max(this sbyte a, sbyte b) => Math.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Max(this sbyte a, params sbyte[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Max(params sbyte[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Min(this sbyte a, params sbyte[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Min(params sbyte[] args) => args.Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Min(this byte a, byte b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Max(this byte a, byte b) => Math.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Max(this byte a, params byte[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Max(params byte[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Min(this byte a, params byte[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Min(params byte[] args) => args.Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Min(this short a, short b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Max(this short a, short b) => Math.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Max(this short a, params short[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Max(params short[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Min(this short a, params short[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Min(params short[] args) => args.Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Min(this ushort a, ushort b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Max(this ushort a, ushort b) => Math.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Max(this ushort a, params ushort[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Max(params ushort[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Min(this ushort a, params ushort[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Min(params ushort[] args) => args.Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(this int a, int b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(this int a, int b) => Math.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(this int a, params int[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(params int[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(this int a, params int[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(params int[] args) => args.Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Min(this uint a, uint b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Max(this uint a, uint b) => Math.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Max(this uint a, params uint[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Max(params uint[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Min(this uint a, params uint[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Min(params uint[] args) => args.Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Min(this long a, long b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Max(this long a, long b) => Math.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Max(this long a, params long[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Max(params long[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Min(this long a, params long[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Min(params long[] args) => args.Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Min(this ulong a, ulong b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Max(this ulong a, ulong b) => Math.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Max(this ulong a, params ulong[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Max(params ulong[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Min(this ulong a, params ulong[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Min(params ulong[] args) => args.Min();

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Min(this nint a, nint b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Max(this nint a, nint b) => Math.Max(a, b);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Min(this nint a, nint b) => a <= b ? a : b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Max(this nint a, nint b) => a >= b ? a : b;
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Max(this nint a, params nint[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Max(params nint[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Min(this nint a, params nint[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Min(params nint[] args) => args.Min();

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Min(this nuint a, nuint b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Max(this nuint a, nuint b) => Math.Max(a, b);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Min(this nuint a, nuint b) => a <= b ? a : b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Max(this nuint a, nuint b) => a >= b ? a : b;
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Max(this nuint a, params nuint[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Max(params nuint[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Min(this nuint a, params nuint[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Min(params nuint[] args) => args.Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Min(this BigInteger a, BigInteger b) => BigInteger.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Max(this BigInteger a, BigInteger b) => BigInteger.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Max(this BigInteger a, params BigInteger[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Max(params BigInteger[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Min(this BigInteger a, params BigInteger[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Min(params BigInteger[] args) => args.Min();

#if !UNITY

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Min(this Half a, Half b) => Half.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Max(this Half a, Half b) => Half.Min(a, b);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Min(this Half a, Half b) => a < b || Half.IsNaN(a) ? a : b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Max(this Half a, Half b) => a > b || Half.IsNaN(a) ? a : b;
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Max(this Half a, params Half[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Max(params Half[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Min(this Half a, params Half[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Min(params Half[] args) => args.Min();

#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(this float a, float b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(this float a, float b) => Math.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(this float a, params float[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(params float[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(this float a, params float[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(params float[] args) => args.Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Min(this double a, double b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Max(this double a, double b) => Math.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Max(this double a, params double[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Max(params double[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Min(this double a, params double[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Min(params double[] args) => args.Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Min(this decimal a, decimal b) => Math.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Max(this decimal a, decimal b) => Math.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Max(this decimal a, params decimal[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Max(params decimal[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Min(this decimal a, params decimal[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Min(params decimal[] args) => args.Min();

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Min(this NFloat a, NFloat b) => NFloat.Min(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Max(this NFloat a, NFloat b) => NFloat.Max(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Max(this NFloat a, params NFloat[] args) => args.Prepend(a).Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Max(params NFloat[] args) => args.Max();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Min(this NFloat a, params NFloat[] args) => args.Prepend(a).Min();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Min(params NFloat[] args) => args.Min();
#endif

    #endregion

    #region Abs

#if !UNITY

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Abs<T>(this T v) where T : INumberBase<T> => T.Abs(v);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Abs(this sbyte v) => Math.Abs(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Abs(this byte v) => v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Abs(this short v) => Math.Abs(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Abs(this ushort v) => v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Abs(this int v) => Math.Abs(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Abs(this uint v) => v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Abs(this long v) => Math.Abs(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Abs(this ulong v) => v;

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Abs(this nint v) => Math.Abs(v);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Abs(this nint v) => v >= 0 ? v : AbsHelper(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nint AbsHelper(nint v)
    {
        Contract.Requires(v < 0, "AbsHelper should only be called for negative values! (hack for JIT inlining)");
#if NETSTANDARD
        if (v == NIntMinMaxValue.MinValue) return Math.Abs(int.MinValue); // throw Overflow
#else
        if (v == nint.MinValue) return Math.Abs(int.MinValue); // throw Overflow
#endif
        Contract.EndContractBlock();
        return -v;
    }
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Abs(this nuint v) => v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Abs(this BigInteger v) => BigInteger.Abs(v);
#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Abs(this Half v) => Half.Abs(v);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Abs(this Half v) => v >= (Half)0 ? v : (Half)(-(float)v);
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Abs(this float v) => Math.Abs(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Abs(this double v) => Math.Abs(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Abs(this decimal v) => Math.Abs(v);
#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Abs(this NFloat v) => NFloat.Abs(v);
#endif

#endif

    #endregion

    #region Remap

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Remap<T>(this T v, T low1, T high1, T low2, T high2)
        where T : IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>, IMultiplyOperators<T, T, T>,
        IDivisionOperators<T, T, T>
        => low2 + (v - low1) * (high2 - low2) / (high1 - low1);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Remap(this sbyte v, sbyte low1, sbyte high1, sbyte low2, sbyte high2) =>
        (sbyte)(low2 + (v - low1) * (high2 - low2) / (high1 - low1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Remap(this byte v, byte low1, byte high1, byte low2, byte high2) =>
        (byte)(low2 + (v - low1) * (high2 - low2) / (high1 - low1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Remap(this short v, short low1, short high1, short low2, short high2) =>
        (short)(low2 + (v - low1) * (high2 - low2) / (high1 - low1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Remap(this ushort v, ushort low1, ushort high1, ushort low2, ushort high2) =>
        (ushort)(low2 + (v - low1) * (high2 - low2) / (high1 - low1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Remap(this int v, int low1, int high1, int low2, int high2) =>
        low2 + (v - low1) * (high2 - low2) / (high1 - low1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Remap(this uint v, uint low1, uint high1, uint low2, uint high2) =>
        low2 + (v - low1) * (high2 - low2) / (high1 - low1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Remap(this long v, long low1, long high1, long low2, long high2) =>
        low2 + (v - low1) * (high2 - low2) / (high1 - low1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Remap(this ulong v, ulong low1, ulong high1, ulong low2, ulong high2) =>
        low2 + (v - low1) * (high2 - low2) / (high1 - low1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Remap(this nint v, nint low1, nint high1, nint low2, nint high2) =>
        low2 + (v - low1) * (high2 - low2) / (high1 - low1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Remap(this nuint v, nuint low1, nuint high1, nuint low2, nuint high2) =>
        low2 + (v - low1) * (high2 - low2) / (high1 - low1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Remap(this BigInteger v, BigInteger low1, BigInteger high1, BigInteger low2,
        BigInteger high2) => low2 + (v - low1) * (high2 - low2) / (high1 - low1);
#if !UNITY
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Remap(this Half v, Half low1, Half high1, Half low2, Half high2) =>
        (Half)Remap((float)v, (float)low1, (float)high1, (float)low2, (float)high2);
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Remap(this float v, float low1, float high1, float low2, float high2) =>
        low2 + (v - low1) * (high2 - low2) / (high1 - low1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Remap(this double v, double low1, double high1, double low2, double high2) =>
        low2 + (v - low1) * (high2 - low2) / (high1 - low1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Remap(this decimal v, decimal low1, decimal high1, decimal low2, decimal high2) =>
        low2 + (v - low1) * (high2 - low2) / (high1 - low1);

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Remap(this NFloat v, NFloat low1, NFloat high1, NFloat low2, NFloat high2) =>
        low2 + (v - low1) * (high2 - low2) / (high1 - low1);
#endif

    #endregion

    #region Clamp

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Clamp<T>(this T v, T min, T max) where T : INumber<T> => T.Clamp(v, min, max);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Clamp(this sbyte v, sbyte min, sbyte max) => Math.Clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Clamp(this byte v, byte min, byte max) => Math.Clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Clamp(this short v, short min, short max) => Math.Clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Clamp(this ushort v, ushort min, ushort max) => Math.Clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(this int v, int min, int max) => Math.Clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Clamp(this uint v, uint min, uint max) => Math.Clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Clamp(this long v, long min, long max) => Math.Clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Clamp(this ulong v, ulong min, ulong max) => Math.Clamp(v, min, max);
#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Clamp(this nint v, nint min, nint max) => Math.Clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Clamp(this nuint v, nuint min, nuint max) => Math.Clamp(v, min, max);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Clamp(this nint v, nint min, nint max) => v < min ? min : v > max ? max : v;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Clamp(this nuint v, nuint min, nuint max) => v < min ? min : v > max ? max : v;
#endif
#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Clamp(this BigInteger v, BigInteger min, BigInteger max) => BigInteger.Clamp(v, min, max);
#if !UNITY
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Clamp(this Half v, Half min, Half max) => Half.Clamp(v, min, max);
#endif
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Clamp(this BigInteger v, BigInteger min, BigInteger max) => v < min ? min : v > max ? max : v;
#if !UNITY
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Clamp(this Half v, Half min, Half max) => v < min ? min : v > max ? max : v;
#endif
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(this float v, float min, float max) => Math.Clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Clamp(this double v, double min, double max) => Math.Clamp(v, min, max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Clamp(this decimal v, decimal min, decimal max) => Math.Clamp(v, min, max);

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Clamp(this NFloat v, NFloat min, NFloat max) => NFloat.Clamp(v, min, max);
#endif

    #endregion

    #region Pow

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Pow<T>(this T v, T exp) where T : IPowerFunctions<T> => T.Pow(v, exp);
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Pow(this sbyte v, uint exp)
    {
        sbyte ret = 1;
        while (exp != 0)
        {
            if ((exp & 1) == 1)
                ret *= v;
            v *= v;
            exp >>= 1;
        }
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Pow(this byte v, uint exp)
    {
        byte ret = 1;
        while (exp != 0)
        {
            if ((exp & 1) == 1)
                ret *= v;
            v *= v;
            exp >>= 1;
        }
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Pow(this short v, uint exp)
    {
        short ret = 1;
        while (exp != 0)
        {
            if ((exp & 1) == 1)
                ret *= v;
            v *= v;
            exp >>= 1;
        }
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Pow(this ushort v, uint exp)
    {
        ushort ret = 1;
        while (exp != 0)
        {
            if ((exp & 1) == 1)
                ret *= v;
            v *= v;
            exp >>= 1;
        }
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Pow(this int v, uint exp)
    {
        int ret = 1;
        while (exp != 0)
        {
            if ((exp & 1) == 1)
                ret *= v;
            v *= v;
            exp >>= 1;
        }
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Pow(this uint v, uint exp)
    {
        uint ret = 1;
        while (exp != 0)
        {
            if ((exp & 1) == 1)
                ret *= v;
            v *= v;
            exp >>= 1;
        }
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Pow(this nint v, uint exp)
    {
        nint ret = 1;
        while (exp != 0)
        {
            if ((exp & 1) == 1)
                ret *= v;
            v *= v;
            exp >>= 1;
        }
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Pow(this nuint v, uint exp)
    {
        nuint ret = 1;
        while (exp != 0)
        {
            if ((exp & 1) == 1)
                ret *= v;
            v *= v;
            exp >>= 1;
        }
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Pow(this BigInteger v, int exp) => BigInteger.Pow(v, exp);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger Pow(this BigInteger v, uint exp)
    {
        BigInteger ret = 1;
        while (exp != 0)
        {
            if ((exp & 1) == 1)
                ret *= v;
            v *= v;
            exp >>= 1;
        }
        return ret;
    }
#if !UNITY
#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Pow(this Half v, Half exp) => Half.Pow(v, exp);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Pow(this Half v, Half exp) => (Half)Pow((float)v, (float)exp);
#endif
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Pow(this float v, float exp) => MathF.Pow(v, exp);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Pow(this double v, double exp) => Math.Pow(v, exp);

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Pow(this NFloat v, NFloat exp) => NFloat.Pow(v, exp);
#endif

    #endregion

    #region Cos Sin Tan

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sin<T>(this T v) where T : ITrigonometricFunctions<T> => T.Sin(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T SinPi<T>(this T v) where T : ITrigonometricFunctions<T> => T.SinPi(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sinh<T>(this T v) where T : IHyperbolicFunctions<T> => T.Sinh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cos<T>(this T v) where T : ITrigonometricFunctions<T> => T.Cos(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CosPi<T>(this T v) where T : ITrigonometricFunctions<T> => T.CosPi(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cosh<T>(this T v) where T : IHyperbolicFunctions<T> => T.Cosh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Tan<T>(this T v) where T : ITrigonometricFunctions<T> => T.Tan(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T TanPi<T>(this T v) where T : ITrigonometricFunctions<T> => T.TanPi(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Tanh<T>(this T v) where T : IHyperbolicFunctions<T> => T.Tanh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Asin<T>(this T v) where T : ITrigonometricFunctions<T> => T.Asin(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AsinPi<T>(this T v) where T : ITrigonometricFunctions<T> => T.AsinPi(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Asinh<T>(this T v) where T : IHyperbolicFunctions<T> => T.Asinh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Acos<T>(this T v) where T : ITrigonometricFunctions<T> => T.Acos(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AcosPi<T>(this T v) where T : ITrigonometricFunctions<T> => T.AcosPi(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Acosh<T>(this T v) where T : IHyperbolicFunctions<T> => T.Acosh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Atan<T>(this T v) where T : ITrigonometricFunctions<T> => T.Atan(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AtanPi<T>(this T v) where T : ITrigonometricFunctions<T> => T.AtanPi(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Atanh<T>(this T v) where T : IHyperbolicFunctions<T> => T.Atanh(v);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sin(this float v) => MathF.Sin(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sin(this double v) => Math.Sin(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sinh(this float v) => MathF.Sinh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sinh(this double v) => Math.Sinh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cos(this float v) => MathF.Cos(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cos(this double v) => Math.Cos(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cosh(this float v) => MathF.Cosh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cosh(this double v) => Math.Cosh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Tan(this float v) => MathF.Tan(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Tan(this double v) => Math.Tan(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Tanh(this float v) => MathF.Tanh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Tanh(this double v) => Math.Tanh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Asin(this float v) => MathF.Asin(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Asin(this double v) => Math.Asin(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Asinh(this float v) => MathF.Asinh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Asinh(this double v) => Math.Asinh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Acos(this float v) => MathF.Acos(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Acos(this double v) => Math.Acos(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Acosh(this float v) => MathF.Acosh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ACosh(this double v) => Math.Acosh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atan(this float v) => MathF.Atan(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atan(this double v) => Math.Atan(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atanh(this float v) => MathF.Atanh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atanh(this double v) => Math.Atanh(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atan2(this float v, float t) => MathF.Atan2(v, t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atan2(this double v, double t) => Math.Atan2(v, t);

    #endregion

    #region Exp

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Exp<T>(this T v) where T : IExponentialFunctions<T> => T.Exp(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Exp10<T>(this T v) where T : IExponentialFunctions<T> => T.Exp10(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Exp10M1<T>(this T v) where T : IExponentialFunctions<T> => T.Exp10M1(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Exp2<T>(this T v) where T : IExponentialFunctions<T> => T.Exp2(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Exp2M1<T>(this T v) where T : IExponentialFunctions<T> => T.Exp2M1(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ExpM1<T>(this T v) where T : IExponentialFunctions<T> => T.ExpM1(v);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Exp(this float v) => MathF.Exp(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Exp(this double v) => Math.Exp(v);

    #endregion

    #region Rad Deg

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Radians<T>(this T degress) where T : INumberBase<T>, IFloatingPointConstants<T> =>
        degress * RadDegConsts<T>.RAD_PER_DEG;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Degress<T>(this T radians) where T : INumberBase<T>, IFloatingPointConstants<T> =>
        radians * RadDegConsts<T>.DEG_PER_RAD;
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Radians(this float degress) => degress * RAD_PER_DEG_F;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Radians(this double degress) => degress * RAD_PER_DEG;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Radians(this decimal degress) => degress * RAD_PER_DEG_M;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Degress(this float radians) => radians * DEG_PER_RAD_F;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Degress(this double radians) => radians * DEG_PER_RAD;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Degress(this decimal radians) => radians * DEG_PER_RAD_M;

    #endregion

    #region Round

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Round<T>(this T v) where T : IFloatingPoint<T> => T.Round(v);
#endif

#if !UNITY
#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Round(this Half v) => Half.Round(v);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Round(this Half v) => (Half)MathF.Round((float)v);
#endif
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(this float v) => MathF.Round(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this double v) => Math.Round(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(this decimal v) => decimal.Round(v);

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Round(this NFloat v) => NFloat.Round(v);
#endif

    #endregion

    #region Ceiling

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Ceiling<T>(this T v) where T : IFloatingPoint<T> => T.Ceiling(v);
#endif

#if !UNITY
#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Ceiling(this Half v) => Half.Ceiling(v);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Ceiling(this Half v) => (Half)MathF.Ceiling((float)v);
#endif
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Ceiling(this float v) => MathF.Ceiling(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Ceiling(this double v) => Math.Ceiling(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Ceiling(this decimal v) => decimal.Ceiling(v);

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Ceiling(this NFloat v) => NFloat.Ceiling(v);
#endif

    #endregion

    #region Floor

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Floor<T>(this T v) where T : IFloatingPoint<T> => T.Floor(v);
#endif

#if !UNITY
#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Floor(this Half v) => Half.Floor(v);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Floor(this Half v) => (Half)MathF.Floor((float)v);
#endif
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Floor(this float v) => MathF.Floor(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Floor(this double v) => Math.Floor(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Floor(this decimal v) => decimal.Floor(v);

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NFloat Floor(this NFloat v) => NFloat.Floor(v);
#endif

    #endregion

    #region Log

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Log<T>(this T v) where T : ILogarithmicFunctions<T> => T.Log(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Log<T>(this T v, T newBase) where T : ILogarithmicFunctions<T> => T.Log(v, newBase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Log10<T>(this T v) where T : ILogarithmicFunctions<T> => T.Log10(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Log10P1<T>(this T v) where T : ILogarithmicFunctions<T> => T.Log10P1(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Log2<T>(this T v) where T : ILogarithmicFunctions<T> => T.Log2(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Log2P1<T>(this T v) where T : ILogarithmicFunctions<T> => T.Log2P1(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LogP1<T>(this T v) where T : ILogarithmicFunctions<T> => T.LogP1(v);
#endif

#if !UNITY
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Log(this Half v) => (Half)MathF.Log((float)v);
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log(this float v) => MathF.Log(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log(this double v) => Math.Log(v);

#if !UNITY
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Log(this Half v, Half newBase) => (Half)MathF.Log((float)v, (float)newBase);
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log(this float v, float newBase) => MathF.Log(v, newBase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log(this double v, double newBase) => Math.Log(v, newBase);

#if !UNITY
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Log10(this Half v) => (Half)MathF.Log10((float)v);
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log10(this float v) => MathF.Log10(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log10(this double v) => Math.Log10(v);

#if !NETSTANDARD
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Log2(this Half v) => (Half)MathF.Log2((float)v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log2(this float v) => MathF.Log2(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log2(this double v) => Math.Log2(v);
#endif

    #endregion

    #region BinCeil

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static int Ceil2(this int a) => (a + 1) & ~1;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static int Ceil4(this int a) => (a + 3) & ~3;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static int Ceil8(this int a) => (a + 7) & ~7;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static int Ceil16(this int a) => (a + 15) & ~15;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static int Ceil32(this int a) => (a + 31) & ~31;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static int Ceil64(this int a) => (a + 63) & ~63;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static int Ceil128(this int a) => (a + 127) & ~127;

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    public static int Ceil256(this int a) => (a + 255) & ~255;

    #endregion
}

#region Consts

#if NET7_0_OR_GREATER
/// <summary></summary>
public static class NumberConsts<T> where T : INumberBase<T>
{
    /// <summary><c>1</c></summary>
    public static readonly T V1 = T.One;
    /// <summary><c>2</c></summary>
    public static readonly T V2 = V1 + V1;
    /// <summary><c>3</c></summary>
    public static readonly T V3 = V2 + V1;
    /// <summary><c>4</c></summary>
    public static readonly T V4 = V2 * V2;
    /// <summary><c>5</c></summary>
    public static readonly T V5 = V4 + V1;
    /// <summary><c>6</c></summary>
    public static readonly T V6 = V3 * V2;
    /// <summary><c>7</c></summary>
    public static readonly T V7 = V6 + V1;
    /// <summary><c>8</c></summary>
    public static readonly T V8 = V4 * V2;
    /// <summary><c>9</c></summary>
    public static readonly T V9 = V3 * V3;
    /// <summary><c>10</c></summary>
    public static readonly T V10 = V5 * V2;

    /// <summary><c>90</c></summary>
    public static readonly T V90 = V9 * V10;
    /// <summary><c>180</c></summary>
    public static readonly T V180 = V90 * V2;
}

/// <summary></summary>
public static class RadDegConsts<T> where T : INumberBase<T>, IFloatingPointConstants<T>
{
    /// <summary><c>π ÷ 180</c></summary>
    public static readonly T RAD_PER_DEG = T.Pi / NumberConsts<T>.V180;
    /// <summary><c>180 ÷ π</c></summary>
    public static readonly T DEG_PER_RAD = NumberConsts<T>.V180 / T.Pi;
}
#endif

#endregion
