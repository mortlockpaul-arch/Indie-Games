using System.Runtime.Versioning;

namespace System.Runtime.CompilerServices;

[InlineArray(8)]
[NonVersionable]
public struct InlineArray8<T>
{
	private T t;
}
