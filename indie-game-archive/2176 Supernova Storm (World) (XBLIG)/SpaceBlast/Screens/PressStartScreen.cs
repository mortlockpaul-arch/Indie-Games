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
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		m_SpriteBatch = new SpriteBatch(((Game)MainGame.Instance).GraphicsDevice);
		m_Font = MainGame.ContentMan.Load<SpriteFont>("Fonts/MenuFont");
		base.LoadContent();
	}

	public override void Draw(float alpha)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		int num = Math.Abs(m_FrameCount % 100 - 50) * 4;
		Color darkRed = Color.DarkRed;
		((Color)(ref darkRed)).A = (byte)(alpha * (float)num);
		m_SpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		Vector2 center = default(Vector2);
		Viewport viewport = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		center.X = (float)((Viewport)(ref viewport)).Width * 0.5f;
		Viewport viewport2 = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		center.Y = (float)((Viewport)(ref viewport2)).Height * 0.75f;
		DrawText(m_Font, center, "Press Start", darkRed);
		m_SpriteBatch.End();
		base.Draw(alpha);
	}

	private void DrawText(SpriteFont font, Vector2 center, string text, Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = font.MeasureString(text);
		val.Y = 0f;
		val.X /= 2f;
		m_SpriteBatch.DrawString(font, text, center, color, 0f, val, 1f, (SpriteEffects)0, 1f);
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Rectangle.Empty;
	}
}
