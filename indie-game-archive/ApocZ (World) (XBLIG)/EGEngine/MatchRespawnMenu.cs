namespace EGEngine;

public class MatchRespawnMenu : Menu
{
	private static Menu[] RespawnLoadoutMenu = new MatchLoadoutMenu[4];

	public MatchRespawnMenu(GameMenus id)
		: base(id)
	{
	}

	public override void LoadContent()
	{
		base.LoadContent();
		SetupMenus();
	}

	public override void Update(float eTime)
	{
		if (Menu.ActivePlayer.ToggledRespawn)
		{
			if (RespawnLoadoutMenu[(int)Menu.ActivePlayer.playerIndex].State == MenuState.Active)
			{
				RespawnLoadoutMenu[(int)Menu.ActivePlayer.playerIndex].State = MenuState.TransitionOff;
			}
		}
		else if (RespawnLoadoutMenu[(int)Menu.ActivePlayer.playerIndex].State == MenuState.Hidden)
		{
			RespawnLoadoutMenu[(int)Menu.ActivePlayer.playerIndex].State = MenuState.TransitionOn;
		}
		if (RespawnLoadoutMenu[(int)Menu.ActivePlayer.playerIndex].IsActive)
		{
			RespawnLoadoutMenu[(int)Menu.ActivePlayer.playerIndex].Update(eTime);
		}
		else
		{
			base.Update(eTime);
		}
	}

	public override void Draw()
	{
		base.Draw();
		if (RespawnLoadoutMenu[(int)Menu.ActivePlayer.playerIndex].IsActive)
		{
			RespawnLoadoutMenu[(int)Menu.ActivePlayer.playerIndex].Draw();
		}
		for (int i = 0; i < 4; i++)
		{
			if (!LevelBaseMenu.Players[i].ToggledRespawn)
			{
				LevelBaseMenu.Players[i].SetViewPortTestCoOp(PlayerBase.RenderPass.ForwardPass, 0);
				Menu.spriteBatch.Begin();
				Menu.DrawEnterLoadoutButton(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport, "TO RESPAWN");
				Menu.spriteBatch.End();
			}
		}
	}

	public override void DrawBackground()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		RespawnLoadoutMenu[(int)Menu.ActivePlayer.playerIndex].BackMenuDelegate += LoadoutFunc;
	}

	private void SetupMenus()
	{
		for (int i = 0; i < 4; i++)
		{
			RespawnLoadoutMenu[i] = new MatchLoadoutMenu(GameMenus.MatchLoadoutMenu);
			RespawnLoadoutMenu[i].LoadContent();
			RespawnLoadoutMenu[i].State = MenuState.Hidden;
		}
	}

	private void LoadoutFunc(object sender, MenuEntry e)
	{
	}
}
