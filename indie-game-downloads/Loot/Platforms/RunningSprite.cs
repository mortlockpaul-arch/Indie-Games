using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Platforms;

public class RunningSprite
{
	private Texture2D texture;

	private int frame;

	private int numFrames = 8;

	private float curFrameTime;

	private int frameDuration = 500;

	private Rectangle headRect = new Rectangle(128, 0, 64, 64);

	private Rectangle jumpRect = new Rectangle(128, 64, 64, 64);

	public RunningSprite(Texture2D texture)
	{
		this.texture = texture;
	}

	public void Update(GameTime gameTime, float speed)
	{
		curFrameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds * speed;
		if (!((double)curFrameTime < (double)frameDuration))
		{
			curFrameTime -= frameDuration;
			frame++;
			if (frame >= numFrames)
			{
				frame = 0;
			}
		}
	}

	public void Draw(SpriteBatch spriteBatch, Vector2 v)
	{
		spriteBatch.Draw(texture, v, new Rectangle(frame * 64, 64, 64, 64), DM.Player.Sprite.BodyColor, 0f, new Vector2(32f, 32f), 1f, SpriteEffects.None, 0f);
		DM.Player.Sprite.DrawHead(spriteBatch, v, headRect);
	}

	public void DrawJump(SpriteBatch spriteBatch, Vector2 v)
	{
		spriteBatch.Draw(texture, v, jumpRect, DM.Player.Sprite.BodyColor, 0f, new Vector2(32f, 32f), 1f, SpriteEffects.None, 0f);
		DM.Player.Sprite.DrawHead(spriteBatch, v, headRect);
	}
}
