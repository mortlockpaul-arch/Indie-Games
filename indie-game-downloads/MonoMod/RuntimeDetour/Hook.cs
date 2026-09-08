using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour;

public class Hook : IDetour, IDisposable
{
	public readonly MethodBase Method;

	public readonly MethodBase Target;

	private MethodInfo _Hook;

	private Detour _Detour;

	private int? _RefTarget;

	private int? _RefTrampoline;

	public bool IsValid => _Detour.IsValid;

	public Hook(MethodBase from, MethodInfo to, object target)
	{
		Method = from;
		_Hook = to;
		if (_Hook.ReturnType != ((from as MethodInfo)?.ReturnType ?? typeof(void)))
		{
			throw new InvalidOperationException($"Return type of hook for {from} doesn't match, must be {((from as MethodInfo)?.ReturnType ?? typeof(void)).FullName}");
		}
		if (target == null && !to.IsStatic)
		{
			throw new InvalidOperationException($"Hook for method {from} must be static, or you must pass a target instance.");
		}
		ParameterInfo[] parameters = _Hook.GetParameters();
		ParameterInfo[] parameters2 = Method.GetParameters();
		Type[] array;
		if (!Method.IsStatic)
		{
			array = new Type[parameters2.Length + 1];
			array[0] = Method.DeclaringType;
			for (int i = 0; i < parameters2.Length; i++)
			{
				array[i + 1] = parameters2[i].ParameterType;
			}
		}
		else
		{
			array = new Type[parameters2.Length];
			for (int j = 0; j < parameters2.Length; j++)
			{
				array[j] = parameters2[j].ParameterType;
			}
		}
		Type type = null;
		if (parameters.Length == array.Length + 1 && typeof(Delegate).IsAssignableFrom(parameters[0].ParameterType))
		{
			type = parameters[0].ParameterType;
		}
		else if (parameters.Length != array.Length)
		{
			throw new InvalidOperationException($"Parameter count of hook for {from} doesn't match, must be {array.Length}");
		}
		for (int k = 0; k < array.Length; k++)
		{
			Type type2 = array[k];
			Type parameterType = parameters[k + ((type != null) ? 1 : 0)].ParameterType;
			if (!type2.IsAssignableFrom(parameterType) && !parameterType.IsAssignableFrom(type2))
			{
				throw new InvalidOperationException($"Parameter #{k} of hook for {from} doesn't match, must be {type2.FullName} or related");
			}
		}
		MethodInfo signature = type?.GetMethod("Invoke");
		DynamicMethod dynamicMethod = null;
		if (type != null)
		{
			dynamicMethod = new DynamicMethod($"trampoline_{Method.Name}_{GetHashCode()}", _Hook.ReturnType, array, Method.DeclaringType, skipVisibility: false).StubCriticalDetour().Pin();
		}
		DynamicMethod dynamicMethod2 = new DynamicMethod($"hook_{Method.Name}_{GetHashCode()}", (Method as MethodInfo)?.ReturnType ?? typeof(void), array, Method.DeclaringType, skipVisibility: true);
		ILGenerator iLGenerator = dynamicMethod2.GetILGenerator();
		if (target != null)
		{
			_RefTarget = iLGenerator.EmitReference(target);
		}
		if (dynamicMethod != null)
		{
			_RefTrampoline = iLGenerator.EmitReference(dynamicMethod.CreateDelegate(type));
		}
		for (int l = 0; l < array.Length; l++)
		{
			iLGenerator.Emit(OpCodes.Ldarg, l);
		}
		iLGenerator.Emit(OpCodes.Call, _Hook);
		iLGenerator.Emit(OpCodes.Ret);
		Target = dynamicMethod2.Pin();
		_Detour = new Detour(Method, Target);
		if (dynamicMethod != null)
		{
			NativeDetourData detour = DetourManager.Native.Create(dynamicMethod.GetNativeStart(), GenerateTrampoline(signature).GetNativeStart());
			DetourManager.Native.MakeWritable(detour);
			DetourManager.Native.Apply(detour);
			DetourManager.Native.MakeExecutable(detour);
			DetourManager.Native.Free(detour);
		}
	}

	public Hook(MethodBase from, MethodInfo to)
		: this(from, to, null)
	{
	}

	public Hook(MethodBase method, IntPtr to)
		: this(method, DetourManager.GenerateNativeProxy(to, method), null)
	{
	}

	public Hook(MethodBase method, Delegate to)
		: this(method, to.Method, to.Target)
	{
	}

	public Hook(Delegate from, IntPtr to)
		: this(from.Method, to)
	{
	}

	public Hook(Delegate from, Delegate to)
		: this(from.Method, to)
	{
	}

	public Hook(Expression from, IntPtr to)
		: this(((MethodCallExpression)from).Method, to)
	{
	}

	public Hook(Expression from, Delegate to)
		: this(((MethodCallExpression)from).Method, to)
	{
	}

	public Hook(Expression<Action> from, IntPtr to)
		: this(from.Body, to)
	{
	}

	public Hook(Expression<Action> from, Delegate to)
		: this(from.Body, to)
	{
	}

	public void Apply()
	{
		if (!IsValid)
		{
			throw new InvalidOperationException("This hook has been undone.");
		}
		_Detour.Apply();
	}

	public void Undo()
	{
		if (!IsValid)
		{
			throw new InvalidOperationException("This hook has been undone.");
		}
		_Detour.Undo();
		if (!IsValid)
		{
			_Free();
		}
	}

	public void Free()
	{
		if (IsValid)
		{
			_Detour.Free();
			_Free();
		}
	}

	public void Dispose()
	{
		Undo();
		Free();
	}

	private void _Free()
	{
		if (_RefTarget.HasValue)
		{
			DynamicMethodHelper.FreeReference(_RefTarget.Value);
		}
		if (_RefTrampoline.HasValue)
		{
			DynamicMethodHelper.FreeReference(_RefTrampoline.Value);
		}
	}

	public MethodBase GenerateTrampoline(MethodBase signature = null)
	{
		if (!IsValid)
		{
			throw new InvalidOperationException("This hook has been undone.");
		}
		return _Detour.GenerateTrampoline(signature);
	}

	public T GenerateTrampoline<T>() where T : class
	{
		if (!IsValid)
		{
			throw new InvalidOperationException("This hook has been undone.");
		}
		return _Detour.GenerateTrampoline<T>();
	}
}
public class Hook<T> : Hook
{
	public Hook(Expression<Action> from, T to)
		: base(from.Body, to as Delegate)
	{
	}

	public Hook(Expression<Func<T>> from, IntPtr to)
		: base(from.Body, to)
	{
	}

	public Hook(Expression<Func<T>> from, Delegate to)
		: base(from.Body, to)
	{
	}

	public Hook(T from, IntPtr to)
		: base(from as Delegate, to)
	{
	}

	public Hook(T from, T to)
		: base(from as Delegate, to as Delegate)
	{
	}
}
public class Hook<TFrom, TTo> : Hook
{
	public Hook(Expression<Func<TFrom>> from, TTo to)
		: base(from.Body, to as Delegate)
	{
	}

	public Hook(TFrom from, TTo to)
		: base(from as Delegate, to as Delegate)
	{
	}
}
