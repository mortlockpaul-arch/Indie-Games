using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace SpaceBlast.Screens;

internal class PauseMenuScreen : MenuScreen
{
	private GamerPresenceMode m_PrevRichPresenceMode;

	private Rectangle m_ScreenRect;

	public PauseMenuScreen(ScreenManager manager)
		: base(manager)
	{
		MenuItems.Add(new MenuItem("Resume Game", 0));
		MenuItems.Add(new MenuItem("Exit To Main Menu", 2));
		MenuItems.Add(new MenuItem("Exit To Dashboard", 3));
		RedButtonText = "Back";
		GreenButtonText = "Select";
		YellowButtonText = null;
		BlueButtonText = null;
	}

	public override void LoadContent()
	{
		RecalcScreenRect();
		base.LoadContent();
	}

	protected override void OnGreenButtonPressed()
	{
		switch (base.SelectedItemID)
		{
		case 0:
			m_ScreenManager.HideScreen();
			MainGame.Instance.ResumeGame();
			break;
		case 1:
			Guide.ShowGameInvite(PlayerIndex.One, null);
			break;
		case 2:
			MainGame.Instance.LeaveGame();
			break;
		case 3:
			MainGame.Instance.Exit();
			break;
		}
		base.OnGreenButtonPressed();
	}

	protected override void OnRedButtonPressed()
	{
		m_ScreenManager.HideScreen();
		MainGame.Instance.ResumeGame();
		base.OnRedButtonPressed();
	}

	public override void OnShowScreen()
	{
		m_PrevRichPresenceMode = Utils.GetRichPresenceMode();
		Utils.SetRichPresence(GamerPresenceMode.Paused, null);
		MenuItems.Clear();
		MenuItems.Add(new MenuItem("Resume Game", 0));
		if (MainGame.NetMan.IsXBoxLiveGame)
		{
			MenuItems.Add(new MenuItem("Invite Friends to Game", 1));
		}
		MenuItems.Add(new MenuItem("Exit To Main Menu", 2));
		MenuItems.Add(new MenuItem("Exit To Dashboard", 3));
		base.OnShowScreen();
	}

	public override void OnHideScreen()
	{
		Utils.SetRichPresence(m_PrevRichPresenceMode, null);
		base.OnHideScreen();
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
		Vector2 vector = new Vector2(400f, 200f);
		if (MainGame.NetMan.IsXBoxLiveGame)
		{
			vector.Y = 225f;
		}
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
