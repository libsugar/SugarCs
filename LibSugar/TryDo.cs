using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#if !NETSTANDARD
using System.Collections.Immutable;
#endif
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.ObjectModel;
#if NET6_0_OR_GREATER
using System.Text.Json.Nodes;
#endif
using System.Diagnostics.CodeAnalysis;

namespace LibSugar;

public static partial class Sugar
{
    public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> self, K key, V val) where K : notnull =>
        self.AddOrUpdate(key, val, (_, _) => val);

    public static V? TryGet<K,
#if NETSTANDARD
        V
#else
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        V
#endif
    >(this ConditionalWeakTable<K, V> self, K key) where K : class where V : class =>
        self.TryGetValue(key, out var val) ? val : null;

#if NET6_0_OR_GREATER
    public static JsonNode? TryGetProperty(this JsonObject self, string propertyName) =>
        self.TryGetPropertyValue(propertyName, out var val) ? val : null;
#endif
}

public static partial class SugarClass
{
    public static V? TryGet<K, V>(this IDictionary<K, V> self, K key) where V : class =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this IReadOnlyDictionary<K, V> self, K key) where V : class =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this Dictionary<K, V> self, K key) where V : class where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static T? TryGet<T>(this HashSet<T> self, T key) where T : class =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this ConcurrentDictionary<K, V> self, K key) where V : class where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryRemove<K, V>(this ConcurrentDictionary<K, V> self, K key) where V : class where K : notnull =>
        self.TryRemove(key, out var val) ? val : null;

    public static T? TryPeek<T>(this ConcurrentBag<T> self) where T : class =>
        self.TryPeek(out var val) ? val : null;

    public static T? TryTake<T>(this ConcurrentBag<T> self) where T : class =>
        self.TryTake(out var val) ? val : null;

    public static T? TryTake<T>(this BlockingCollection<T> self) where T : class =>
        self.TryTake(out var val) ? val : null;

    public static T? TryTake<T>(this BlockingCollection<T> self, TimeSpan timeout) where T : class =>
        self.TryTake(out var val, timeout) ? val : null;

    public static T? TryTake<T>(this BlockingCollection<T> self, int millisecondsTimeout,
        CancellationToken cancellationToken) where T : class =>
        self.TryTake(out var val, millisecondsTimeout, cancellationToken) ? val : null;

    public static T? TryTake<T>(this BlockingCollection<T> self, int millisecondsTimeout) where T : class =>
        self.TryTake(out var val, millisecondsTimeout) ? val : null;

    public static T? TryPeek<T>(this ConcurrentQueue<T> self) where T : class =>
        self.TryPeek(out var val) ? val : null;

    public static T? TryDequeue<T>(this ConcurrentQueue<T> self) where T : class =>
        self.TryDequeue(out var val) ? val : null;

    public static T? TryPeek<T>(this ConcurrentStack<T> self) where T : class =>
        self.TryPeek(out var val) ? val : null;

    public static T? TryPop<T>(this ConcurrentStack<T> self) where T : class =>
        self.TryPop(out var val) ? val : null;

    public static V? TryGet<K, V>(this SortedList<K, V> self, K key) where V : class where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this SortedDictionary<K, V> self, K key) where V : class where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

#if !NETSTANDARD
    public static V? TryGet<K, V>(this IImmutableDictionary<K, V> self, K key) where V : class =>
        self.TryGetValue(key, out var val) ? val : null;

    public static T? TryGet<T>(this IImmutableSet<T> self, T key) where T : class =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this ImmutableDictionary<K, V> self, K key) where V : class where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static T? TryGet<T>(this ImmutableHashSet<T> self, T key) where T : class =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this ImmutableSortedDictionary<K, V> self, K key) where V : class where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static T? TryGet<T>(this ImmutableSortedSet<T> self, T key) where T : class =>
        self.TryGetValue(key, out var val) ? val : null;
#endif

    public static ReadOnlyMemory<T>? TryGet<T>(this in ReadOnlySequence<T> self, ref SequencePosition position,
        bool advance = true) where T : class =>
        self.TryGet(ref position, out var memory, advance) ? memory : null;

#if NET6_0_OR_GREATER
    public static T? TryGet<T>(this JsonValue self) where T : class =>
        self.TryGetValue<T>(out var val) ? val : null;

    public static E? TryPeek<E, P>(this PriorityQueue<E, P> self, out P? priority) where E : class =>
        self.TryPeek(out var val, out priority) ? val : null;

    public static E? TryDequeue<E, P>(this PriorityQueue<E, P> self, out P? priority) where E : class =>
        self.TryDequeue(out var val, out priority) ? val : null;
#endif

    public static T? TryPeek<T>(this Queue<T> self) where T : class =>
        self.TryPeek(out var val) ? val : null;

    public static T? TryDequeue<T>(this Queue<T> self) where T : class =>
        self.TryDequeue(out var val) ? val : null;

    public static T? TryPeek<T>(this Stack<T> self) where T : class =>
        self.TryPeek(out var val) ? val : null;

    public static T? TryPop<T>(this Stack<T> self) where T : class =>
        self.TryPop(out var val) ? val : null;

    public static T? TryFirst<T>(this IEnumerable<T> self) where T : class
    {
        foreach (var val in self) return val;
        return null;
    }

    public static V? TryGet<K, V>(this KeyedCollection<K, V> self, K key) where V : class where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this ReadOnlyDictionary<K, V> self, K key) where V : class where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;
}

