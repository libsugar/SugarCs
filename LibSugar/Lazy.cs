using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LibSugar;

/// <summary>
/// Lazy value with params
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class LazyBy<T>
{
    /// <summary>
    /// Make an assoc Lazy
    /// </summary>
    public static LazyBy<T> Create<P>(Func<P> selector, Func<T> getter, bool threadSafe = true) => threadSafe ? new Impl<P>(getter, selector) : new ImplSync<P>(getter, selector);

    /// <summary>
    /// Check is the value created
    /// </summary>
    public abstract bool IsValueCreated { get; }
    /// <summary>
    /// Get the value
    /// </summary>
    public abstract T Value { get; }

    private LazyBy() { }

    class Impl<P> : LazyBy<T>
    {
        readonly object locker = new();

        readonly Func<T> Getter;
        readonly Func<P> Selector;

        public Impl(Func<T> getter, Func<P> selector)
        {
            Getter = getter;
            Selector = selector;
        }

        T lastValue = default!;
        P lastParam = default!;

        int isValueCreated = 0;

        public override bool IsValueCreated => isValueCreated == 1;

        public override T Value
        {
            get
            {
                lock (locker)
                {
                    if (Interlocked.Exchange(ref isValueCreated, 1) == 1)
                    {
                        var param = Selector();
                        if (EqualityComparer<P>.Default.Equals(lastParam, param)) return lastValue;
                        lastParam = param;
                        return lastValue = Getter();
                    }
                    else
                    {
                        lastParam = Selector();
                        return lastValue = Getter();
                    }
                }
            }
        }
    }

    class ImplSync<P> : LazyBy<T>
    {
        readonly Func<T> Getter;
        readonly Func<P> Selector;

        public ImplSync(Func<T> getter, Func<P> selector)
        {
            Getter = getter;
            Selector = selector;
        }

        T lastValue = default!;
        P lastParam = default!;

        int isValueCreated = 0;

        public override bool IsValueCreated => isValueCreated == 1;

        public override T Value
        {
            get
            {
                if (Interlocked.Exchange(ref isValueCreated, 1) == 1)
                {
                    var param = Selector();
                    if (EqualityComparer<P>.Default.Equals(lastParam, param)) return lastValue;
                    lastParam = param;
                    return lastValue = Getter();
                }
                else
                {
                    lastParam = Selector();
                    return lastValue = Getter();
                }
            }
        }
    }
}

/// <summary>
/// Lazy functions
/// </summary>
public static class LazyFunc
{
    /// <summary>
    /// Make a lazy function
    /// </summary>
    public static Func<T> Create<T>(Func<T> fn, bool threadSafe = true) => new Fn<T>(fn, threadSafe).Function;
    /// <summary>
    /// Make a lazy function
    /// </summary>
    public static Func<P1, T> Create<P1, T>(Func<P1, T> fn, bool threadSafe = true) => threadSafe ? new Fn<P1, T>(fn).Function : new FnSync<P1, T>(fn).Function;
    /// <summary>
    /// Make a lazy function
    /// </summary>
    public static Func<P1, P2, T> Create<P1, P2, T>(Func<P1, P2, T> fn, bool threadSafe = true) => threadSafe ? new Fn<P1, P2, T>(fn).Function : new FnSync<P1, P2, T>(fn).Function;
    /// <summary>
    /// Make a lazy function
    /// </summary>
    public static Func<P1, P2, P3, T> Create<P1, P2, P3, T>(Func<P1, P2, P3, T> fn, bool threadSafe = true) => threadSafe ? new Fn<P1, P2, P3, T>(fn).Function : new FnSync<P1, P2, P3, T>(fn).Function;

    abstract record FnBase<P, T>
    {
        protected readonly object locker = new();
        protected T lastValue = default!;
        protected P lastParam = default!;
        protected int isValueCreated = 0;

        public bool Get(P param)
        {
            if (Interlocked.Exchange(ref isValueCreated, 1) == 1)
            {
                if (EqualityComparer<P>.Default.Equals(lastParam, param)) return true;
                lastParam = param;
                return false;
            }
            else
            {
                lastParam = param;
                return false;
            }
        }
    }

    abstract record FnBaseSync<P, T>
    {
        protected T lastValue = default!;
        protected P lastParam = default!;
        protected int isValueCreated = 0;

        public bool Get(P param)
        {
            if (Interlocked.Exchange(ref isValueCreated, 1) == 1)
            {
                if (EqualityComparer<P>.Default.Equals(lastParam, param)) return true;
                lastParam = param;
                return false;
            }
            else
            {
                lastParam = param;
                return false;
            }
        }
    }

    class Fn<T> : Lazy<T>
    {
        public Fn(Func<T> valueFactory, bool threadSafe) : base(valueFactory, threadSafe) { }

        public T Function() => Value;
    }

    record Fn<P1, T>(Func<P1, T> Getter) : FnBase<P1, T>()
    {
        public T Function(P1 p1) { lock (locker) return Get(p1) ? lastValue : lastValue = Getter(p1); }
    }
    record FnSync<P1, T>(Func<P1, T> Getter) : FnBaseSync<P1, T>()
    {
        public T Function(P1 p1) => Get(p1) ? lastValue : lastValue = Getter(p1);
    }

    record Fn<P1, P2, T>(Func<P1, P2, T> Getter) : FnBase<(P1, P2), T>()
    {
        public T Function(P1 p1, P2 p2) { lock (locker) return Get((p1, p2)) ? lastValue : lastValue = Getter(p1, p2); }
    }
    record FnSync<P1, P2, T>(Func<P1, P2, T> Getter) : FnBaseSync<(P1, P2), T>()
    {
        public T Function(P1 p1, P2 p2) => Get((p1, p2)) ? lastValue : lastValue = Getter(p1, p2);
    }

    record Fn<P1, P2, P3, T>(Func<P1, P2, P3, T> Getter) : FnBase<(P1, P2, P3), T>()
    {
        public T Function(P1 p1, P2 p2, P3 p3) { lock (locker) return Get((p1, p2, p3)) ? lastValue : lastValue = Getter(p1, p2, p3); }
    }
    record FnSync<P1, P2, P3, T>(Func<P1, P2, P3, T> Getter) : FnBaseSync<(P1, P2, P3), T>()
    {
        public T Function(P1 p1, P2 p2, P3 p3) => Get((p1, p2, p3)) ? lastValue : lastValue = Getter(p1, p2, p3);
    }
}
