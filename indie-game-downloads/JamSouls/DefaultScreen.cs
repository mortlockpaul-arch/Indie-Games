using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

internal class DefaultScreen : GameScreen
{
	private ContentManager content;

	public DefaultScreen()
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(1.5);
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		Thread.Sleep(1000);
		base.ScreenManager.Game.ResetElapsedTime();
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		_ = base.IsActive;
	}

	public override void HandleInput()
	{
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		spriteBatch.Begin();
		spriteBatch.End();
		if (base.TransitionPosition > 0f)
		{
			base.ScreenManager.FadeBackBufferToBlack(255 - base.TransitionAlpha);
		}
	}
}
