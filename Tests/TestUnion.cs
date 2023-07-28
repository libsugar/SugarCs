using System;
using System.Numerics;

namespace Tests;

public partial class TestUnion
{
    [SetUp]
    public void Setup() { }

    [Union, For("TA", "TB", "TC")]
    public enum Union1Kind
    {
        A,
        [Name("Some"), Of<int>]
        B,
        [Of<Task>]
        C,
        [Of(typeof(float))]
        D,
        [Of<string>]
        E,
        [Of("TA")]
        F,
        [Of("TB")]
        G,
        [Of("TC")]
        H,
    }

    public readonly partial struct Union1<TA, TB, TC> where TB : unmanaged where TC : class
    {

    }

    [Union]
    public enum Union2Kind
    {
        A,
        [Of<int>]
        B,
        [Of<object>]
        C,
    }

    [Union]
    public enum Union3Kind
    {
        A,
        [Of<int>]
        B,
        [Of<double>]
        C,
    }

    public partial struct Union4<T> where T : unmanaged
    {

    }

    [Union, For("T")]
    public enum Union4Kind
    {
        A,
        [Of<int>]
        B,
        [Of("T")]
        C,
    }

    [Union]
    public enum Union5Kind
    {
        A,
        [Of<int>]
        B,
        [Of(typeof(double))]
        C,
    }
}
