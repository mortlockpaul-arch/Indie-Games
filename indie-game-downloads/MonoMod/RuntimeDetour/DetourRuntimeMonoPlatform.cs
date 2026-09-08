using System;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour;

public sealed class DetourRuntimeMonoPlatform : DetourRuntimeILPlatform
{
	private static readonly MethodInfo m_DynamicMethod_CreateDynMethod = typeof(DynamicMethod).GetMethod("CreateDynMethod", BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly FastReflectionDelegate dmd_DynamicMethod_CreateDynMethod = m_DynamicMethod_CreateDynMethod?.CreateFastDelegate();

	private static readonly FieldInfo f_DynamicMethod_mhandle = typeof(DynamicMethod).GetField("mhandle", BindingFlags.Instance | BindingFlags.NonPublic);

	protected override RuntimeMethodHandle GetMethodHandle(MethodBase method)
	{
		if (method is DynamicMethod)
		{
			dmd_DynamicMethod_CreateDynMethod?.Invoke(method);
			if (f_DynamicMethod_mhandle != null)
			{
				return (RuntimeMethodHandle)f_DynamicMethod_mhandle.GetValue(method);
			}
		}
		return method.MethodHandle;
	}
}
