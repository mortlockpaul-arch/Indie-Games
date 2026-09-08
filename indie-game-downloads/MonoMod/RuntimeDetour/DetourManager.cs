using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour;

public static class DetourManager
{
	public static IDetourRuntimePlatform Runtime;

	public static IDetourNativePlatform Native;

	private static readonly FieldInfo _Native;

	private static readonly MethodInfo _ToNativeDetourData;

	private static readonly MethodInfo _Copy;

	private static readonly MethodInfo _Apply;

	private static readonly ConstructorInfo _ctor_Exception;

	static DetourManager()
	{
		_Native = typeof(DetourManager).GetField("Native");
		_ToNativeDetourData = typeof(DetourManager).GetMethod("ToNativeDetourData", BindingFlags.Static | BindingFlags.NonPublic);
		_Copy = typeof(IDetourNativePlatform).GetMethod("Copy");
		_Apply = typeof(IDetourNativePlatform).GetMethod("Apply");
		_ctor_Exception = typeof(Exception).GetConstructor(new Type[1] { typeof(string) });
		if (Type.GetType("Mono.Runtime") != null)
		{
			Runtime = new DetourRuntimeMonoPlatform();
		}
		else
		{
			Runtime = new DetourRuntimeNETPlatform();
		}
		Native = new DetourNativeX86Platform();
		if ((PlatformHelper.Current & Platform.Windows) == Platform.Windows)
		{
			Native = new DetourNativeWindowsPlatform(Native);
		}
	}

	public unsafe static void Write(this IntPtr to, ref int offs, byte value)
	{
		*(byte*)((long)to + offs) = value;
		offs++;
	}

	public unsafe static void Write(this IntPtr to, ref int offs, ushort value)
	{
		*(ushort*)((long)to + offs) = value;
		offs += 2;
	}

	public unsafe static void Write(this IntPtr to, ref int offs, uint value)
	{
		*(uint*)((long)to + offs) = value;
		offs += 4;
	}

	public unsafe static void Write(this IntPtr to, ref int offs, ulong value)
	{
		*(ulong*)((long)to + offs) = value;
		offs += 8;
	}

	public static IntPtr GetNativeStart(this MethodBase method)
	{
		return Runtime.GetNativeStart(method.Pin());
	}

	public static IntPtr GetNativeStart(this Delegate method)
	{
		return method.Method.GetNativeStart();
	}

	public static IntPtr GetNativeStart(this Expression method)
	{
		return ((MethodCallExpression)method).Method.GetNativeStart();
	}

	public static DynamicMethod CreateILCopy(this MethodBase method)
	{
		return Runtime.CreateCopy(method);
	}

	public static T Pin<T>(this T method) where T : MethodBase
	{
		Runtime.Pin(method);
		return method;
	}

	public static DynamicMethod GenerateNativeProxy(IntPtr target, MethodBase signature)
	{
		Type returnType = (signature as MethodInfo)?.ReturnType ?? typeof(void);
		ParameterInfo[] parameters = signature.GetParameters();
		Type[] array = new Type[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			array[i] = parameters[i].ParameterType;
		}
		DynamicMethod method = new DynamicMethod(string.Format("native_{0}", ((long)target).ToString("X16")), returnType, array, restrictedSkipVisibility: true).StubCriticalDetour();
		NativeDetourData detour = Native.Create(method.GetNativeStart(), target);
		Native.MakeWritable(detour);
		Native.Apply(detour);
		Native.MakeExecutable(detour);
		Native.Free(detour);
		return method.Pin();
	}

	private static NativeDetourData ToNativeDetourData(IntPtr method, IntPtr target, int size, IntPtr extra)
	{
		return new NativeDetourData
		{
			Method = method,
			Target = target,
			Size = size,
			Extra = extra
		};
	}

	public static DynamicMethod StubCriticalDetour(this DynamicMethod dm)
	{
		ILGenerator iLGenerator = dm.GetILGenerator();
		for (int i = 0; i < 10; i++)
		{
			iLGenerator.Emit(OpCodes.Nop);
		}
		iLGenerator.Emit(OpCodes.Ldstr, $"{dm.Name} should've been detoured!");
		iLGenerator.Emit(OpCodes.Newobj, _ctor_Exception);
		iLGenerator.Emit(OpCodes.Throw);
		return dm;
	}

	public static void EmitDetourCopy(this ILGenerator il, IntPtr src, IntPtr dst, int size)
	{
		il.Emit(OpCodes.Ldsfld, _Native);
		il.Emit(OpCodes.Ldc_I8, (long)src);
		il.Emit(OpCodes.Conv_I);
		il.Emit(OpCodes.Ldc_I8, (long)dst);
		il.Emit(OpCodes.Conv_I);
		il.Emit(OpCodes.Ldc_I4, size);
		il.Emit(OpCodes.Callvirt, _Copy);
	}

	public static void EmitDetourApply(this ILGenerator il, NativeDetourData data)
	{
		il.Emit(OpCodes.Ldsfld, _Native);
		il.Emit(OpCodes.Ldc_I8, (long)data.Method);
		il.Emit(OpCodes.Conv_I);
		il.Emit(OpCodes.Ldc_I8, (long)data.Target);
		il.Emit(OpCodes.Conv_I);
		il.Emit(OpCodes.Ldc_I4, data.Size);
		il.Emit(OpCodes.Ldc_I8, (long)data.Extra);
		il.Emit(OpCodes.Conv_I);
		il.Emit(OpCodes.Call, _ToNativeDetourData);
		il.Emit(OpCodes.Callvirt, _Apply);
	}
}
