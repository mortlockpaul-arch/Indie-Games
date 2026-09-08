using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using XnaToFna.ProxyForms;

namespace XnaToFna;

public static class PInvoke
{
	public static int MessageSize;

	public static Dictionary<HookType, List<Delegate>> Hooks;

	public static List<Tuple<HookType, Delegate, int>> AllHooks;

	public static ThreadLocal<List<Delegate>> CurrentHookChain;

	public static ThreadLocal<int> CurrentHookIndex;

	static PInvoke()
	{
		MessageSize = Marshal.SizeOf(typeof(Message));
		Hooks = new Dictionary<HookType, List<Delegate>>();
		AllHooks = new List<Tuple<HookType, Delegate, int>>();
		CurrentHookChain = new ThreadLocal<List<Delegate>>();
		CurrentHookIndex = new ThreadLocal<int>();
		foreach (HookType value in Enum.GetValues(typeof(HookType)))
		{
			Hooks[value] = new List<Delegate>();
		}
	}

	public static void CallHooks(Messages Msg, IntPtr wParam, IntPtr lParam, bool global = true, bool window = true, bool allWindows = false)
	{
		CallHooks(Msg, wParam, new Message
		{
			HWnd = IntPtr.Zero,
			Msg = (int)Msg,
			WParam = wParam,
			LParam = lParam
		}, global, window, allWindows);
	}

	public static void CallHooks(Messages Msg, IntPtr wParam, Message lParamMsg, bool global = true, bool window = true, bool allWindows = false)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(MessageSize);
		Marshal.StructureToPtr((object)lParamMsg, intPtr, false);
		CallHooks(Msg, wParam, intPtr, ref lParamMsg, global, window, allWindows);
		Marshal.FreeHGlobal(intPtr);
	}

	public static void CallHooks(Messages Msg, IntPtr wParam, IntPtr lParam, ref Message lParamMsg, bool global = true, bool window = true, bool allWindows = false)
	{
		if (global)
		{
			CallHookChain(HookType.WH_GETMESSAGE, (IntPtr)1, lParam, ref lParamMsg);
		}
		if (allWindows)
		{
			for (int i = 0; i < Control.AllControls.Count; i++)
			{
				lParamMsg.Result = CallWindowHook((IntPtr)(i + 1), Msg, wParam, lParam);
			}
		}
		else if (window)
		{
			lParamMsg.Result = CallWindowHook(Msg, wParam, lParam);
		}
	}

	public static IntPtr CallHookChain(HookType hookType, IntPtr wParam, IntPtr lParam, ref Message lParamMsg)
	{
		List<Delegate> list = Hooks[hookType];
		if (list.Count == 0)
		{
			return IntPtr.Zero;
		}
		CurrentHookChain.Value = list;
		for (int i = 0; i < list.Count; i++)
		{
			Delegate obj = list[i];
			if ((object)obj != null)
			{
				CurrentHookIndex.Value = i;
				object[] array = new object[3] { 0, wParam, lParamMsg };
				object obj2 = obj.DynamicInvoke(array);
				lParamMsg = (Message)array[2];
				if (obj2 == null)
				{
					return IntPtr.Zero;
				}
				return (IntPtr)Convert.ToInt32(obj2);
			}
		}
		return IntPtr.Zero;
	}

	public static IntPtr ContinueHookChain(int nCode, IntPtr wParam, IntPtr lParam)
	{
		List<Delegate> value = CurrentHookChain.Value;
		for (int i = CurrentHookIndex.Value + 1; i < value.Count; i++)
		{
			Delegate obj = value[i];
			if ((object)obj != null)
			{
				CurrentHookIndex.Value = i;
				return (IntPtr)obj.DynamicInvoke((nCode < 0) ? (nCode + 1) : 0, wParam, lParam);
			}
		}
		return IntPtr.Zero;
	}

	public static IntPtr CallWindowHook(Messages Msg, IntPtr wParam, IntPtr lParam)
	{
		return CallWindowHook(GameForm.Instance?.Handle ?? IntPtr.Zero, (uint)Msg, wParam, lParam);
	}

	public static IntPtr CallWindowHook(IntPtr hWnd, Messages Msg, IntPtr wParam, IntPtr lParam)
	{
		return CallWindowHook(hWnd, (uint)Msg, wParam, lParam);
	}

	public static IntPtr CallWindowHook(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam)
	{
		if (!(Control.FromHandle(hWnd) is Form form) || form.WindowHookPtr == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		return (IntPtr)form.WindowHook.DynamicInvoke(hWnd, Msg, wParam, lParam);
	}
}
