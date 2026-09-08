using System.Runtime.Versioning;

namespace System.Runtime.InteropServices.Java;

[CLSCompliant(false)]
[SupportedOSPlatform("android")]
public struct StronglyConnectedComponent
{
	public nuint Count;

	public unsafe void** Contexts;
}
