using System.Runtime.Versioning;

namespace System.Runtime.CompilerServices;

[InlineArray(10)]
[NonVersionable]
public struct InlineArray10<T>
{
	private T t;
}
