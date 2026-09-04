using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class CreditsScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	public CreditsScreen(ScreenManager manager)
		: base(manager)
	{
		RedButtonText = "Back";
		GreenButtonText = null;
		YellowButtonText = null;
		BlueButtonText = null;
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
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		Color white = Color.White;
		((Color)(ref white)).A = (byte)(alpha * 255f);
		Spritebatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		Rectangle screenRect = GetScreenRect();
		Vector2 center = new Vector2
		{
			X = ((Rectangle)(ref screenRect)).Center.X,
			Y = (float)((Rectangle)(ref screenRect)).Top + 20f
		};
		DrawText(FontMenuItem, center, "Developed by", white, TextAlign.textCentered);
		center.Y += 25f;
		DrawText(FontMenuItem, center, "Ben Sleat", white, TextAlign.textCentered);
		center.Y += 40f;
		DrawText(FontMenuItem, center, "Please send feedback,", white, TextAlign.textCentered);
		center.Y += 25f;
		DrawText(FontMenuItem, center, "bug reports and suggestions to:", white, TextAlign.textCentered);
		center.Y += 32f;
		DrawText(FontMenuItem, center, "Ben@SupernovaStorm.com", white, TextAlign.textCentered);
		center.Y += 40f;
		DrawText(FontMenuItem, center, "Some images courtesy of:", white, TextAlign.textCentered);
		center.Y += 25f;
		DrawText(FontMenuItem, center, "NASA", white, TextAlign.textCentered);
		center.Y += 32f;
		DrawText(FontMenuItem, center, "Some sounds courtesy of:", white, TextAlign.textCentered);
		center.Y += 25f;
		DrawText(FontMenuItem, center, "www.SoundSnap.com", white, TextAlign.textCentered);
		center.Y += 40f;
		DrawText(FontSmallMenuItem, center, "(C) 2009 Ben Sleat", white, TextAlign.textCentered);
		center.Y += 25f;
		Spritebatch.End();
		base.Draw(alpha);
	}

	protected override void OnRedButtonPressed()
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
		((Vector2)(ref val))._002Ector(600f, 370f);
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
