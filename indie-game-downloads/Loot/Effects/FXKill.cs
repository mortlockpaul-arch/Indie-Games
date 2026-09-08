using System;
using Loot.Dungeon;
using Loot.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXKill : FXLocation
{
	private const int poolSize = 8;

	private const int goreSize = 8;

	private static int poolId = 0;

	private static FXKill[] pool = new FXKill[8];

	private static Texture2D texture;

	private static readonly Vector2 origin = new Vector2(16f, 16f);

	private static readonly TimeSpan duration = TimeSpan.FromSeconds(4.0);

	private static readonly TimeSpan npcDuration = TimeSpan.FromMilliseconds(350.0);

	private NPC npc;

	private TimeSpan timer;

	private TimeSpan npcTimer;

	private GoreParticle[] gore;

	private static readonly Color xpColor = new Color(0, 204, 0);

	private static readonly Color xpShadow = new Color(0, 102, 0);

	private static readonly Vector2 xpShadowOffset = new Vector2(2f, 2f);

	private FXKill()
	{
	}

	public static FXKill GetFX(NPC npc)
	{
		FXKill fXKill = pool[poolId];
		fXKill.Location = npc.Location;
		fXKill.npc = npc;
		fXKill.npcTimer = npcDuration;
		fXKill.timer = duration;
		for (int i = 0; i < 8; i++)
		{
			fXKill.gore[i].pos = new Vector2(0f, 0f);
			fXKill.gore[i].vel = new Vector2((float)(240.0 * DM.Random.NextDouble() - 120.0), -240f * (float)DM.Random.NextDouble());
			fXKill.gore[i].rotation = 0f;
			fXKill.gore[i].srcRect = new Rectangle(32 * DM.Random.Next(8), 0, 32, 32);
		}
		PlaySound.Hit();
		poolId++;
		if (poolId == pool.Length)
		{
			poolId = 0;
		}
		return fXKill;
	}

	public static void Load(ContentManager content)
	{
		texture = content.Load<Texture2D>("Sprites\\Effects\\Gore");
		for (int i = 0; i < pool.Length; i++)
		{
			pool[i] = new FXKill();
			pool[i].gore = new GoreParticle[8];
		}
	}

	public override void Update(GameTime gameTime)
	{
		npcTimer -= gameTime.ElapsedGameTime;
		timer -= gameTime.ElapsedGameTime;
		if (timer <= TimeSpan.Zero)
		{
			DM.RemoveEffect(this);
			return;
		}
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		for (int i = 0; i < 8; i++)
		{
			gore[i].rotation += (float)Math.PI * num;
			gore[i].vel.Y += 240f * num;
			gore[i].pos += gore[i].vel * num;
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		float zoom = DungeonView.Camera.Zoom;
		if (npcTimer > TimeSpan.Zero)
		{
			Color color = Color.White * (float)(npcTimer.TotalSeconds / npcDuration.TotalSeconds);
			npc.Draw(spriteBatch, color);
		}
		if (Profile.Preferences.Gore)
		{
			for (int i = 0; i < 8; i++)
			{
				spriteBatch.Draw(texture, Position + gore[i].pos * zoom, gore[i].srcRect, Color.White, gore[i].rotation, origin, zoom, SpriteEffects.None, 0f);
			}
		}
	}
}
