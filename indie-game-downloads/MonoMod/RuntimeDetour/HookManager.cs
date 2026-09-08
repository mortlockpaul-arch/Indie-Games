using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonoMod.RuntimeDetour;

public static class HookManager
{
	private struct HookKey(MethodBase method, Delegate hook)
	{
		public MethodBase Method = method;

		public Delegate Hook = hook;

		public override int GetHashCode()
		{
			return Method.GetHashCode() ^ Hook.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is HookKey hookKey))
			{
				return false;
			}
			if (Method == hookKey.Method)
			{
				return (object)Hook == hookKey.Hook;
			}
			return false;
		}

		public override string ToString()
		{
			return $"[HookKey ({Method}) ({Hook})]";
		}
	}

	private class HookKeyEqualityComparer : EqualityComparer<HookKey>
	{
		public override bool Equals(HookKey x, HookKey y)
		{
			if (x.Method == y.Method)
			{
				return (object)x.Hook == y.Hook;
			}
			return false;
		}

		public override int GetHashCode(HookKey obj)
		{
			return obj.Method.GetHashCode() ^ obj.Hook.GetHashCode();
		}
	}

	private static Dictionary<HookKey, Stack<Hook>> _HookMap = new Dictionary<HookKey, Stack<Hook>>();

	public static void Add(MethodBase method, Delegate hookDelegate)
	{
		HookKey key = new HookKey(method, hookDelegate);
		if (!_HookMap.TryGetValue(key, out var value))
		{
			value = (_HookMap[key] = new Stack<Hook>());
		}
		Hook item = new Hook(method, hookDelegate);
		value.Push(item);
	}

	public static void Remove(MethodBase method, Delegate hookDelegate)
	{
		HookKey key = new HookKey(method, hookDelegate);
		if (_HookMap.TryGetValue(key, out var value))
		{
			value.Pop().Undo();
			value.Pop().Free();
			if (value.Count == 0)
			{
				_HookMap.Remove(key);
			}
		}
	}
}
