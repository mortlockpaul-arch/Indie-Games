using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace SpaceBlast.Screens;

internal class MultiplayerMenuScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	public MultiplayerMenuScreen(ScreenManager manager)
		: base(manager)
	{
		MenuItems.Add(new MenuItem("Instant Action", 0));
		MenuItems.Add(new MenuItem("Create Private Game", 3));
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
			MainGame.NetMan.SignIn(NetworkSessionType.PlayerMatch, XBoxLiveInstantActionSignInCompleteEvent, null);
			break;
		case 2:
			m_ScreenManager.ShowScreen(ScreenType.MultiplayerCreateCustomGameScreen);
			break;
		case 3:
			MainGame.NetMan.SignIn(NetworkSessionType.PlayerMatch, XBoxLivePrivateGameSignInCompleteEvent, null);
			break;
		}
		base.OnGreenButtonPressed();
	}

	private void XBoxLiveInstantActionSignInCompleteEvent(object sender, EventArgs e)
	{
		m_ScreenManager.ShowScreen(ScreenType.MultiplayerGameSearch);
	}

	private void XBoxLivePrivateGameSignInCompleteEvent(object sender, EventArgs e)
	{
		MainGame.NetMan.CreateGame(publicgame: false);
		m_ScreenManager.ShowScreen(ScreenType.PrivateGameScreen);
	}

	protected override void OnRedButtonPressed()
	{
		m_ScreenManager.ShowScreen(ScreenType.MainMenu);
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
		Vector2 vector = new Vector2(400f, 200f);
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
