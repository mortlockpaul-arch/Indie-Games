using System;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXHit : FXCharacter
{
	protected const int poolSize = 10;

	protected static Texture2D hit;

	protected static Texture2D crit;

	private bool isCrit;

	private float rotation;

	private TimeSpan timer;

	private static readonly Vector2 origin = new Vector2(32f, 32f);

	private static readonly Vector2 critOrigin = new Vector2(64f, 64f);

	private static readonly TimeSpan fade = TimeSpan.FromMilliseconds(150.0);

	private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(350.0);

	protected static int poolId = 0;

	protected static FXHit[] fxPool;

	public static FXHit GetFX(Character c, bool crit)
	{
		FXHit fXHit = fxPool[poolId];
		fXHit.Character = c;
		fXHit.isCrit = crit;
		fXHit.rotation = (float)(Math.PI * 2.0 * DM.Random.NextDouble());
		fXHit.timer = delay;
		PlaySound.Hit();
		poolId++;
		if (poolId == fxPool.Length)
		{
			poolId = 0;
		}
		return fXHit;
	}

	public static void Load(ContentManager content)
	{
		hit = content.Load<Texture2D>("Sprites\\Effects\\Hit");
		crit = content.Load<Texture2D>("Sprites\\Effects\\Crit");
		fxPool = new FXHit[10];
		for (int i = 0; i < fxPool.Length; i++)
		{
			fxPool[i] = new FXHit();
		}
	}

	public override void Update(GameTime gameTime)
	{
		timer -= gameTime.ElapsedGameTime;
		if (timer <= TimeSpan.Zero)
		{
			DM.RemoveEffect(this);
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		Color white = Color.White;
		if (timer < fade)
		{
			white *= (float)(timer.TotalSeconds / fade.TotalSeconds);
		}
		if (isCrit)
		{
			spriteBatch.Draw(crit, Position, null, white, rotation, critOrigin, DungeonView.Camera.Zoom, SpriteEffects.None, 0f);
		}
		else
		{
			spriteBatch.Draw(hit, Position, null, white, rotation, origin, DungeonView.Camera.Zoom, SpriteEffects.None, 0f);
		}
	}
}
