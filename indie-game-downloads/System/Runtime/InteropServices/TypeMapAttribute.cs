using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class TypeMapAttribute<TTypeMapGroup> : Attribute
{
	public TypeMapAttribute(string value, Type target)
	{
	}

	[RequiresUnreferencedCode("Interop types may be removed by trimming")]
	public TypeMapAttribute(string value, Type target, Type trimTarget)
	{
	}
}
