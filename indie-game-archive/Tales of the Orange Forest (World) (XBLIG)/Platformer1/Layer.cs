using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer1;

internal class Layer
{
	public int Segments = 400;

	public int Background_Class;

	public Texture2D[] Textures { get; private set; }

	public float ScrollRateX { get; private set; }

	public float ScrollRateY { get; private set; }

	public Layer(ContentManager content, string basePath, float scrollRateX, float scrollRateY, int Background_Class)
	{
		this.Background_Class = Background_Class;
		if (Background_Class == 4)
		{
			Segments = 2;
		}
		Textures = new Texture2D[Segments];
		Texture2D texture2D = content.Load<Texture2D>(basePath);
		for (int i = 0; i < Segments; i++)
		{
			Textures[i] = texture2D;
		}
		ScrollRateX = scrollRateX;
		ScrollRateY = scrollRateY;
	}

	public void Draw(SpriteBatch spriteBatch, LevelBuilder levelBuilder, Level level, float cameraPosition, float cameraHeightPosition, Color color, Vector2 Offset)
	{
		int width = Textures[0].Width;
		float num = (cameraPosition - -1000f) * ScrollRateX;
		int num2 = (int)MathHelper.Clamp((float)Math.Floor(num / (float)width), 0f, 1000000f);
		num = (num / (float)width - (float)num2) * (float)(-width);
		int height = Textures[0].Height;
		float num3 = cameraHeightPosition * ScrollRateY;
		num3 = num3 / (float)height * (float)(-height);
		if (Background_Class == 4)
		{
			spriteBatch.Draw(Textures[0], new Vector2(num, num3) + Offset, null, color, 0f, new Vector2(Textures[0].Width / 2, Textures[0].Width / 2), new Vector2(200f, 1f), SpriteEffects.None, 1f);
			return;
		}
		for (int i = 0; i < Segments; i++)
		{
			if (level != null)
			{
				spriteBatch.Draw(Textures[i], new Vector2(num + (float)width * level.MasterScale * (float)i, num3) + Offset, null, color, 0f, new Vector2(Textures[i].Width / 2, Textures[i].Width / 2), 2f * level.MasterScale, SpriteEffects.None, 1f);
			}
			else
			{
				spriteBatch.Draw(Textures[i], new Vector2(num + (float)width * levelBuilder.mainGame.Global_Scaler * (float)i, num3) + Offset, null, color, 0f, new Vector2(Textures[i].Width / 2, Textures[i].Width / 2), 2f * levelBuilder.mainGame.Global_Scaler, SpriteEffects.None, 1f);
			}
		}
	}
}
