using System;

namespace MonoMod.RuntimeDetour;

public interface IDetourNativePlatform
{
	NativeDetourData Create(IntPtr from, IntPtr to);

	void Free(NativeDetourData detour);

	void Apply(NativeDetourData detour);

	void Copy(IntPtr src, IntPtr dst, int size);

	void MakeWritable(NativeDetourData detour);

	void MakeExecutable(NativeDetourData detour);

	IntPtr MemAlloc(int size);

	void MemFree(IntPtr ptr);
}
