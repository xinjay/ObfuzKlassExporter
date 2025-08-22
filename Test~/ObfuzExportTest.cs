[ObfuzExport(ExportName = "XXXXXDelegateSample")]
public delegate void MyDelegateSample(int a);
[ObfuzExport]
public enum MyEnum { }
[ObfuzExport]
public interface MyInterface { }
[ObfuzExport]
public struct MyStruct { }
[ObfuzExport]
public class MyNormalClass { }
namespace GenericNS
{
    [ObfuzExport]
    public class GenericKlass
    {
        public class InternalClass { }
        [ObfuzExport]
        public class InternalClass<T0> { }
        public class InternalClass<T, T1> { }
    }
    [ObfuzExport]
    public class GenericKlass<T0> { }
    [ObfuzExport]
    public class GenericKlass<T0, T1> { }
    [ObfuzExport]
    public class GenericKlass<T0, T1, T2> { }
    public class GenericKlass<T0, T1, T2, T3> { }
    public class GenericKlass<T0, T1, T2, T3, T4> { }
    public class GenericKlass<T0, T1, T2, T3, T4, T5> { }
}