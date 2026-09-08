using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Harmony.ILCopying;

namespace MonoMod.RuntimeDetour;

public abstract class DetourRuntimeILPlatform : IDetourRuntimePlatform
{
	protected HashSet<DynamicMethod> PinnedDynamicMethods = new HashSet<DynamicMethod>();

	protected abstract RuntimeMethodHandle GetMethodHandle(MethodBase method);

	public IntPtr GetNativeStart(MethodBase method)
	{
		return GetMethodHandle(method).GetFunctionPointer();
	}

	public void Pin(MethodBase method)
	{
		RuntimeMethodHandle methodHandle = GetMethodHandle(method);
		if (method is DynamicMethod)
		{
			DynamicMethod item = (DynamicMethod)method;
			PinnedDynamicMethods.Add(item);
		}
		RuntimeHelpers.PrepareMethod(methodHandle);
	}

	public DynamicMethod CreateCopy(MethodBase method)
	{
		if (method.GetMethodBody() == null)
		{
			throw new InvalidOperationException("P/Invoke methods cannot be copied!");
		}
		ParameterInfo[] parameters = method.GetParameters();
		Type[] array;
		if (!method.IsStatic)
		{
			array = new Type[parameters.Length + 1];
			array[0] = method.DeclaringType;
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i + 1] = parameters[i].ParameterType;
			}
		}
		else
		{
			array = new Type[parameters.Length];
			for (int j = 0; j < parameters.Length; j++)
			{
				array[j] = parameters[j].ParameterType;
			}
		}
		DynamicMethod dynamicMethod = new DynamicMethod($"orig_{method.Name}", (method as MethodInfo)?.ReturnType ?? typeof(void), array, method.DeclaringType, skipVisibility: false);
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		List<Label> list = new List<Label>();
		List<ExceptionBlock> list2 = new List<ExceptionBlock>();
		new MethodCopier(method, iLGenerator).Finalize(list, list2);
		foreach (Label item in list)
		{
			iLGenerator.MarkLabel(item);
		}
		foreach (ExceptionBlock item2 in list2)
		{
			Emitter.MarkBlockAfter(iLGenerator, item2);
		}
		return dynamicMethod.Pin();
	}
}
