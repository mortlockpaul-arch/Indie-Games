using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class FrameRateCounter
{
	private int frameRate;

	private int frameCounter;

	private TimeSpan elapsedTime = TimeSpan.Zero;

	public void Update(GameTime gameTime)
	{
		elapsedTime += gameTime.ElapsedGameTime;
		if (elapsedTime > TimeSpan.FromSeconds(1.0))
		{
			elapsedTime -= TimeSpan.FromSeconds(1.0);
			frameRate = frameCounter;
			frameCounter = 0;
		}
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
	{
		frameCounter++;
		spriteBatch.Begin();
		spriteBatch.DrawString(spriteFont, "fps:" + frameRate, new Vector2(50f, 500f), Color.White);
		spriteBatch.End();
	}
}
