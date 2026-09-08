using System.Runtime.Versioning;

namespace System.Runtime.InteropServices.Java;

[CLSCompliant(false)]
[SupportedOSPlatform("android")]
public struct MarkCrossReferencesArgs
{
	public nuint ComponentCount;

	public unsafe StronglyConnectedComponent* Components;

	public nuint CrossReferenceCount;

	public unsafe ComponentCrossReference* CrossReferences;
}
