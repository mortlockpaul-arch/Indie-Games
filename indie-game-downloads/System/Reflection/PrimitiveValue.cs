using System.Runtime.InteropServices;

namespace System.Reflection;

[StructLayout(LayoutKind.Explicit)]
internal struct PrimitiveValue
{
	[FieldOffset(0)]
	public int Byte4;

	[FieldOffset(0)]
	public long Byte8;
}
