using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class FPSGameMenu : Menu
{
	public const float TRIAL_TIME_START = 90f;

	public static float TrialTime = 90f;

	public static float FinalTime = 0f;

	public static float BulletHitRatio = 0f;

	public static Vector3 DestinationPos = new Vector3(-2700f, 140f, 6720f);

	private static MenuMgr FPSResultMenuMgr = new MenuMgr();

	private static MenuMgr FPSLobbyMenuMgr = new MenuMgr();

	private static MenuMgr FPSRespawnMenuMgr = new MenuMgr();

	private static MenuMgr FPSInGameMenuMgr = new MenuMgr();

	private static MenuMgr FPSCurrentScoreMenuMgr = new MenuMgr();

	private static MenuMgr FPSExploreMenuMgr = new MenuMgr();

	public static bool MyLeaderboardRequest = false;

	public static bool isCurrentScore = false;

	private static bool InGameMenuVisable = false;

	private Rectangle lobbyListRec = new Rectangle(680, 180, 350, 400);

	private static float PacketTime = 0.5f;

	private static float DownloadTimer = 0f;

	public static bool isVisable
	{
		get
		{
			return InGameMenuVisable;
		}
		set
		{
			InGameMenuVisable = value;
		}
	}

	public static void Close()
	{
		isVisable = false;
		AIBase.DispHelpInfo = 0f;
	}

	public static void SetVisable(bool isBackButton)
	{
		if (isBackButton)
		{
			if (!isVisable)
			{
				if (isCurrentScore)
				{
					isCurrentScore = false;
					AIBase.DispHelpInfo = 12f;
					return;
				}
				isVisable = false;
				isCurrentScore = true;
				AIBase.DispHelpInfo = 0f;
				FPSCurrentScoreMenuMgr.SetBackMenuFunction(GameMenus.MatchCurrentScoreMenu, ResumeGameFunc);
				FPSCurrentScoreMenuMgr.MakeActive(GameMenus.MatchCurrentScoreMenu);
			}
		}
		else if (isVisable)
		{
			isVisable = false;
			AIBase.DispHelpInfo = 12f;
		}
		else
		{
			isVisable = true;
			isCurrentScore = false;
			AIBase.DispHelpInfo = 0f;
			FPSInGameMenuMgr.SetBackMenuFunction(GameMenus.MatchInGameMenuMgr, ResumeGameFunc);
			FPSInGameMenuMgr.MakeActive(GameMenus.MatchInGameMenuMgr);
			if (EndGameEngine.GameSettings.GameName.Contains("_AvR_") || EndGameEngine.GameSettings.GameName.Contains("EndOfDays_Survivor") || EndGameEngine.GameSettings.GameName.Contains("ApocalypseZ"))
			{
				FPSExploreMenuMgr.HideAll();
				FPSExploreMenuMgr.MakeActive(GameMenus.ExploreMenu);
			}
		}
	}

	public FPSGameMenu(GameMenus id)
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
		if (EGENetWorkNext.inviteToGameScheduled)
		{
			EGENetWorkNext.JoinInvite();
			return;
		}
		if (LevelBaseMenu.gameMode == GameMode.XboxLive)
		{
			UpdateXboxLive(eTime);
		}
		else if (LevelBaseMenu.gameMode != GameMode.CoOpPlayer)
		{
			if (LevelBaseMenu.gameMode == GameMode.CombatTraining)
			{
				UpdateCombatTraining(eTime);
			}
			else if (LevelBaseMenu.gameMode == GameMode.SurvivorLocal)
			{
				UpdateSurvivorLocal(eTime);
			}
		}
		UpdateTransition(eTime);
	}

	public override void Draw()
	{
		if (LevelBaseMenu.gameMode == GameMode.XboxLive)
		{
			DrawXboxLive();
		}
		else if (LevelBaseMenu.gameMode != GameMode.CoOpPlayer)
		{
			if (LevelBaseMenu.gameMode == GameMode.CombatTraining)
			{
				DrawCombatTraining();
			}
			else if (LevelBaseMenu.gameMode == GameMode.SurvivorLocal)
			{
				DrawSurvivorLocal();
			}
		}
	}

	public override void DrawBackground()
	{
	}

	private void UpdateXboxLive(float eTime)
	{
	}

	private void UpdateCombatTraining(float eTime)
	{
		if (!LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].Spawned)
		{
			if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].TargetPraticeMessage && LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuSelect)
			{
				LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].TargetPraticeMessage = false;
			}
			if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].AvRStartMessage && LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuSelect)
			{
				LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].AvRStartMessage = false;
			}
		}
		if (isVisable)
		{
			FPSExploreMenuMgr.Update(eTime);
			base.Update(eTime);
		}
		else if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].MatchCoolDownTimer > 0f && LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].MatchCoolDownTimer < 0.5f)
		{
			LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].MatchCoolDownTimer = 0f;
			EndGameEngine.LevelMgr.UpdateMenuReset();
			EndGameEngine.menuMgr.MakeActive(GameMenus.MainMenu);
			LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].Spawned = false;
		}
	}

	private void UpdateSurvivorLocal(float eTime)
	{
		if (!Menu.ActivePlayer.Spawned)
		{
			if (Menu.ActivePlayer.TargetPraticeMessage && LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuSelect)
			{
				Menu.ActivePlayer.TargetPraticeMessage = false;
			}
			if (Menu.ActivePlayer.AvRStartMessage && LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuSelect)
			{
				Menu.ActivePlayer.AvRStartMessage = false;
			}
		}
		if (isVisable)
		{
			for (int i = 0; i < 4; i++)
			{
				if (LevelBaseMenu.Players[i].IsValid)
				{
					LevelBaseMenu.Players[i].fpsWeapon.KillSound();
				}
			}
			AIBase.KillSound();
			FPSExploreMenuMgr.Update(eTime);
			base.Update(eTime);
			return;
		}
		if (Menu.ActivePlayer.RespawnTimeActive())
		{
			if (Menu.ActivePlayer.currentGamePadState.IsButtonDown(Buttons.X) && Menu.ActivePlayer.lastGamePadState.IsButtonUp(Buttons.X))
			{
				Menu.ActivePlayer.ToggledRespawn = !Menu.ActivePlayer.ToggledRespawn;
			}
			FPSRespawnMenuMgr.Update(eTime);
		}
		if (Menu.ActivePlayer.MatchCoolDownTimer > 0f && Menu.ActivePlayer.MatchCoolDownTimer < 0.5f)
		{
			Menu.ActivePlayer.MatchCoolDownTimer = 0f;
			EndGameEngine.LevelMgr.UpdateMenuReset();
			EndGameEngine.menuMgr.MakeActive(GameMenus.MainMenu);
			Menu.ActivePlayer.Spawned = false;
		}
	}

	private void DrawXboxLive()
	{
	}

	private void DrawCombatTraining()
	{
		if (isVisable)
		{
			FPSExploreMenuMgr.Draw();
			base.Draw();
		}
	}

	private void DrawSurvivorLocal()
	{
		if (Menu.ActivePlayer.RespawnTimeActive())
		{
			FPSRespawnMenuMgr.Draw();
		}
		if (isVisable)
		{
			FPSExploreMenuMgr.Draw();
			base.Draw();
		}
	}

	public void UpdateInLobby(float eTime)
	{
	}

	public void UpdateInGame(float eTime)
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		LevelBaseMenu.LoadPlayers();
		isVisable = false;
		TrialTime = 90f;
		BulletHitRatio = 0f;
		FPSExploreMenuMgr.HideAll();
		FPSExploreMenuMgr.MakeActive(GameMenus.ExploreMenu);
		FPSRespawnMenuMgr.HideAll();
		FPSRespawnMenuMgr.MakeActive(GameMenus.MatchRespawnMenu);
		base.BackMenuDelegate += BackMatchSetupMenuFunc;
		LevelBaseMenu.FPSCameraActive = true;
		MyLeaderboardRequest = true;
	}

	private void SetupMenus()
	{
		FPSLobbyMenuMgr.AddMenu(new MatchLobbyMenu(GameMenus.MatchLobbyMenu));
		FPSLobbyMenuMgr.AddMenu(new MatchLoadoutMenu(GameMenus.MatchLoadoutMenu));
		FPSLobbyMenuMgr.AddMenu(new ControllerMenu(GameMenus.FPSControllerMenu));
		FPSLobbyMenuMgr.AddMenu(new AudioVideoMenu(GameMenus.AudioVideoMenu));
		FPSRespawnMenuMgr.AddMenu(new MatchLoadoutMenu(GameMenus.MatchLoadoutMenu));
		FPSRespawnMenuMgr.AddMenu(new MatchRespawnMenu(GameMenus.MatchRespawnMenu));
		FPSInGameMenuMgr.AddMenu(new ControllerMenu(GameMenus.FPSControllerMenu));
		FPSInGameMenuMgr.AddMenu(new AudioVideoMenu(GameMenus.AudioVideoMenu));
		FPSExploreMenuMgr.AddMenu(new ExploreMapMenu(GameMenus.ExploreMenu));
		FPSExploreMenuMgr.AddMenu(new ControllerMenu(GameMenus.FPSControllerMenu));
		FPSExploreMenuMgr.AddMenu(new AudioVideoMenu(GameMenus.AudioVideoMenu));
		FPSExploreMenuMgr.AddMenu(new PlayersMenu(GameMenus.PlayersMenu));
		FPSExploreMenuMgr.SetBackMenuFunction(GameMenus.ExploreMenu, ResumeGameFunc);
	}

	private static void ResumeGameFunc(object sender, MenuEntry e)
	{
		if (!EndGameEngine.GameSettings.GameName.Contains("Testing"))
		{
			if (isVisable)
			{
				isVisable = false;
				LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].ClearInput = true;
				return;
			}
			isVisable = true;
			isCurrentScore = false;
			FPSInGameMenuMgr.SetBackMenuFunction(GameMenus.MatchInGameMenuMgr, ResumeGameFunc);
			FPSInGameMenuMgr.MakeActive(GameMenus.MatchInGameMenuMgr);
			if (EndGameEngine.GameSettings.GameName.Contains("_AvR_") || EndGameEngine.GameSettings.GameName.Contains("EndOfDays_Survivor"))
			{
				FPSExploreMenuMgr.HideAll();
				FPSExploreMenuMgr.MakeActive(GameMenus.ExploreMenu);
			}
		}
		else
		{
			FPSExploreMenuMgr.HideAll();
			FPSInGameMenuMgr.HideAll();
			FPSCurrentScoreMenuMgr.HideAll();
			isVisable = false;
			isCurrentScore = false;
			LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].ClearInput = true;
			if (LevelBaseMenu.gameMode == GameMode.CombatTraining)
			{
				FPSExploreMenuMgr.MakeActive(GameMenus.ExploreMenu);
			}
		}
	}

	private void BackMatchSetupMenuFunc(object sender, MenuEntry e)
	{
		if (!(e.text == "ConfirmMessage"))
		{
			ConfirmMessage.AddMessage("Leave Lobby?", BackMatchSetupMenuFunc);
		}
	}

	private static void BackFPSGameMenuFunc(object sender, MenuEntry e)
	{
		isVisable = false;
		isCurrentScore = false;
	}
}
