//using System;

//namespace LibSugar;

//[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
//public class DeriveAttribute : Attribute
//{
//    public Type[]? Types { get; set; }
    
//    public DeriveAttribute(params Type[] types) => Types = types;
//}

//[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
//public class DeriveAttribute<T> : DeriveAttribute
//{
//    public DeriveAttribute() : base(typeof(T)) { }
//}

//[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
//public class DeriveAttribute<T1, T2> : DeriveAttribute
//{
//    public DeriveAttribute() : base(typeof(T1), typeof(T2)) { }
//}

//[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
//public class DeriveAttribute<T1, T2, T3> : DeriveAttribute
//{
//    public DeriveAttribute() : base(typeof(T1), typeof(T2), typeof(T3)) { }
//}

//[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
//public class DeriveAttribute<T1, T2, T3, T4> : DeriveAttribute
//{
//    public DeriveAttribute() : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4)) { }
//}

//[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
//public class DeriveAttribute<T1, T2, T3, T4, T5> : DeriveAttribute
//{
//    public DeriveAttribute() : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)) { }
//}

//[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
//public class DeriveAttribute<T1, T2, T3, T4, T5, T6> : DeriveAttribute
//{
//    public DeriveAttribute() : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)) { }
//}

//[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
//public class DeriveAttribute<T1, T2, T3, T4, T5, T6, T7> : DeriveAttribute
//{
//    public DeriveAttribute() : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)) { }
//}

//[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
//public class DeriveAttribute<T1, T2, T3, T4, T5, T6, T7, T8> : DeriveAttribute
//{
//    public DeriveAttribute() : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)) { }
//}

//public interface IDerive { }

//public readonly struct Clone : IDerive { }

//public readonly struct Equatable : IDerive { }
