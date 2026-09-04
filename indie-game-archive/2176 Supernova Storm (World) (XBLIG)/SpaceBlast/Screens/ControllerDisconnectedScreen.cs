using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
		Color white = Color.White;
		((Color)(ref white)).A = (byte)(alpha * 255f);
		Spritebatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		Rectangle screenRect = GetScreenRect();
		Vector2 center = new Vector2
		{
			X = ((Rectangle)(ref screenRect)).Center.X,
			Y = (float)((Rectangle)(ref screenRect)).Top + 20f
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		bool isConnected;
		if (!m_PrimaryPlayer)
		{
			GamePadState player2Input = InputManager.GetPlayer2Input();
			isConnected = ((GamePadState)(ref player2Input)).IsConnected;
		}
		else
		{
			GamePadState player1Input = InputManager.GetPlayer1Input();
			isConnected = ((GamePadState)(ref player1Input)).IsConnected;
		}
		if (isConnected)
		{
			m_ScreenManager.HideScreen();
		}
		base.Update();
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
		((Vector2)(ref val))._002Ector(600f, 110f);
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
