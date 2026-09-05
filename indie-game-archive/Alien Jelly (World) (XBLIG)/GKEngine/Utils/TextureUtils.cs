using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Utils;

public class TextureUtils
{
	public static Texture2D TexturesToSheet(Texture2D[] aTextures, Point oGrid)
	{
		Texture2D texture2D = null;
		Color[] array = null;
		Point point = default(Point);
		if (aTextures.Length > 0)
		{
			int width = aTextures[0].Width;
			int height = aTextures[0].Height;
			array = new Color[width * height];
			texture2D = new Texture2D(GameEngine.Graphics.GraphicsDevice, width * oGrid.X, height * oGrid.Y);
			for (int i = 0; i < aTextures.Length; i++)
			{
				point.X = i % oGrid.X;
				point.Y = (int)Math.Floor((float)i / (float)oGrid.X);
				aTextures[i].GetData(array);
				texture2D.SetData(0, new Rectangle(point.X * width, point.Y * height, width, height), array, 0, width * height);
			}
		}
		return texture2D;
	}

	public static Texture2D[] SheetToTextures(Texture2D oTexture, int xGridX, int xGridY, int xCount)
	{
		Texture2D[] array = new Texture2D[xCount];
		Color[] array2 = null;
		Point point = default(Point);
		int num = oTexture.Width / xGridX;
		int num2 = oTexture.Height / xGridY;
		array2 = new Color[num * num2];
		for (int i = 0; i < xCount; i++)
		{
			point.X = i % xGridX;
			point.Y = (int)Math.Floor((float)i / (float)xGridX);
			oTexture.GetData(0, new Rectangle(point.X * num, point.Y * num2, num, num2), array2, 0, num * num2);
			array[i] = new Texture2D(GameEngine.Graphics.GraphicsDevice, num, num2);
			array[i].SetData(array2);
		}
		return array;
	}
}
