using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices.Swift;

[CLSCompliant(false)]
[Intrinsic]
public unsafe readonly struct SwiftSelf(void* value)
{
	public unsafe void* Value { get; } = value;
}
[Intrinsic]
public readonly struct SwiftSelf<T>(T value) where T : unmanaged
{
	public T Value { get; } = value;
}
