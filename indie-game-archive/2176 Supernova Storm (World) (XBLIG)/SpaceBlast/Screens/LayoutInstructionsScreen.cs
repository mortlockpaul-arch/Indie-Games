using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class LayoutInstructionsScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	private Texture2D m_TexController;

	private Vector2 m_PosInstructions;

	public LayoutInstructionsScreen(ScreenManager manager)
		: base(manager)
	{
		RedButtonText = "Back";
		GreenButtonText = "Controls";
		YellowButtonText = null;
		BlueButtonText = null;
	}

	public override void LoadContent()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		RecalcScreenRect();
		m_TexController = MainGame.ContentMan.Load<Texture2D>("Textures/HUDInstructions");
		m_PosInstructions = new Vector2((float)((Rectangle)(ref m_ScreenRect)).Center.X - (float)m_TexController.Width * 0.5f, (float)((Rectangle)(ref m_ScreenRect)).Center.Y - (float)m_TexController.Height * 0.5f);
		base.LoadContent();
	}

	public override void Draw(float alpha)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Color white = Color.White;
		((Color)(ref white)).A = (byte)(alpha * 255f);
		Spritebatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
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
		m_ScreenManager.ShowScreen(ScreenType.Instructions);
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
		((Vector2)(ref val))._002Ector(625f, 525f);
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
