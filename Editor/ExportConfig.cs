using System;
using System.Collections.Generic;

public partial class ObfuzKlassExporter
{
    /// <summary>
    /// 需要导出的类型列表
    /// </summary>
    public static List<Type> exportList = new()
    {
        //typeof(GenericKlass<float, string>),
        //typeof(GenericKlass<int, int>),
        //typeof(GenericKlass),
        //typeof(GenericKlass<string>),
        //typeof(GenericKlass<object, object>),
        //typeof(GenericKlass<int, object, string, float>),
        //typeof(GenericKlass<,,,>),
        //typeof(GenericKlass<,,,,>),
        //typeof(GenericKlass<,,,,,>),
        //typeof(List<>),
        //typeof(GenericKlass<>),
        //typeof(ValueTuple<>),
        //typeof(GenericKlass<,>),
        //typeof(ValueTuple<,>),
        //typeof(Dictionary<,>),
        //typeof(Dictionary<string, string>),
        //typeof(ValueTuple<string, string>),
        //typeof(ValueTuple<string, string, object>),
        //typeof(ValueTuple<,,>),
        //typeof(ValueTuple<,,,,,,,>),
        //typeof(GenericKlass.InternalClass),
        //typeof(GenericKlass.InternalClass<,>),
        //typeof(GenericKlass.InternalClass<int, int>),
        //typeof(GenericKlass.InternalClass<int, int>),
        //在这里添加需要导出的类型，或者给指定类型添加ObfuzExportAttribute特性
        
    };

    /// <summary>
    /// 自定义类型映射
    /// </summary>
    public static Dictionary<Type, string> customExportMap = new()
    {
        //{ typeof(GenericKlass<float, string>), "Hello GenericKlass" }
        //在这里添加自定义的类型映射
        
    };
}