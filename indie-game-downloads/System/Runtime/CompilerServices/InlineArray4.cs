using System.Runtime.Versioning;

namespace System.Runtime.CompilerServices;

[InlineArray(4)]
[NonVersionable]
public struct InlineArray4<T>
{
	private T t;
}
