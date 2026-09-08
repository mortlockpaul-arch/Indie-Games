using System;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXPoof : FXCharacter
{
	private TimeSpan timer;

	private static Texture2D poof;

	private static readonly Color poofColor = new Color(192, 192, 192);

	public FXPoof(Character c)
		: base(c)
	{
		timer = TimeSpan.FromMilliseconds(250.0);
		if (!((double)DM.Player.Location.Distance(c.Location) >= 4.0))
		{
			PlaySound.Poof();
		}
	}

	public static void Load(ContentManager content)
	{
		poof = content.Load<Texture2D>("Sprites\\Effects\\Poof");
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
		spriteBatch.Draw(poof, Position, null, poofColor, 0f, new Vector2(32f, 32f), DungeonView.Camera.Zoom, SpriteEffects.None, 0f);
	}
}
