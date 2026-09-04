using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

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
			Guide.ShowGameInvite((PlayerIndex)0, (IEnumerable<Gamer>)null);
			break;
		case 2:
			MainGame.Instance.LeaveGame();
			break;
		case 3:
			((Game)MainGame.Instance).Exit();
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_PrevRichPresenceMode = Utils.GetRichPresenceMode();
		Utils.SetRichPresence((GamerPresenceMode)48, null);
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Utils.SetRichPresence(m_PrevRichPresenceMode, null);
		base.OnHideScreen();
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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(400f, 200f);
		if (MainGame.NetMan.IsXBoxLiveGame)
		{
			val.Y = 225f;
		}
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
