using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class PromotionScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	public PromotionScreen(ScreenManager manager)
		: base(manager)
	{
		RedButtonText = "Back";
		GreenButtonText = "Exit";
		YellowButtonText = null;
		BlueButtonText = "Purchase";
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
		DrawText(FontMenuItem, center, "For just 200 points you can get the full version, which includes:", white);
		center.Y += 40f;
		DrawText(FontMenuItem, center, "* XBox Live and System Link Multiplayer Games with upto 8 players *", white);
		center.Y += 32f;
		DrawText(FontMenuItem, center, "* Invite your friends to multiplayer games *", white);
		center.Y += 40f;
		DrawText(FontMenuItem, center, "* Create Private Games for just you and your friends *", white);
		center.Y += 40f;
		DrawText(FontMenuItem, center, "* Co-operative Play *", white);
		center.Y += 40f;
		DrawText(FontMenuItem, center, "* 15 Weapon Variations *", white);
		center.Y += 40f;
		DrawText(FontMenuItem, center, "* 16 PowerUps *", white);
		center.Y += 40f;
		DrawText(FontMenuItem, center, "* Loads more Levels *", white);
		center.Y += 40f;
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
		m_ScreenManager.ShowScreen(ScreenType.Purchase);
		base.OnGreenButtonPressed();
	}

	protected override void OnRedButtonPressed()
	{
		m_ScreenManager.ShowScreen(ScreenType.MainMenu);
		base.OnRedButtonPressed();
	}

	protected override void OnGreenButtonPressed()
	{
		MainGame.Instance.Exit();
		base.OnBlueButtonPressed();
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
		Vector2 vector = new Vector2(975f, 400f);
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
}
