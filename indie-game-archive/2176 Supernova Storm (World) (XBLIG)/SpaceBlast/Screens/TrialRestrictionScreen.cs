using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class TrialRestrictionScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	public TrialRestrictionScreen(ScreenManager manager)
		: base(manager)
	{
		RedButtonText = "Main Menu";
		GreenButtonText = null;
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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		Color white = Color.White;
		((Color)(ref white)).A = (byte)(alpha * 255f);
		Spritebatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		Rectangle screenRect = GetScreenRect();
		Vector2 center = new Vector2
		{
			X = ((Rectangle)(ref screenRect)).Center.X,
			Y = (float)((Rectangle)(ref screenRect)).Top + 20f
		};
		DrawText(FontMenuItem, center, "Sorry but this feature is only available in the Full Version.", white);
		center.Y += 40f;
		center.Y += 40f;
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
		m_ScreenManager.ShowScreen(ScreenType.MainMenu);
		base.OnRedButtonPressed();
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
		((Vector2)(ref val))._002Ector(975f, 475f);
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
