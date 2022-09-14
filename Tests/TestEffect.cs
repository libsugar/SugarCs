namespace Tests;

public class TestEffect
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestRun1()
    {
        Sugar.Run(() => { });
    }

    [Test]
    public void TestRun2()
    {
        var v = Sugar.Run(() => 1);
        Assert.That(v, Is.EqualTo(1));
    }

    [Test]
    public void TestLet1()
    {
        var v = 1.Let(a => a + 1);
        Assert.That(v, Is.EqualTo(2));
    }

    [Test]
    public void TestLetIf1()
    {
        var v = 1.LetIf(true, a => a + 1);
        Assert.That(v, Is.EqualTo(2));
    }

    [Test]
    public void TestLetIf2()
    {
        var v = 1.LetIf(false, a => a + 1);
        Assert.That(v, Is.EqualTo(1));
    }

    [Test]
    public void TestAlso1()
    {
        int i = 0;
        var v = 1.Also(a => i = a);
        Assert.That(v, Is.EqualTo(1));
        Assert.That(i, Is.EqualTo(1));
    }

    [Test]
    public void TestAlsoIf1()
    {
        int i = 0;
        var v = 1.AlsoIf(true, a => i = a);
        Assert.That(v, Is.EqualTo(1));
        Assert.That(i, Is.EqualTo(1));
    }

    [Test]
    public void TestAlsoIf2()
    {
        int i = 0;
        var v = 1.AlsoIf(false, a => i = a);
        Assert.That(v, Is.EqualTo(1));
        Assert.That(i, Is.EqualTo(0));
    }
}
