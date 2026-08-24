using System;
using Microsoft.Xna.Framework;

namespace RenegadeEngine.MenuSystem;

internal class MainMenuScreen : MenuScreen
{
	private TimeSpan timer = TimeSpan.Zero;

	public MainMenuScreen()
	{
		Boundary.Width = Global.ScreenWidth;
		Boundary.Height = Global.ScreenHeight;
		buttons = new MenuButton[1];
		buttons[0] = new MenuButton("Ambiance", resizeToFont: true);
		buttons[0].Font = AssetManager.GetAsset(FontKeys.TitleFont);
		buttons[0].ResizeToLabel();
		buttons[0].TextColor = Color.Violet;
		buttons[0].BackgroundColor = Color.Transparent;
		buttons[0].CenterVertically = true;
		buttons[0].CenterHorizontally = true;
		buttons[0].Y = Global.ScreenHeight / 2 - 100;
		highlightedButton = buttons[0];
		highlightedButton.HasFocus(hasFocus: false);
		backGround = AssetManager.GetAsset(ImageKeys.titleCredits);
		SoundMgr.PlaySong();
	}

	public override void Update(GameTime gameTime)
	{
		if (currentState == ScreenState.Active)
		{
			timer += gameTime.ElapsedGameTime;
			if (timer.Seconds >= 5)
			{
				EngineManager.StartGameplay(ControllingPlayer);
				Dispose();
			}
		}
		base.Update(gameTime);
	}
}
