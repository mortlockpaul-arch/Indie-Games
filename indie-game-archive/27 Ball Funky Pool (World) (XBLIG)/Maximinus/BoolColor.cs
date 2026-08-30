using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class BoolColor
{
	public static Color TrueColor = Color.Green;

	public static Color FalseColor = Color.Red;

	public static string ToString(Menus.BoolEntryType type, bool value)
	{
		switch (type)
		{
		case Menus.BoolEntryType.OnOff:
			if (!value)
			{
				return "Off";
			}
			return "On";
		case Menus.BoolEntryType.TrueFalse:
			if (!value)
			{
				return "False";
			}
			return "True";
		case Menus.BoolEntryType.YesNo:
			if (!value)
			{
				return "No";
			}
			return "Yes";
		default:
			throw new Exception("not supported " + type);
		}
	}

	public static string TrueString(Menus.BoolEntryType type)
	{
		return ToString(type, value: true);
	}

	public static string FalseString(Menus.BoolEntryType type)
	{
		return ToString(type, value: false);
	}

	public static Color ToColor(bool value)
	{
		if (!value)
		{
			return FalseColor;
		}
		return TrueColor;
	}

	public static Vector2 Size(Menus.BoolEntryType type, bool value, SpriteFont font)
	{
		return font.MeasureString(ToString(type, value));
	}
}
