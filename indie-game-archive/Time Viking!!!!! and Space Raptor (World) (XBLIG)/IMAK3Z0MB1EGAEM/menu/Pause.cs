using IMAK3Z0MB1EGAEM.director;
using Viking_x86.director;

namespace IMAK3Z0MB1EGAEM.menu;

public class Pause : MenuLevel
{
	public Pause()
	{
		title = "paused!!!1";
		item = new string[2] { "resume game", "quit" };
	}

	public override void Accept()
	{
		switch (sel)
		{
		case 0:
			TimeMgr.CurTMgr().UnPause();
			break;
		case 1:
		{
			switch (GameState.state)
			{
			case GameState.State.ZombiesPlaying:
				GameState.state = GameState.State.ZombiesMenu;
				break;
			case GameState.State.EndlessZombiesPlaying:
				GameState.state = GameState.State.EndlessZombiesMenu;
				break;
			case GameState.State.VikingPlaying:
				GameState.state = GameState.State.VikingMenu;
				Menu.Reset();
				break;
			}
			for (int i = 0; i < 4; i++)
			{
				Menu.playerState[i] = Menu.PlayerState.Out;
			}
			Menu.timeGo = 0f;
			Menu.grace = 3;
			break;
		}
		}
		base.Accept();
	}

	public override void Cancel()
	{
		TimeMgr.CurTMgr().UnPause();
		base.Cancel();
	}
}
