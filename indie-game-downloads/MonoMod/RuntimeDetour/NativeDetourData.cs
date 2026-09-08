using System;

namespace MonoMod.RuntimeDetour;

public struct NativeDetourData
{
	public IntPtr Method;

	public IntPtr Target;

	public int Size;

	public IntPtr Extra;
}
