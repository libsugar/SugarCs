using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LibSugar
{
    /// <summary>
    /// Math Ex
    /// </summary>
    public static class LibMath
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
        /// <c>180 ÷ π</c>
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
        /// <c>180 ÷ π</c>
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
        /// <c>180 ÷ π</c>
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

        public static sbyte Min(this sbyte a, sbyte b) => Math.Min(a, b);
        public static sbyte Max(this sbyte a, sbyte b) => Math.Max(a, b);
        public static sbyte Max(this sbyte a, params sbyte[] args) => args.Prepend(a).Max();
        public static sbyte Max(params sbyte[] args) => args.Max();
        public static sbyte Min(this sbyte a, params sbyte[] args) => args.Prepend(a).Min();
        public static sbyte Min(params sbyte[] args) => args.Min();

        public static byte Min(this byte a, byte b) => Math.Min(a, b);
        public static byte Max(this byte a, byte b) => Math.Max(a, b);
        public static byte Max(this byte a, params byte[] args) => args.Prepend(a).Max();
        public static byte Max(params byte[] args) => args.Max();
        public static byte Min(this byte a, params byte[] args) => args.Prepend(a).Min();
        public static byte Min(params byte[] args) => args.Min();

        public static short Min(this short a, short b) => Math.Min(a, b);
        public static short Max(this short a, short b) => Math.Max(a, b);
        public static short Max(this short a, params short[] args) => args.Prepend(a).Max();
        public static short Max(params short[] args) => args.Max();
        public static short Min(this short a, params short[] args) => args.Prepend(a).Min();
        public static short Min(params short[] args) => args.Min();

        public static ushort Min(this ushort a, ushort b) => Math.Min(a, b);
        public static ushort Max(this ushort a, ushort b) => Math.Max(a, b);
        public static ushort Max(this ushort a, params ushort[] args) => args.Prepend(a).Max();
        public static ushort Max(params ushort[] args) => args.Max();
        public static ushort Min(this ushort a, params ushort[] args) => args.Prepend(a).Min();
        public static ushort Min(params ushort[] args) => args.Min();

        public static int Min(this int a, int b) => Math.Min(a, b);
        public static int Max(this int a, int b) => Math.Max(a, b);
        public static int Max(this int a, params int[] args) => args.Prepend(a).Max();
        public static int Max(params int[] args) => args.Max();
        public static int Min(this int a, params int[] args) => args.Prepend(a).Min();
        public static int Min(params int[] args) => args.Min();

        public static uint Min(this uint a, uint b) => Math.Min(a, b);
        public static uint Max(this uint a, uint b) => Math.Max(a, b);
        public static uint Max(this uint a, params uint[] args) => args.Prepend(a).Max();
        public static uint Max(params uint[] args) => args.Max();
        public static uint Min(this uint a, params uint[] args) => args.Prepend(a).Min();
        public static uint Min(params uint[] args) => args.Min();

        public static long Min(this long a, long b) => Math.Min(a, b);
        public static long Max(this long a, long b) => Math.Max(a, b);
        public static long Max(this long a, params long[] args) => args.Prepend(a).Max();
        public static long Max(params long[] args) => args.Max();
        public static long Min(this long a, params long[] args) => args.Prepend(a).Min();
        public static long Min(params long[] args) => args.Min();

        public static ulong Min(this ulong a, ulong b) => Math.Min(a, b);
        public static ulong Max(this ulong a, ulong b) => Math.Max(a, b);
        public static ulong Max(this ulong a, params ulong[] args) => args.Prepend(a).Max();
        public static ulong Max(params ulong[] args) => args.Max();
        public static ulong Min(this ulong a, params ulong[] args) => args.Prepend(a).Min();
        public static ulong Min(params ulong[] args) => args.Min();

        public static nint Min(this nint a, nint b) => a <= b ? a : b;
        public static nint Max(this nint a, nint b) => a >= b ? a : b;
        public static nint Max(this nint a, params nint[] args) => args.Prepend(a).Max();
        public static nint Max(params nint[] args) => args.Max();
        public static nint Min(this nint a, params nint[] args) => args.Prepend(a).Min();
        public static nint Min(params nint[] args) => args.Min();

        public static nuint Min(this nuint a, nuint b) => a <= b ? a : b;
        public static nuint Max(this nuint a, nuint b) => a >= b ? a : b;
        public static nuint Max(this nuint a, params nuint[] args) => args.Prepend(a).Max();
        public static nuint Max(params nuint[] args) => args.Max();
        public static nuint Min(this nuint a, params nuint[] args) => args.Prepend(a).Min();
        public static nuint Min(params nuint[] args) => args.Min();

        public static BigInteger Min(this BigInteger a, BigInteger b) => BigInteger.Min(a, b);
        public static BigInteger Max(this BigInteger a, BigInteger b) => BigInteger.Max(a, b);
        public static BigInteger Max(this BigInteger a, params BigInteger[] args) => args.Prepend(a).Max();
        public static BigInteger Max(params BigInteger[] args) => args.Max();
        public static BigInteger Min(this BigInteger a, params BigInteger[] args) => args.Prepend(a).Min();
        public static BigInteger Min(params BigInteger[] args) => args.Min();

        public static Half Min(this Half a, Half b) => a < b || Half.IsNaN(a) ? a : b;
        public static Half Max(this Half a, Half b) => a > b || Half.IsNaN(a) ? a : b;
        public static Half Max(this Half a, params Half[] args) => args.Prepend(a).Max();
        public static Half Max(params Half[] args) => args.Max();
        public static Half Min(this Half a, params Half[] args) => args.Prepend(a).Min();
        public static Half Min(params Half[] args) => args.Min();

        public static float Min(this float a, float b) => Math.Min(a, b);
        public static float Max(this float a, float b) => Math.Max(a, b);
        public static float Max(this float a, params float[] args) => args.Prepend(a).Max();
        public static float Max(params float[] args) => args.Max();
        public static float Min(this float a, params float[] args) => args.Prepend(a).Min();
        public static float Min(params float[] args) => args.Min();

        public static double Min(this double a, double b) => Math.Min(a, b);
        public static double Max(this double a, double b) => Math.Max(a, b);
        public static double Max(this double a, params double[] args) => args.Prepend(a).Max();
        public static double Max(params double[] args) => args.Max();
        public static double Min(this double a, params double[] args) => args.Prepend(a).Min();
        public static double Min(params double[] args) => args.Min();

        public static decimal Min(this decimal a, decimal b) => Math.Min(a, b);
        public static decimal Max(this decimal a, decimal b) => Math.Max(a, b);
        public static decimal Max(this decimal a, params decimal[] args) => args.Prepend(a).Max();
        public static decimal Max(params decimal[] args) => args.Max();
        public static decimal Min(this decimal a, params decimal[] args) => args.Prepend(a).Min();
        public static decimal Min(params decimal[] args) => args.Min();

        #endregion

        #region Abs

        public static sbyte Abs(this sbyte v) => Math.Abs(v);
        public static byte Abs(this byte v) => v;

        public static int Abs(this short v) => Math.Abs(v);
        public static ushort Abs(this ushort v) => v;

        public static int Abs(this int v) => Math.Abs(v);
        public static uint Abs(this uint v) => v;

        public static long Abs(this long v) => Math.Abs(v);
        public static ulong Abs(this ulong v) => v;

        public static nint Abs(this nint v) => v >= 0 ? v : AbsHelper(v);
        private static nint AbsHelper(nint v)
        {
            Contract.Requires(v < 0, "AbsHelper should only be called for negative values! (hack for JIT inlining)");
            if (v == nint.MinValue) return Math.Abs(int.MinValue); // throw Overflow
            Contract.EndContractBlock();
            return -v;
        }
        public static nuint Abs(this nuint v) => v;

        public static BigInteger Abs(this BigInteger v) => BigInteger.Abs(v);

        public static Half Abs(this Half v) => v >= (Half)0 ? v : (Half)(-(float)v);
        public static float Abs(this float v) => Math.Abs(v);
        public static double Abs(this double v) => Math.Abs(v);
        public static decimal Abs(this decimal v) => Math.Abs(v);

        #endregion

        #region Remap

        public static sbyte Remap(this sbyte v, sbyte low1, sbyte high1, sbyte low2, sbyte high2) => (sbyte)(low2 + (v - low1) * (high2 - low2) / (high1 - low1));
        public static byte Remap(this byte v, byte low1, byte high1, byte low2, byte high2) => (byte)(low2 + (v - low1) * (high2 - low2) / (high1 - low1));
        public static short Remap(this short v, short low1, short high1, short low2, short high2) => (short)(low2 + (v - low1) * (high2 - low2) / (high1 - low1));
        public static ushort Remap(this ushort v, ushort low1, ushort high1, ushort low2, ushort high2) => (ushort)(low2 + (v - low1) * (high2 - low2) / (high1 - low1));
        public static int Remap(this int v, int low1, int high1, int low2, int high2) => low2 + (v - low1) * (high2 - low2) / (high1 - low1);
        public static uint Remap(this uint v, uint low1, uint high1, uint low2, uint high2) => low2 + (v - low1) * (high2 - low2) / (high1 - low1);
        public static long Remap(this long v, long low1, long high1, long low2, long high2) => low2 + (v - low1) * (high2 - low2) / (high1 - low1);
        public static ulong Remap(this ulong v, ulong low1, ulong high1, ulong low2, ulong high2) => low2 + (v - low1) * (high2 - low2) / (high1 - low1);
        public static nint Remap(this nint v, nint low1, nint high1, nint low2, nint high2) => low2 + (v - low1) * (high2 - low2) / (high1 - low1);
        public static nuint Remap(this nuint v, nuint low1, nuint high1, nuint low2, nuint high2) => low2 + (v - low1) * (high2 - low2) / (high1 - low1);
        public static BigInteger Remap(this BigInteger v, BigInteger low1, BigInteger high1, BigInteger low2, BigInteger high2) => low2 + (v - low1) * (high2 - low2) / (high1 - low1);
        public static Half Remap(this Half v, Half low1, Half high1, Half low2, Half high2) => (Half)Remap((float)v, (float)low1, (float)high1, (float)low2, (float)high2);
        public static float Remap(this float v, float low1, float high1, float low2, float high2) => low2 + (v - low1) * (high2 - low2) / (high1 - low1);
        public static double Remap(this double v, double low1, double high1, double low2, double high2) => low2 + (v - low1) * (high2 - low2) / (high1 - low1);
        public static decimal Remap(this decimal v, decimal low1, decimal high1, decimal low2, decimal high2) => low2 + (v - low1) * (high2 - low2) / (high1 - low1);

        #endregion

        #region Clamp

        public static sbyte Clamp(this sbyte v, sbyte min, sbyte max) => Math.Clamp(v, min, max);
        public static byte Clamp(this byte v, byte min, byte max) => Math.Clamp(v, min, max);
        public static short Clamp(this short v, short min, short max) => Math.Clamp(v, min, max);
        public static ushort Clamp(this ushort v, ushort min, ushort max) => Math.Clamp(v, min, max);
        public static int Clamp(this int v, int min, int max) => Math.Clamp(v, min, max);
        public static uint Clamp(this uint v, uint min, uint max) => Math.Clamp(v, min, max);
        public static long Clamp(this long v, long min, long max) => Math.Clamp(v, min, max);
        public static ulong Clamp(this ulong v, ulong min, ulong max) => Math.Clamp(v, min, max);
        public static nint Clamp(this nint v, nint min, nint max) => v < min ? min : v > max ? max : v;
        public static nuint Clamp(this nuint v, nuint min, nuint max) => v < min ? min : v > max ? max : v;
        public static BigInteger Clamp(this BigInteger v, BigInteger min, BigInteger max) => v < min ? min : v > max ? max : v;
        public static Half Clamp(this Half v, Half min, Half max) => v < min ? min : v > max ? max : v;
        public static float Clamp(this float v, float min, float max) => Math.Clamp(v, min, max);
        public static double Clamp(this double v, double min, double max) => Math.Clamp(v, min, max);
        public static decimal Clamp(this decimal v, decimal min, decimal max) => Math.Clamp(v, min, max);

        #endregion

        #region Pow

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
        public static Half Pow(this Half v, Half exp) => (Half)Pow((float)v, (float)exp);
        public static float Pow(this float v, float exp) => MathF.Pow(v, exp);
        public static double Pow(this double v, double exp) => Math.Pow(v, exp);

        #endregion

        #region Cos Sin Tan

        public static float Sin(this float v) => MathF.Sin(v);
        public static double Sin(this double v) => Math.Sin(v);

        public static float Sinh(this float v) => MathF.Sinh(v);
        public static double Sinh(this double v) => Math.Sinh(v);

        public static float Cos(this float v) => MathF.Cos(v);
        public static double Cos(this double v) => Math.Cos(v);

        public static float Cosh(this float v) => MathF.Cosh(v);
        public static double Cosh(this double v) => Math.Cosh(v);

        public static float Tan(this float v) => MathF.Tan(v);
        public static double Tan(this double v) => Math.Tan(v);

        public static float Tanh(this float v) => MathF.Tanh(v);
        public static double Tanh(this double v) => Math.Tanh(v);

        public static float Asin(this float v) => MathF.Asin(v);
        public static double Asin(this double v) => Math.Asin(v);

        public static float Asinh(this float v) => MathF.Asinh(v);
        public static double Asinh(this double v) => Math.Asinh(v);

        public static float Acos(this float v) => MathF.Acos(v);
        public static double Acos(this double v) => Math.Acos(v);

        public static float Acosh(this float v) => MathF.Acosh(v);
        public static double ACosh(this double v) => Math.Acosh(v);

        public static float Atan(this float v) => MathF.Atan(v);
        public static double Atan(this double v) => Math.Atan(v);

        public static float Atanh(this float v) => MathF.Atanh(v);
        public static double Atanh(this double v) => Math.Atanh(v);

        public static float Atan2(this float v, float t) => MathF.Atan2(v, t);
        public static double Atan2(this double v, double t) => Math.Atan2(v, t);

        #endregion

        #region Exp

        public static float Exp(this float v) => MathF.Exp(v);
        public static double Exp(this double v) => Math.Exp(v);

        #endregion

        #region Rad Deg

        public static float Radians(this float degress) => degress * RAD_PER_DEG_F;
        public static double Radians(this double degress) => degress * RAD_PER_DEG;
        public static decimal Radians(this decimal degress) => degress * RAD_PER_DEG_M;

        public static float Degress(this float radians) => radians * DEG_PER_RAD_F;
        public static double Degress(this double radians) => radians * DEG_PER_RAD;
        public static decimal Degress(this decimal radians) => radians * DEG_PER_RAD_M;

        #endregion
    }
}
