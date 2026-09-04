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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		Color white = Color.White;
		((Color)(ref white)).A = (byte)(alpha * 255f);
		Spritebatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		Vector2 center = new Vector2
		{
			X = ((Rectangle)(ref m_ScreenRect)).Center.X,
			Y = (float)((Rectangle)(ref m_ScreenRect)).Top + 20f
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = font.MeasureString(text);
		val.Y = 0f;
		val.X /= 2f;
		Spritebatch.DrawString(font, text, center, color, 0f, val, 1f, (SpriteEffects)0, 1f);
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
		((Game)MainGame.Instance).Exit();
		base.OnBlueButtonPressed();
	}

	public override Rectangle GetScreenRect()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_ScreenRect;
	}

	public override void OnScreenResize()
	{
		RecalcScreenRect();
		base.OnScreenResize();
	}

	private void RecalcScreenRect()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(975f, 400f);
		Viewport viewport = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		float num = ((Viewport)(ref viewport)).Width;
		float num2 = num * 0.5f;
		Viewport viewport2 = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		float num3 = ((Viewport)(ref viewport2)).Height;
		float num4 = num3 * 0.5f;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(num2 - val.X * 0.5f, num4 - val.Y * 0.5f);
		m_ScreenRect = default(Rectangle);
		m_ScreenRect.X = (int)val2.X;
		m_ScreenRect.Y = (int)val2.Y;
		m_ScreenRect.Width = (int)val.X;
		m_ScreenRect.Height = (int)val.Y;
	}
}
