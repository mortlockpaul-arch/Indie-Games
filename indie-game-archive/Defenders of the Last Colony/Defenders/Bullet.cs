using System;
using Hammer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Bullet
{
	public Texture2D texture;

	public Vector2 position;

	public Vector2 target;

	public string type;

	public float angle;

	private float drawingAngle;

	private float rotDir;

	private Vector2 finalSize;

	private Vector2 size;

	private float speed;

	public int time;

	public int life;

	public int maxLife;

	private float transp;

	private Color color;

	private Random random;

	public int id;

	public float Damage;

	public bool Active;

	public int Width => texture.Width;

	public int Height => texture.Height;

	public Bullet(int id, string type, Vector2 position, float angle, Texture2D texture, Color color, float speed, Vector2 size, int life, float damage)
	{
		Initialize(id, type, position, angle, texture, color, speed, size, life, damage, Vector2.Zero);
	}

	public Bullet(int id, string type, Vector2 position, float angle, Texture2D texture, Color color, float speed, Vector2 size, int life, float damage, Vector2 target)
	{
		Initialize(id, type, position, angle, texture, color, speed, size, life, damage, target);
	}

	public void Initialize(int id, string type, Vector2 position, float angle, Texture2D texture, Color color, float speed, Vector2 size, int life, float damage, Vector2 t)
	{
		this.id = id;
		this.texture = texture;
		this.type = type;
		this.position = position;
		target = t;
		this.angle = angle;
		this.speed = speed;
		finalSize = size;
		this.size = new Vector2(0.5f, 5f);
		this.color = color;
		time = life;
		this.life = life;
		maxLife = life;
		transp = (float)(int)color.A / 255f;
		Damage = damage;
		random = new Random();
		drawingAngle = angle;
		if (random.Next(100) < 50)
		{
			rotDir = 0.5f;
		}
		else
		{
			rotDir = -0.5f;
		}
		if (type == "Radial")
		{
			speed = (float)random.Next(100, 200) / 10f;
		}
		Active = true;
	}

	public void LoadContent()
	{
	}

	public void Update(Vector2 t)
	{
		switch (type)
		{
		case "Pmissile":
			time--;
			if (time < 0)
			{
				Active = false;
			}
			angle = Math2.TurnToFace(position, target, angle, speed * 0.01f);
			position = new Vector2(position.X + (float)Math.Cos(angle) * speed, position.Y + (float)Math.Sin(angle) * speed);
			size = new Vector2(MathHelper.Lerp(size.X, finalSize.X, 0.5f), MathHelper.Lerp(size.Y, finalSize.Y, 0.5f));
			drawingAngle = angle;
			if (Vector2.Distance(position, target) < (float)Width)
			{
				Active = false;
			}
			break;
		case "Radial":
		{
			time--;
			if (time < 0)
			{
				Active = false;
			}
			angle += 0.05f + speed / 50f;
			float num = 20f + (70f + (float)Math.Sin((float)time / speed / 5f) * 50f) * (float)(1 - time / maxLife);
			position = new Vector2(t.X + (float)Math.Cos(angle) * num, t.Y + (float)Math.Sin(angle) * num);
			size = new Vector2(MathHelper.Lerp(size.X, finalSize.X * 0.6f, 0.01f), MathHelper.Lerp(size.Y, finalSize.Y * 4f, 0.01f));
			drawingAngle = Math2.TurnToFace(position, t, angle, 10f);
			if (Vector2.Distance(position, target) < (float)Width)
			{
				Active = false;
			}
			break;
		}
		default:
			position.X += (float)(Math.Cos(angle) * (double)speed);
			position.Y += (float)(Math.Sin(angle) * (double)speed);
			time--;
			if (time < 0)
			{
				transp -= 0.2f;
			}
			if (transp <= 0.2f)
			{
				Active = false;
			}
			color.A = (byte)(transp * 255f);
			if (type == "WIDE")
			{
				drawingAngle += rotDir;
				size = finalSize;
			}
			else
			{
				size.X = MathHelper.Lerp(size.X, finalSize.X, 0.5f);
				size.Y = MathHelper.Lerp(size.Y, finalSize.Y, 0.5f);
				drawingAngle = angle;
			}
			break;
		}
		if (position.X < -640f || position.X > 1920f || position.Y < -360f || position.Y > 1080f)
		{
			Active = false;
		}
	}

	public void Draw(SpriteBatch spriteBatch, int type)
	{
		if (time >= life)
		{
			return;
		}
		switch (type)
		{
		case 1:
			spriteBatch.Draw(texture, position, null, color, drawingAngle, new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, 0f);
			break;
		case 2:
			switch (this.type)
			{
			case "Pmissile":
				spriteBatch.Draw(texture, position, null, Color.White, drawingAngle, new Vector2((float)texture.Width * 0.6f, texture.Height / 2), size / 2f, SpriteEffects.None, 0f);
				spriteBatch.Draw(texture, position, null, Color.White * 0.5f, drawingAngle, new Vector2((float)texture.Width * 0.7f, texture.Height / 2), size, SpriteEffects.None, 0f);
				spriteBatch.Draw(texture, position, null, Color.White * 0.5f, drawingAngle, new Vector2((float)texture.Width * 0.8f, texture.Height / 2), size * 2f, SpriteEffects.None, 0f);
				spriteBatch.Draw(texture, position, null, new Color(0.8f, 0.9f, 1f, 0.5f) * ((float)random.Next(0, 75) / 100f), 0f, new Vector2((float)texture.Width * 0.6f, texture.Height / 2), (Vector2.UnitX * 4f * ((float)random.Next(50, 100) / 100f) + Vector2.One) * ((float)random.Next(10, 200) / 100f), SpriteEffects.None, 0f);
				break;
			case "Radial":
				spriteBatch.Draw(texture, position, null, color, drawingAngle, new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, 0f);
				spriteBatch.Draw(texture, position + Vector2.One, null, color * ((float)random.Next(0, 75) / 100f), drawingAngle + (float)life / 10f, new Vector2(texture.Width / 2, texture.Height / 2), size * 0.5f, SpriteEffects.None, 0f);
				spriteBatch.Draw(texture, position, null, new Color(0.8f, 0.9f, 1f, 0.5f) * ((float)random.Next(0, 75) / 100f), 0f, new Vector2((float)texture.Width * 0.6f, texture.Height / 2), (Vector2.UnitX * 4f * ((float)random.Next(50, 100) / 100f) + Vector2.One) * ((float)random.Next(10, 200) / 100f), SpriteEffects.None, 0f);
				spriteBatch.Draw(texture, position, null, color, drawingAngle, new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, 0f);
				break;
			default:
				spriteBatch.Draw(texture, position - Vector2.One, null, color, drawingAngle, new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, 0f);
				spriteBatch.Draw(texture, position + Vector2.One, null, color, drawingAngle, new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, 0f);
				break;
			}
			break;
		default:
			spriteBatch.Draw(texture, position, null, color, drawingAngle, new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, 0f);
			break;
		}
	}
}
