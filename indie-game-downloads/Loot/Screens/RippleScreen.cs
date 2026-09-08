using System;
using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Screens;

public class RippleScreen : Screen
{
	public delegate void Callback();

	private const float distortion = 1f;

	private Game Game;

	private Callback callback;

	private float wave;

	private static readonly Vector2 center = new Vector2(0.5f, 0.5f);

	private TimeSpan elapsed;

	private static readonly TimeSpan duration = TimeSpan.FromSeconds(2.0);

	private static Effect ripple;

	private static EffectParameter waveParam;

	private static EffectParameter distortionParam;

	private static EffectParameter centerParam;

	private static Texture2D pixel;

	private Color color = MC.ScreenManager.BackgroundColor;

	public RippleScreen(Callback c)
		: base(modal: true)
	{
		base.IsBackground = true;
		Game = MC.Game;
		callback = c;
		elapsed = TimeSpan.Zero;
	}

	public static void Load(ContentManager content)
	{
		pixel = content.Load<Texture2D>("Sprites\\Pixel");
	}

	public override void loadContent(ContentManager content)
	{
	}

	public override void transitionOn()
	{
	}

	public override void transitionOff()
	{
	}

	public override void update(GameTime gameTime)
	{
		elapsed += gameTime.ElapsedGameTime;
		if (elapsed > duration)
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
		float num = MathHelper.Clamp((float)(1.25 * elapsed.TotalSeconds / duration.TotalSeconds), 0f, 1f);
		base.spriteBatch.Begin();
		base.spriteBatch.Draw(pixel, base.viewportRect, color * num);
		base.spriteBatch.End();
	}
}
