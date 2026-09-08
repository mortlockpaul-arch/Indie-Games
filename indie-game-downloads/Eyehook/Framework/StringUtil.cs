using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eyehook.Framework;

public static class StringUtil
{
	public static string WordWrap(SpriteFont spriteFont, string text, int maxWidth)
	{
		float x = spriteFont.MeasureString(" ").X;
		StringBuilder stringBuilder = new StringBuilder();
		float num = 0f;
		char[] separator = new char[1] { ' ' };
		string[] array = text.Split(separator);
		foreach (string text2 in array)
		{
			char[] separator2 = new char[1] { '\n' };
			string[] array2 = text2.Split(separator2);
			for (int j = 0; j < array2.Length; j++)
			{
				if (j > 0)
				{
					stringBuilder.Append("\n");
					num = 0f;
				}
				string text3 = array2[j];
				Vector2 vector = spriteFont.MeasureString(text3);
				if ((double)num + (double)vector.X < (double)maxWidth)
				{
					stringBuilder.Append(text3);
					stringBuilder.Append(" ");
					num += vector.X + x;
					continue;
				}
				stringBuilder.Append("\n");
				stringBuilder.Append(text3);
				num = vector.X;
				if ((double)num > 0.0)
				{
					stringBuilder.Append(" ");
					num += x;
				}
			}
		}
		return stringBuilder.ToString();
	}
}
