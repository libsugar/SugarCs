namespace Tests;

public partial class TestDeref
{
    [SetUp]
    public void Setup() { }
}

public class TestDerefFooBase2
{
    public void Base2() { }
}

public class TestDerefFooBase : TestDerefFooBase2
{
    public void Base() { }
}

public class TestDerefFoo<X, Y> : TestDerefFooBase
{
    private int a = 1;

    /// <inheritdoc cref="B"/>
    public ref readonly int A => ref a;
    /// <summary>
    /// Test doc
    /// </summary>
    public int B { get; set; }

    /// <inheritdoc cref="B"/>
    public int this[int i]
    {
        get => a;
        set => a = value;
    }

    /// <inheritdoc cref="this[int]"/>
    public int this[int x, int y]
    {
        get => a;
        set => a = value;
    }

    /// <inheritdoc cref="B"/>
    public ref int Foo<T>(in T a) where T : TestDeref, new()
    {
        return ref this.a;
    }

    /// <inheritdoc cref="B"/>
    public int Foo<T>(T a, int b) where T : TestDeref, new()
    {
        return this.a;
    }

    /// <inheritdoc cref="Foo{T}(in T)"/>
    public void Bar() { }
    public void Asd<T>(T a) { }
}

[Deref(InheritLevels = -1, EndBaseClass = typeof(TestDerefFooBase2))]
public partial class Deref1<T> : IDeref<TestDerefFoo<T, int>>
{
    public Deref1(TestDerefFoo<T, int> inner)
    {
        Deref = inner;
    }

    public TestDerefFoo<T, int> Deref { get; }
    
}

public partial class TestDeref
{
    [Test]
    public void Test1()
    {
        var a = new Deref1<int>(new TestDerefFoo<int, int>());
        var r = a.Foo(this);
        Assert.That(r, Is.EqualTo(1));
    }
}
