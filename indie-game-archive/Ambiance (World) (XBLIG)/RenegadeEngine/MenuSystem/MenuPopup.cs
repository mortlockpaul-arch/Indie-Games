using Microsoft.Xna.Framework;

namespace RenegadeEngine.MenuSystem;

internal class MenuPopup : MenuScreen
{
	public string Message = "";

	public MenuPopup()
	{
		IsPopUp = true;
		buttons = new MenuButton[4];
		buttons[0] = new MenuButton("Return", resizeToFont: true);
		buttons[1] = new MenuButton("Mute Music", resizeToFont: true);
		buttons[2] = new MenuButton("Reset Screen Saver", resizeToFont: true);
		buttons[3] = new MenuButton("Exit App", resizeToFont: true);
		highlightedButton = buttons[0];
		highlightedButton.HasFocus(hasFocus: true);
		buttons[0].Activated += On_Return_Activated;
		buttons[1].Activated += On_Mute_Activated;
		buttons[2].Activated += On_Reset_Activated;
		buttons[3].Activated += On_Exit_Activated;
		Boundary.X = Global.ScreenWidth / 2 - 100;
		Boundary.Y = Global.ScreenHeight / 2 - 100;
		Boundary.Width = 200;
		Boundary.Height = 200;
		OrganizeButtonsVertically(new Point(Boundary.X + 10, Boundary.Y + 10), buttons, 4);
	}

	public override void UpdateInput(GameTime gameTime)
	{
		base.UpdateInput(gameTime);
		if (Input.MenuQuit(ControllingPlayer))
		{
			On_Return_Activated(this, new PlayerIndexEventArgs(ControllingPlayer));
		}
	}

	private void On_Return_Activated(object sender, PlayerIndexEventArgs e)
	{
		Dispose();
	}

	private void On_Mute_Activated(object sender, PlayerIndexEventArgs e)
	{
		if (SoundMgr.MusicMuted)
		{
			buttons[1].Label = "Mute Music";
			SoundMgr.MuteMusic(muteMusic: false);
		}
		else
		{
			buttons[1].Label = "Unmute Music";
			SoundMgr.MuteMusic(muteMusic: true);
		}
	}

	private void On_Reset_Activated(object sender, PlayerIndexEventArgs e)
	{
		EngineManager.ResetGameplay(e.Index);
		Dispose();
	}

	private void On_Exit_Activated(object sender, PlayerIndexEventArgs e)
	{
		Dispose();
		EngineManager.EndGameplay(e.Index);
	}
}
