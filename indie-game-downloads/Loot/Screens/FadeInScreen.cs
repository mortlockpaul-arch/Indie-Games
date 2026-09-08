using System;
using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Screens;

public class FadeInScreen : Screen
{
	public delegate void FadeInDelegate();

	private TimeSpan timer;

	private TimeSpan fadeDuration;

	private Color fadeColor;

	private FadeInDelegate callback;

	private Texture2D pixel;

	public FadeInScreen(TimeSpan fadeDuration, Color fadeColor, FadeInDelegate callback)
		: base(modal: true)
	{
		this.fadeDuration = fadeDuration;
		this.fadeColor = fadeColor;
		this.callback = callback;
		timer = TimeSpan.Zero;
	}

	public override void loadContent(ContentManager content)
	{
		pixel = content.Load<Texture2D>("Sprites\\Pixel");
	}

	public override void update(GameTime gameTime)
	{
		timer += gameTime.ElapsedGameTime;
		if (timer >= fadeDuration)
		{
			MC.ScreenManager.removeScreen(this);
			if (callback != null)
			{
				callback();
			}
		}
	}

	public override void draw(GameTime gameTime)
	{
		float num = MathHelper.Clamp((float)(1.25 * (1.0 - timer.TotalSeconds / fadeDuration.TotalSeconds)), 0f, 1f);
		base.spriteBatch.Begin();
		base.spriteBatch.Draw(pixel, base.viewportRect, fadeColor * num);
		base.spriteBatch.End();
	}
}
