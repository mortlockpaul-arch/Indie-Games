using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour;

public sealed class DetourRuntimeNETPlatform : DetourRuntimeILPlatform
{
	private static readonly FieldInfo f_DynamicMethod_m_method = typeof(DynamicMethod).GetField("m_method", BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly MethodInfo m_DynamicMethod_GetMethodDescriptor = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly FastReflectionDelegate dmd_DynamicMethod_GetMethodDescriptor = m_DynamicMethod_GetMethodDescriptor?.CreateFastDelegate();

	private static readonly MethodInfo m_RuntimeHelpers__CompileMethod = typeof(RuntimeHelpers).GetMethod("_CompileMethod", BindingFlags.Static | BindingFlags.NonPublic);

	private static readonly FastReflectionDelegate dmd_RuntimeHelpers__CompileMethod = m_RuntimeHelpers__CompileMethod?.CreateFastDelegate();

	private static readonly bool m_RuntimeHelpers__CompileMethod_TakesIntPtr = m_RuntimeHelpers__CompileMethod != null && m_RuntimeHelpers__CompileMethod.GetParameters()[0].ParameterType.FullName == "System.IntPtr";

	private static readonly bool m_RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo = m_RuntimeHelpers__CompileMethod != null && m_RuntimeHelpers__CompileMethod.GetParameters()[0].ParameterType.FullName == "System.IRuntimeMethodInfo";

	private static readonly MethodInfo m_RuntimeMethodHandle_GetMethodInfo = typeof(RuntimeMethodHandle).GetMethod("GetMethodInfo", BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly FastReflectionDelegate dmd_RuntimeMethodHandle_GetMethodInfo = m_RuntimeMethodHandle_GetMethodInfo?.CreateFastDelegate();

	protected override RuntimeMethodHandle GetMethodHandle(MethodBase method)
	{
		if (method is DynamicMethod)
		{
			DynamicMethod dynamicMethod = (DynamicMethod)method;
			try
			{
				dynamicMethod.CreateDelegate(typeof(MulticastDelegate));
			}
			catch
			{
			}
			if (f_DynamicMethod_m_method != null)
			{
				return (RuntimeMethodHandle)f_DynamicMethod_m_method.GetValue(method);
			}
			if (dmd_DynamicMethod_GetMethodDescriptor != null)
			{
				return (RuntimeMethodHandle)dmd_DynamicMethod_GetMethodDescriptor(method);
			}
		}
		return method.MethodHandle;
	}
}
