using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tests;

public class TestGenericJsonConvert
{
    [SetUp]
    public void Setup() { }
    
    public record Foo<T>(T A)
    {
        public T A { get; set; } = A;
    }

    public class FooConverter<T> : JsonConverter<Foo<T>>
    {
        public override Foo<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var conv = (JsonConverter<T>)options.GetConverter(typeof(T));
            var a = conv.Read(ref reader, typeToConvert, options);
            return new Foo<T>(a!);
        }

        public override void Write(Utf8JsonWriter writer, Foo<T> value, JsonSerializerOptions options)
        {
            var conv = (JsonConverter<T>)options.GetConverter(typeof(T));
            conv.Write(writer, value.A, options);
        }
    }

    [Test]
    public void Test()
    {
        var opt = new JsonSerializerOptions { Converters = { new FooConverter<int>(), } };
        var a = new Foo<int>(1);

        var json = JsonSerializer.Serialize(a, opt);
        Console.WriteLine(json);

        var b = JsonSerializer.Deserialize<Foo<int>>(json, opt);
    }
}
