using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hammer;

public class Text
{
	public SpriteFont font;

	public Text(SpriteFont font)
	{
		this.font = font;
	}

	public static void print(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color col, Texture2D buttons)
	{
		string[] separator = new string[4] { "(A)", "(B)", "(X)", "(Y)" };
		string[] array = text.Split(separator, StringSplitOptions.None);
		int num = 0;
		float num2 = 0f;
		int num3 = -1;
		for (int i = 0; i < text.Length - 2; i++)
		{
			string text2 = text.Substring(i, 3);
			num3 = -1;
			switch (text2)
			{
			case "(A)":
				num3 = 0;
				break;
			case "(B)":
				num3 = 1;
				break;
			case "(X)":
				num3 = 2;
				break;
			case "(Y)":
				num3 = 3;
				break;
			}
			if (num3 >= 0)
			{
				num2 = ((num <= 0) ? (num2 + font.MeasureString(array[num]).X) : (num2 + font.MeasureString(array[num] + "   ").X));
				sb.Draw(buttons, new Vector2(pos.X + num2, pos.Y), new Rectangle(num3 * buttons.Width / 4, 0, buttons.Width / 4, buttons.Height), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
				num++;
			}
		}
		string text3 = "";
		for (int j = 0; j < array.Length; j++)
		{
			text3 = text3 + array[j] + "   ";
		}
		sb.DrawString(font, text3, pos, col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
	}
}
