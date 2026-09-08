using System.Runtime.Versioning;

namespace System.Runtime.CompilerServices;

[InlineArray(2)]
[NonVersionable]
public struct InlineArray2<T>
{
	private T t;
}
