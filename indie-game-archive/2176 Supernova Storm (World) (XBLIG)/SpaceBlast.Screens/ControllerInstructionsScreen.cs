using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class ControllerInstructionsScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	private Texture2D m_TexController;

	private Vector2 m_PosInstructions;

	public ControllerInstructionsScreen(ScreenManager manager)
		: base(manager)
	{
		RedButtonText = "Back";
		GreenButtonText = "Screen Layout";
		YellowButtonText = null;
		BlueButtonText = null;
	}

	public override void LoadContent()
	{
		RecalcScreenRect();
		m_TexController = MainGame.ContentMan.Load<Texture2D>("Textures/ControllerInstructions");
		m_PosInstructions = new Vector2((float)m_ScreenRect.Center.X - (float)m_TexController.Width * 0.5f, (float)m_ScreenRect.Center.Y - (float)m_TexController.Height * 0.5f);
		base.LoadContent();
	}

	public override void Draw(float alpha)
	{
		Color white = Color.White;
		white.A = (byte)(alpha * 255f);
		Spritebatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
		Spritebatch.Draw(m_TexController, m_PosInstructions, white);
		Spritebatch.End();
		base.Draw(alpha);
	}

	protected override void OnRedButtonPressed()
	{
		m_ScreenManager.ShowScreen(ScreenType.MainMenu);
		base.OnRedButtonPressed();
	}

	protected override void OnGreenButtonPressed()
	{
		m_ScreenManager.ShowScreen(ScreenType.LayoutInstructions);
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
		Vector2 vector = new Vector2(625f, 475f);
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
