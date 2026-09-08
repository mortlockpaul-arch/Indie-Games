using System.Runtime.Versioning;

namespace System.Runtime.CompilerServices;

[InlineArray(3)]
[NonVersionable]
public struct InlineArray3<T>
{
	private T t;
}
