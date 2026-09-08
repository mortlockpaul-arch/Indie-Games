using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[StructLayout(LayoutKind.Explicit)]
internal struct EntryInfo
{
	[FieldOffset(0)]
	internal uint _version;

	[FieldOffset(0)]
	internal byte hashShift;

	[FieldOffset(1)]
	internal byte victimCounter;
}
