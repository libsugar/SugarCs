using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace LibSugar;

/// <summary>
/// RAII
/// </summary>
public interface IRAII<T> : IDisposable
{
    /// <summary>
    /// Wrapped value
    /// </summary>
    public T Value { get; }
}

#region Array

/// <summary>
/// RAII wrap for ArrayPool
/// </summary>
public class PooledArray<T> : IRAII<T[]>, IList, IStructuralComparable, IStructuralEquatable, ICloneable, IList<T>, IReadOnlyList<T>, IEquatable<PooledArray<T>?>
{
    readonly ArrayPool<T> pool;
    readonly bool clearArray;

    internal PooledArray(ArrayPool<T> pool, bool clearArray, T[] value)
    {
        this.pool = pool;
        this.clearArray = clearArray;
        Value = value;
    }

    /// <summary>
    /// The Array
    /// </summary>
    public T[] Value { get; }

    int disposed = 0;
    /// <summary>
    /// Return the array
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if (Interlocked.CompareExchange(ref disposed, 1, 0) != 0) return;
        pool.Return(Value, clearArray);
    }

    /// <summary>
    /// Return the array
    /// </summary>
    ~PooledArray() => Dispose();

    /// <summary>
    /// Get value ref in array
    /// </summary>
    public ref T this[int index] => ref Value[index];
    /// <summary>
    /// Get value ref in array
    /// </summary>
    public ref T this[long index] => ref Value[index];

    /// <summary>
    /// implicit to array
    /// </summary>
    public static implicit operator T[](PooledArray<T> self) => self.Value;

    /// <summary>==</summary>
    public static bool operator ==(PooledArray<T>? left, PooledArray<T>? right) => EqualityComparer<PooledArray<T>>.Default.Equals(left!, right!);
    /// <summary>!=</summary>
    public static bool operator !=(PooledArray<T>? left, PooledArray<T>? right) => !(left == right);

    #region Array forward

    /// <summary>
    /// Gets an object that can be used to synchronize access to the System.Array
    /// </summary>
    public object SyncRoot => Value.SyncRoot;
    /// <summary>
    /// Gets a 64-bit integer that represents the total number of elements in all the dimensions of the System.Array
    /// </summary>
    public long LongLength => Value.LongLength;
    /// <summary>
    /// Gets the total number of elements in all the dimensions of the System.Array
    /// </summary>
    /// <exception cref="OverflowException">The array is multidimensional and contains more than System.Int32.MaxValue elements</exception>
    public int Length => Value.Length;
    /// <summary>
    /// Gets a value indicating whether access to the System.Array is synchronized (thread safe)
    /// </summary>
    public bool IsSynchronized => Value.IsSynchronized;
    /// <summary>
    /// Gets a value indicating whether the System.Array is read-only
    /// </summary>
    public bool IsReadOnly => Value.IsReadOnly;
    /// <summary>
    /// Gets a value indicating whether the System.Array has a fixed size
    /// </summary>
    public bool IsFixedSize => Value.IsFixedSize;
    /// <summary>
    /// Gets the rank (number of dimensions) of the System.Array. For example, a one-dimensional array returns 1, a two-dimensional array returns 2, and so on
    /// </summary>
    public int Rank => Value.Rank;

    /// <summary>
    /// Creates a shallow copy of the System.Array.
    /// </summary>
    /// <returns>A shallow copy of the System.Array.</returns>
    public object Clone() => Value.Clone();
    /// <summary>
    /// Copies all the elements of the current one-dimensional array to the specified one-dimensional array starting at the specified destination array index<br/>
    /// The index is specified as a 64-bit integer
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from the current array</param>
    /// <param name="index">A 64-bit integer that represents the index in array at which copying begins</param>
    /// <exception cref="ArgumentNullException">array is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">index is outside the range of valid indexes for array</exception>
    /// <exception cref="ArgumentException">array is multidimensional. -or- The number of elements in the source array is greater 
    /// than the available number of elements from index to the end of the destination array</exception>
    /// <exception cref="ArrayTypeMismatchException">The type of the source System.Array cannot be cast automatically to the type of the destination array</exception>
    /// <exception cref="RankException">The source System.Array is multidimensional</exception>
    /// <exception cref="InvalidCastException">At least one element in the source System.Array cannot be cast to the type of destination array</exception>
    public void CopyTo(Array array, long index) => Value.CopyTo(array, index);
    /// <summary>
    /// Copies all the elements of the current one-dimensional array to the specified
    /// one-dimensional array starting at the specified destination array index. 
    /// The index is specified as a 32-bit integer.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from the current array.</param>
    /// <param name="index">A 32-bit integer that represents the index in array at which copying begins</param>
    /// <exception cref="ArgumentNullException">array is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">index is less than the lower bound of array</exception>
    /// <exception cref="ArgumentException">array is multidimensional. -or- The number of elements in the source array is
    /// greater than the available number of elements from index to the end of the destination array</exception>
    /// <exception cref="ArrayTypeMismatchException">The type of the source System.Array cannot be cast automatically to the type of the destination array</exception>
    /// <exception cref="RankException">The source array is multidimensional</exception>
    /// <exception cref="InvalidCastException">At least one element in the source System.Array cannot be cast to the type of destination array</exception>
    public void CopyTo(Array array, int index) => Value.CopyTo(array, index);
    /// <summary>
    /// Returns an System.Collections.IEnumerator for the System.Array.
    /// </summary>
    /// <returns>An System.Collections.IEnumerator for the System.Array.</returns>
    public IEnumerator GetEnumerator() => Value.GetEnumerator();
    /// <summary>
    /// Initializes every element of the value-type System.Array by calling the parameterless constructor of the value type
    /// </summary>
    public void Initialize() => Value.Initialize();

    #endregion

    #region MemoryEx

    /// <summary>
    /// Copies the contents of the array into a memory region.
    /// </summary>
    /// <param name="destination">The memory to copy items into.</param>
    /// <exception cref="ArgumentException">The destination is shorter than the source array</exception>
    public void CopyTo(Memory<T> destination) => Value.CopyTo(destination);
    /// <summary>
    /// Copies the contents of the array into the span.
    /// </summary>
    /// <param name="destination">The span to copy items into.</param>
    /// <exception cref="ArgumentException">The destination Span is shorter than the source array</exception>
    public void CopyTo(Span<T> destination) => Value.CopyTo(destination);

    /// <summary>
    /// Creates a new span over a portion of a target array defined by a System.Range value.
    /// </summary>
    /// <param name="range">The range of the array to convert.</param>
    /// <returns>The span representation of the array.</returns>
    public Span<T> AsSpan(Range range) => Value.AsSpan(range);
    /// <summary>
    /// Creates a new span over the portion of the target array beginning at a specified position for a specified length.
    /// </summary>
    /// <param name="start">The index at which to begin the span.</param>
    /// <param name="length">The number of items in the span.</param>
    /// <returns>The span representation of the array.</returns>
    /// <exception cref="ArrayTypeMismatchException">array is covariant, and the array's type is not exactly T[]".</exception>
    /// <exception cref="ArgumentOutOfRangeException">start, length, or start + length is not in the range of text.</exception>
    public Span<T> AsSpan(int start, int length) => Value.AsSpan(start, length);
    /// <summary>
    /// Creates a new span over a portion of the target array starting at a specified position to the end of the array.
    /// </summary>
    /// <param name="start">The initial index from which the array will be converted.</param>
    /// <returns>The span representation of the array.</returns>
    public Span<T> AsSpan(int start) => Value.AsSpan(start);
    /// <summary>
    /// Creates a new span over a target array.
    /// </summary>
    /// <returns>The span representation of the array.</returns>
    public Span<T> AsSpan() => Value.AsSpan();
    /// <summary>
    /// Creates a new span over the portion of the target array defined by an System.Index value.
    /// </summary>
    /// <param name="startIndex">The starting index.</param>
    /// <returns>The span representation of the array.</returns>
    public Span<T> AsSpan(Index startIndex) => Value.AsSpan(startIndex);

    /// <summary>
    /// Creates a new memory region over the portion of the target array beginning at inclusive start index of the range and ending at the exclusive end index of the range.
    /// </summary>
    /// <param name="range">The range to convert from the array.</param>
    /// <returns>The memory representation of the whole or part of the array.</returns>
    public Memory<T> AsMemory(Range range) => Value.AsMemory(range);
    /// <summary>
    /// Creates a new memory region over the portion of the target array beginning at a specified position with a specified length.
    /// </summary>
    /// <param name="start">The index at which to begin the memory region.</param>
    /// <param name="length">The number of items in the memory region.</param>
    /// <returns>The memory representation of the whole or part of the array.</returns>
    /// <exception cref="ArrayTypeMismatchException">array is covariant, and the array's type is not exactly T[].</exception>
    /// <exception cref="ArgumentOutOfRangeException">start, length, or start + length is not in the range of array.</exception>
    public Memory<T> AsMemory(int start, int length) => Value.AsMemory(start, length);
    /// <summary>
    /// Creates a new memory region over the portion of the target array starting at   a specified position to the end of the array.
    /// </summary>
    /// <param name="start"> The index at which to begin the memory.</param>
    /// <returns>The memory representation of the whole or part of the array.</returns>
    /// <exception cref="ArrayTypeMismatchException">array is covariant, and the array's type is not exactly T[].</exception>
    /// <exception cref="ArgumentOutOfRangeException">start, length, or start + length is not in the range of array.</exception>
    public Memory<T> AsMemory(int start) => Value.AsMemory(start);
    /// <summary>
    ///  Creates a new memory region over the target array.
    /// </summary>
    /// <returns>The memory representation of the whole or part of the array.</returns>
    public Memory<T> AsMemory() => Value.AsMemory();
    /// <summary>
    /// Creates a new memory region over the portion of the target array starting at a specified index to the end of the array.
    /// </summary>
    /// <param name="startIndex">The first position of the array.</param>
    /// <returns>The memory representation of the whole or part of the array.</returns>
    public Memory<T> AsMemory(Index startIndex) => Value.AsMemory(startIndex);

    #endregion

    #region interface

    int ICollection.Count => ((ICollection)Value).Count;
    int ICollection<T>.Count => ((ICollection<T>)Value).Count;
    int IReadOnlyCollection<T>.Count => ((IReadOnlyCollection<T>)Value).Count;

    int IList.Add(object value) => ((IList)Value).Add(value);
    void IList.Clear() => ((IList)Value).Clear();
    bool IList.Contains(object value) => ((IList)Value).Contains(value);
    int IList.IndexOf(object value) => ((IList)Value).IndexOf(value);
    void IList.Insert(int index, object value) => ((IList)Value).Insert(index, value);
    void IList.Remove(object value) => ((IList)Value).Remove(value);
    void IList.RemoveAt(int index) => ((IList)Value).RemoveAt(index);
    int IStructuralComparable.CompareTo(object other, IComparer comparer) => ((IStructuralComparable)Value).CompareTo(other, comparer);
    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer) => ((IStructuralEquatable)Value).Equals(other, comparer);
    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => ((IStructuralEquatable)Value).GetHashCode(comparer);
    void ICollection<T>.Add(T item) => ((ICollection<T>)Value).Add(item);
    void ICollection<T>.Clear() => ((ICollection<T>)Value).Clear();
    bool ICollection<T>.Contains(T item) => ((ICollection<T>)Value).Contains(item);
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => ((ICollection<T>)Value).CopyTo(array, arrayIndex);
    bool ICollection<T>.Remove(T item) => ((ICollection<T>)Value).Remove(item);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)Value).GetEnumerator();
    int IList<T>.IndexOf(T item) => ((IList<T>)Value).IndexOf(item);
    void IList<T>.Insert(int index, T item) => ((IList<T>)Value).Insert(index, item);
    void IList<T>.RemoveAt(int index) => ((IList<T>)Value).RemoveAt(index);
    /// <summary>Equals</summary>
    public override bool Equals(object? obj) => Equals(obj as PooledArray<T>);
    /// <summary>Equals</summary>
    public bool Equals(PooledArray<T>? other) => other is not null && EqualityComparer<T[]>.Default.Equals(Value, other.Value);
    /// <summary>GetHashCode</summary>
    public override int GetHashCode() => HashCode.Combine(Value);

    object IList.this[int index] { get => ((IList)Value)[index]; set => ((IList)Value)[index] = value; }
    T IList<T>.this[int index] { get => Value[index]; set => Value[index] = value; }
    T IReadOnlyList<T>.this[int index] => Value[index];

    #endregion

}

public static partial class Sugar
{
    /// <summary>
    /// Rent a array by RAII
    /// </summary>
    /// <typeparam name="T">The type of the objects that are in the resource pool</typeparam>
    /// <param name="pool">The ArrayPool</param>
    /// <param name="minimumLength">The minimum length of the array</param>
    /// <returns></returns>
    public static PooledArray<T> Alloc<T>(this ArrayPool<T> pool, int minimumLength) => new(pool, false, pool.Rent(minimumLength));
    /// <summary>
    /// Rent a array by RAII
    /// </summary>
    /// <typeparam name="T">The type of the objects that are in the resource pool</typeparam>
    /// <param name="pool">The ArrayPool</param>
    /// <param name="minimumLength">The minimum length of the array</param>
    /// <param name="clearArray">Indicates whether the contents of the buffer should be cleared before reuse</param>
    /// <returns></returns>
    public static PooledArray<T> Alloc<T>(this ArrayPool<T> pool, int minimumLength, bool clearArray) => new(pool, clearArray, pool.Rent(minimumLength));
}

#endregion
