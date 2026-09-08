using System;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXDustCloud : FXLocation
{
	private const int numParticles = 8;

	private Particle[] clouds = new Particle[8];

	private TimeSpan timer = TimeSpan.Zero;

	private TimeSpan duration = TimeSpan.FromSeconds(1.0);

	private static Sprite dustSprite;

	private float alpha;

	private float rotation;

	public FXDustCloud(Location loc)
		: base(loc)
	{
		float num = 0f;
		float num2 = 92f;
		for (int i = 0; i < clouds.Length; i++)
		{
			clouds[i].pos = Vector2.Zero;
			clouds[i].vel = new Vector2((float)Math.Cos(num) * num2, (float)Math.Sin(num) * num2);
			num += (float)Math.PI * 2f / (float)clouds.Length;
		}
		alpha = 1f;
		rotation = 0f;
		PlaySound.DustCloud();
	}

	public static void Load(ContentManager content)
	{
		dustSprite = new StillSprite(content.Load<Texture2D>("Sprites\\Effects\\DustCloud"));
	}

	public override void Update(GameTime gameTime)
	{
		timer += gameTime.ElapsedGameTime;
		if (timer > duration)
		{
			DM.RemoveEffect(this);
			return;
		}
		float num = (float)(timer.TotalSeconds / duration.TotalSeconds);
		alpha = 1f - num;
		rotation = (float)Math.PI * 2f * num;
		float num2 = (float)gameTime.ElapsedGameTime.TotalSeconds;
		for (int i = 0; i < clouds.Length; i++)
		{
			clouds[i].pos += clouds[i].vel * num2;
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		dustSprite.Rotation = rotation;
		for (int i = 0; i < clouds.Length; i++)
		{
			dustSprite.Draw(spriteBatch, Position + clouds[i].pos, Color.White * alpha, 1f);
		}
	}
}
