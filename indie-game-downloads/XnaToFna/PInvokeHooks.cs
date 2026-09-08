using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;
using XnaToFna.ProxyForms;

namespace XnaToFna;

public static class PInvokeHooks
{
	public unsafe static bool GetClipCursor(ref Rectangle rect)
	{
		fixed (Rectangle* ptr = &rect)
		{
			if (ptr == null)
			{
				return true;
			}
		}
		if (MouseEvents.Clip.HasValue)
		{
			rect = MouseEvents.Clip.Value;
		}
		else
		{
			DisplayMode currentDisplayMode = XnaToFnaHelper.Game.GraphicsDevice.Adapter.CurrentDisplayMode;
			rect = new Rectangle(0, 0, currentDisplayMode.Width, currentDisplayMode.Height);
		}
		return true;
	}

	public unsafe static bool ClipCursor(ref Rectangle rect)
	{
		fixed (Rectangle* ptr = &rect)
		{
			if (ptr == null)
			{
				XnaToFnaHelper.Log("[CursorEvents] Cursor released from ClipCursor");
				MouseEvents.Clip = null;
				return true;
			}
		}
		XnaToFnaHelper.Log($"[CursorEvents] Game tries to ClipCursor inside {rect}");
		MouseEvents.Clip = rect;
		return true;
	}

	public static IntPtr GetForegroundWindow()
	{
		if (XnaToFnaHelper.Game.IsActive)
		{
			return GameForm.Instance.Handle;
		}
		return IntPtr.Zero;
	}

	public static bool SetForegroundWindow(IntPtr hWnd)
	{
		if (GameForm.Instance.Handle != hWnd)
		{
			return false;
		}
		SDL.SDL_RaiseWindow(XnaToFnaHelper.Game.Window.Handle);
		return true;
	}

	public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
	{
		if (nIndex == -4)
		{
			Form form = Control.FromHandle(hWnd)?.Form;
			if (form == null)
			{
				return 0;
			}
			IntPtr windowHookPtr = form.WindowHookPtr;
			form.WindowHookPtr = (IntPtr)dwNewLong;
			form.WindowHook = Marshal.GetDelegateForFunctionPointer(form.WindowHookPtr, typeof(WndProc));
			XnaToFnaHelper.Log($"[PInvokeHooks] Window hook set on ProxyForms.Form #{form.GlobalIndex}");
			return (int)windowHookPtr;
		}
		return 0;
	}

	public static IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam)
	{
		if (lpPrevWndFunc == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		return (IntPtr)Marshal.GetDelegateForFunctionPointer(lpPrevWndFunc, typeof(MulticastDelegate)).DynamicInvoke(hWnd, Msg, wParam, lParam);
	}

	public static IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId)
	{
		int num = PInvoke.AllHooks.Count + 1;
		List<Delegate> list = PInvoke.Hooks[hookType];
		PInvoke.AllHooks.Add(Tuple.Create(hookType, (Delegate)lpfn, list.Count));
		list.Add(lpfn);
		XnaToFnaHelper.Log($"[PInvokeHooks] Added global hook #{num} of type {hookType}");
		return (IntPtr)num;
	}

	public static bool UnhookWindowsHookEx(IntPtr hhk)
	{
		int num = (int)hhk - 1;
		if (num < 0 || PInvoke.Hooks.Count <= num || PInvoke.AllHooks[num] == null)
		{
			return true;
		}
		Tuple<HookType, Delegate, int> tuple = PInvoke.AllHooks[num];
		PInvoke.AllHooks[num] = null;
		PInvoke.Hooks[tuple.Item1].RemoveAt(tuple.Item3);
		return true;
	}

	public static IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam)
	{
		return PInvoke.ContinueHookChain(nCode, wParam, lParam);
	}

	public static bool TranslateMessage(ref Message m)
	{
		return true;
	}

	public unsafe static uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId)
	{
		Form form = Control.FromHandle(hWnd) as Form;
		if (form == null)
		{
			XnaToFnaHelper.Log($"[PInvokeHooks] Called GetWindowThreadProcessId for non-existing hWnd {hWnd}");
			form = GameForm.Instance;
		}
		fixed (uint* ptr = &lpdwProcessId)
		{
			if (ptr != null)
			{
				lpdwProcessId = 0u;
			}
		}
		return (uint)(form?.ThreadId ?? 0);
	}

	public static int GetCurrentThreadId()
	{
		return (int)PInvokeHelper.CurrentThreadId;
	}

	public static IntPtr LoadCursorFromFile(string str)
	{
		return new Cursor(str).Handle;
	}

	public static IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam)
	{
		Form form = Control.FromHandle(hWnd) as Form;
		if (form == null)
		{
			XnaToFnaHelper.Log($"[PInvokeHooks] Called GetWindowThreadProcessId for non-existing hWnd {hWnd}");
			form = GameForm.Instance;
		}
		if (Msg == 16)
		{
			form.Close();
			return IntPtr.Zero;
		}
		return IntPtr.Zero;
	}

	public static IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC)
	{
		return IntPtr.Zero;
	}

	public static IntPtr ImmGetContext(IntPtr hWnd)
	{
		return IntPtr.Zero;
	}

	public static bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC)
	{
		return true;
	}

	public static short GetAsyncKeyState(int vKey)
	{
		return (short)(Keyboard.GetState().IsKeyDown((Keys)vKey) ? 128 : 0);
	}

	public static IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags)
	{
		return (IntPtr)1033;
	}

	public static bool UnloadKeyboardLayout(IntPtr hkl)
	{
		return true;
	}

	public unsafe static bool GetKeyboardLayoutName(object pwszKLID)
	{
		if (pwszKLID is StringBuilder)
		{
			((StringBuilder)pwszKLID).Append("00000409");
		}
		else if (pwszKLID is IntPtr intPtr)
		{
			char* ptr = (char*)intPtr.ToPointer();
			for (int i = 0; i < "00000409".Length; i++)
			{
				ptr[i] = "00000409"[i];
			}
		}
		return true;
	}
}
