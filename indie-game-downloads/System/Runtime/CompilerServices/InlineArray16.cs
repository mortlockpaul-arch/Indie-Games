using System.Runtime.Versioning;

namespace System.Runtime.CompilerServices;

[InlineArray(16)]
[NonVersionable]
public struct InlineArray16<T>
{
	private T t;
}
