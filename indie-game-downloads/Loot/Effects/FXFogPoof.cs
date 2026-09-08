using System;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXFogPoof : FXLocation
{
	private const int poolSize = 32;

	private static int poolId = 0;

	private static FXFogPoof[] pool = new FXFogPoof[32];

	private TimeSpan timer;

	private static TimeSpan timerDuration = TimeSpan.FromMilliseconds(800.0);

	private AnimatedSprite sprite;

	private Color poofColor = new Color(96, 96, 96);

	private FXFogPoof()
	{
	}

	public static FXFogPoof GetFX(Location loc)
	{
		FXFogPoof fXFogPoof = pool[poolId];
		fXFogPoof.Location = loc;
		fXFogPoof.timer = timerDuration;
		fXFogPoof.sprite.Reset();
		poolId++;
		if (poolId >= 32)
		{
			poolId = 0;
		}
		return fXFogPoof;
	}

	public static void Load(ContentManager content)
	{
		Texture2D texture = content.Load<Texture2D>("Sprites\\Effects\\FogPoof");
		for (int i = 0; i < 32; i++)
		{
			pool[i] = new FXFogPoof();
			pool[i].sprite = new AnimatedSprite(texture, new Rectangle(0, 0, 128, 128), null, 8, TimeSpan.FromMilliseconds(100.0));
		}
	}

	public override void Update(GameTime gameTime)
	{
		sprite.Update(gameTime);
		timer -= gameTime.ElapsedGameTime;
		if (timer <= TimeSpan.Zero)
		{
			DM.RemovePoof(this);
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		sprite.Draw(spriteBatch, Position, poofColor * (float)(timer.TotalSeconds / timerDuration.TotalSeconds), DungeonView.Camera.Zoom);
	}
}
