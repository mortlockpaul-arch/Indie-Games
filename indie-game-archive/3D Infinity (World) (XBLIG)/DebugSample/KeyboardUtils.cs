using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace DebugSample;

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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between I4 and Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		character = ' ';
		CharPair value;
		if ((65 <= (int)key && (int)key <= 90) || (int)key == 32)
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
		AddKeyMap((Keys)192, "`~");
		AddKeyMap((Keys)49, "1!");
		AddKeyMap((Keys)50, "2@");
		AddKeyMap((Keys)51, "3#");
		AddKeyMap((Keys)52, "4$");
		AddKeyMap((Keys)53, "5%");
		AddKeyMap((Keys)54, "6^");
		AddKeyMap((Keys)55, "7&");
		AddKeyMap((Keys)56, "8*");
		AddKeyMap((Keys)57, "9(");
		AddKeyMap((Keys)48, "0)");
		AddKeyMap((Keys)189, "-_");
		AddKeyMap((Keys)187, "=+");
		AddKeyMap((Keys)219, "[{");
		AddKeyMap((Keys)221, "]}");
		AddKeyMap((Keys)220, "\\|");
		AddKeyMap((Keys)186, ";:");
		AddKeyMap((Keys)222, "'\"");
		AddKeyMap((Keys)188, ",<");
		AddKeyMap((Keys)190, ".>");
		AddKeyMap((Keys)191, "/?");
		AddKeyMap((Keys)97, "1");
		AddKeyMap((Keys)98, "2");
		AddKeyMap((Keys)99, "3");
		AddKeyMap((Keys)100, "4");
		AddKeyMap((Keys)101, "5");
		AddKeyMap((Keys)102, "6");
		AddKeyMap((Keys)103, "7");
		AddKeyMap((Keys)104, "8");
		AddKeyMap((Keys)105, "9");
		AddKeyMap((Keys)96, "0");
		AddKeyMap((Keys)107, "+");
		AddKeyMap((Keys)111, "/");
		AddKeyMap((Keys)106, "*");
		AddKeyMap((Keys)109, "-");
		AddKeyMap((Keys)110, ".");
	}

	private static void AddKeyMap(Keys key, string charPair)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		char normalChar = charPair[0];
		char? shiftChar = null;
		if (charPair.Length > 1)
		{
			shiftChar = charPair[1];
		}
		keyMap.Add(key, new CharPair(normalChar, shiftChar));
	}
}
