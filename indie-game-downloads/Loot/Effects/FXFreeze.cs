using System;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXFreeze : FXLocation
{
	private const int size = 8;

	private const float scale = 192f;

	private static FXFreeze fx;

	private Texture2D texture;

	private TimeSpan time;

	private Particle[] flakes = new Particle[8];

	private static readonly double baseAngle = MathHelper.ToRadians(45f);

	private static readonly Vector2 origin = new Vector2(32f, 32f);

	private static readonly TimeSpan fadeAt = TimeSpan.FromSeconds(0.5);

	private static readonly TimeSpan duration = TimeSpan.FromSeconds(1.0);

	public static FX GetFX(Location loc)
	{
		fx.Location = loc;
		fx.time = duration;
		for (int i = 0; i < fx.flakes.Length; i++)
		{
			fx.flakes[i].pos = new Vector2(0f, 0f);
			double num = baseAngle * (double)i;
			float x = (float)Math.Cos(num) * 192f;
			float y = (float)Math.Sin(num) * 192f;
			fx.flakes[i].vel = new Vector2(x, y);
			fx.flakes[i].rotation = 0f;
		}
		PlaySound.Wind();
		return fx;
	}

	public static void Load(ContentManager content)
	{
		fx = new FXFreeze();
		fx.texture = content.Load<Texture2D>("Sprites\\Effects\\Snowflake");
	}

	public override void Update(GameTime gameTime)
	{
		time -= gameTime.ElapsedGameTime;
		if (time <= TimeSpan.Zero)
		{
			DM.RemoveEffect(this);
			return;
		}
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		float num2 = (float)Math.PI * num;
		for (int i = 0; i < flakes.Length; i++)
		{
			flakes[i].pos += flakes[i].vel * num;
			flakes[i].rotation += num2;
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		Color color = ((!(time > fadeAt)) ? (Color.White * (float)(time.TotalSeconds / fadeAt.TotalSeconds)) : Color.White);
		for (int i = 0; i < flakes.Length; i++)
		{
			spriteBatch.Draw(texture, Position + flakes[i].pos * DungeonView.Camera.Zoom, null, color, flakes[i].rotation, origin, DungeonView.Camera.Zoom, SpriteEffects.None, 0f);
		}
	}
}
