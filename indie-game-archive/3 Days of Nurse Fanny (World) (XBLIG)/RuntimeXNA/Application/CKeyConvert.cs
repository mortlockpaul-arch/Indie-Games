using Microsoft.Xna.Framework.Input;

namespace RuntimeXNA.Application;

internal class CKeyConvert
{
	private const int NB_SPECIAL_KEYS = 29;

	public static int[] pcKeys = new int[107]
	{
		1, 2, 4, 27, 13, 16, 17, 18, 32, 37,
		38, 39, 40, 144, 111, 106, 109, 107, 110, 226,
		221, 186, 219, 187, 8, 45, 36, 46, 35, 33,
		34, 9, 188, 190, 191, 223, 112, 113, 114, 115,
		116, 117, 118, 119, 120, 121, 122, 123, 124, 125,
		126, 127, 128, 129, 130, 131, 132, 133, 134, 135,
		48, 49, 50, 51, 52, 53, 54, 55, 56, 57,
		65, 66, 67, 68, 69, 70, 71, 72, 73, 74,
		75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
		85, 86, 87, 88, 89, 90, 96, 97, 98, 99,
		100, 101, 102, 103, 104, 105, -1
	};

	public static Keys[] xnaKeys = new Keys[106]
	{
		Keys.None,
		Keys.None,
		Keys.None,
		Keys.Escape,
		Keys.Enter,
		Keys.LeftShift,
		Keys.LeftControl,
		Keys.None,
		Keys.Space,
		Keys.Left,
		Keys.Up,
		Keys.Right,
		Keys.Down,
		Keys.NumLock,
		Keys.Divide,
		Keys.Multiply,
		Keys.Subtract,
		Keys.Add,
		Keys.Decimal,
		Keys.None,
		Keys.OemOpenBrackets,
		Keys.OemCloseBrackets,
		Keys.OemCloseBrackets,
		Keys.OemPlus,
		Keys.Back,
		Keys.Insert,
		Keys.Home,
		Keys.Delete,
		Keys.End,
		Keys.PageUp,
		Keys.PageDown,
		Keys.Tab,
		Keys.OemComma,
		Keys.OemSemicolon,
		Keys.None,
		Keys.None,
		Keys.F1,
		Keys.F2,
		Keys.F3,
		Keys.F4,
		Keys.F5,
		Keys.F6,
		Keys.F7,
		Keys.F8,
		Keys.F9,
		Keys.F10,
		Keys.F11,
		Keys.F12,
		Keys.F13,
		Keys.F14,
		Keys.F15,
		Keys.F16,
		Keys.F17,
		Keys.F18,
		Keys.F19,
		Keys.F20,
		Keys.F21,
		Keys.F22,
		Keys.F23,
		Keys.F24,
		Keys.D0,
		Keys.D1,
		Keys.D2,
		Keys.D3,
		Keys.D4,
		Keys.D5,
		Keys.D6,
		Keys.D7,
		Keys.D8,
		Keys.D9,
		Keys.A,
		Keys.B,
		Keys.C,
		Keys.D,
		Keys.E,
		Keys.F,
		Keys.G,
		Keys.H,
		Keys.I,
		Keys.J,
		Keys.K,
		Keys.L,
		Keys.M,
		Keys.N,
		Keys.O,
		Keys.P,
		Keys.Q,
		Keys.R,
		Keys.S,
		Keys.T,
		Keys.U,
		Keys.V,
		Keys.W,
		Keys.X,
		Keys.Y,
		Keys.Z,
		Keys.NumPad0,
		Keys.NumPad1,
		Keys.NumPad2,
		Keys.NumPad3,
		Keys.NumPad4,
		Keys.NumPad5,
		Keys.NumPad6,
		Keys.NumPad7,
		Keys.NumPad8,
		Keys.NumPad9
	};

	public static string[] keyNames = new string[37]
	{
		"LButton", "MButton", "RButton", "Escape", "Return", "Shift", "Control", "Alt", "Space", "Left",
		"Up", "Right", "Down", "Numlock", "Divide", "Multiply", "Subtract", "Add", "Decimal", "Key1",
		"Key2", "Key3", "Close bracket", "Equal", "Backspace", "Insert", "Home", "Delete", "End", "Previous page",
		"Next page", "Tab", "Comma", "Semi colon", "Colon", "Exclamation", "Unknown"
	};

	public static Keys getXnaKey(int pcKey)
	{
		for (int i = 0; pcKeys[i] != -1; i++)
		{
			if (pcKeys[i] == pcKey)
			{
				return xnaKeys[i];
			}
		}
		return Keys.None;
	}

	public static string getKeyText(Keys vkCode)
	{
		string result = "";
		if (vkCode >= Keys.NumPad0 && vkCode <= Keys.NumPad9)
		{
			result = $"Numpad{(int)(vkCode - 96)}";
		}
		else if (vkCode >= Keys.F1 && vkCode <= Keys.F24)
		{
			result = $"F{(int)(vkCode - 96)}";
		}
		else if (vkCode >= Keys.D0 && vkCode <= Keys.D9)
		{
			result = $"{(int)(vkCode - 96)}";
		}
		else
		{
			if (vkCode >= Keys.A && vkCode <= Keys.Z)
			{
				return new string(new char[1] { (char)(65 + (ushort)vkCode) }, 0, 1);
			}
			for (int i = 0; i < 29; i++)
			{
				if (xnaKeys[i] == vkCode)
				{
					result = keyNames[i];
					break;
				}
			}
		}
		return result;
	}
}
