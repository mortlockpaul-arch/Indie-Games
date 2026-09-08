using System;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXResurrect : FXLocation
{
	private float scale;

	private float alpha;

	private TimeSpan timer;

	private static TimeSpan duration = TimeSpan.FromSeconds(1.0);

	private static Sprite Resurrect;

	private static Sprite ResurrectWings;

	public FXResurrect(Location loc)
		: base(loc)
	{
		timer = TimeSpan.Zero;
		scale = 0f;
		alpha = 0f;
		PlaySound.Resurrect();
	}

	public static void Load(ContentManager content)
	{
		Resurrect = new StillSprite(content.Load<Texture2D>("Sprites\\Effects\\Resurrect"));
		ResurrectWings = new StillSprite(content.Load<Texture2D>("Sprites\\Effects\\ResurrectWings"));
	}

	public override void Update(GameTime gameTime)
	{
		timer += gameTime.ElapsedGameTime;
		if (timer > duration)
		{
			DM.RemoveEffect(this);
		}
		float num = (float)(timer.TotalSeconds / duration.TotalSeconds);
		scale = num * 4f;
		alpha = (float)Math.Sin((double)num * Math.PI);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		Resurrect.Draw(spriteBatch, Position, Color.White * alpha, scale);
		ResurrectWings.Draw(spriteBatch, Position, Color.White * alpha, 1f);
	}
}
