using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class PressStartScreen : GameScreen
{
	private SpriteBatch m_SpriteBatch;

	private SpriteFont m_Font;

	private int m_FrameCount;

	public PressStartScreen(ScreenManager manager)
		: base(manager)
	{
		m_NoBackground = true;
	}

	public override void LoadContent()
	{
		m_SpriteBatch = new SpriteBatch(MainGame.Instance.GraphicsDevice);
		m_Font = MainGame.ContentMan.Load<SpriteFont>("Fonts/MenuFont");
		base.LoadContent();
	}

	public override void Draw(float alpha)
	{
		int num = Math.Abs(m_FrameCount % 100 - 50) * 4;
		Color darkRed = Color.DarkRed;
		darkRed.A = (byte)(alpha * (float)num);
		m_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
		DrawText(center: new Vector2
		{
			X = (float)MainGame.Instance.GraphicsDevice.Viewport.Width * 0.5f,
			Y = (float)MainGame.Instance.GraphicsDevice.Viewport.Height * 0.75f
		}, font: m_Font, text: "Press Start", color: darkRed);
		m_SpriteBatch.End();
		base.Draw(alpha);
	}

	private void DrawText(SpriteFont font, Vector2 center, string text, Color color)
	{
		Vector2 origin = font.MeasureString(text);
		origin.Y = 0f;
		origin.X /= 2f;
		m_SpriteBatch.DrawString(font, text, center, color, 0f, origin, 1f, SpriteEffects.None, 1f);
	}

	public override void Update()
	{
		m_FrameCount++;
		if (InputManager.ListenForPlayer1Controller())
		{
			m_ScreenManager.ShowScreen(ScreenType.MainMenu);
		}
		base.Update();
	}

	public override Rectangle GetScreenRect()
	{
		return Rectangle.Empty;
	}
}
