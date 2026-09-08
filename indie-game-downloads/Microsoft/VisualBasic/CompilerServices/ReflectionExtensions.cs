using System;

namespace Microsoft.VisualBasic.CompilerServices;

[StandardModule]
internal sealed class ReflectionExtensions
{
	public static TypeCode GetTypeCode(this Type type)
	{
		return Type.GetTypeCode(type);
	}
}
