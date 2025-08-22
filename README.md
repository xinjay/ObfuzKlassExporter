# ObfuzKlassExporter

[![license](http://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/xinjay/ObfuzKlassExporter/blob/master/LICENSE)

[CSDN](https://blog.csdn.net/xinjay1992/article/details/150548652) | [Github](https://github.com/xinjay/ObfuzKlassExporter) | [Gitee](https://gitee.com/xinjay/ObfuzKlassExporter)

ObfuzKlassExporter是面向[Obfuz](https://github.com/focus-creative-games/obfuz)的一项辅助工具，方便开发者快捷批量导出指定类型，用于ObfuscationTypeMapper.RegisterType()类型注册。

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

有些时候我们需要建立类型和类型名之间的映射关系，而类型名不一定需要和类型的原本名字一样，此时自定义类型映射就尤为关键，添加自定义类型映射的方式有两种：
