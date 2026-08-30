using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerFromHell;

internal class Layer
{
	public int worldNumber;

	public Texture2D[] Textures { get; private set; }

	public float ScrollRate { get; private set; }

	public Layer(ContentManager content, string basePath, float scrollRate, int worldNumber)
	{
		basePath = "Backgrounds/" + worldNumber + "/" + basePath;
		this.worldNumber = worldNumber;
		Textures = new Texture2D[1];
		for (int i = 0; i < 1; i++)
		{
			Textures[i] = content.Load<Texture2D>(basePath + "_" + i);
		}
		ScrollRate = scrollRate;
	}

	public void Draw(SpriteBatch spriteBatch, float cameraPosition, float cameraPositionY)
	{
		float num = 0.2f;
		int width = Textures[0].Width;
		float num2 = ScrollRate;
		float num3 = ScrollRate;
		if (worldNumber != 3)
		{
			num2 = cameraPosition * ScrollRate;
			num3 = cameraPositionY * ScrollRate;
		}
		int num4 = (int)Math.Floor(num2 / (float)width);
		int num5 = num4 + 1;
		int num6 = num5 + 1;
		num2 = (num2 / (float)width - (float)num4) * (float)(-width);
		spriteBatch.Draw(Textures[num4 % Textures.Length], new Vector2(num2, (0f - num3) * num), Color.White);
		if (worldNumber != 2)
		{
			spriteBatch.Draw(Textures[num5 % Textures.Length], new Vector2(num2 + (float)width, (0f - num3) * num), Color.White);
			spriteBatch.Draw(Textures[num6 % Textures.Length], new Vector2(num2 + (float)width + (float)width, (0f - num3) * num), Color.White);
		}
	}
}
