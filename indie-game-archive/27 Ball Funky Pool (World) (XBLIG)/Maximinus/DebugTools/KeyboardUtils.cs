using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Maximinus.DebugTools;

public static class KeyboardUtils
{
	private class CharPair
	{
		public char NormalChar;

		public char? ShiftChar;

		public CharPair(char normalChar, char? shiftChar)
		{
			NormalChar = normalChar;
			ShiftChar = shiftChar;
		}
	}

	private static Dictionary<Keys, CharPair> keyMap;

	public static bool KeyToString(Keys key, bool shitKeyPressed, out char character)
	{
		bool result = false;
		character = ' ';
		CharPair value;
		if ((Keys.A <= key && key <= Keys.Z) || key == Keys.Space)
		{
			character = (shitKeyPressed ? ((char)key) : char.ToLower((char)key));
			result = true;
		}
		else if (keyMap.TryGetValue(key, out value))
		{
			if (!shitKeyPressed)
			{
				character = value.NormalChar;
				result = true;
			}
			else if (value.ShiftChar.HasValue)
			{
				character = value.ShiftChar.Value;
				result = true;
			}
		}
		return result;
	}

	static KeyboardUtils()
	{
		keyMap = new Dictionary<Keys, CharPair>();
		InitializeKeyMap();
	}

	private static void InitializeKeyMap()
	{
		AddKeyMap(Keys.OemTilde, "`~");
		AddKeyMap(Keys.D1, "1!");
		AddKeyMap(Keys.D2, "2@");
		AddKeyMap(Keys.D3, "3#");
		AddKeyMap(Keys.D4, "4$");
		AddKeyMap(Keys.D5, "5%");
		AddKeyMap(Keys.D6, "6^");
		AddKeyMap(Keys.D7, "7&");
		AddKeyMap(Keys.D8, "8*");
		AddKeyMap(Keys.D9, "9(");
		AddKeyMap(Keys.D0, "0)");
		AddKeyMap(Keys.OemMinus, "-_");
		AddKeyMap(Keys.OemPlus, "=+");
		AddKeyMap(Keys.OemOpenBrackets, "[{");
		AddKeyMap(Keys.OemCloseBrackets, "]}");
		AddKeyMap(Keys.OemPipe, "\\|");
		AddKeyMap(Keys.OemSemicolon, ";:");
		AddKeyMap(Keys.OemQuotes, "'\"");
		AddKeyMap(Keys.OemComma, ",<");
		AddKeyMap(Keys.OemPeriod, ".>");
		AddKeyMap(Keys.OemQuestion, "/?");
		AddKeyMap(Keys.NumPad1, "1");
		AddKeyMap(Keys.NumPad2, "2");
		AddKeyMap(Keys.NumPad3, "3");
		AddKeyMap(Keys.NumPad4, "4");
		AddKeyMap(Keys.NumPad5, "5");
		AddKeyMap(Keys.NumPad6, "6");
		AddKeyMap(Keys.NumPad7, "7");
		AddKeyMap(Keys.NumPad8, "8");
		AddKeyMap(Keys.NumPad9, "9");
		AddKeyMap(Keys.NumPad0, "0");
		AddKeyMap(Keys.Add, "+");
		AddKeyMap(Keys.Divide, "/");
		AddKeyMap(Keys.Multiply, "*");
		AddKeyMap(Keys.Subtract, "-");
		AddKeyMap(Keys.Decimal, ".");
	}

	private static void AddKeyMap(Keys key, string charPair)
	{
		char normalChar = charPair[0];
		char? shiftChar = null;
		if (charPair.Length > 1)
		{
			shiftChar = charPair[1];
		}
		keyMap.Add(key, new CharPair(normalChar, shiftChar));
	}
}
