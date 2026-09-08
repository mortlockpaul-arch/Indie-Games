using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MonoMod.RuntimeDetour;

public sealed class DetourNativeWindowsPlatform : IDetourNativePlatform
{
	[Flags]
	private enum Protection
	{
		PAGE_NOACCESS = 1,
		PAGE_READONLY = 2,
		PAGE_READWRITE = 4,
		PAGE_WRITECOPY = 8,
		PAGE_EXECUTE = 0x10,
		PAGE_EXECUTE_READ = 0x20,
		PAGE_EXECUTE_READWRITE = 0x40,
		PAGE_EXECUTE_WRITECOPY = 0x80,
		PAGE_GUARD = 0x100,
		PAGE_NOCACHE = 0x200,
		PAGE_WRITECOMBINE = 0x400
	}

	private IDetourNativePlatform Inner;

	public DetourNativeWindowsPlatform(IDetourNativePlatform inner)
	{
		Inner = inner;
	}

	[DllImport("kernel32.dll")]
	private static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, Protection flNewProtect, out Protection lpflOldProtect);

	public void MakeWritable(NativeDetourData detour)
	{
		if (!VirtualProtect(detour.Method, (IntPtr)detour.Size, Protection.PAGE_EXECUTE_READWRITE, out var _))
		{
			throw new Win32Exception();
		}
		Inner.MakeWritable(detour);
	}

	public void MakeExecutable(NativeDetourData detour)
	{
		if (!VirtualProtect(detour.Method, (IntPtr)detour.Size, Protection.PAGE_EXECUTE_READWRITE, out var _))
		{
			throw new Win32Exception();
		}
		Inner.MakeExecutable(detour);
	}

	public NativeDetourData Create(IntPtr from, IntPtr to)
	{
		return Inner.Create(from, to);
	}

	public void Free(NativeDetourData detour)
	{
		Inner.Free(detour);
	}

	public void Apply(NativeDetourData detour)
	{
		Inner.Apply(detour);
	}

	public void Copy(IntPtr src, IntPtr dst, int size)
	{
		Inner.Copy(src, dst, size);
	}

	public IntPtr MemAlloc(int size)
	{
		return Inner.MemAlloc(size);
	}

	public void MemFree(IntPtr ptr)
	{
		Inner.MemFree(ptr);
	}
}
