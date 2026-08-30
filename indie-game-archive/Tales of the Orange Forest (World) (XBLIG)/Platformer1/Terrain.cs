using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer1;

internal class Terrain
{
	public int Segments = 300;

	public int Background_Class;

	public Vector2[] points;

	private Random random = new Random(354668);

	private int XStep;

	private int YStep;

	private int HoldAmount;

	private int[] RisingVote;

	private int RisingVoteSum;

	private bool Rising = true;

	private int Terrain_Height = 1000;

	private int Terrain_Width = 1;

	public Texture2D[] Textures { get; private set; }

	public float ScrollRateX { get; private set; }

	public float ScrollRateY { get; private set; }

	public Terrain(ContentManager content, Game mainGame, float scrollRateX, float scrollRateY, int Background_Class)
	{
		int num = 0;
		int num2 = 0;
		points = new Vector2[100];
		ScrollRateX = scrollRateX;
		ScrollRateY = scrollRateY;
		ref Vector2 reference = ref points[0];
		reference = new Vector2(0f, -mainGame.GraphicsDevice.Viewport.Y / 4);
		if (Background_Class == 0)
		{
			XStep = 5;
			YStep = 5;
			HoldAmount = 5;
			Terrain_Width = XStep * 2;
		}
		RisingVote = new int[HoldAmount];
		for (int i = 1; i < points.Length; i++)
		{
			num2 = random.Next(YStep);
			num += XStep;
			for (int j = 0; j < HoldAmount && i - j >= 0 && i - (j - 1) < points.Length && i - (j - 1) >= 0; j++)
			{
				if (points[i - j].Y > points[i - (j - 1)].Y)
				{
					RisingVote[j] = 1;
				}
				else
				{
					RisingVote[j] = -1;
				}
			}
			RisingVoteSum = 0;
			for (int k = 0; k < HoldAmount; k++)
			{
				RisingVoteSum += RisingVote[k];
			}
			RisingVote = new int[HoldAmount];
			if (Rising)
			{
				if (RisingVoteSum >= 0)
				{
					if (RisingVoteSum > HoldAmount - 1)
					{
						Rising = false;
					}
					else
					{
						Rising = true;
					}
				}
			}
			else if (RisingVoteSum <= 0)
			{
				if (RisingVoteSum > -HoldAmount + 1)
				{
					Rising = true;
				}
				else
				{
					Rising = false;
				}
			}
			if (Rising)
			{
				_ = points[i - 1];
				_ = points[i];
				ref Vector2 reference2 = ref points[i];
				reference2 = new Vector2(num, (float)num2 + points[i - 1].Y);
			}
			else
			{
				_ = points[i - 1];
				_ = points[i];
				ref Vector2 reference3 = ref points[i];
				reference3 = new Vector2(num, (float)num2 - points[i - 1].Y);
			}
		}
	}

	public void Draw(SpriteBatch spriteBatch, Level level, float cameraPosition, float cameraHeightPosition, Color color, Vector2 Offset)
	{
		for (int i = 0; i < points.Length; i++)
		{
			if (i + 1 <= points.Length)
			{
				Vector2 value = new Vector2(points[i].X, points[i].Y);
				Vector2 value2 = new Vector2(points[i + 1].X, points[i + 1].Y);
				float rotation = (float)Math.Atan2(value2.Y - value.Y, value2.X - value.X);
				float num = Vector2.Distance(value, value2);
				Texture2D texture2D = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
				texture2D.SetData(new Color[1] { color });
				spriteBatch.Draw(texture2D, points[i], null, color, rotation, Vector2.Zero, new Vector2(num * level.mainGame.Global_Scaler, (float)Terrain_Width * level.mainGame.Global_Scaler), SpriteEffects.None, 0f);
				Vector2 value3 = new Vector2(points[i].X, Terrain_Height);
				float rotation2 = (float)Math.Atan2(points[i].Y - value3.Y, points[i].X - value3.X);
				float num2 = Vector2.Distance(value3, points[i]);
				Texture2D texture2D2 = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
				texture2D2.SetData(new Color[1] { color });
				spriteBatch.Draw(texture2D2, points[i], null, color, rotation2, Vector2.Zero, new Vector2(num2 * level.mainGame.Global_Scaler, (float)Terrain_Width * level.mainGame.Global_Scaler), SpriteEffects.None, 0f);
			}
		}
	}
}
