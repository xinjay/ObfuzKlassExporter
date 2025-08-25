# ObfuzKlassExporter

[![license](http://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/xinjay/ObfuzKlassExporter/blob/master/LICENSE)

[CSDN](https://blog.csdn.net/xinjay1992/article/details/150762977) | [Github](https://github.com/xinjay/ObfuzKlassExporter) | [Gitee](https://gitee.com/xinjay/ObfuzKlassExporter)

ObfuzKlassExporter是面向[Obfuz](https://github.com/focus-creative-games/obfuz)的一项辅助工具，方便开发者快捷批量导出指定类型，用于ObfuscationTypeMapper.RegisterType()类型注册，方便用指定类型名获取被加固混淆后的类。

## 特性

- 支持对自定义导出类型列表中的类型进行导出；

- 支持对添加了`[ObfuzExportAttribute]`特性的类型进行导出；

- 支持自动按AOT程序集和HotUpdate程序集分类输出；

- 支持自定义脚本导出路径；

## 使用说明

### 安装

ObfuzResolver的Unity Package Manager URL安装地址：

- gitee `https://gitee.com/xinjay/ObfuzKlassExporter.git`
- github `https://github.com/xinjay/ObfuzKlassExporter.git`

打开Unity Package Manager窗口，点击`Add package from URL...`，填入以上地址之一即可完成安装。

### 添加导出类型

添加导出类型的方式有两种：

- 向**ExportConfig**脚本的**exportList**中添加需要导出的类型，如下：
  
  ```csharp
  public partial class ObfuzKlassExporter
  {
      /// <summary>
      /// 需要导出的类型列表
      /// </summary>
      public static List<Type> exportList = new()
      {
          typeof(GenericKlass<float, string>),
          typeof(GenericKlass<int, int>),
          typeof(GenericKlass),
          typeof(GenericKlass<string>),
          typeof(GenericKlass<object, object>),
          typeof(GenericKlass<int, object, string, float>),
          typeof(GenericKlass<,,,>),
          typeof(GenericKlass<,,,,>),
          typeof(GenericKlass<,,,,,>),
          typeof(List<>),
          typeof(GenericKlass<>),
          typeof(ValueTuple<>),
          typeof(GenericKlass<,>),
          typeof(ValueTuple<,>),
          typeof(Dictionary<,>),
          typeof(Dictionary<string, string>),
          typeof(ValueTuple<string, string>),
          typeof(ValueTuple<string, string, object>),
          typeof(ValueTuple<,,>),
          typeof(ValueTuple<,,,,,,,>),
          typeof(GenericKlass.InternalClass),
          typeof(GenericKlass.InternalClass<,>),
          typeof(GenericKlass.InternalClass<int, int>),
          typeof(GenericKlass.InternalClass<int, int>),
          //在这里添加需要导出的类型，或者给指定类型添加ObfuzExportAttribute特性
  
      };
  }
  ```

- 给需要导出的类型添加`[ObfuzExportAttribute]`特性，如下：
  
  ```csharp
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
  ```

### 添加自定义类型映射

有些时候我们需要建立类型和类型名之间的映射关系，而类型名不一定需要和类型的原本名字完全一样，此时自定义类型映射就尤为关键，添加自定义类型映射的方式有两种：

- 向**ExportConfig**脚本的**customExportMap**中添加自定义类型映射，如下：
  
  ```csharp
  public partial class ObfuzKlassExporter
  {
      /// <summary>
      /// 自定义类型映射
      /// </summary>
      public static Dictionary<Type, string> customExportMap = new()
      {
          { typeof(GenericKlass<float, string>), "Hello GenericKlass" }
          //在这里添加自定义的类型映射
  
      };
  }   
  ```

- 给需要导出的类型添加`[ObfuzExportAttribute]`特性，并指定**ExportName**参数，如下：

- ```csharp
  [ObfuzExport(ExprtName = "XXXXXDelegateSample")]
  public delegate void MyDelegateSample(int a);
  ```

### 指定AOT和HotUpdate脚本导出路径

指定按AOT和热更分类后导出的注册脚本文件路径，并调用`ObfuzKlassExporter.ExportObfuzKlasses(string aotpath, string hotupdatepath)`即可完成注册脚本的导出，如下；

```csharp
    [MenuItem("Obfuz/ExportKlasses")]
    public static void ExportKlasses()
    {
        var output_aot = "Assets/ObfuzKlassExporter/AOT";
        var output_hotupdate = "Assets/ObfuzKlassExporter/HotUpdate";
        ObfuzKlassExporter.ExportObfuzKlasses(output_aot, output_hotupdate);
    }
```

### 完成类型注册

在运行时代码中适当位置调用注册脚本中的指定方法，完成类型注册：

- 在AOT代码中适当位置调用`ObfuzKlassRegisterAutoGen_AOT.Register()`完成AOT导出类型注册；

- 在HotUpdate代码中适当位置调用`ObfuzKlassRegisterAutoGen_Hotupdate.Register()`完成HotUpdate导出类型注册；

至此即完成了所有指定类型的导出与注册，便能在代码中自由通过特定名称获取指定类型，尤其是类型反射过程中，或者是在XLua LuaWrap注册过程中，大量需要类似的操作。

特殊说明：由于`ObfuzExportAttribute`添加了`[Conditional("UNITY_EDITOR")]`特性，因此在正常编译非"UNITY_EDITOR"程序集时会将该特性从被应用的类型上剥离下来，因而不必担心编译后的程序集会带入该特性。特别需要注意的是`[Conditional("XXX")]`只适用于方法，或者继承自`Attribute`的类型，如`ObfuzExportAttribute`。
