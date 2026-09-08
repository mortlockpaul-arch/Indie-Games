using System;
using System.Runtime.InteropServices;

namespace MonoMod.RuntimeDetour;

public sealed class DetourNativeX86Platform : IDetourNativePlatform
{
	private const int SIZE64BIT = 14;

	private const int SIZE32BIT = 6;

	private static bool Is64BitTarget(IntPtr to)
	{
		return ((long)to & 0xFFFFFFFFu) != (long)to;
	}

	private static int Size(IntPtr to)
	{
		if (Is64BitTarget(to))
		{
			return 14;
		}
		return 6;
	}

	public NativeDetourData Create(IntPtr from, IntPtr to)
	{
		NativeDetourData result = new NativeDetourData
		{
			Method = from,
			Target = to
		};
		result.Size = Size(to);
		return result;
	}

	public void Free(NativeDetourData detour)
	{
	}

	public void Apply(NativeDetourData detour)
	{
		int offs = 0;
		if (Is64BitTarget(detour.Target))
		{
			detour.Method.Write(ref offs, byte.MaxValue);
			detour.Method.Write(ref offs, 37);
			detour.Method.Write(ref offs, 0u);
			detour.Method.Write(ref offs, (ulong)(long)detour.Target);
		}
		else
		{
			detour.Method.Write(ref offs, 104);
			detour.Method.Write(ref offs, (uint)(int)detour.Target);
			detour.Method.Write(ref offs, 195);
		}
	}

	public unsafe void Copy(IntPtr src, IntPtr dst, int size)
	{
		switch (size)
		{
		case 14:
			*(long*)(long)dst = *(long*)(long)src;
			*(int*)((long)dst + 8) = *(int*)((long)src + 8);
			*(short*)((long)dst + 12) = *(short*)((long)src + 12);
			break;
		case 6:
			*(int*)(uint)(int)dst = *(int*)(uint)(int)src;
			*(short*)(uint)((int)dst + 4) = *(short*)(uint)((int)src + 4);
			break;
		default:
			throw new Exception($"Unknown X86 detour size {size}");
		}
	}

	public void MakeWritable(NativeDetourData detour)
	{
	}

	public void MakeExecutable(NativeDetourData detour)
	{
	}

	public IntPtr MemAlloc(int size)
	{
		return Marshal.AllocHGlobal(size);
	}

	public void MemFree(IntPtr ptr)
	{
		Marshal.FreeHGlobal(ptr);
	}
}
