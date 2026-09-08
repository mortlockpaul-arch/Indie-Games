using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using XnaToFna.ProxyForms;

namespace XnaToFna;

public static class KeyboardEvents
{
	public static HashSet<Keys> LastDown = new HashSet<Keys>();

	public static HashSet<Keys> Down = new HashSet<Keys>();

	public static void KeyDown(Keys key)
	{
		PInvoke.CallHooks(Messages.WM_KEYDOWN, (IntPtr)(int)key, IntPtr.Zero);
	}

	public static void KeyUp(Keys key)
	{
		PInvoke.CallHooks(Messages.WM_KEYUP, (IntPtr)(int)key, IntPtr.Zero);
	}

	public static void CharEntered(char c)
	{
		PInvoke.CallHooks(Messages.WM_CHAR, (IntPtr)c, IntPtr.Zero);
	}

	public static void SetContext(bool wParam)
	{
		PInvoke.CallHooks(Messages.WM_IME_SETCONTEXT, (IntPtr)(wParam ? 1 : 0), IntPtr.Zero);
	}

	public static void Update()
	{
		Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
		Down.Clear();
		foreach (Keys keys in pressedKeys)
		{
			if (!LastDown.Contains(keys))
			{
				KeyDown(keys);
			}
			Down.Add(keys);
		}
		foreach (Keys item in LastDown)
		{
			if (!Down.Contains(item))
			{
				KeyUp(item);
			}
		}
		LastDown.Clear();
		LastDown.UnionWith(Down);
	}
}
