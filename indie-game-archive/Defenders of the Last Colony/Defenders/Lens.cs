using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Lens
{
	public bool active;

	public Vector2 position;

	private Vector2 screenPosition;

	private Vector2 screen;

	public float originalSize;

	public float size;

	public Texture2D txGlow;

	public Color col;

	public bool visible = true;

	private float centerDist;

	public Texture2D txDirt;

	private List<Aberration> aberrations;

	public Lens(Vector2 position, float R, float G, float B, float A, float size, Texture2D txGlow, Texture2D txAberration1, Texture2D txAberration2, Texture2D txAberration3, Texture2D txAberration4, Texture2D txDirt)
	{
		active = true;
		Random random = new Random();
		this.txDirt = txDirt;
		this.size = size;
		originalSize = size;
		this.position = position;
		col.R = (byte)(R * 255f);
		col.G = (byte)(G * 255f);
		col.B = (byte)(B * 255f);
		col.A = (byte)(A * 255f);
		this.txGlow = txGlow;
		col = new Color(0.1f, 0.5f, 1.1f);
		int num = random.Next(4, 22);
		aberrations = new List<Aberration>(num);
		int num2 = -1;
		int num3 = 1;
		for (int i = 0; i < num; i++)
		{
			Aberration aberration = new Aberration(new Color(R, G, B, A), random.Next(100));
			while (num3 == num2)
			{
				num3 = random.Next(7);
			}
			num2 = num3;
			switch (num2)
			{
			case 1:
				aberration.tx = txAberration1;
				break;
			case 2:
				aberration.tx = txAberration2;
				break;
			case 3:
				aberration.tx = txAberration3;
				break;
			default:
				aberration.tx = txAberration4;
				break;
			}
			aberrations.Add(aberration);
		}
	}

	public void Update(Vector2 screenPosition, Vector2 screen)
	{
		this.screenPosition = screenPosition;
		this.screen = screen;
		int num = 4;
		Rectangle value = new Rectangle(0, 0, (int)screen.X, (int)screen.Y);
		Rectangle rectangle = new Rectangle((int)screenPosition.X - num / 2, (int)screenPosition.Y - num / 2, num, num);
		centerDist = MathHelper.Clamp(Vector2.Distance(screen / 2f, screenPosition) / 400f, 0f, 1f);
		centerDist = 1f - centerDist;
		active = true;
		if (rectangle.Intersects(value))
		{
			col.A = (byte)(MathHelper.Lerp((float)(int)col.A / 255f, 1f, 0.1f) * 255f);
		}
		else
		{
			col.A = (byte)(MathHelper.Lerp((float)(int)col.A / 255f, 0f, 0.2f) * 255f);
		}
		if (!visible)
		{
			col.A = (byte)(MathHelper.Lerp((float)(int)col.A / 255f, 0f, 0.5f) * 255f);
		}
		visible = true;
		for (int i = 0; i < aberrations.Count; i++)
		{
			aberrations[i].Update(screen, screenPosition, col);
		}
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		Color color = new Color(((float)(int)col.R / 255f + 128f) / 255f, ((float)(int)col.G / 255f + 128f) / 255f, ((float)(int)col.B / 255f + 128f) / 255f, (float)(int)col.A / 255f);
		spriteBatch.Draw(txGlow, screenPosition, null, color, 0f, new Vector2((float)txGlow.Width / 2f, (float)txGlow.Height / 2f), size, SpriteEffects.None, 0f);
		spriteBatch.Draw(txGlow, screenPosition, null, col, 0f, new Vector2((float)txGlow.Width / 2f, (float)txGlow.Height / 2f), new Vector2(5f, 0.2f) * size, SpriteEffects.None, 0f);
		spriteBatch.Draw(txGlow, screenPosition, null, color * ((float)(int)col.A / 255f) * 0.5f, 0f, new Vector2((float)txGlow.Width / 2f, (float)txGlow.Height / 2f), size * centerDist * 20f, SpriteEffects.None, 0f);
		spriteBatch.Draw(txGlow, new Rectangle(0, 0, (int)screen.X, (int)screen.Y), null, Color.White * centerDist * 0.5f * ((float)(int)col.A / 255f));
		spriteBatch.Draw(txDirt, new Rectangle(0, 0, (int)screen.X, (int)screen.Y), null, Color.White * centerDist * ((float)(int)col.A / 255f));
		spriteBatch.Draw(txDirt, new Rectangle(0, 0, (int)screen.X, (int)screen.Y), null, Color.White * centerDist * centerDist * ((float)(int)col.A / 255f));
		for (int i = 0; i < aberrations.Count; i++)
		{
			aberrations[i].Draw(spriteBatch);
		}
	}
}
