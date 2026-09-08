namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
public sealed class UnsafeAccessorTypeAttribute : Attribute
{
	public string TypeName { get; }

	public UnsafeAccessorTypeAttribute(string typeName)
	{
		TypeName = typeName;
	}
}
