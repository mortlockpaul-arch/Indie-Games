using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer1;

internal class Clouds
{
	public Texture2D[] Textures { get; private set; }

	public float ScrollRateX { get; private set; }

	public float ScrollRateY { get; private set; }

	public float CloudSpeed { get; set; }

	public float CloudSpeedRate { get; set; }

	public Clouds(ContentManager content, string basePath, float scrollRateX, float scrollRateY, float cloudSpeedRate)
	{
		Textures = new Texture2D[3];
		for (int i = 0; i < 3; i++)
		{
			Textures[i] = content.Load<Texture2D>(basePath);
		}
		CloudSpeedRate = cloudSpeedRate;
		ScrollRateX = scrollRateX;
		ScrollRateY = scrollRateY;
	}

	public void Draw(SpriteBatch spriteBatch, float cameraPosition, float cameraHeightPosition, Color color, Vector2 Offset)
	{
		int width = Textures[0].Width;
		float num = (cameraPosition - -5000f) * ScrollRateX + CloudSpeed;
		int num2 = (int)MathHelper.Clamp((float)Math.Floor(num / (float)width), 0f, 1000000f);
		int num3 = num2 + 1;
		num = (num / (float)width - (float)num2) * (float)(-width);
		CloudSpeed += CloudSpeedRate;
		int height = Textures[0].Height;
		float num4 = cameraHeightPosition * ScrollRateY;
		num4 = num4 / (float)height * (float)(-height);
		spriteBatch.Draw(Textures[num2 % Textures.Length], new Vector2(num, num4) + Offset, color);
		spriteBatch.Draw(Textures[num3 % Textures.Length], new Vector2(num + (float)width, num4) + Offset, color);
	}
}
