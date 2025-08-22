using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

public partial class ObfuzKlassExporter
{
    [MenuItem("Obfuz/ExportKlasses")]
    public static void ExportKlasses()
    {
        var output_aot = "Assets/AOT";
        var output_hotupdate = "Assets/HotUpdate";
        ExportObfuzKlasses(output_aot, output_hotupdate);
    }

    /// <summary>
    /// 导出并生成类型注册脚本
    /// </summary>
    /// <param name="aotpath"></param>
    /// <param name="hotupdatepath"></param>
    public static void ExportObfuzKlasses(string aotpath, string hotupdatepath)
    {
        var klassesWithObfuzExport = GetExportKlassWithObfuzExportAttribute();
        exportList.AddRange(klassesWithObfuzExport);
        var all = exportList.Distinct().ToList();
        SplitKlassList(all, out var aotKlassList, out var hotupdateKlassList);
        ExportKlasses(aotKlassList, "AOT", aotpath);
        ExportKlasses(hotupdateKlassList, "Hotupdate", hotupdatepath);
    }

    /// <summary>
    /// 输出代码文件
    /// </summary>
    /// <param name="klassList"></param>
    /// <param name="suffix"></param>
    /// <param name="savePath"></param>
    private static void ExportKlasses(List<Type> klassList, string suffix, string savePath)
    {
        var splitCount = 50;
        var groups = (klassList.Count + splitCount - 1) / splitCount;
        var klassName = $"ObfuzKlassRegisterAutoGen_{suffix}";
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("using Obfuz;");
        stringBuilder.AppendLine($"public class {klassName}");
        stringBuilder.AppendLine("{");

        stringBuilder.AppendLine("    public static void Register()");
        stringBuilder.AppendLine("    {");
        for (var index = 0; index < groups; index++)
        {
            stringBuilder.AppendLine($"        _internalRegister{index}();");
        }

        stringBuilder.AppendLine("    }");
        stringBuilder.AppendLine();
        for (var gindex = 0; gindex < groups; gindex++)
        {
            stringBuilder.AppendLine($"    private static void _internalRegister{gindex}()");
            stringBuilder.AppendLine("     {");
            for (var kindex = 0; kindex < splitCount; kindex++)
            {
                var tindex = kindex + splitCount * gindex;
                if (tindex >= klassList.Count)
                    break;
                var klass = klassList[tindex];
                var klassEx = GetTypeExpression(klass);

                var fullName = customExportMap.TryGetValue(klass, out var value) ? value : GetFullTypeName(klass);
                stringBuilder.AppendLine(
                    $"        ObfuscationTypeMapper.RegisterType(typeof({klassEx}), \"{fullName}\");");
            }

            stringBuilder.AppendLine("     }");
        }

        stringBuilder.AppendLine("}");
        var fileName = Path.Combine(savePath, $"{klassName}.cs");
        File.WriteAllText(fileName, stringBuilder.ToString());
        AssetDatabase.ImportAsset(fileName);
    }

