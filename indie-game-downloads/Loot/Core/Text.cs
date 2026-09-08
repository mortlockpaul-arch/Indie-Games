using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Core;

public static class Text
{
	private const string fontResource = "Sprites\\UI\\FontGothic";

	public const int CharWidth = 24;

	public const int CharHeight = 40;

	public const int ButtonWidth = 48;

	public const int LineHeight = 40;

	public const string ButtonA = "\u0080\u0081";

	public const string ButtonB = "\u0082\u0083";

	public const string ButtonX = "\u0084\u0085";

	public const string ButtonY = "\u0086\u0087";

	public const string LeftStick = "\u0088\u0089";

	public const string RightStick = "\u008a\u008b";

	public const string ButtonBack = "\u008c\u008d";

	public const string ButtonStart = "\u008e\u008f";

	public const string ButtonLB = "\u0090\u0091";

	public const string ButtonRB = "\u0092\u0093";

	public const string ButtonLT = "\u0094\u0095";

	public const string ButtonRT = "\u0096\u0097";

	public const char ArrowUp = '\u001d';

	public const char ArrowDown = '\u001e';

	public const char ArrowRight = '\u001f';

	private static Texture2D font;

	private static readonly Vector2 charOrigin = new Vector2(12f, 16f);

	private static Rectangle[] charSrc = new Rectangle[256];

	private static Vector2 buttonOrigin = new Vector2(16f, 16f);

	private static Vector2 shadowOffset = new Vector2(2f, 2f);

	public static void LoadContent(ContentManager content)
	{
		font = content.Load<Texture2D>("Sprites\\UI\\FontGothic");
		for (int i = 0; i < 256; i++)
		{
			int x = i % 16 * 24;
			int y = i / 16 * 40;
			charSrc[i] = new Rectangle(x, y, 24, 40);
		}
	}

	public static bool IsVowelFirst(string s)
	{
		switch (s[0])
		{
		case 'A':
		case 'E':
		case 'I':
		case 'O':
		case 'U':
		case 'a':
		case 'e':
		case 'i':
		case 'o':
		case 'u':
			return true;
		default:
			return false;
		}
	}

	public static void Draw(SpriteBatch spriteBatch, ref Vector2 v, char c, Color color)
	{
		Draw(spriteBatch, ref v, c, color, 1f);
	}

	public static void Draw(SpriteBatch spriteBatch, ref Vector2 v, char c, Color color, float scale)
	{
		if (c != 0)
		{
			Vector2 vector = Vector2.Zero;
			if (c >= '\u0080')
			{
				color = Color.White * ((float)(int)color.A / 255f);
				vector = new Vector2(0f, -3f) * scale;
			}
			spriteBatch.Draw(font, v + vector, charSrc[(uint)c], color, 0f, charOrigin, scale, SpriteEffects.None, 0f);
			v.X += 24f * scale;
		}
	}

	public static void Draw(SpriteBatch spriteBatch, ref Vector2 v, char[] chars, Color color)
	{
		Draw(spriteBatch, ref v, chars, color, 1f);
	}

	public static void Draw(SpriteBatch spriteBatch, ref Vector2 v, char[] chars, Color color, float scale)
	{
		for (int i = 0; i < chars.Length; i++)
		{
			Draw(spriteBatch, ref v, chars[i], color, scale);
		}
	}

	public static void Draw(SpriteBatch spriteBatch, Vector2 v, string s, Color color)
	{
		Draw(spriteBatch, ref v, s, color, 1f);
	}

	public static void Draw(SpriteBatch spriteBatch, ref Vector2 v, string s, Color color)
	{
		Draw(spriteBatch, ref v, s, color, 1f);
	}

	public static void Draw(SpriteBatch spriteBatch, ref Vector2 v, string s, Color color, float scale)
	{
		for (int i = 0; i < s.Length; i++)
		{
			Draw(spriteBatch, ref v, s[i], color, scale);
		}
	}

	public static void DrawCentered(SpriteBatch spriteBatch, Vector2 v, string s, Color color)
	{
		Vector2 vector = new Vector2((s.Length - 1) * 24 / 2, 0f);
		Draw(spriteBatch, v - vector, s, color);
	}

	public static void DrawCentered(SpriteBatch spriteBatch, Vector2 v, int number, Color color)
	{
		Draw(spriteBatch, v + new Vector2(12 - Width(number) / 2, 0f), number, color);
	}

	public static void DrawShadowString(SpriteBatch spriteBatch, Vector2 v, string s, Color textColor, Color shadowColor, float scale)
	{
		DrawShadowString(spriteBatch, ref v, s, textColor, shadowColor, scale);
	}

	public static void DrawShadowString(SpriteBatch spriteBatch, ref Vector2 v, string s, Color textColor, Color shadowColor, float scale)
	{
		Vector2 v2 = v + shadowOffset * scale;
		Draw(spriteBatch, ref v2, s, shadowColor, scale);
		Draw(spriteBatch, ref v, s, textColor, scale);
	}

	public static void DrawShadowString(SpriteBatch spriteBatch, ref Vector2 v, int number, Color textColor, Color shadowColor, float scale)
	{
		Vector2 v2 = v + shadowOffset * scale;
		Draw(spriteBatch, ref v2, number, shadowColor, scale);
		Draw(spriteBatch, ref v, number, textColor, scale);
	}

	public static void Draw(SpriteBatch spriteBatch, Vector2 v, FormattedText formattedText, Color color)
	{
		char[,] text = formattedText.text;
		int num = (int)v.X;
		for (int i = 0; i < text.GetLength(0); i++)
		{
			for (int j = 0; j < text.GetLength(1); j++)
			{
				char c = text[i, j];
				Draw(spriteBatch, ref v, c, color);
			}
			v.X = num;
			v.Y += 40f;
		}
	}

	public static void Draw(SpriteBatch spriteBatch, Vector2 v, int number, Color color)
	{
		Draw(spriteBatch, ref v, number, color, 1f);
	}

	public static void Draw(SpriteBatch spriteBatch, ref Vector2 v, int number, Color color)
	{
		Draw(spriteBatch, ref v, number, color, 1f);
	}

	public static void Draw(SpriteBatch spriteBatch, ref Vector2 v, int number, Color color, float scale)
	{
		int num;
		if (number > 0)
		{
			num = 1 + (int)Math.Log10(number);
		}
		else if (number == 0)
		{
			num = 1;
		}
		else
		{
			Draw(spriteBatch, ref v, '-', color, scale);
			number *= -1;
			num = 1 + (int)Math.Log10(number);
		}
		for (int i = 0; i < num; i++)
		{
			int num2 = number / (int)Math.Pow(10.0, num - i - 1) % 10;
			spriteBatch.Draw(font, v, charSrc[48 + num2], color, 0f, charOrigin, scale, SpriteEffects.None, 0f);
			v.X += 24f * scale;
		}
	}

	public static int Width(char[] chars)
	{
		return chars.Length * 24;
	}

	public static int Width(string s)
	{
		return s.Length * 24;
	}

	public static int Width(int number)
	{
		if (number > 0)
		{
			return 24 * (1 + (int)Math.Log10(number));
		}
		if (number == 0)
		{
			return 24;
		}
		number *= -1;
		return 24 * (2 + (int)Math.Log10(number));
	}
}
