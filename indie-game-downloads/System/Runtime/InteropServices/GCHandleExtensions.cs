using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices;

public static class GCHandleExtensions
{
	[CLSCompliant(false)]
	public unsafe static T* GetAddressOfArrayData<T>(this PinnedGCHandle<T[]> handle)
	{
		T[] target = handle.Target;
		if (target == null)
		{
			return null;
		}
		return (T*)Unsafe.AsPointer(in MemoryMarshal.GetArrayDataReference(target));
	}

	[CLSCompliant(false)]
	public unsafe static char* GetAddressOfStringData(this PinnedGCHandle<string> handle)
	{
		string target = handle.Target;
		if (target == null)
		{
			return null;
		}
		return (char*)Unsafe.AsPointer(in target.GetRawStringData());
	}
}
