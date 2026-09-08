using System;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXIce : FXLocation
{
	private const int poolSize = 9;

	private static int poolId = 0;

	private static FXIce[] pool = new FXIce[9];

	private static Texture2D texture;

	private static readonly Vector2 origin = new Vector2(16f, 16f);

	private TimeSpan timer;

	private Particle[] particles = new Particle[4];

	public static FXIce GetFX(Location loc)
	{
		FXIce fXIce = pool[poolId];
		fXIce.Location = loc;
		for (int i = 0; i < fXIce.particles.Length; i++)
		{
			fXIce.particles[i].rotation = 0f;
			fXIce.particles[i].pos = new Vector2(0f, 0f);
			fXIce.particles[i].vel = new Vector2((float)(240.0 * DM.Random.NextDouble() - 120.0), -240f * (float)DM.Random.NextDouble());
		}
		fXIce.timer = TimeSpan.FromSeconds(4.0);
		poolId++;
		if (poolId == pool.Length)
		{
			poolId = 0;
		}
		return fXIce;
	}

	public static void Load(ContentManager content)
	{
		texture = content.Load<Texture2D>("Sprites\\Effects\\Ice");
		for (int i = 0; i < pool.Length; i++)
		{
			pool[i] = new FXIce();
		}
	}

	public override void Update(GameTime gameTime)
	{
		timer -= gameTime.ElapsedGameTime;
		if (timer <= TimeSpan.Zero)
		{
			DM.RemoveEffect(this);
			return;
		}
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].rotation += (float)Math.PI * num;
			particles[i].vel.Y += 240f * num;
			particles[i].pos += particles[i].vel * num;
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		float zoom = DungeonView.Camera.Zoom;
		for (int i = 0; i < particles.Length; i++)
		{
			spriteBatch.Draw(texture, Position + particles[i].pos * zoom, null, Color.White, particles[i].rotation, origin, zoom, SpriteEffects.None, 0f);
		}
	}
}
