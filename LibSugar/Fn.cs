using System;

namespace LibSugar
{
    public static partial class Sugar
    {
        public static T Identity<T>(T v) => v;
        public static void Empty() { }
        
        public static void TODO(string? message = "TODO")
        {
            throw new NotImplementedException(message);
        }
        public static void TODO(string? message, Exception? inner)
        {
            throw new NotImplementedException(message, inner);
        }
        public static void TODO(Exception? inner)
        {
            throw new NotImplementedException("TODO", inner);
        }
        public static T TODO<T>(string? message = "TODO")
        {
            throw new NotImplementedException(message);
        }
        public static T TODO<T>(string? message, Exception? inner)
        {
            throw new NotImplementedException(message, inner);
        }
        public static T TODO<T>(Exception? inner)
        {
            throw new NotImplementedException("TODO", inner);
        }
    }
}
