using System;

namespace Microsoft.Xna.Framework.Input;

public static class TextInputEXT
{
	public static nint WindowHandle { get; set; }

	public static event Action<char> TextInput;

	public static event Action<string, int, int> TextEditing;

	public static bool IsTextInputActive()
	{
		return FNAPlatform.IsTextInputActive(WindowHandle);
	}

	public static bool IsScreenKeyboardShown()
	{
		return FNAPlatform.IsScreenKeyboardShown(WindowHandle);
	}

	public static bool IsScreenKeyboardShown(nint window)
	{
		return FNAPlatform.IsScreenKeyboardShown(window);
	}

	public static void StartTextInput()
	{
		FNAPlatform.StartTextInput(WindowHandle);
	}

	public static void StopTextInput()
	{
		FNAPlatform.StopTextInput(WindowHandle);
	}

	public static void SetInputRectangle(Rectangle rectangle)
	{
		FNAPlatform.SetTextInputRectangle(WindowHandle, rectangle);
	}

	internal static void OnTextInput(char c)
	{
		if (TextInput != null)
		{
			TextInput(c);
		}
	}

	internal static void OnTextEditing(string text, int start, int length)
	{
		if (TextEditing != null)
		{
			TextEditing(text, start, length);
		}
	}
}
