using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace MonoMod.Utils;

[MonoMod__OldName__("MonoMod.Helpers.ReflectionHelper")]
public static class FastReflectionHelper
{
	private static readonly Type[] _DynamicMethodDelegateArgs = new Type[2]
	{
		typeof(object),
		typeof(object[])
	};

	private static readonly IDictionary<MethodInfo, FastReflectionDelegate> _MethodCache = new Dictionary<MethodInfo, FastReflectionDelegate>();

	private static readonly MethodInfo m_Console_WriteLine = typeof(Console).GetMethod("WriteLine", new Type[1] { typeof(object) });

	private static readonly MethodInfo m_object_GetType = typeof(object).GetMethod("GetType");

	[Obsolete("Use CreateFastDelegate instead.")]
	public static FastReflectionDelegate CreateDelegate(MethodBase method, bool directBoxValueAccess = true)
	{
		return method.CreateFastDelegate(directBoxValueAccess);
	}

	public unsafe static FastReflectionDelegate CreateFastDelegate(this MethodBase method, bool directBoxValueAccess = true)
	{
		DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), _DynamicMethodDelegateArgs, typeof(FastReflectionHelper).Module, skipVisibility: true);
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		ParameterInfo[] parameters = method.GetParameters();
		bool flag = true;
		if (!method.IsStatic)
		{
			iLGenerator.Emit(OpCodes.Ldarg_0);
			if (method.DeclaringType.IsValueType)
			{
				iLGenerator.Emit(OpCodes.Unbox_Any, method.DeclaringType);
			}
		}
		for (int i = 0; i < parameters.Length; i++)
		{
			Type type = parameters[i].ParameterType;
			bool isByRef = type.IsByRef;
			if (isByRef)
			{
				type = type.GetElementType();
			}
			bool isValueType = type.IsValueType;
			if ((isByRef & isValueType) && !directBoxValueAccess)
			{
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.EmitFast_Ldc_I4(i);
			}
			iLGenerator.Emit(OpCodes.Ldarg_1);
			iLGenerator.EmitFast_Ldc_I4(i);
			if (isByRef && !isValueType)
			{
				iLGenerator.Emit(OpCodes.Ldelema, typeof(object));
				continue;
			}
			iLGenerator.Emit(OpCodes.Ldelem_Ref);
			if (!isValueType)
			{
				continue;
			}
			if (!isByRef || !directBoxValueAccess)
			{
				iLGenerator.Emit(OpCodes.Unbox_Any, type);
				if (isByRef)
				{
					iLGenerator.Emit(OpCodes.Box, type);
					iLGenerator.Emit(OpCodes.Dup);
					iLGenerator.Emit(OpCodes.Unbox, type);
					if (flag)
					{
						flag = false;
						iLGenerator.DeclareLocal(typeof(void*), pinned: true);
					}
					iLGenerator.Emit(OpCodes.Stloc_0);
					iLGenerator.Emit(OpCodes.Stelem_Ref);
					iLGenerator.Emit(OpCodes.Ldloc_0);
				}
			}
			else
			{
				iLGenerator.Emit(OpCodes.Unbox, type);
			}
		}
		if (method.IsConstructor)
		{
			iLGenerator.Emit(OpCodes.Newobj, method as ConstructorInfo);
		}
		else if (method.IsFinal || !method.IsVirtual)
		{
			iLGenerator.Emit(OpCodes.Call, method as MethodInfo);
		}
		else
		{
			iLGenerator.Emit(OpCodes.Callvirt, method as MethodInfo);
		}
		Type type2 = (method.IsConstructor ? method.DeclaringType : (method as MethodInfo).ReturnType);
		if (type2 != typeof(void))
		{
			if (type2.IsValueType)
			{
				iLGenerator.Emit(OpCodes.Box, type2);
			}
		}
		else
		{
			iLGenerator.Emit(OpCodes.Ldnull);
		}
		iLGenerator.Emit(OpCodes.Ret);
		return (FastReflectionDelegate)dynamicMethod.CreateDelegate(typeof(FastReflectionDelegate));
	}

	public static T CreateJmpDelegate<T>(this MethodBase method)
	{
		Type typeFromHandle = typeof(T);
		MethodInfo method2 = typeFromHandle.GetMethod("Invoke");
		ParameterInfo[] parameters = method2.GetParameters();
		Type[] array = new Type[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			array[i] = parameters[i].ParameterType;
		}
		DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, method2.ReturnType, array, typeof(FastReflectionHelper).Module, skipVisibility: true);
		dynamicMethod.GetILGenerator().Emit(OpCodes.Jmp, (MethodInfo)method);
		return (T)(object)dynamicMethod.CreateDelegate(typeFromHandle);
	}

	[Obsolete("Use GetFastDelegate instead.")]
	public static FastReflectionDelegate GetDelegate(MethodInfo method, bool directBoxValueAccess = true)
	{
		return method.GetFastDelegate(directBoxValueAccess);
	}

	public static FastReflectionDelegate GetFastDelegate(this MethodInfo method, bool directBoxValueAccess = true)
	{
		if (_MethodCache.TryGetValue(method, out var value))
		{
			return value;
		}
		value = method.CreateFastDelegate(directBoxValueAccess);
		_MethodCache.Add(method, value);
		return value;
	}

	private static void EmitFast_Ldc_I4(this ILGenerator il, int value)
	{
		switch (value)
		{
		case -1:
			il.Emit(OpCodes.Ldc_I4_M1);
			return;
		case 0:
			il.Emit(OpCodes.Ldc_I4_0);
			return;
		case 1:
			il.Emit(OpCodes.Ldc_I4_1);
			return;
		case 2:
			il.Emit(OpCodes.Ldc_I4_2);
			return;
		case 3:
			il.Emit(OpCodes.Ldc_I4_3);
			return;
		case 4:
			il.Emit(OpCodes.Ldc_I4_4);
			return;
		case 5:
			il.Emit(OpCodes.Ldc_I4_5);
			return;
		case 6:
			il.Emit(OpCodes.Ldc_I4_6);
			return;
		case 7:
			il.Emit(OpCodes.Ldc_I4_7);
			return;
		case 8:
			il.Emit(OpCodes.Ldc_I4_8);
			return;
		}
		if (value > -129 && value < 128)
		{
			il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
		}
		else
		{
			il.Emit(OpCodes.Ldc_I4, value);
		}
	}
}
