using System.Runtime.Versioning;

namespace System.Runtime.CompilerServices;

[InlineArray(5)]
[NonVersionable]
public struct InlineArray5<T>
{
	private T t;
}
