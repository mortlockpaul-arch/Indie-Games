using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.text;

public class Shape
{
	private bool[,] dot;

	public int width;

	public Shape(int[] src)
	{
		dot = new bool[5, 5];
		width = 0;
		for (int i = 0; i < src.Length; i++)
		{
			int num = 0;
			while (src[i] > 0)
			{
				dot[i, num] = src[i] % 10 == 1;
				src[i] /= 10;
				num++;
				if (num > width)
				{
					width = num;
				}
			}
		}
	}

	public void Draw(Vector2 loc, float scale, Color c, Texture2D nullTex)
	{
		loc.X += (float)width * scale;
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				if (dot[i, j])
				{
					SpriteTools.sprite.Draw(nullTex, loc + new Vector2((float)j * (0f - scale), (float)i * scale), new Rectangle(0, 0, 1, 1), c, 0f, default(Vector2), scale, SpriteEffects.None, 1f);
				}
			}
		}
	}

	public void Draw(Vector2 loc, float scale, Color c, float angle, Texture2D nullTex)
	{
		loc += new Vector2((float)width * scale * VScroll.xVec.X, (float)width * scale * VScroll.xVec.Y);
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				if (dot[i, j])
				{
					SpriteTools.sprite.Draw(Game1.nullTex, loc + new Vector2(VScroll.xVec.X * (0f - (float)j) * scale, VScroll.xVec.Y * (0f - (float)j) * scale) + new Vector2(VScroll.yVec.X * (0f - (float)i) * scale, VScroll.yVec.Y * (0f - (float)i) * scale), new Rectangle(0, 0, 1, 1), c, angle, default(Vector2), scale, SpriteEffects.None, 1f);
				}
			}
		}
	}
}
