using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace MonoMod.Utils;

public static class DynamicMethodHelper
{
	private static List<object> References = new List<object>();

	private static readonly MethodInfo _GetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[1] { typeof(RuntimeMethodHandle) });

	private static readonly MethodInfo _GetReference = typeof(DynamicMethodHelper).GetMethod("GetReference");

	public static object GetReference(int id)
	{
		return References[id];
	}

	private static int AddReference(object obj)
	{
		lock (References)
		{
			References.Add(obj);
			return References.Count - 1;
		}
	}

	public static void FreeReference(int id)
	{
		References[id] = null;
	}

	public static DynamicMethod Stub(this DynamicMethod dm)
	{
		ILGenerator iLGenerator = dm.GetILGenerator();
		for (int i = 0; i < 10; i++)
		{
			iLGenerator.Emit(OpCodes.Nop);
		}
		if (dm.ReturnType != typeof(void))
		{
			iLGenerator.DeclareLocal(dm.ReturnType);
			iLGenerator.Emit(OpCodes.Ldloca_S, (sbyte)0);
			iLGenerator.Emit(OpCodes.Initobj, dm.ReturnType);
			iLGenerator.Emit(OpCodes.Ldloc_0);
		}
		iLGenerator.Emit(OpCodes.Ret);
		return dm;
	}

	public static void EmitMethodOf(this ILGenerator il, MethodBase method)
	{
		if (method is MethodInfo)
		{
			il.Emit(OpCodes.Call, (MethodInfo)method);
		}
		else
		{
			if (!(method is ConstructorInfo))
			{
				throw new NotSupportedException($"Method type {method.GetType().FullName} not supported.");
			}
			il.Emit(OpCodes.Call, (ConstructorInfo)method);
		}
		il.Emit(OpCodes.Call, _GetMethodFromHandle);
	}

	public static int EmitReference<T>(this ILGenerator il, T obj)
	{
		Type typeFromHandle = typeof(T);
		int num = AddReference(obj);
		il.Emit(OpCodes.Ldc_I4, num);
		il.Emit(OpCodes.Call, _GetReference);
		if (typeFromHandle.IsValueType)
		{
			il.Emit(OpCodes.Unbox_Any, typeFromHandle);
		}
		return num;
	}
}
