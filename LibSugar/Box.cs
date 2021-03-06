
namespace LibSugar
{
    /// <summary>
    /// Pack the value into the box so that it is passed by reference
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBox<T>
    {
        T Value { get; }
    }
    /// <summary>
    /// Pack the value into the box so that it is passed by reference
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRef<T> : IBox<T>
    {
        new T Value { get; set; }
    }

    /// <summary>
    /// Pack the value into the box so that it is passed by reference
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Value"></param>
    public record Box<T>(T Value) : IBox<T>
    {
        public static implicit operator T(Box<T> s) => s.Value;
        public static implicit operator Box<T>(T b) => new(b);
    }

    /// <summary>
    /// Pack the value into the box so that it is passed by reference
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Ref<T> : IRef<T>
    {
        public T Value { get; set; }

        public Ref(T val) => Value = val;

        public static implicit operator T(Ref<T> s) => s.Value;
        public static implicit operator Ref<T>(T b) => new(b);
    }

    public static partial class Sugar
    {
        /// <summary>
        /// Pack the value into the box so that it is passed by reference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Box<T> Box<T>(this T v) => new(v);
        /// <summary>
        /// Pack the value into the box so that it is passed by reference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Ref<T> Ref<T>(this T v) => new(v);
    }
}
