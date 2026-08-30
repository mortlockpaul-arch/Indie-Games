using System;
using Hammer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Pickup
{
	public enum item
	{
		coins,
		health,
		emp,
		bomb,
		itemBomb,
		pathNode,
		orb,
		relic
	}

	public Texture2D texture;

	public Animation animation;

	private Primitive2D p2d;

	public Vector2 position;

	private Vector2 positionFinal;

	private Vector2 positionOffset;

	private Vector2 positionColony;

	public item pickupType;

	public float speed;

	public float angle;

	public float size;

	private Color color;

	public int target = -1;

	public int qeue = -1;

	public int Health;

	public int Damage;

	public bool Active;

	public bool picked;

	public float frame;

	private float increment;

	private float offset;

	private float transparency;

	public int time;

	private float orbit;

	private float rot;

	private float speedVar;

	private float speedFollowing = 0f;

	private float speedVariation = 0f;

	public float interest;

	private Random random;

	public int Width => texture.Width;

	public int Height => texture.Height;

	public float Scale()
	{
		return animation.Scale() * size;
	}

	public Pickup(Texture2D tx, GraphicsDevice graphicsDevice)
		: this(tx, Vector2.Zero, Vector2.One, item.coins, 0, 1, 0, graphicsDevice)
	{
	}

	public Pickup(Texture2D texture, Vector2 positionIni, Vector2 position, item pickupType, int frames, int frameRows, int seed, GraphicsDevice graphicsDevice)
	{
		Initialize(texture, positionIni, position, pickupType, frames, frameRows, seed, graphicsDevice);
	}

	public void Initialize(Texture2D texture, Vector2 positionIni, Vector2 position, item pickupType, int frames, int frameRows, int seed, GraphicsDevice graphicsDevice)
	{
		random = new Random(seed);
		p2d = new Primitive2D(graphicsDevice);
		animation = new Animation();
		animation.Initialize(texture, Vector2.Zero, texture.Height, texture.Height, frames, frameRows, random.Next(50, 100), Color.White, 1f, looping: true);
		this.position = positionIni;
		positionFinal = position;
		this.pickupType = pickupType;
		this.texture = texture;
		interest = 1f;
		size = 1f;
		speed = 4f;
		Damage = 1;
		Health = 3;
		transparency = (float)random.Next(200) / 100f;
		frame = (float)random.Next(-100000, 100000) / 10f;
		increment = (float)random.Next(50, 100) / 1000f;
		offset = 0f;
		picked = false;
		time = 1000;
		orbit = random.Next(-20, 20);
		rot = (float)random.Next(-150, 150) / 10000f;
		speedVar = (float)random.Next(3000, 7000) / 5f;
		qeue = -1;
		target = -1;
		switch (pickupType)
		{
		case item.pathNode:
			size = 0.5f;
			color = new Color(255, 255, 255, 0);
			speed = 1.5f;
			Damage = 5;
			Health = 25;
			time = 600;
			break;
		case item.coins:
			color = new Color(255, 255, 255, 0);
			speed = 1.5f;
			Damage = 5;
			Health = 25;
			time = 600;
			break;
		case item.orb:
			speed = (float)random.Next(3000, 7000) / 20f;
			break;
		case item.relic:
			speed = (float)random.Next(3000, 7000) / 20f;
			break;
		}
		Active = true;
	}

	public void Update(GameTime gameTime, Vector2 destination, Vector2 posColony)
	{
		if (Game1.gameState != GameState.Challenge)
		{
			time--;
		}
		if (pickupType == item.orb)
		{
			time = 100;
		}
		if (pickupType == item.relic)
		{
			time = 150;
		}
		if (pickupType == item.pathNode)
		{
			Active = true;
		}
		if (time <= 0)
		{
			size -= 0.02f;
		}
		frame += increment + 0.001f;
		angle += rot;
		size = MathHelper.Clamp(size, 0f, 1f);
		positionColony = posColony;
		if (pickupType == item.orb || pickupType == item.relic)
		{
			frame++;
			positionOffset.X = (float)Math.Sin((double)frame / 21.1) / 1.31f;
			positionOffset.Y = (float)Math.Cos((double)frame / 33.9) / 1.13f;
			if (pickupType == item.orb || pickupType == item.relic)
			{
				positionOffset *= 0.5f;
			}
			position += positionOffset;
			destination += positionOffset;
		}
		float num = Vector2.Distance(position, destination);
		if (target == -1)
		{
			speedFollowing = 0f;
			interest = 0f;
			if (Game1.gameState != GameState.Challenge)
			{
				position.X = MathHelper.Lerp(position.X, positionFinal.X, 0.05f);
				position.Y = MathHelper.Lerp(position.Y, positionFinal.Y, 0.05f);
			}
			transparency = (float)(Math.Sin(frame / 4f) / 3.0 + 0.550000011920929);
			if (offset < 4.2f)
			{
				offset += 0.02f;
			}
		}
		if (target >= 0 && pickupType != item.pathNode)
		{
			if (interest <= 0f)
			{
				interest = 1f;
				qeue = target;
				frame = 0f;
			}
			if (frame > 1000f)
			{
				interest -= 0.02f;
				if (interest <= 0f)
				{
					target = -1;
					interest = 0f;
					frame = 0f;
				}
			}
			positionFinal = position;
			frame++;
			if (!picked)
			{
				frame = 0f;
			}
			picked = true;
			speedVariation = (float)Math.Sin(frame / 100f) / 10f + 0.2f;
			speedFollowing = MathHelper.Lerp(speedFollowing, num / speed + 0.01f, 0.5f);
			num = MathHelper.Clamp(num, 0f, 100f);
			if (pickupType == item.orb)
			{
				position.X = MathHelper.Lerp(position.X, destination.X, (speedFollowing - speedVariation) * interest);
				position.Y = MathHelper.Lerp(position.Y, destination.Y, (speedFollowing - speedVariation) * interest);
			}
			else if (Game1.gameState != GameState.Challenge)
			{
				position.X = MathHelper.Lerp(position.X, destination.X, frame * frame / 1000f);
				position.Y = MathHelper.Lerp(position.Y, destination.Y, frame * frame / 1000f);
			}
			if (offset > 0f)
			{
				offset -= 0.2f;
			}
			if (transparency < 2f)
			{
				transparency += 0.2f;
			}
			angle += 0.1f;
			if (pickupType == item.health)
			{
				angle += 0.15f;
				size -= 0.05f;
				if (size < 0f)
				{
					size = 0f;
				}
			}
		}
		if (!picked && size <= 0.01f)
		{
			Active = false;
		}
		if (Game1.gameState == GameState.ChubbyRain)
		{
			position.Y++;
			positionFinal.Y++;
			positionOffset.Y++;
		}
		if (Game1.gameState == GameState.Sidescroller)
		{
			position.X--;
			positionFinal.X--;
			positionOffset.X--;
		}
		if (Game1.gameState != GameState.ChubbyRain)
		{
			position.X = MathHelper.Clamp(position.X, -540f, 1800f);
			position.Y = MathHelper.Clamp(position.Y, -300f, 1100f);
			positionFinal.X = MathHelper.Clamp(positionFinal.X, -540f, 1800f);
			positionFinal.Y = MathHelper.Clamp(positionFinal.Y, -300f, 1100f);
			positionOffset.X = MathHelper.Clamp(positionOffset.X, -540f, 1800f);
			positionOffset.Y = MathHelper.Clamp(positionOffset.Y, -300f, 1100f);
		}
		if (Game1.gameState == GameState.Challenge)
		{
			if (pickupType == item.coins)
			{
				size = 2f;
			}
			angle = 0f;
			positionOffset = position;
			positionFinal = position;
		}
		animation.Position = position;
		animation.scale = 0.5f + (float)(Math.Sin(frame / 100f) / 100.0);
		animation.Update(gameTime);
		animation.Active = Active;
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		switch (pickupType)
		{
		case item.pathNode:
			spriteBatch.Draw(texture, position, null, Color.White * (float)(Math.Abs(Math.Sin(frame * 1.1f) * 0.5) + 0.5) * 0.33f, angle + (float)(Math.Abs(Math.Sin(frame / 23.3f) * 0.5) + 0.5) * ((float)Math.PI * 2f), new Vector2((float)texture.Width / 2.01f, (float)texture.Height / 2.01f), new Vector2(size * (float)Math.Abs(Math.Sin(frame)), size), SpriteEffects.None, 0f);
			spriteBatch.Draw(texture, position, null, Color.White * (float)(Math.Abs(Math.Sin(frame * 0.97f) * 0.5) + 0.5) * 0.33f, angle - (float)(Math.Abs(Math.Sin(frame / 11.37f) * 0.5) + 0.5) * ((float)Math.PI * 2f), new Vector2(texture.Width / 2, texture.Height / 2), new Vector2(size * (float)Math.Abs(Math.Sin(frame)), size) * 0.9f, SpriteEffects.None, 0f);
			spriteBatch.Draw(texture, position, null, Color.White * (float)(Math.Abs(Math.Sin(frame * 2.3f) * 0.5) + 0.5) * 0.33f, 0f - angle + (float)(Math.Abs(Math.Sin(frame / 5.07f) * 0.5) + 0.5) * ((float)Math.PI * 2f), new Vector2((float)texture.Width / 1.9f, (float)texture.Height / 1.9f), new Vector2(size * (float)Math.Abs(Math.Sin(frame)), size) * 0.75f, SpriteEffects.None, 0f);
			break;
		case item.coins:
			if (Game1.gameState == GameState.Challenge)
			{
				spriteBatch.Draw(texture, position, null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), new Vector2(size * (float)(Math.Abs(Math.Sin(frame) * 0.5) + 0.5), size), SpriteEffects.None, 0f);
			}
			else
			{
				spriteBatch.Draw(texture, position, null, new Color(transparency / 3f * size + 0.2f, transparency / 2f * size + 0.2f, transparency * size + 0.2f, transparency * size + 0.2f), angle, new Vector2((float)(texture.Width / 2) * offset * transparency, (float)(texture.Height / 2) * transparency), size, SpriteEffects.None, 0f);
			}
			break;
		case item.health:
			spriteBatch.Draw(texture, position, null, Color.White, angle, new Vector2(orbit, orbit), size, SpriteEffects.None, 0f);
			break;
		case item.orb:
			spriteBatch.Draw(texture, position, null, new Color(0f, 0.7f, 1f, 0.25f), angle, new Vector2(Width / 2, Height / 2), size * ((positionOffset.X + positionOffset.Y) * 0.05f + 0.5f), SpriteEffects.None, 0f);
			p2d.drawLine(spriteBatch, position, positionColony, 2f, Color.Cyan * MathHelper.Clamp((float)Math.Sin(frame / 100f) * 0.25f, 0f, 0.25f));
			break;
		case item.relic:
			animation.Draw(spriteBatch);
			break;
		default:
			spriteBatch.Draw(texture, position, null, Color.White, angle, new Vector2((float)(texture.Width / 2) * offset * transparency, (float)(texture.Height / 2) * transparency), 0.5f, SpriteEffects.None, 0f);
			break;
		}
	}
}
