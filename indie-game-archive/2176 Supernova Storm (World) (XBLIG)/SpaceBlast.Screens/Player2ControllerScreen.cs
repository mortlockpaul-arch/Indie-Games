using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class Player2ControllerScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	public Player2ControllerScreen(ScreenManager manager)
		: base(manager)
	{
		RedButtonText = "Cancel";
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
			Y = (float)screenRect.Top + 20f
		};
		DrawText(FontMenuItem, center, "Player 2, please Press Start...", white);
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

	public override void OnShowScreen()
	{
		MainGame.Instance.PauseGame(forceFreeze: true, showMenu: false);
		base.OnShowScreen();
	}

	public override void Update()
	{
		if (InputManager.ListenForPlayer2Controller())
		{
			m_ScreenManager.HideScreen();
			MainGame.Instance.ResumeGame();
		}
		base.Update();
	}

	protected override void OnRedButtonPressed()
	{
		MainGame.Instance.ShowMainScreen(showmenu: true);
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
		Vector2 vector = new Vector2(500f, 175f);
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
