using System.Runtime.Versioning;

namespace System.Runtime.InteropServices.Java;

[CLSCompliant(false)]
[SupportedOSPlatform("android")]
public struct ComponentCrossReference
{
	public nuint SourceGroupIndex;

	public nuint DestinationGroupIndex;
}
