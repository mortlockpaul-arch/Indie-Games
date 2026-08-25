using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

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
			MainGame.Instance.Exit();
			break;
		}
		base.OnGreenButtonPressed();
	}

	public override void OnShowScreen()
	{
		RecalcScreenRect();
		Utils.SetRichPresence(GamerPresenceMode.AtMenu, null);
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
		Vector2 vector = new Vector2(600f, 400f);
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

	public override void Update()
	{
		if (!Guide.IsTrialMode)
		{
			m_ScreenManager.ShowScreen(ScreenType.MainMenu);
		}
		base.Update();
	}
}
