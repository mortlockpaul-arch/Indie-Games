using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class GameSearchScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	public GameSearchScreen(ScreenManager manager)
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
		Color white = Color.White;
		white.A = (byte)(alpha * 255f);
		Spritebatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
		Rectangle screenRect = GetScreenRect();
		Vector2 center = new Vector2
		{
			X = screenRect.Center.X,
			Y = screenRect.Center.Y
		};
		DrawText(FontMenuItem, center, "Searching for games...", white, TextAlign.textCentered);
		center.Y += 40f;
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
		return m_ScreenRect;
	}

	public override void OnScreenResize()
	{
		RecalcScreenRect();
		base.OnScreenResize();
	}

	private void RecalcScreenRect()
	{
		Vector2 vector = new Vector2(800f, 600f);
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
