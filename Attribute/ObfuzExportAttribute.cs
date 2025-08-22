using System;
using System.Diagnostics;

[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Delegate |
                AttributeTargets.Interface | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
public class ObfuzExportAttribute : Attribute
{
    public string ExportName;
}