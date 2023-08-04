using System.Collections.ObjectModel;
using LibSugar;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace TestGen2;

public record Foo(int A, double B);

[Union, UnionJson]
public enum UnionKind
{
    A,
    [Of<int>]
    B,
    [Of<Foo>]
    C,
}

internal class Program
{
    static void Main(string[] args)
    {
        var opt = new JsonSerializerOptions { };

        {
            var a = Union.MakeA();
            var json = JsonSerializer.Serialize(a, opt);
            Console.WriteLine(json);
            var b = JsonSerializer.Deserialize<Union>(json, opt);
            Console.WriteLine(b);
        }
        Console.WriteLine();
        {
            var a = Union.MakeB(123);
            var json = JsonSerializer.Serialize(a, opt);
            Console.WriteLine(json);
            var b = JsonSerializer.Deserialize<Union>(json, opt);
            Console.WriteLine(b);
        }
        Console.WriteLine();
        {
            var a = Union.MakeC(new Foo(1, 2));
            var json = JsonSerializer.Serialize(a, opt);
            Console.WriteLine(json);
            var b = JsonSerializer.Deserialize<Union>(json, opt);
            Console.WriteLine(b);
        }
    }
}
