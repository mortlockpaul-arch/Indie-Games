using System.Runtime.InteropServices;

namespace System.Runtime;

[StructLayout(LayoutKind.Explicit, Size = 3056)]
internal struct REGDISPLAY
{
	[FieldOffset(3024)]
	internal nuint SP;

	[FieldOffset(3032)]
	internal nint ControlPC;

	[FieldOffset(8)]
	internal unsafe EH.PAL_LIMITED_CONTEXT* m_pCurrentContext;
}
