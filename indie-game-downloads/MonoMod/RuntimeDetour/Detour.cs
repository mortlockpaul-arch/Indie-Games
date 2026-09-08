using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour;

public class Detour : IDetour, IDisposable
{
	private static Dictionary<MethodBase, List<Detour>> _DetourMap = new Dictionary<MethodBase, List<Detour>>();

	private static Dictionary<MethodBase, DynamicMethod> _BackupMethods = new Dictionary<MethodBase, DynamicMethod>();

	public readonly MethodBase Method;

	public readonly MethodBase Target;

	private NativeDetour _TopDetour;

	private DynamicMethod _ChainedTrampoline;

	public bool IsValid => _DetourMap[Method].Contains(this);

	public int Index
	{
		get
		{
			return _DetourMap[Method].IndexOf(this);
		}
		set
		{
			List<Detour> list = _DetourMap[Method];
			lock (list)
			{
				int num = list.IndexOf(this);
				if (num == -1)
				{
					throw new InvalidOperationException("This detour has been undone.");
				}
				list.RemoveAt(num);
				if (value > num)
				{
					value--;
				}
				try
				{
					list.Insert(value, this);
				}
				catch
				{
					list.Insert(num, this);
					throw;
				}
				Detour detour = list[list.Count - 1];
				if (detour != this)
				{
					_TopUndo();
				}
				detour._TopApply();
				_UpdateChainedTrampolines(Method);
			}
		}
	}

	public Detour(MethodBase from, MethodBase to)
	{
		Method = from;
		Target = to;
		if (!_BackupMethods.ContainsKey(Method))
		{
			_BackupMethods[Method] = Method.CreateILCopy();
		}
		ParameterInfo[] parameters = Method.GetParameters();
		Type[] array;
		if (!Method.IsStatic)
		{
			array = new Type[parameters.Length + 1];
			array[0] = Method.DeclaringType;
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
		_ChainedTrampoline = new DynamicMethod($"chain_{Method.Name}_{GetHashCode()}", (Method as MethodInfo)?.ReturnType ?? typeof(void), array, Method.DeclaringType, skipVisibility: false).StubCriticalDetour().Pin();
		List<Detour> value;
		lock (_DetourMap)
		{
			if (!_DetourMap.TryGetValue(Method, out value))
			{
				value = (_DetourMap[Method] = new List<Detour>());
			}
		}
		lock (value)
		{
			if (value.Count > 0)
			{
				value[value.Count - 1]._TopUndo();
			}
			_TopApply();
			NativeDetourData detour = ((value.Count <= 0) ? DetourManager.Native.Create(_ChainedTrampoline.GetNativeStart(), _BackupMethods[Method].GetNativeStart()) : DetourManager.Native.Create(_ChainedTrampoline.GetNativeStart(), value[value.Count - 1].Target.GetNativeStart()));
			DetourManager.Native.MakeWritable(detour);
			DetourManager.Native.Apply(detour);
			DetourManager.Native.MakeExecutable(detour);
			DetourManager.Native.Free(detour);
			value.Add(this);
		}
	}

	public Detour(MethodBase method, IntPtr to)
		: this(method, DetourManager.GenerateNativeProxy(to, method))
	{
	}

	public Detour(Delegate from, IntPtr to)
		: this(from.Method, to)
	{
	}

	public Detour(Delegate from, Delegate to)
		: this(from.Method, to.Method)
	{
	}

	public Detour(Expression from, IntPtr to)
		: this(((MethodCallExpression)from).Method, to)
	{
	}

	public Detour(Expression from, Expression to)
		: this(((MethodCallExpression)from).Method, ((MethodCallExpression)to).Method)
	{
	}

	public Detour(Expression<Action> from, IntPtr to)
		: this(from.Body, to)
	{
	}

	public Detour(Expression<Action> from, Expression<Action> to)
		: this(from.Body, to.Body)
	{
	}

	public void Apply()
	{
		if (!IsValid)
		{
			throw new InvalidOperationException("This detour has been undone.");
		}
	}

	public void Undo()
	{
		if (!IsValid)
		{
			throw new InvalidOperationException("This detour has been undone.");
		}
		List<Detour> list = _DetourMap[Method];
		lock (list)
		{
			list.Remove(this);
			_TopUndo();
			if (list.Count > 0)
			{
				list[list.Count - 1]._TopApply();
			}
			_UpdateChainedTrampolines(Method);
		}
	}

	public void Free()
	{
		if (IsValid)
		{
			Undo();
		}
	}

	public void Dispose()
	{
		Undo();
		Free();
	}

	public MethodBase GenerateTrampoline(MethodBase signature = null)
	{
		if (signature == null)
		{
			signature = Target;
		}
		Type returnType = (signature as MethodInfo)?.ReturnType ?? typeof(void);
		ParameterInfo[] parameters = signature.GetParameters();
		Type[] array = new Type[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			array[i] = parameters[i].ParameterType;
		}
		DynamicMethod dynamicMethod = new DynamicMethod($"trampoline_{Method.Name}_{GetHashCode()}", returnType, array, Method.DeclaringType, skipVisibility: true);
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		for (int j = 0; j < array.Length; j++)
		{
			iLGenerator.Emit(OpCodes.Ldarg, j);
		}
		iLGenerator.Emit(OpCodes.Call, _ChainedTrampoline);
		iLGenerator.Emit(OpCodes.Ret);
		return dynamicMethod.Pin();
	}

	public T GenerateTrampoline<T>() where T : class
	{
		if (!IsValid)
		{
			throw new InvalidOperationException("This detour has been undone.");
		}
		if (!typeof(Delegate).IsAssignableFrom(typeof(T)))
		{
			throw new InvalidOperationException($"Type {typeof(T)} not a delegate type.");
		}
		return GenerateTrampoline(typeof(T).GetMethod("Invoke")).CreateDelegate(typeof(T)) as T;
	}

	private void _TopUndo()
	{
		if (_TopDetour != null)
		{
			_TopDetour.Undo();
			_TopDetour.Free();
			_TopDetour = null;
		}
	}

	private void _TopApply()
	{
		if (_TopDetour == null)
		{
			_TopDetour = new NativeDetour(Method.GetNativeStart(), Target.GetNativeStart());
		}
	}

	private static void _UpdateChainedTrampolines(MethodBase method)
	{
		List<Detour> list = _DetourMap[method];
		lock (list)
		{
			if (list.Count != 0)
			{
				NativeDetourData detour;
				for (int i = 1; i < list.Count; i++)
				{
					detour = DetourManager.Native.Create(list[i]._ChainedTrampoline.GetNativeStart(), list[i - 1].Target.GetNativeStart());
					DetourManager.Native.MakeWritable(detour);
					DetourManager.Native.Apply(detour);
					DetourManager.Native.MakeExecutable(detour);
					DetourManager.Native.Free(detour);
				}
				detour = DetourManager.Native.Create(list[0]._ChainedTrampoline.GetNativeStart(), _BackupMethods[method].GetNativeStart());
				DetourManager.Native.MakeWritable(detour);
				DetourManager.Native.Apply(detour);
				DetourManager.Native.MakeExecutable(detour);
				DetourManager.Native.Free(detour);
			}
		}
	}
}
public class Detour<T> : Detour where T : class
{
	public Detour(T from, IntPtr to)
		: base(from as Delegate, to)
	{
	}

	public Detour(T from, T to)
		: base(from as Delegate, to as Delegate)
	{
	}
}
