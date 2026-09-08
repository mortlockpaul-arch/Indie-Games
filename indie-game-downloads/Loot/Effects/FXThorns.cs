using System;
using Loot.Dungeon;
using Loot.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXThorns : FXLocation
{
	private const int poolSize = 10;

	private float angle;

	private TimeSpan timer;

	private Vector2 offset;

	private Vector2 dv;

	protected static int poolId = 0;

	protected static FXThorns[] pool;

	protected static Texture2D thorns;

	private static Vector2 origin = new Vector2(0f, 32f);

	private FXThorns()
	{
	}

	public static FXThorns GetFX(NPC target)
	{
		FXThorns fXThorns = pool[poolId];
		fXThorns.Location = DM.Player.Location;
		fXThorns.angle = DM.Player.Location.AngleTo(target.Location);
		fXThorns.dv = Convert(fXThorns.angle, 1f);
		fXThorns.timer = TimeSpan.FromSeconds(0.3);
		fXThorns.offset = fXThorns.dv * -32f;
		poolId++;
		if (poolId == pool.Length)
		{
			poolId = 0;
		}
		return fXThorns;
	}

	public static void Load(ContentManager content)
	{
		thorns = content.Load<Texture2D>("Sprites\\Effects\\Thorns");
		pool = new FXThorns[10];
		for (int i = 0; i < pool.Length; i++)
		{
			pool[i] = new FXThorns();
		}
	}

	public override void Update(GameTime gameTime)
	{
		timer -= gameTime.ElapsedGameTime;
		if (timer <= TimeSpan.Zero)
		{
			DM.RemoveEffect(this);
		}
		offset += 192f * dv * (float)gameTime.ElapsedGameTime.TotalSeconds;
	}

	private static Vector2 Convert(float angle, float length)
	{
		return new Vector2((float)Math.Cos(angle) * length, (float)Math.Sin(angle) * length);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		spriteBatch.Draw(thorns, Position + offset, null, Color.White, angle, origin, DungeonView.Camera.Zoom, SpriteEffects.None, 0f);
	}
}
