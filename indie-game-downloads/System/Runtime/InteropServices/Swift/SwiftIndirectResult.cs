using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices.Swift;

[CLSCompliant(false)]
[Intrinsic]
public unsafe readonly struct SwiftIndirectResult(void* value)
{
	public unsafe void* Value { get; } = value;
}
