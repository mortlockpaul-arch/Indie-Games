using System.Collections.Generic;
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
		Color white = Color.White;
		((Color)(ref white)).A = (byte)(alpha * 255f);
		Spritebatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		Vector2 center = new Vector2
		{
			X = ((Rectangle)(ref m_ScreenRect)).Center.X,
			Y = (float)((Rectangle)(ref m_ScreenRect)).Top + 20f
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
		Guide.ShowGameInvite((PlayerIndex)0, (IEnumerable<Gamer>)null);
		base.OnGreenButtonPressed();
	}

	protected override void OnGreenButtonPressed()
	{
		m_ScreenManager.HideScreen();
		base.OnGreenButtonPressed();
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
		((Vector2)(ref val))._002Ector(700f, 250f);
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

	public override void OnHideScreen()
	{
		MainGame.Instance.ResumeGame();
		base.OnHideScreen();
	}
}
