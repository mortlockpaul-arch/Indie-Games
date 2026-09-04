using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class TrialMainMenuScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	private int m_LastSelectedIndex;

	public TrialMainMenuScreen(ScreenManager manager)
		: base(manager)
	{
	}

	public override void LoadContent()
	{
		RecalcScreenRect();
		base.LoadContent();
	}

	protected override void OnGreenButtonPressed()
	{
		m_LastSelectedIndex = SelectedItemIndex;
		switch (base.SelectedItemID)
		{
		case 0:
			m_ScreenManager.ShowScreen(ScreenType.SinglePlayerMenu);
			break;
		case 1:
			m_ScreenManager.ShowScreen(ScreenType.SplitScreenMenu);
			break;
		case 2:
			m_ScreenManager.ShowScreen(ScreenType.TrialRestriction);
			break;
		case 3:
			m_ScreenManager.ShowScreen(ScreenType.TrialRestriction);
			break;
		case 4:
			m_ScreenManager.ShowScreen(ScreenType.Purchase);
			break;
		case 5:
			m_ScreenManager.ShowScreen(ScreenType.Instructions);
			break;
		case 6:
			m_ScreenManager.ShowScreen(ScreenType.Credits);
			break;
		case 7:
			m_ScreenManager.ShowScreen(ScreenType.Promotion);
			break;
		case 8:
			((Game)MainGame.Instance).Exit();
			break;
		}
		base.OnGreenButtonPressed();
	}

	public override void OnShowScreen()
	{
		RecalcScreenRect();
		Utils.SetRichPresence((GamerPresenceMode)46, null);
		MenuItems.Clear();
		MenuItems.Add(new MenuItem("Single Player", 0));
		MenuItems.Add(new MenuItem("Split Screen", 1));
		MenuItems.Add(new MenuItem("System Link", 2));
		MenuItems.Add(new MenuItem("XBox Live", 3));
		MenuItems.Add(new MenuItem("Purchase Full Game", 4));
		MenuItems.Add(new MenuItem("Instructions", 5));
		MenuItems.Add(new MenuItem("Credits", 6));
		MenuItems.Add(new MenuItem("Exit to Dashboard", 7));
		RedButtonText = null;
		GreenButtonText = "Select";
		YellowButtonText = null;
		BlueButtonText = null;
		SelectedItemIndex = m_LastSelectedIndex;
		base.OnShowScreen();
	}

	public override Rectangle GetScreenRect()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		RecalcScreenRect();
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
		((Vector2)(ref val))._002Ector(600f, 400f);
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

	public override void Update()
	{
		if (!Guide.IsTrialMode)
		{
			m_ScreenManager.ShowScreen(ScreenType.MainMenu);
		}
		base.Update();
	}
}
