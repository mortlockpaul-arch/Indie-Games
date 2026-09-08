using System;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXPoison : FXLocation
{
	private const int size = 8;

	private const float radius = 192f;

	private static FXPoison fx;

	private Sprite sprite;

	private TimeSpan time;

	private Particle[] particles = new Particle[8];

	private static readonly double baseAngle = MathHelper.ToRadians(45f);

	private static readonly Vector2 origin = new Vector2(32f, 32f);

	private static readonly TimeSpan fadeAt = TimeSpan.FromSeconds(0.5);

	private static readonly TimeSpan duration = TimeSpan.FromSeconds(1.0);

	public static FX GetFX(Location loc)
	{
		fx.Location = loc;
		fx.time = duration;
		for (int i = 0; i < fx.particles.Length; i++)
		{
			fx.particles[i].pos = new Vector2(0f, 0f);
			double num = baseAngle * (double)i;
			float x = (float)Math.Sin(num) * 192f;
			float y = (float)((0.0 - Math.Cos(num)) * 192.0);
			fx.particles[i].vel = new Vector2(x, y);
			fx.particles[i].rotation = (float)num;
		}
		PlaySound.Wind();
		return fx;
	}

	public static void Load(ContentManager content)
	{
		fx = new FXPoison();
		fx.sprite = new AnimatedSprite(content.Load<Texture2D>("Sprites\\Effects\\Poison"), new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 8, TimeSpan.FromMilliseconds(33.0));
	}

	public override void Update(GameTime gameTime)
	{
		sprite.Update(gameTime);
		time -= gameTime.ElapsedGameTime;
		if (time <= TimeSpan.Zero)
		{
			DM.RemoveEffect(this);
			return;
		}
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].pos += particles[i].vel * num;
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		Color color = ((!(time > fadeAt)) ? (Color.White * (float)(time.TotalSeconds / fadeAt.TotalSeconds)) : Color.White);
		float zoom = DungeonView.Camera.Zoom;
		for (int i = 0; i < particles.Length; i++)
		{
			sprite.Rotation = particles[i].rotation;
			sprite.Draw(spriteBatch, Position + particles[i].pos * zoom, color, zoom);
		}
	}
}
