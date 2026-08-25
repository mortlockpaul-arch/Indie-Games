using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class ControllerDisconnectedScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	private bool m_PrimaryPlayer;

	public ControllerDisconnectedScreen(ScreenManager manager)
		: base(manager)
	{
		RedButtonText = null;
		GreenButtonText = null;
		YellowButtonText = null;
		BlueButtonText = null;
	}

	public override void LoadContent()
	{
		RecalcScreenRect();
		base.LoadContent();
	}

	public void SetController(bool primaryPlayer)
	{
		m_PrimaryPlayer = primaryPlayer;
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
		DrawText(FontMenuItem, center, "The controller has disconnected.", white, TextAlign.textCentered);
		center.Y += 25f;
		DrawText(FontMenuItem, center, "Please reconnect the controller to continue...", white, TextAlign.textCentered);
		center.Y += 40f;
		Spritebatch.End();
		base.Draw(alpha);
	}

	public override void Update()
	{
		if (m_PrimaryPlayer ? InputManager.GetPlayer1Input().IsConnected : InputManager.GetPlayer2Input().IsConnected)
		{
			m_ScreenManager.HideScreen();
		}
		base.Update();
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
		Vector2 vector = new Vector2(600f, 110f);
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
