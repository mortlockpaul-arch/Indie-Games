using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace MonoMod.Detour;

[Obsolete("Please switch to the new MonoMod.RuntimeDetour namespace. A subset of the old API is still available, using the new ")]
public static class RuntimeDetour
{
	private static Dictionary<long, Stack<NativeDetour>> _Detours = new Dictionary<long, Stack<NativeDetour>>();

	public static MethodInfo TrampolinePrefix = null;

	public static MethodInfo TrampolineSuffix = null;

	public static bool IsX64 { get; } = IntPtr.Size == 8;

	public static int DetourSize
	{
		get
		{
			throw new NotSupportedException("Use MonoMod.RuntimeDetour.DetourManager.Native.Size(...) instead.");
		}
	}

	private static Stack<NativeDetour> _GetDetours(long from)
	{
		if (_Detours.TryGetValue(from, out var value))
		{
			return value;
		}
		return _Detours[from] = new Stack<NativeDetour>();
	}

	public unsafe static void* GetMethodStart(long token)
	{
		throw new NotSupportedException("Tokens no longer supported.");
	}

	public unsafe static void* GetMethodStart(MethodBase method)
	{
		return method.GetNativeStart().ToPointer();
	}

	public unsafe static void* GetDelegateStart(Delegate d)
	{
		return d.Method.GetNativeStart().ToPointer();
	}

	public static long GetToken(MethodBase method)
	{
		throw new NotSupportedException("Tokens no longer supported.");
	}

	public unsafe static void Detour(this MethodBase from, IntPtr to)
	{
		Detour(GetMethodStart(from), to.ToPointer());
	}

	public unsafe static void Detour(this MethodBase from, MethodBase to)
	{
		Detour(GetMethodStart(from), GetMethodStart(to));
	}

	public unsafe static void Detour(this MethodBase from, Delegate to)
	{
		Detour(GetMethodStart(from), GetDelegateStart(to));
	}

	public unsafe static void DetourMethod(IntPtr from, IntPtr to)
	{
		Detour(from.ToPointer(), to.ToPointer());
	}

	public static T Detour<T>(this MethodBase from, IntPtr to)
	{
		from.Detour(to);
		return from.GetTrampoline<T>();
	}

	public static T Detour<T>(this MethodBase from, MethodBase to)
	{
		from.Detour(to);
		return from.GetTrampoline<T>();
	}

	public static T Detour<T>(this MethodBase from, Delegate to)
	{
		from.Detour(to);
		return from.GetTrampoline<T>();
	}

	public unsafe static void Detour(void* from, void* to, bool store = true)
	{
		NativeDetour item = new NativeDetour((IntPtr)from, (IntPtr)to);
		if (store)
		{
			_GetDetours((long)from).Push(item);
		}
	}

	public unsafe static void Undetour(this MethodBase target, int level = -1)
	{
		Undetour(GetMethodStart(target), level);
	}

	public unsafe static void Undetour(this Delegate target, int level = -1)
	{
		Undetour(GetDelegateStart(target), level);
	}

	public unsafe static void Undetour(void* target, int level = -1)
	{
		Stack<NativeDetour> stack = _GetDetours((long)target);
		if (stack.Count != 0)
		{
			NativeDetour nativeDetour = stack.Pop();
			nativeDetour.Undo();
			nativeDetour.Free();
		}
	}

	public unsafe static int GetDetourLevel(this MethodBase target)
	{
		return GetDetourLevel(GetMethodStart(target));
	}

	public unsafe static int GetDetourLevel(this Delegate target)
	{
		return GetDetourLevel(GetDelegateStart(target));
	}

	public unsafe static int GetDetourLevel(void* target)
	{
		return _GetDetours((long)target).Count;
	}

	public unsafe static bool Refresh(this MethodBase target)
	{
		return Refresh(GetMethodStart(target));
	}

	public unsafe static bool Refresh(this Delegate target)
	{
		return Refresh(GetDelegateStart(target));
	}

	public unsafe static bool Refresh(void* target)
	{
		Stack<NativeDetour> stack = _GetDetours((long)target);
		if (stack.Count == 0)
		{
			return false;
		}
		stack.Peek().Apply();
		return true;
	}

	private static T _GenerateTrampoline<T>(this NativeDetour detour)
	{
		return (T)(object)detour.GenerateTrampoline(typeof(T).GetMethod("Invoke")).CreateDelegate(typeof(T));
	}

	public static T GetOrigTrampoline<T>(this MethodBase target)
	{
		Stack<NativeDetour> stack = _GetDetours((long)target.GetNativeStart());
		if (stack.Count == 0)
		{
			return default(T);
		}
		return stack.Last()._GenerateTrampoline<T>();
	}

	public static T GetTrampoline<T>(this MethodBase target)
	{
		Stack<NativeDetour> stack = _GetDetours((long)target.GetNativeStart());
		if (stack.Count == 0)
		{
			return default(T);
		}
		return stack.Peek()._GenerateTrampoline<T>();
	}

	public static T GetNextTrampoline<T>(this MethodBase target)
	{
		throw new NotSupportedException("Old trampoline generator no longer available. This usage is no longer supported.");
	}

	public static T CreateTrampolineDirect<T>(MethodBase target)
	{
		Stack<NativeDetour> stack = _GetDetours((long)target.GetNativeStart());
		if (stack.Count == 0)
		{
			return default(T);
		}
		return stack.Peek()._GenerateTrampoline<T>();
	}

	public static T CreateTrampolineDirect<T>(MethodBase target, IntPtr code)
	{
		throw new NotSupportedException("Old trampoline generator no longer available. This usage is no longer supported.");
	}

	public static DynamicMethod CreateOrigTrampoline(this MethodBase target, MethodInfo invokeInfo = null)
	{
		Stack<NativeDetour> stack = _GetDetours((long)target.GetNativeStart());
		if (stack.Count == 0)
		{
			return null;
		}
		return (DynamicMethod)stack.Last().GenerateTrampoline(invokeInfo);
	}

	public static DynamicMethod CreateTrampoline(this MethodBase target, MethodInfo invokeInfo = null)
	{
		Stack<NativeDetour> stack = _GetDetours((long)target.GetNativeStart());
		if (stack.Count == 0)
		{
			return null;
		}
		return (DynamicMethod)stack.Peek().GenerateTrampoline(invokeInfo);
	}

	public static DynamicMethod CreateTrampolineDirect(MethodBase target)
	{
		throw new NotSupportedException("Old trampoline generator no longer available. This usage is no longer supported.");
	}

	public static DynamicMethod CreateTrampolineDirect(MethodBase target, IntPtr code, MethodInfo invokeInfo = null)
	{
		throw new NotSupportedException("Old trampoline generator no longer available. This usage is no longer supported.");
	}

	public static void PrepareOrig(long targetToken)
	{
		throw new NotSupportedException("Old trampoline generator no longer available.");
	}

	public static void UnprepareOrig(long targetToken)
	{
		throw new NotSupportedException("Old trampoline generator no longer available.");
	}
}
