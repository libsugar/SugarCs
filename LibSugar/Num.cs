using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSugar
{
    #region IntIter

    public struct IntIter : IEnumerable<int>
    {
        public readonly int From { get; private init; }
        public readonly int To { get; private init; }

        public int Length => Math.Max(From, To) - Math.Min(From, To) + 1;
        public int this[int index] => From <= To ? From + index : From - index;

        public IntIter Slice(int start) => new(this[start], To);
        public IntIter Slice(int start, int length)
        {
            var from = this[start];
            return new(from, length == 0 ? from : length > 0 ? from + length - 1 : from - length + 1);
        }

        public IntIter(int from, int to = int.MaxValue)
        {
            From = from;
            To = to;
        }
        public static IntIter New(int from = 0, int to = int.MaxValue) => new(from, to);

        public Enumerator GetEnumerator() => new();
        IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<int>
        {
            int i;
            readonly IntIter range;

            public Enumerator(IntIter r)
            {
                range = r;
                i = r.From;
            }

            public int Current => i;

            object IEnumerator.Current => i;

            public void Dispose() { }

            public bool MoveNext()
            {
                i++;
                return i <= range.To;
            }

            public void Reset() => i = range.From;
        }
    }

    #endregion

    #region UIntIter

    public struct UIntIter : IEnumerable<uint>
    {
        public readonly uint From { get; private init; }
        public readonly uint To { get; private init; }

        public int Length => (int)(From.Max(To) - From.Min(To) + 1);
        public uint this[int index] => From <= To ? From + (uint)index : From - (uint)index;

        public UIntIter Slice(int start) => new(this[start], To);
        public UIntIter Slice(int start, int length)
        {
            var from = this[start];
            return new(from, length == 0 ? from : length > 0 ? from + (uint)length - 1 : from - (uint)length + 1);
        }

        public UIntIter(uint from, uint to = uint.MaxValue)
        {
            From = from;
            To = to;
        }
        public static UIntIter New(uint from = 0, uint to = uint.MaxValue) => new(from, to);

        public Enumerator GetEnumerator() => new();
        IEnumerator<uint> IEnumerable<uint>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<uint>
        {
            uint i;
            readonly UIntIter range;

            public Enumerator(UIntIter r)
            {
                range = r;
                i = r.From;
            }

            public uint Current => i;

            object IEnumerator.Current => i;

            public void Dispose() { }

            public bool MoveNext()
            {
                i++;
                return i <= range.To;
            }

            public void Reset() => i = range.From;
        }
    }

    #endregion

    public static partial class Sugar
    {
        /// <summary>
        /// Iterator in range <c>[0, int.MaxValue]</c>
        /// </summary>
        public static readonly IntIter Int = IntIter.New();
        /// <summary>
        /// Iterator in range <c>[int.MinValue, int.MaxValue]</c>
        /// </summary>
        public static readonly IntIter IntFull = IntIter.New(int.MinValue);
        /// <summary>
        /// Iterator in range <c>[0, uint.MaxValue]</c>
        /// </summary>
        public static readonly UIntIter UInt = UIntIter.New();

    }
}
