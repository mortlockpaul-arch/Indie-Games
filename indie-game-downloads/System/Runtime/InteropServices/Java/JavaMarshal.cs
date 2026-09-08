using System.Runtime.Versioning;

namespace System.Runtime.InteropServices.Java;

[CLSCompliant(false)]
[SupportedOSPlatform("android")]
public static class JavaMarshal
{
	public unsafe static void Initialize(delegate* unmanaged<MarkCrossReferencesArgs*, void> markCrossReferences)
	{
		throw new PlatformNotSupportedException();
	}

	public unsafe static GCHandle CreateReferenceTrackingHandle(object obj, void* context)
	{
		throw new PlatformNotSupportedException();
	}

	public unsafe static void* GetContext(GCHandle obj)
	{
		throw new PlatformNotSupportedException();
	}

	public unsafe static void FinishCrossReferenceProcessing(MarkCrossReferencesArgs* crossReferences, ReadOnlySpan<GCHandle> unreachableObjectHandles)
	{
		throw new PlatformNotSupportedException();
	}
}
