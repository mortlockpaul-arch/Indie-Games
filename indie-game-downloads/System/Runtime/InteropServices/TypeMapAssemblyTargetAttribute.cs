namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class TypeMapAssemblyTargetAttribute<TTypeMapGroup> : Attribute
{
	public TypeMapAssemblyTargetAttribute(string assemblyName)
	{
	}
}
