using System;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour;

public class NativeDetour : IDetour, IDisposable
{
	public readonly NativeDetourData Data;

	public readonly MethodBase Method;

	private DynamicMethod _BackupMethod;

	private IntPtr _BackupNative;

	private bool _IsFree;

	public bool IsValid => !_IsFree;

	public NativeDetour(MethodBase method, IntPtr from, IntPtr to)
	{
		Data = DetourManager.Native.Create(from, to);
		Method = method;
		if (Method != null && Method.GetMethodBody() != null)
		{
			_BackupMethod = method.CreateILCopy();
		}
		_BackupNative = DetourManager.Native.MemAlloc(Data.Size);
		DetourManager.Native.Copy(Data.Method, _BackupNative, Data.Size);
		Apply();
	}

	public NativeDetour(IntPtr from, IntPtr to)
		: this(null, from, to)
	{
	}

	public NativeDetour(MethodBase from, IntPtr to)
		: this(from, from.GetNativeStart(), to)
	{
	}

	public NativeDetour(IntPtr from, MethodBase to)
		: this(from, to.GetNativeStart())
	{
	}

	public NativeDetour(MethodBase from, MethodBase to)
		: this(from, to.GetNativeStart())
	{
	}

	public NativeDetour(Delegate from, IntPtr to)
		: this(from.Method, to)
	{
	}

	public NativeDetour(IntPtr from, Delegate to)
		: this(from, to.Method)
	{
	}

	public NativeDetour(Delegate from, Delegate to)
		: this(from.Method, to.Method)
	{
	}

	public void Apply()
	{
		if (_IsFree)
		{
			throw new InvalidOperationException("Free() has been called on this detour.");
		}
		DetourManager.Native.MakeWritable(Data);
		DetourManager.Native.Apply(Data);
		DetourManager.Native.MakeExecutable(Data);
	}

	public void Undo()
	{
		if (_IsFree)
		{
			throw new InvalidOperationException("Free() has been called on this detour.");
		}
		DetourManager.Native.Copy(_BackupNative, Data.Method, Data.Size);
	}

	public void Free()
	{
		if (_IsFree)
		{
			throw new InvalidOperationException("Free() has been called on this detour.");
		}
		_IsFree = true;
		DetourManager.Native.MemFree(_BackupNative);
		DetourManager.Native.Free(Data);
	}

	public void Dispose()
	{
		Undo();
		Free();
	}

	public MethodBase GenerateTrampoline(MethodBase signature = null)
	{
		if (_IsFree)
		{
			throw new InvalidOperationException("Free() has been called on this detour.");
		}
		if (_BackupMethod != null)
		{
			return _BackupMethod;
		}
		if (signature == null)
		{
			signature = _BackupMethod;
		}
		if (signature == null)
		{
			throw new ArgumentNullException("A signature must be given if the NativeDetour doesn't hold a reference to a managed method.");
		}
		MethodBase methodBase = Method;
		if (methodBase == null)
		{
			methodBase = DetourManager.GenerateNativeProxy(Data.Method, signature);
		}
		Type type = (signature as MethodInfo)?.ReturnType ?? typeof(void);
		ParameterInfo[] parameters = signature.GetParameters();
		Type[] array = new Type[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			array[i] = parameters[i].ParameterType;
		}
		string name = string.Format("trampoline_native_{0}_{1}", Method?.Name.ToString() ?? ((long)Data.Method).ToString("X16"), GetHashCode());
		DynamicMethod dynamicMethod = ((Method == null) ? new DynamicMethod(name, type, array, restrictedSkipVisibility: true) : new DynamicMethod(name, type, array, Method.DeclaringType, skipVisibility: true));
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		iLGenerator.EmitDetourCopy(_BackupNative, Data.Method, Data.Size);
		LocalBuilder localBuilder = null;
		if (type != typeof(void))
		{
			localBuilder = iLGenerator.DeclareLocal(type);
		}
		iLGenerator.BeginExceptionBlock();
		for (int j = 0; j < array.Length; j++)
		{
			iLGenerator.Emit(OpCodes.Ldarg, j);
		}
		if (methodBase is MethodInfo)
		{
			iLGenerator.Emit(OpCodes.Call, (MethodInfo)methodBase);
		}
		else
		{
			if (!(methodBase is ConstructorInfo))
			{
				throw new NotSupportedException($"Method type {methodBase.GetType().FullName} not supported.");
			}
			iLGenerator.Emit(OpCodes.Call, (ConstructorInfo)methodBase);
		}
		if (localBuilder != null)
		{
			iLGenerator.Emit(OpCodes.Stloc_0);
		}
		iLGenerator.BeginFinallyBlock();
		iLGenerator.EmitDetourApply(Data);
		iLGenerator.EndExceptionBlock();
		if (localBuilder != null)
		{
			iLGenerator.Emit(OpCodes.Ldloc_0);
		}
		iLGenerator.Emit(OpCodes.Ret);
		return dynamicMethod.Pin();
	}

	public T GenerateTrampoline<T>() where T : class
	{
		if (_IsFree)
		{
			throw new InvalidOperationException("Free() has been called on this detour.");
		}
		if (!typeof(Delegate).IsAssignableFrom(typeof(T)))
		{
			throw new InvalidOperationException($"Type {typeof(T)} not a delegate type.");
		}
		return GenerateTrampoline(typeof(T).GetMethod("Invoke")).CreateDelegate(typeof(T)) as T;
	}
}
