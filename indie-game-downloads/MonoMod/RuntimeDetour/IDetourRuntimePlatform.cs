using System;
using System.Reflection;
using System.Reflection.Emit;

namespace MonoMod.RuntimeDetour;

public interface IDetourRuntimePlatform
{
	IntPtr GetNativeStart(MethodBase method);

	DynamicMethod CreateCopy(MethodBase method);

	void Pin(MethodBase method);
}
