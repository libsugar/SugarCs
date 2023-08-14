using Newtonsoft.Json.Linq;
using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tests;

public partial class TestUnion
{
    [SetUp]
    public void Setup() { }

    /// <summary>TestDoc</summary>
    [Union, For("TA", "TB", "TC"), UnionJson]
    public enum Union1Kind
    {
        A,
        /// <summary>TestDoc</summary>
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

    [Union, UnionJson]
    public enum Union2Kind
    {
        A,
        [Of<int>]
        B,
        [Of<Task>]
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

    [Union, For("T"), UnionJson]
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

    [Union, Class, UnionJson]
    public enum Union6Kind
    {
        A,
        [Of<int>]
        B,
        [Of<float>]
        C,
    }

    [Union, For("T")]
    public enum Union7Kind
    {
        [Of("List<T>", TryResolveSymbol = true)]
        List,
        [Of("T[]", TryResolveSymbol = true)]
        Array,
    }
}
