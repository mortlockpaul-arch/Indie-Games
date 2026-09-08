using System;

namespace MonoMod.RuntimeDetour;

public sealed class DetourNativeARMPlatform : IDetourNativePlatform
{
	public void Apply(NativeDetourData detour)
	{
		throw new NotImplementedException();
	}

	public void Copy(IntPtr src, IntPtr dst, int size)
	{
		throw new NotImplementedException();
	}

	public NativeDetourData Create(IntPtr from, IntPtr to)
	{
		throw new NotImplementedException();
	}

	public void Free(NativeDetourData detour)
	{
		throw new NotImplementedException();
	}

	public void MakeExecutable(NativeDetourData detour)
	{
		throw new NotImplementedException();
	}

	public void MakeWritable(NativeDetourData detour)
	{
		throw new NotImplementedException();
	}

	public IntPtr MemAlloc(int size)
	{
		throw new NotImplementedException();
	}

	public void MemFree(IntPtr ptr)
	{
		throw new NotImplementedException();
	}
}
