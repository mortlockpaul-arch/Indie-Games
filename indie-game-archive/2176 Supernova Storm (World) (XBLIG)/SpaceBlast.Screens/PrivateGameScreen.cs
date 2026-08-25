using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class PrivateGameScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	public PrivateGameScreen(ScreenManager manager)
		: base(manager)
	{
		RedButtonText = null;
		GreenButtonText = "Play";
		YellowButtonText = null;
		BlueButtonText = "Invite Friends";
	}

	public override void LoadContent()
	{
		RecalcScreenRect();
		base.LoadContent();
	}

	public override void Draw(float alpha)
	{
		Color white = Color.White;
		white.A = (byte)(alpha * 255f);
		Spritebatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
		Vector2 center = new Vector2
		{
			X = m_ScreenRect.Center.X,
			Y = (float)m_ScreenRect.Top + 20f
		};
		DrawText(FontMenuItem, center, "A private XBox Live game has been created.", white);
		center.Y += 40f;
		DrawText(FontMenuItem, center, "Press X to invite your friends to the game now,", white);
		center.Y += 32f;
		DrawText(FontMenuItem, center, "or you can invite people during the game", white);
		center.Y += 32f;
		DrawText(FontMenuItem, center, "from the Pause menu.", white);
		center.Y += 32f;
		Spritebatch.End();
		base.Draw(alpha);
	}

	private void DrawText(SpriteFont font, Vector2 center, string text, Color color)
	{
		Vector2 origin = font.MeasureString(text);
		origin.Y = 0f;
		origin.X /= 2f;
		Spritebatch.DrawString(font, text, center, color, 0f, origin, 1f, SpriteEffects.None, 1f);
	}

	protected override void OnBlueButtonPressed()
	{
		Guide.ShowGameInvite(PlayerIndex.One, null);
		base.OnGreenButtonPressed();
	}

	protected override void OnGreenButtonPressed()
	{
		m_ScreenManager.HideScreen();
		base.OnGreenButtonPressed();
	}

	public override Rectangle GetScreenRect()
	{
		return m_ScreenRect;
	}

	public override void OnScreenResize()
	{
		RecalcScreenRect();
		base.OnScreenResize();
	}

	private void RecalcScreenRect()
	{
		Vector2 vector = new Vector2(700f, 250f);
		float num = MainGame.Instance.GraphicsDevice.Viewport.Width;
		float num2 = num * 0.5f;
		float num3 = MainGame.Instance.GraphicsDevice.Viewport.Height;
		float num4 = num3 * 0.5f;
		Vector2 vector2 = new Vector2(num2 - vector.X * 0.5f, num4 - vector.Y * 0.5f);
		m_ScreenRect = default(Rectangle);
		m_ScreenRect.X = (int)vector2.X;
		m_ScreenRect.Y = (int)vector2.Y;
		m_ScreenRect.Width = (int)vector.X;
		m_ScreenRect.Height = (int)vector.Y;
	}

	public override void OnHideScreen()
	{
		MainGame.Instance.ResumeGame();
		base.OnHideScreen();
	}
}