public static partial class SugarStruct
{
    public static V? TryGet<K, V>(this IDictionary<K, V> self, K key) where V : struct =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this IReadOnlyDictionary<K, V> self, K key) where V : struct =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this Dictionary<K, V> self, K key) where V : struct where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static T? TryGet<T>(this HashSet<T> self, T key) where T : struct =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this ConcurrentDictionary<K, V> self, K key) where V : struct where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryRemove<K, V>(this ConcurrentDictionary<K, V> self, K key) where V : struct where K : notnull =>
        self.TryRemove(key, out var val) ? val : null;

    public static T? TryPeek<T>(this ConcurrentBag<T> self) where T : struct =>
        self.TryPeek(out var val) ? val : null;

    public static T? TryTake<T>(this ConcurrentBag<T> self) where T : struct =>
        self.TryTake(out var val) ? val : null;

    public static T? TryTake<T>(this BlockingCollection<T> self) where T : struct =>
        self.TryTake(out var val) ? val : null;

    public static T? TryTake<T>(this BlockingCollection<T> self, TimeSpan timeout) where T : struct =>
        self.TryTake(out var val, timeout) ? val : null;

    public static T? TryTake<T>(this BlockingCollection<T> self, int millisecondsTimeout,
        CancellationToken cancellationToken) where T : struct =>
        self.TryTake(out var val, millisecondsTimeout, cancellationToken) ? val : null;

    public static T? TryTake<T>(this BlockingCollection<T> self, int millisecondsTimeout) where T : struct =>
        self.TryTake(out var val, millisecondsTimeout) ? val : null;

    public static T? TryPeek<T>(this ConcurrentQueue<T> self) where T : struct =>
        self.TryPeek(out var val) ? val : null;

    public static T? TryDequeue<T>(this ConcurrentQueue<T> self) where T : struct =>
        self.TryDequeue(out var val) ? val : null;

    public static T? TryPeek<T>(this ConcurrentStack<T> self) where T : struct =>
        self.TryPeek(out var val) ? val : null;

    public static T? TryPop<T>(this ConcurrentStack<T> self) where T : struct =>
        self.TryPop(out var val) ? val : null;

    public static V? TryGet<K, V>(this SortedList<K, V> self, K key) where V : struct where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this SortedDictionary<K, V> self, K key) where V : struct where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

#if !NETSTANDARD
    public static V? TryGet<K, V>(this IImmutableDictionary<K, V> self, K key) where V : struct =>
        self.TryGetValue(key, out var val) ? val : null;

    public static T? TryGet<T>(this IImmutableSet<T> self, T key) where T : struct =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this ImmutableDictionary<K, V> self, K key) where V : struct where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static T? TryGet<T>(this ImmutableHashSet<T> self, T key) where T : struct =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this ImmutableSortedDictionary<K, V> self, K key)
        where V : struct where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static T? TryGet<T>(this ImmutableSortedSet<T> self, T key) where T : struct =>
        self.TryGetValue(key, out var val) ? val : null;
#endif

    public static ReadOnlyMemory<T>? TryGet<T>(this in ReadOnlySequence<T> self, ref SequencePosition position,
        bool advance = true) where T : struct =>
        self.TryGet(ref position, out var memory, advance) ? memory : null;

#if NET6_0_OR_GREATER
    public static T? TryGet<T>(this JsonValue self) where T : struct =>
        self.TryGetValue<T>(out var val) ? val : null;

    public static E? TryPeek<E, P>(this PriorityQueue<E, P> self, out P? priority) where E : struct =>
        self.TryPeek(out var val, out priority) ? val : null;

    public static E? TryDequeue<E, P>(this PriorityQueue<E, P> self, out P? priority) where E : struct =>
        self.TryDequeue(out var val, out priority) ? val : null;
#endif

    public static T? TryPeek<T>(this Queue<T> self) where T : struct =>
        self.TryPeek(out var val) ? val : null;

    public static T? TryDequeue<T>(this Queue<T> self) where T : struct =>
        self.TryDequeue(out var val) ? val : null;

    public static T? TryPeek<T>(this Stack<T> self) where T : struct =>
        self.TryPeek(out var val) ? val : null;

    public static T? TryPop<T>(this Stack<T> self) where T : struct =>
        self.TryPop(out var val) ? val : null;

    public static T? TryFirst<T>(this IEnumerable<T> self) where T : struct
    {
        foreach (var val in self) return val;
        return null;
    }

    public static V? TryGet<K, V>(this KeyedCollection<K, V> self, K key) where V : struct where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;

    public static V? TryGet<K, V>(this ReadOnlyDictionary<K, V> self, K key) where V : struct where K : notnull =>
        self.TryGetValue(key, out var val) ? val : null;
}
