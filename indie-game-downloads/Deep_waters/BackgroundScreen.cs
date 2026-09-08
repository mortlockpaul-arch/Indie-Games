using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

internal class BackgroundScreen : GameScreen
{
	private ContentManager content;

	private Texture2D backgroundTexture;

	private string nameimg;

	public BackgroundScreen(string name)
	{
		nameimg = name;
		base.TransitionOnTime = TimeSpan.FromSeconds(0.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		backgroundTexture = content.Load<Texture2D>("Background/" + nameimg);
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen: false);
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		Viewport viewport = base.ScreenManager.GraphicsDevice.Viewport;
		Rectangle destinationRectangle = new Rectangle(0, 0, viewport.Width, viewport.Height);
		spriteBatch.Begin();
		spriteBatch.Draw(backgroundTexture, destinationRectangle, new Color(base.TransitionAlpha, base.TransitionAlpha, base.TransitionAlpha));
		spriteBatch.End();
	}
}