    /// <summary>
    /// 获取带有ObfuzExportAttribute特性的类型
    /// </summary>
    /// <returns></returns>
    private static List<Type> GetExportKlassWithObfuzExportAttribute()
    {
        var typesWithAttribute = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
            {
                var attributes = type.GetCustomAttributes(typeof(ObfuzExportAttribute), false);
                var valid = attributes.Length > 0;
                if (valid && attributes[0] is ObfuzExportAttribute export)
                {
                    var exportName = export.ExportName;
                    if (!string.IsNullOrEmpty(exportName))
                        customExportMap.TryAdd(type, exportName);
                }

                return valid;
            })
            .ToList();
        return typesWithAttribute;
    }

    /// <summary>
    /// 将类型列表按AOT程序集和Hotupdate程序集拆分成两组
    /// </summary>
    /// <param name="all"></param>
    /// <param name="aotKlassList"></param>
    /// <param name="hotupdateKlassList"></param>
    private static void SplitKlassList(List<Type> all, out List<Type> aotKlassList, out List<Type> hotupdateKlassList)
    {
        aotKlassList = new List<Type>();
        hotupdateKlassList = new List<Type>();
        var allhotUpdateAssemblies = HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyNamesIncludePreserved;
        foreach (var klass in all)
        {
            var assembly = klass.Assembly.GetName().Name;
            if (allhotUpdateAssemblies.Contains(assembly))
            {
                hotupdateKlassList.Add(klass);
            }
            else
            {
                aotKlassList.Add(klass);
            }
        }
    }

    /// <summary>
    /// 处理类型表达式生成
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetTypeExpression(Type type)
    {
        if (type.IsGenericType && type.IsGenericTypeDefinition)
        {
            return GetOpenGenericTypeExpression(type);
        }
        else if (type.IsGenericType)
        {
            return GetClosedGenericTypeExpression(type);
        }
        else if (type.IsNested)
        {
            return GetNestedTypeExpression(type);
        }
        else
        {
            return GetSimpleTypeExpression(type);
        }
    }

    /// <summary>
    /// 处理未指定泛型参数的泛型类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetOpenGenericTypeExpression(Type type)
    {
        var genericTypeName = type.Name.Split('`')[0];
        var typeParamCount = type.GetGenericArguments().Length;
        var typeParams = typeParamCount > 0 ? $"<{new string(',', typeParamCount - 1)}>" : "";
        if (type.IsNested)
        {
            return GetNestedGenericTypeExpression(type, genericTypeName, typeParams);
        }

        if (string.IsNullOrEmpty(type.Namespace))
        {
            return $"{genericTypeName}{typeParams}";
        }

        return $"{type.Namespace}.{genericTypeName}{typeParams}";
    }

    /// <summary>
    /// 处理已指定泛型参数的泛型类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetClosedGenericTypeExpression(Type type)
    {
        var genericTypeDef = type.GetGenericTypeDefinition();
        var genericArgs = type.GetGenericArguments();
        var genericTypeName = genericTypeDef.Name.Split('`')[0];
        if (string.IsNullOrEmpty(genericTypeDef.Namespace))
        {
            genericTypeName = genericTypeDef.Name.Split('`')[0];
        }
        else
        {
            genericTypeName = $"{genericTypeDef.Namespace}.{genericTypeDef.Name.Split('`')[0]}";
        }

        if (type.IsNested)
        {
            return GetNestedGenericTypeExpression(type, genericTypeName, "");
        }

        var argExpressions = new List<string>();
        foreach (var arg in genericArgs)
        {
            argExpressions.Add(GetTypeExpression(arg));
        }

        return $"{genericTypeName}<{string.Join(", ", argExpressions)}>";
    }

    /// <summary>
    /// 处理嵌套泛型类型
    /// </summary>
    /// <param name="type"></param>
    /// <param name="typeName"></param>
    /// <param name="typeParams"></param>
    /// <returns></returns>
    private static string GetNestedGenericTypeExpression(Type type, string typeName, string typeParams)
    {
        var typePath = new List<string>();
        var currentType = type;
        while (currentType != null)
        {
            if (currentType.IsGenericType)
            {
                var currentTypeName = currentType.Name.Split('`')[0];
                if (currentType.IsGenericTypeDefinition)
                {
                    // 处理未指定参数的泛型类型
                    int currentTypeParamCount = currentType.GetGenericArguments().Length;
                    string currentTypeParams = currentTypeParamCount > 0
                        ? $"<{new string(',', currentTypeParamCount - 1)}>"
                        : "";
                    typePath.Insert(0, $"{currentTypeName}{currentTypeParams}");
                }
                else
                {
                    // 处理已指定参数的泛型类型
                    var currentGenericArgs = currentType.GetGenericArguments();
                    var currentArgExpressions = new List<string>();
                    foreach (var arg in currentGenericArgs)
                    {
                        currentArgExpressions.Add(GetTypeExpression(arg));
                    }

                    typePath.Insert(0, $"{currentTypeName}<{string.Join(", ", currentArgExpressions)}>");
                }
            }
            else
            {
                typePath.Insert(0, currentType.Name);
            }

            currentType = currentType.DeclaringType;
        }

        var namespacePart = string.IsNullOrEmpty(type.Namespace) ? "" : $"{type.Namespace}.";
        return $"{namespacePart}{string.Join(".", typePath)}";
    }

    /// <summary>
    /// 处理嵌套类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetNestedTypeExpression(Type type)
    {
        var typePath = new List<string>();
        var currentType = type;

        while (currentType != null)
        {
            typePath.Insert(0, GetTypeNameWithoutGenericArity(currentType));
            currentType = currentType.DeclaringType;
        }

        var namespacePart = string.IsNullOrEmpty(type.Namespace) ? "" : $"{type.Namespace}.";
        return $"{namespacePart}{string.Join(".", typePath)}";
    }

    /// <summary>
    /// 处理简单类型（非泛型、非嵌套）
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetSimpleTypeExpression(Type type)
    {
        if (string.IsNullOrEmpty(type.Namespace))
        {
            return $"{type.Name}";
        }

        return $"{type.Namespace}.{type.Name}";
    }

    /// <summary>
    /// 获取不带泛型参数的类型名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetTypeNameWithoutGenericArity(Type type)
    {
        return type.Name.Contains("`") ? type.Name.Split('`')[0] : type.Name;
    }

    /// <summary>
    /// 处理嵌套类型的完整名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetFullTypeName(Type type)
    {
        if (type.DeclaringType == null)
        {
            return string.IsNullOrEmpty(type.Namespace) ? type.Name : $"{type.Namespace}.{type.Name}";
        }

        return $"{GetFullTypeName(type.DeclaringType)}+{type.Name}";
    }
}