using System;
using Hammer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Construction
{
	public Texture2D texture;

	public Texture2D texture2;

	public Vector2 position;

	public Vector2 positionIni;

	public Vector2 target = default(Vector2);

	public constructionType type;

	public float speed;

	public float maxSpeed;

	public float angle;

	private float angleCreation;

	public float size;

	public float scale;

	private Color color;

	public float Health;

	public float maxHealth;

	public int Damage;

	public Color shootColor;

	public int level = 0;

	public int rate = 300;

	public bool Active;

	public float shootingDamage = 1f;

	public float repair = 0f;

	public int repairID = 0;

	public float frame;

	private ushort usefulLife;

	private float increment;

	private float layer;

	private Random random;

	public int Width => texture.Width;

	public int Height => texture.Height;

	public float Scale()
	{
		return (float)(Height + Width) / 2f * scale;
	}

	public Construction(Vector2 position, constructionType constructionType, Texture2D t1, Texture2D t2)
	{
		Initialize(position, constructionType, t1, t2);
	}

	public void Initialize(Vector2 position, constructionType constructionType, Texture2D t1, Texture2D t2)
	{
		random = new Random();
		angleCreation = 0f;
		if (type == constructionType.drone)
		{
			angleCreation = (float)random.Next(1000) * 0.01f;
		}
		this.position = position;
		positionIni = position;
		type = constructionType;
		speed = 4f;
		Damage = 1;
		Health = 10f;
		angle = 0f;
		size = 0f;
		scale = 1f;
		frame = 0f;
		if (type != constructionType.hive)
		{
			frame = (float)random.Next(-100000, 100000) / 1000f;
		}
		increment = (float)random.Next(50, 100) / 1000f;
		shootColor = Color.LightYellow;
		layer = 0.19f + (float)random.Next(100) / 10000f;
		texture = t1;
		texture2 = t2;
		usefulLife = 100;
		switch (type)
		{
		case constructionType.barrier:
			color = new Color(255, 255, 255, 0);
			speed = 2f;
			Damage = 5;
			Health = 15f;
			usefulLife = 3000;
			break;
		case constructionType.turret:
			color = new Color(255, 255, 255, 0);
			speed = 0.5f;
			Damage = 5;
			Health = 5f;
			usefulLife = 1500;
			break;
		case constructionType.sanctuary:
			color = new Color(255, 255, 255, 0);
			speed = 0.25f;
			Damage = 5;
			Health = 50f;
			scale = 0.5f;
			usefulLife = 2000;
			break;
		case constructionType.hive:
			color = new Color(255, 255, 255, 0);
			speed = 0.25f;
			Damage = 5;
			Health = 50f;
			scale = 0.5f;
			frame = rate - 30;
			usefulLife = 2000;
			break;
		case constructionType.drone:
			color = new Color(255, 255, 255, 0);
			maxSpeed = (float)random.Next(400) / 100f + 2f;
			speed = 0f;
			Damage = 3;
			Health = 2f;
			scale = 0.5f;
			usefulLife = 1000;
			break;
		default:
			color = new Color(255, 255, 255, 0);
			speed = 0.25f;
			Damage = 5;
			Health = 10f;
			usefulLife = 400;
			break;
		}
		maxHealth = Health;
		Active = true;
	}

	public static float Cost(constructionType pickupType)
	{
		return pickupType switch
		{
			constructionType.barrier => 1f, 
			constructionType.turret => 10f, 
			constructionType.sanctuary => 20f, 
			constructionType.hive => 5f, 
			_ => 1f, 
		};
	}

	public void Update(bool shooting)
	{
		frame++;
		if (frame > (float)(int)usefulLife)
		{
			Health -= 0.05f;
		}
		if (size < 1f)
		{
			size = MathHelper.Lerp(size, 1.1f, speed * 0.1f);
		}
		if (angleCreation < (float)Math.PI * 2f)
		{
			angleCreation = MathHelper.Lerp(angleCreation, (float)Math.PI * 2f, speed * 0.15f);
		}
		if (type == constructionType.drone)
		{
			speed = MathHelper.Lerp(speed, (float)((double)maxSpeed + Math.Sin(frame * 0.025f) * 0.5), 0.01f);
			if (shooting)
			{
				angle = Math2.TurnToFace(position, target, angle, speed * 0.01f);
			}
			else
			{
				angle = Math2.TurnToFace(position, positionIni, angle, (float)((double)(speed * 0.01f) + Math.Sin(frame * 0.1f) * 0.009999999776482582));
			}
			position = new Vector2(position.X + (float)Math.Cos(angle) * speed, position.Y + (float)Math.Sin(angle) * speed);
		}
		else if (shooting)
		{
			angle = Math2.TurnToFace(position, target, angle, speed * 0.05f + (float)level * 0.05f);
		}
		else
		{
			angle += 0.01f;
		}
		if (repair > 0f)
		{
			repair -= 0.01f;
			Health -= 0.0025f;
		}
		if (Health <= 0f)
		{
			Active = false;
		}
	}

	public void Repair(int repairID, Vector2 pos)
	{
		repair = 1f;
		this.repairID = repairID;
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		if (Active)
		{
			switch (type)
			{
			case constructionType.barrier:
				spriteBatch.Draw(texture, position, null, new Color(1f * size, Health / 10f * size, Health / 10f * size, size), angleCreation, new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, layer);
				break;
			case constructionType.hive:
				spriteBatch.Draw(texture, position, null, new Color(1f, Health / maxHealth, Health / maxHealth, size), angleCreation, new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f), size * scale, SpriteEffects.None, layer);
				break;
			case constructionType.sanctuary:
				spriteBatch.Draw(texture, position, null, new Color(1f, Health / maxHealth, Health / maxHealth, size), angleCreation, new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f), size * scale, SpriteEffects.None, layer);
				break;
			case constructionType.turret:
				spriteBatch.Draw(texture, position, null, new Color(1f, Health / maxHealth, Health / maxHealth, size), angleCreation, new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f), size, SpriteEffects.None, layer);
				spriteBatch.Draw(texture2, position, null, new Color(1f, Health / maxHealth, Health / maxHealth, size), angle, new Vector2((float)texture2.Width * 0.25f, (float)texture2.Height / 2f), size, SpriteEffects.None, layer * 0.5f);
				break;
			case constructionType.drone:
				spriteBatch.Draw(texture2, position, null, new Color(1f, 1f, 1f, size * Health / maxHealth), angle, new Vector2((float)texture2.Width * 0.25f, (float)texture2.Height / 2f), size * scale, SpriteEffects.None, layer * 0.5f);
				break;
			default:
				spriteBatch.Draw(texture, position, null, new Color(1f * size, Health / maxHealth * size, Health / maxHealth * size, size), angleCreation, new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, layer);
				break;
			}
		}
	}
}
