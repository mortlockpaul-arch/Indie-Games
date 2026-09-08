using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Quasar.Input;

public class Keyboard
{
	private KeyboardState previousStatus;

	private KeyboardState currentStatus;

	private static Keyboard instance;

	private List<Keys> pressedKeys = new List<Keys>();

	public static Keyboard Instance
	{
		get
		{
			if (instance == null)
			{
				return new Keyboard();
			}
			return instance;
		}
	}

	public List<Keys> PressedKeys => pressedKeys;

	private Keyboard()
	{
		if (instance != null)
		{
			throw new Exception("A Keyboard object is already created");
		}
		instance = this;
		previousStatus = (currentStatus = Microsoft.Xna.Framework.Input.Keyboard.GetState());
	}

	public bool KeyPressed(Keys key)
	{
		if (previousStatus.IsKeyUp(key))
		{
			return currentStatus.IsKeyDown(key);
		}
		return false;
	}

	public bool KeyReleased(Keys key)
	{
		if (previousStatus.IsKeyDown(key))
		{
			return currentStatus.IsKeyUp(key);
		}
		return false;
	}

	public bool KeyState(Keys key)
	{
		return currentStatus.IsKeyDown(key);
	}

	public bool PreviousKeyState(Keys key)
	{
		return previousStatus.IsKeyDown(key);
	}

	public void UpdateStatus()
	{
		previousStatus = currentStatus;
		currentStatus = Microsoft.Xna.Framework.Input.Keyboard.GetState();
		pressedKeys.Clear();
	}

	public bool TextInput(ref string value)
	{
		bool result = false;
		bool shiftPressed = KeyState(Keys.LeftShift) || KeyState(Keys.RightShift);
		foreach (Keys pressedKey in PressedKeys)
		{
			if (pressedKey == Keys.Back)
			{
				if (value.Length > 0)
				{
					value = value.Remove(value.Length - 1, 1);
					result = true;
				}
				continue;
			}
			string text = KeyText(pressedKey, shiftPressed);
			if (text.Length != 0)
			{
				result = true;
				value += text;
			}
		}
		return result;
	}

	public static string KeyText(Keys key, bool shiftPressed)
	{
		switch (key)
		{
		case Keys.A:
			if (!shiftPressed)
			{
				return "a";
			}
			return "A";
		case Keys.B:
			if (!shiftPressed)
			{
				return "b";
			}
			return "B";
		case Keys.C:
			if (!shiftPressed)
			{
				return "c";
			}
			return "C";
		case Keys.D:
			if (!shiftPressed)
			{
				return "d";
			}
			return "D";
		case Keys.E:
			if (!shiftPressed)
			{
				return "e";
			}
			return "E";
		case Keys.F:
			if (!shiftPressed)
			{
				return "f";
			}
			return "F";
		case Keys.G:
			if (!shiftPressed)
			{
				return "g";
			}
			return "G";
		case Keys.H:
			if (!shiftPressed)
			{
				return "h";
			}
			return "H";
		case Keys.I:
			if (!shiftPressed)
			{
				return "i";
			}
			return "I";
		case Keys.J:
			if (!shiftPressed)
			{
				return "j";
			}
			return "J";
		case Keys.K:
			if (!shiftPressed)
			{
				return "k";
			}
			return "K";
		case Keys.L:
			if (!shiftPressed)
			{
				return "l";
			}
			return "L";
		case Keys.M:
			if (!shiftPressed)
			{
				return "m";
			}
			return "M";
		case Keys.N:
			if (!shiftPressed)
			{
				return "n";
			}
			return "N";
		case Keys.O:
			if (!shiftPressed)
			{
				return "o";
			}
			return "O";
		case Keys.P:
			if (!shiftPressed)
			{
				return "p";
			}
			return "P";
		case Keys.Q:
			if (!shiftPressed)
			{
				return "q";
			}
			return "Q";
		case Keys.R:
			if (!shiftPressed)
			{
				return "r";
			}
			return "R";
		case Keys.S:
			if (!shiftPressed)
			{
				return "s";
			}
			return "S";
		case Keys.T:
			if (!shiftPressed)
			{
				return "t";
			}
			return "T";
		case Keys.U:
			if (!shiftPressed)
			{
				return "u";
			}
			return "U";
		case Keys.V:
			if (!shiftPressed)
			{
				return "v";
			}
			return "V";
		case Keys.W:
			if (!shiftPressed)
			{
				return "w";
			}
			return "W";
		case Keys.X:
			if (!shiftPressed)
			{
				return "x";
			}
			return "X";
		case Keys.Y:
			if (!shiftPressed)
			{
				return "y";
			}
			return "Y";
		case Keys.Z:
			if (!shiftPressed)
			{
				return "z";
			}
			return "Z";
		case Keys.D0:
			if (shiftPressed)
			{
				return "=";
			}
			return "0";
		case Keys.D1:
			if (shiftPressed)
			{
				return "!";
			}
			return "1";
		case Keys.D2:
			if (shiftPressed)
			{
				return "\"";
			}
			return "2";
		case Keys.D3:
			if (shiftPressed)
			{
				return "·";
			}
			return "3";
		case Keys.D4:
			if (shiftPressed)
			{
				return "$";
			}
			return "4";
		case Keys.D5:
			if (shiftPressed)
			{
				return "%";
			}
			return "5";
		case Keys.D6:
			if (shiftPressed)
			{
				return "&";
			}
			return "6";
		case Keys.D7:
			if (shiftPressed)
			{
				return "/";
			}
			return "7";
		case Keys.D8:
			if (shiftPressed)
			{
				return "(";
			}
			return "8";
		case Keys.D9:
			if (shiftPressed)
			{
				return ")";
			}
			return "9";
		case Keys.NumPad0:
			return "0";
		case Keys.NumPad1:
			return "1";
		case Keys.NumPad2:
			return "2";
		case Keys.NumPad3:
			return "3";
		case Keys.NumPad4:
			return "4";
		case Keys.NumPad5:
			return "5";
		case Keys.NumPad6:
			return "6";
		case Keys.NumPad7:
			return "7";
		case Keys.NumPad8:
			return "8";
		case Keys.NumPad9:
			return "9";
		case Keys.Decimal:
			return ".";
		case Keys.OemMinus:
			if (shiftPressed)
			{
				return "_";
			}
			return "-";
		case Keys.OemComma:
			return ",";
		case Keys.OemPeriod:
			return ".";
		case Keys.OemPlus:
			return "+";
		case Keys.Space:
			return " ";
		case Keys.Tab:
			return "\t";
		default:
			return "";
		}
	}
}
