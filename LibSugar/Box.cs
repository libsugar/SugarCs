using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSugar
{
    public interface IBox<T>
    {
        T Value { get; }
    }
    public interface IRef<T> : IBox<T>
    {
        new T Value { get; set; }
    }

    public record Box<T>(T Value) : IBox<T>
    {
        public static implicit operator T(Box<T> s) => s.Value;
        public static implicit operator Box<T>(T b) => new(b);
    }

    public class Ref<T> : IRef<T>
    {
        public T Value { get; set; }

        public Ref(T val) => Value = val;

        public static implicit operator T(Ref<T> s) => s.Value;
        public static implicit operator Ref<T>(T b) => new(b);
    }

    public static partial class Sugar
    {
        public static Box<T> Box<T>(this T v) => new(v);
        public static Ref<T> Ref<T>(this T v) => new(v);
    }
}
