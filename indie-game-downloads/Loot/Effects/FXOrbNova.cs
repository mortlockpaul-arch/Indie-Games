using System;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXOrbNova : FXLocation
{
	private const int goreSize = 8;

	private static FXOrbNova fx = new FXOrbNova();

	private static Texture2D texture;

	private static StillSprite nova;

	private static readonly Vector2 origin = new Vector2(16f, 16f);

	private static readonly TimeSpan duration = TimeSpan.FromSeconds(4.0);

	private Orb orb;

	private TimeSpan timer;

	private GoreParticle[] gore;

	private TimeSpan novaTimer;

	private TimeSpan novaUpdate;

	private float novaRotation;

	private static TimeSpan novaDuration = TimeSpan.FromMilliseconds(300.0);

	private static TimeSpan novaUpdateDuration = TimeSpan.FromMilliseconds(100.0);

	private FXOrbNova()
	{
	}

	public static FXOrbNova GetFX(Orb orb)
	{
		fx.Location = orb.Location;
		fx.orb = orb;
		fx.timer = duration;
		fx.novaTimer = novaDuration;
		fx.novaUpdate = novaUpdateDuration;
		fx.novaRotation = (float)(Math.PI * 2.0 * DM.Random.NextDouble());
		for (int i = 0; i < 8; i++)
		{
			fx.gore[i].pos = new Vector2(0f, 0f);
			fx.gore[i].vel = new Vector2((float)(240.0 * DM.Random.NextDouble() - 120.0), -240f * (float)DM.Random.NextDouble());
			fx.gore[i].rotation = 0f;
			fx.gore[i].srcRect = new Rectangle(32 * DM.Random.Next(8), 0, 32, 32);
		}
		PlaySound.Zot();
		return fx;
	}

	public static void Load(ContentManager content)
	{
		texture = content.Load<Texture2D>("Sprites\\Effects\\OrbGore");
		nova = new StillSprite(content.Load<Texture2D>("Sprites\\Effects\\OrbNova"), null, null);
		fx.gore = new GoreParticle[8];
	}

	public override void Update(GameTime gameTime)
	{
		timer -= gameTime.ElapsedGameTime;
		if (timer <= TimeSpan.Zero)
		{
			DM.RemoveEffect(this);
			return;
		}
		novaTimer -= gameTime.ElapsedGameTime;
		novaUpdate -= gameTime.ElapsedGameTime;
		if (novaUpdate <= TimeSpan.Zero)
		{
			novaRotation = (float)(Math.PI * 2.0 * DM.Random.NextDouble());
			novaUpdate += novaUpdateDuration;
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
		if (novaTimer > TimeSpan.Zero)
		{
			nova.Rotation = novaRotation;
			nova.Draw(spriteBatch, Position, Color.White, zoom);
		}
		for (int i = 0; i < 8; i++)
		{
			spriteBatch.Draw(texture, Position + gore[i].pos * zoom, gore[i].srcRect, Color.White, gore[i].rotation, origin, zoom, SpriteEffects.None, 0f);
		}
	}
}
