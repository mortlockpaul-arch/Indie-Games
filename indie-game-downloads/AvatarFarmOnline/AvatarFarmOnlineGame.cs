using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Logic.Mode.Farm.Online;
using AvatarFarmOnline.Scores;
using AvatarFarmOnline.Sections;
using AvatarFarmOnline.Template;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Quasar.GUI;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Network;
using Quasar.GameUtils.Sections;
using Quasar.GameUtils.XBLIG.CrossPromotion;
using Quasar.GameUtils.XBLIG.Scores;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;
using Quasar.Render;
using Quasar.Render.Passes;

namespace AvatarFarmOnline;

internal class AvatarFarmOnlineGame : BaseGame
{
	public const string ContainerName = "AvatarFarmOnlineSave";

	private static RenderPass2D aaPass;

	public static RenderPass2D AntialiasPass
	{
		get
		{
			if (aaPass == null)
			{
				aaPass = new AntialiasRenderPass(Engine.BackBufferSize, 4);
			}
			return aaPass;
		}
	}

	public AvatarFarmOnlineGame()
		: base("AvatarFarmOnlineSave", useXACTJukebox: true)
	{
	}

	public override void LoadConfig()
	{
		AvatarFarmOnline.AvatarFarmOnlineConfig.LoadConfig();
	}

	public override void InitGame()
	{
		base.InitGame();
		AvatarFarmOnline.Logic.Mode.Farm.Online.Online.SetupInvites();
		AvatarFarmOnline.Logic.Mode.Farm.Online.Online.OnInviteAccepted += OnInviteAccepted;
		AvatarFarmOnline.Logic.Mode.Farm.Online.Online.OnSessionEnd += Online_OnSessionEnd;
		CrossPromotionManager.Init();
		AvatarFarmOnline.Scores.GameScoreManager.Instance.Update();
		if (XBLIGDebugScoreSection<AvatarFarmOnline.Scores.GameHighscore>.ReadyToUse)
		{
			AddExtraSection(new XBLIGDebugScoreSection<AvatarFarmOnline.Scores.GameHighscore>(AvatarFarmOnline.Scores.GameScoreManager.Instance));
		}
		Gamepad.SetDeadZone(new Gamepad.DeadZoneSetting(0.14f));
		Settings.AllowSleep = false;
		Settings.MaxPolygonVertices = 12;
	}

	private void OnInviteAccepted(ISignedInGamer obj)
	{
		if (BaseGame.PlayerStorageDevice(obj.PlayerIndex) == null || !BaseGame.PlayerStorageDevice(obj.PlayerIndex).IsConnected)
		{
			if (!BaseGame.Instance.AskForStorageDevice(obj.PlayerIndex, OnInviteStorageSelected))
			{
				DelayedActionManager.AddAction(150, delegate
				{
					OnInviteAccepted(obj);
				});
			}
		}
		else
		{
			JoinInvite(obj.PlayerIndex);
		}
	}

	private void OnInviteStorageSelected(StorageDevice sd, PlayerIndex playerIndex)
	{
		if (sd == null || !sd.IsConnected)
		{
			DelayedActionManager.AddAction(150, delegate
			{
				OnInviteAccepted(PlatformInterface.Instance.GetGamer(playerIndex));
			});
		}
		else
		{
			JoinInvite(playerIndex);
		}
	}

	private void JoinInvite(PlayerIndex playerIndex)
	{
		AvatarFarmOnline.Scores.GameScoreManager.Instance.Enabled = false;
		GameManager.Clear();
		AvatarFarmOnline.Logic.Mode.Farm.FarmGameSetup setup = new AvatarFarmOnline.Logic.Mode.Farm.FarmGameSetup(playerIndex);
		GameManager.SetSetup(setup);
		GameManager.StartGame();
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		farmPersistentGameData.InitOnline();
		InputManager.SetIndex(playerIndex);
		((BaseGame)Engine.Game).NextGameSection = new AvatarFarmOnline.Sections.JoinedGameSection();
	}

	public override void MainLoop()
	{
		if (GameManager.PersistentData != null)
		{
			GameManager.PersistentData.Update();
		}
		base.MainLoop();
		AvatarFarmOnline.Scores.GameScoreManager.Instance.Update();
	}

	protected override void LoadTemplate()
	{
		TemplateManager.Templates.AddTemplate(AvatarFarmOnline.Template.ExtendedGameTemplate.Template, new AvatarFarmOnline.Template.ExtendedGameTemplate());
	}

	protected override void LoadSection(GameSection section, bool initSection)
	{
		if (AvatarFarmOnline.Sections.BGSection.HasInstance)
		{
			RemoveExtraSection(AvatarFarmOnline.Sections.BGSection.Instance);
		}
		base.LoadSection(section, initSection);
		int id = section.Id;
		if (id != 2 && id != 6 && id != 11)
		{
			AddExtraSection(AvatarFarmOnline.Sections.BGSection.Instance);
		}
	}

	protected override bool IsSignInSafe(PlayerIndex playerIndex, int state)
	{
		GameSetup currentSetup = GameManager.CurrentSetup;
		if (currentSetup != null)
		{
			return !currentSetup.PlayerIndices.Contains(playerIndex);
		}
		return true;
	}

	protected override bool IsMenuState(int state)
	{
		if (state != 2)
		{
			return state != 11;
		}
		return false;
	}

	public override GameSection GetSection(int sectionId)
	{
		return sectionId switch
		{
			3 => new AvatarFarmOnline.Sections.AvatarFarmSplashSection(), 
			0 => new AvatarFarmOnline.Sections.MainMenuSection(), 
			12 => new AvatarFarmOnline.Sections.LoadingSection(), 
			13 => new AvatarFarmOnline.Sections.SettingsSection(), 
			14 => new AvatarFarmOnline.Sections.ExtrasSection(), 
			_ => base.GetSection(sectionId), 
		};
	}

	protected override void PreloadThings()
	{
	}

	private void Online_OnSessionEnd(SessionEndReason obj)
	{
		if (obj == SessionEndReason.ClientSignedOut)
		{
			return;
		}
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		if (farmPersistentGameData.Online.MultiplayerState != AvatarFarmOnline.Logic.Mode.Farm.Online.Online.MultiplayerStates.Inactive)
		{
			BaseGame.Instance.NextGameSectionId = 0;
			((GUISection)BaseGame.Instance.NextGameSection).Layout.ShowMessage("DISCONNECTED".Translate(), "DISCONNECTED_FROM_GAME".Translate());
			farmPersistentGameData.Online.Reset();
			if (farmPersistentGameData.CurrentGame != null)
			{
				farmPersistentGameData.ClearRound();
			}
		}
	}

	public override void GoToMainScreen()
	{
		base.NextGameSectionId = 3;
	}

	public override void Dispose()
	{
		AvatarFarmOnline.Scores.GameScoreManager.Instance.Save();
		base.Dispose();
	}
}
