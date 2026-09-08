using System;
using System.Text;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Definition;
using AvatarFarmOnline.Scenes;
using AvatarFarmOnline.Template;
using AvatarFarmOnline.Template.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Quasar;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GUI.Controls.GroupControls;
using Quasar.GameUtils.Audio;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Render;
using Quasar.GameUtils.Sections;
using Quasar.GameUtils.Storage;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;
using Quasar.Render;
using Quasar.Render.Sorters;

namespace AvatarFarmOnline.Sections;

internal class StageSection : GameSection
{
	private AvatarFarmOnline.Scenes.StageScene stageScene;

	private AvatarFarmOnline.Scenes.StageAfterScene stageAfterScene;

	private AvatarFarmOnline.Scenes.HUDScene HUDScene;

	private Scene layoutScene;

	private Scene shopLayoutScene;

	private Scene messageLayoutScene;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private Layout pauseLayout;

	private Layout messageLayout;

	private AvatarFarmOnline.Scenes.ANFinalScene finalPP;

	private Group pauseGroup;

	private RenderPass2D sceneRenderPass;

	private AvatarFarmOnline.Template.Controls.Shop shop;

	private Layout shopLayout;

	private FakeDOFRenderProcess fakeDOF;

	private Selector friendSelector;

	private Selector publicSelector;

	private bool trialCausedPause;

	private bool guideCausedPause;

	public StageSection(AvatarFarmOnline.Logic.Stage.Stage stage)
		: base(11)
	{
		StorageManager.Instance.SaveEnabled = false;
		GameManager.CurrentGame.OnStageFinished += OnStageFinished;
		this.stage = stage;
		messageLayout = new Layout("Message", "", AvatarFarmOnline.Template.ExtendedGameTemplate.Template);
		pauseLayout = new Layout("Pause", "PAUSE_TITLE".Translate(), AvatarFarmOnline.Template.ExtendedGameTemplate.Template);
		pauseGroup = new Group(pauseLayout);
		pauseGroup.SetId("MainMenuGroup");
		pauseGroup.OnCancel += OnResume;
		Button activeControl = pauseLayout.AddButton(pauseGroup, "Resume", "RESUME".Translate(), OnResume);
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		if (farmPersistentGameData.IsOnline)
		{
			pauseLayout.AddButton(pauseGroup, "Invite", "INVITE".Translate(), OnInvite);
			pauseLayout.AddButton(pauseGroup, "InviteParty", "INVITE_PARTY".Translate(), OnInviteParty);
			if (farmPersistentGameData.Online.IsHost)
			{
				friendSelector = pauseLayout.AddSelector(pauseGroup, "FriendPermissions", "FRIEND_PERMISSIONS".Translate(), loop: true);
				publicSelector = pauseLayout.AddSelector(pauseGroup, "PublicPermissions", "PUBLIC_PERMISSIONS".Translate(), loop: true);
				fillSelectors();
			}
		}
		pauseLayout.AddButton(pauseGroup, "Exit", "EXIT".Translate(), OnExit);
		pauseLayout.AddControl(pauseGroup);
		pauseGroup.ActiveControl = activeControl;
		ButtonInstructions buttonInstructions = new ButtonInstructions(pauseLayout);
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Interact, "OK".Translate());
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
		pauseLayout.AddControl(buttonInstructions);
		pauseLayout.OnCancel += OnResume;
		shopLayout = new Layout("Shop", "", AvatarFarmOnline.Template.ExtendedGameTemplate.Template);
		shop = new AvatarFarmOnline.Template.Controls.Shop(stage.FarmData, shopLayout);
		shop.OnSelect += shop_OnSelect;
		shop.OnCancel += shop_OnCancel;
		shopLayout.AddControl(shop);
		buttonInstructions = new ButtonInstructions(pauseLayout);
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Interact, "PURCHASE".Translate());
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
		shopLayout.AddControl(buttonInstructions);
		XACTJukebox.Instance.StartSonglist("Music");
		stage.OnShowShop += stage_OnShowShop;
		stage.OnGameFinished += stage_OnGameFinished;
		stage.OnShowMessage += stage_OnShowMessage;
	}

	private void fillSelectors()
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		friendSelector.addOption(0, "FULL".Translate());
		friendSelector.addOption(1, "PLANT".Translate());
		friendSelector.addOption(2, "HARVEST".Translate());
		friendSelector.addOption(3, "GUEST".Translate());
		friendSelector.CurrentOption = (int)farmPersistentGameData.CurrentFarm.FriendPermissions;
		publicSelector.addOption(0, "FULL".Translate());
		publicSelector.addOption(1, "PLANT".Translate());
		publicSelector.addOption(2, "HARVEST".Translate());
		publicSelector.addOption(3, "GUEST".Translate());
		publicSelector.CurrentOption = (int)farmPersistentGameData.CurrentFarm.PublicPermissions;
	}

	private void OnInvite(Button b, PlayerIndex whoPressed)
	{
		if (!(GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData { IsOnline: not false } farmPersistentGameData) || !farmPersistentGameData.Online.LocalGamer.SignedInGamer.IsSignedInToService || PlatformInterface.Instance.IsGuideVisible)
		{
			return;
		}
		try
		{
			Guide.ShowGameInvite(farmPersistentGameData.Online.LocalGamer.SignedInGamer.PlayerIndex, null);
		}
		catch (Exception)
		{
		}
	}

	private void OnInviteParty(Button b, PlayerIndex whoPressed)
	{
		if (!(GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData { IsOnline: not false } farmPersistentGameData) || !farmPersistentGameData.Online.LocalGamer.SignedInGamer.IsSignedInToService || PlatformInterface.Instance.IsGuideVisible)
		{
			return;
		}
		try
		{
			if (InputManager.MenuBack())
			{
				farmPersistentGameData.Online.SendPartyInvites();
			}
		}
		catch (Exception)
		{
		}
	}

	private void stage_OnShowMessage(AvatarFarmOnline.Logic.Stage.Stage stage, string title, string message)
	{
		messageLayout.ShowDialog(title, message, DialogOptions.Ok, OnMessageConfirm);
	}

	private void OnMessageConfirm(DialogResult result, PlayerIndex whoPressed)
	{
		stage.MessageRead();
	}

	private void stage_OnShowShop(AvatarFarmOnline.Logic.Stage.Stage stage, bool goToPlantsCategory)
	{
		shop.InvokeEvent(goToPlantsCategory);
	}

	private void shop_OnCancel()
	{
		stage.EndShop(null);
	}

	private void shop_OnSelect(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition obj)
	{
		stage.EndShop(obj);
	}

	private void stage_OnGameFinished(AvatarFarmOnline.Logic.Stage.Stage obj)
	{
		XACTJukebox.Instance.Stop();
	}

	private void ShowTrialDialog()
	{
		pauseLayout.ShowDialog(LanguageManager.Texts["TRIAL_ENDED_TITLE"], string.Format(LanguageManager.Texts["TRIAL_ENDED_{0}_{1}"], InputManager.GetInputGlyph(InputManager.MenuInputCodes.Interact), InputManager.GetInputGlyph(InputManager.MenuInputCodes.Cancel)), DialogOptions.YesNo, 1000, OnTrialModeDialog);
	}

	private void OnTrialModeDialog(DialogResult result, PlayerIndex whoPressed)
	{
		if (result == DialogResult.OkYes)
		{
			PlatformInterface.Instance.TryBuy(whoPressed, pauseLayout);
		}
		else
		{
			ForceExit();
		}
	}

	private bool OnResume(Group group, PlayerIndex whoPressed)
	{
		Resume();
		return true;
	}

	private void Resume()
	{
		if (!trialCausedPause || !PlatformInterface.Instance.IsTrial)
		{
			trialCausedPause = false;
			InputManager.ClearForcedPlayerIndex();
			stage.Resume();
			if (GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData { IsOnline: not false } farmPersistentGameData && farmPersistentGameData.Online.IsHost)
			{
				farmPersistentGameData.CurrentFarm.SetPermissions(farmPersistentGameData.CurrentFarm.PlayMode, (AvatarFarmOnline.Logic.PlayerPermissions)friendSelector.CurrentOption, (AvatarFarmOnline.Logic.PlayerPermissions)publicSelector.CurrentOption);
				farmPersistentGameData.Online.SetPermissions((AvatarFarmOnline.Logic.PlayerPermissions)friendSelector.CurrentOption, (AvatarFarmOnline.Logic.PlayerPermissions)publicSelector.CurrentOption);
			}
		}
	}

	private bool OnResume(Layout layout, PlayerIndex whoPressed)
	{
		Resume();
		return true;
	}

	private void OnResume(Button buttonPressed, PlayerIndex whoPressed)
	{
		Resume();
	}

	public override void InitSection()
	{
		base.InitSection();
	}

	public void InitScenes()
	{
		stageScene = new AvatarFarmOnline.Scenes.StageScene(stage);
		AddScene(stageScene, isDefault: true);
		stageAfterScene = new AvatarFarmOnline.Scenes.StageAfterScene(stage, stageScene.GameCamera);
		AddScene(stageAfterScene, isDefault: false);
		HUDScene = new AvatarFarmOnline.Scenes.HUDScene(stage);
		HUDScene.StageCamera = stageScene.GameCamera;
		HUDScene.OnFinishConfirm += OnFinishConfirm;
		AddScene(HUDScene, isDefault: false);
		layoutScene = new LayoutScene(pauseLayout);
		AddScene(layoutScene, isDefault: false);
		messageLayoutScene = new LayoutScene(messageLayout);
		AddScene(messageLayoutScene, isDefault: false);
	}

	public void InitRenderProcesses()
	{
		base.initRenderProcesses();
	}

	protected override void initScenes()
	{
	}

	protected override void initRenderProcesses()
	{
		shopLayoutScene = new LayoutScene(shopLayout);
		AddScene(shopLayoutScene, isDefault: false);
		sceneRenderPass = AvatarFarmOnline.AvatarFarmOnlineGame.AntialiasPass;
		sceneRenderPass.Clear();
		sceneRenderPass.MustClearColor = true;
		sceneRenderPass.BackgroundColor = Color.White;
		sceneRenderPass.MustClearDepth = true;
		sceneRenderPass.addSource(stageScene);
		AddExtraRenderProcess(new RenderProcess(sceneRenderPass));
		fakeDOF = new FakeDOFRenderProcess(sceneRenderPass.RenderTarget);
		fakeDOF.Deviation = 3f;
		fakeDOF.BlurAmount = 2f;
		fakeDOF.BaseDistance = new Vector2(0.5f, 0.3f);
		fakeDOF.Aperture = new Vector2(3f);
		fakeDOF.SetRenderTarget(RenderPass2D.CreateRenderTarget(), setOwner: true);
		BloomRenderProcess bloomRenderProcess = new BloomRenderProcess(fakeDOF.RenderTarget);
		bloomRenderProcess.BloomThreshold = 0.65f;
		bloomRenderProcess.BloomMultiplier = 1.7f;
		bloomRenderProcess.Deviation = 3f;
		bloomRenderProcess.BlurAmount = 4f;
		bloomRenderProcess.SetRenderTarget(RenderPass2D.CreateRenderTarget(), setOwner: true);
		finalPP = new AvatarFarmOnline.Scenes.ANFinalScene(stageScene.Camera, sceneRenderPass.RenderTarget, stage);
		AddScene(finalPP, isDefault: false);
		mainRenderPass = new RenderPass2D(createRenderTarget: false);
		base.MainRenderPass.addSource(finalPP, SimpleSorter.Instance);
		base.MainRenderPass.addSource(stageAfterScene, SimpleSorter.Instance);
		base.MainRenderPass.addSource(HUDScene, SimpleSorter.Instance);
		base.MainRenderPass.addSource(layoutScene, SimpleSorter.Instance);
		base.MainRenderPass.addSource(messageLayoutScene, SimpleSorter.Instance);
		base.MainRenderPass.addSource(shopLayoutScene, SimpleSorter.Instance);
	}

	private void OnExit(Button buttonPressed, PlayerIndex whoPressed)
	{
		if (stage.FarmData.IsLocal)
		{
			pauseLayout.ShowDialog("EXIT".Translate(), (PlatformInterface.Instance.IsTrial ? "EXIT_TRIAL_PROMPT" : "EXIT_STAGE_PROMPT").Translate(), DialogOptions.YesNo, OnExitConfirm);
		}
		else
		{
			pauseLayout.ShowDialog("EXIT".Translate(), "EXIT_ONLINE_PROMPT".Translate(), DialogOptions.YesNo, OnExitConfirm2);
		}
	}

	private void OnExitConfirm(DialogResult result, PlayerIndex whoPressed)
	{
		if (result == DialogResult.OkYes)
		{
			int ticksToNextDeath = stage.FarmData.TicksToNextDeath;
			if (ticksToNextDeath > 0 && !PlatformInterface.Instance.IsTrial)
			{
				StringBuilder stringBuilder = new StringBuilder(64);
				AvatarFarmOnline.Logic.Parsing.SetTimeText((ticksToNextDeath + 59) / 60 * 60, stringBuilder);
				pauseLayout.ShowDialog("INFO".Translate(), string.Format("EXIT_TIME_REMINDER_{0}".Translate(), stringBuilder.ToString()), DialogOptions.Ok, OnExitConfirm2);
			}
			else
			{
				ForceExit();
			}
		}
	}

	private void OnExitConfirm2(DialogResult result, PlayerIndex whoPressed)
	{
		if (result == DialogResult.OkYes)
		{
			ForceExit();
		}
	}

	private void ForceExit()
	{
		InputManager.ClearForcedPlayerIndex();
		GameManager.CurrentGame.FinishStage();
		Finish(AvatarFarmOnline.Scenes.HUDScene.EFinishAction.Exit);
	}

	private void OnFinishConfirm(AvatarFarmOnline.Scenes.HUDScene.EFinishAction action)
	{
		Finish(action);
	}

	private void OnStageFinished(GameData data)
	{
		if (data is AvatarFarmOnline.Logic.Mode.Farm.FarmGameData)
		{
			XACTJukebox.Instance.Stop();
		}
		HUDScene.ShowResults();
	}

	private void Finish(AvatarFarmOnline.Scenes.HUDScene.EFinishAction action)
	{
		if (GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData { IsOnline: not false } farmPersistentGameData)
		{
			farmPersistentGameData.DisposeOnline();
		}
		BaseGame.Instance.NextGameSectionId = 0;
	}

	public override void MainLoop()
	{
		CompatHooks.ProcessStageCommandFile();
		HUDScene.Enabled = !stage.IsPaused;
		messageLayoutScene.Enabled = stage.IsShowingMessage;
		if (stage.IsShowingMessage)
		{
			messageLayout.Update();
		}
		fakeDOF.BaseDistance = new Vector2(fakeDOF.BaseDistance.X, GameMath.Damping(fakeDOF.BaseDistance.Y, stage.IsInPlayableState ? 0.3f : (-0.3f), 0.94f));
		fakeDOF.BlurAmount = GameMath.Damping(fakeDOF.BlurAmount, stage.IsInPlayableState ? 2f : 1f, 0.94f);
		switch (stage.StageState)
		{
		case StageState.Paused:
			XACTJukebox.Instance.Volume = 0.4f;
			if (guideCausedPause && !PlatformInterface.Instance.IsGuideVisible)
			{
				guideCausedPause = false;
				Resume();
			}
			else
			{
				pauseLayout.Update();
			}
			if (trialCausedPause && PlatformInterface.Instance.IsTrial && !pauseLayout.IsDialogShown)
			{
				ShowTrialDialog();
			}
			break;
		case StageState.Shop:
			shopLayout.Update();
			break;
		default:
		{
			XACTJukebox.Instance.Volume = 1f;
			PlayerIndex owner = stage.LocalPlayer.Owner;
			if (!Gamepad.Instance(owner).Connected)
			{
				guideCausedPause = false;
				stage.Pause();
			}
			if (InputManager.MenuStart(owner))
			{
				guideCausedPause = false;
				InputManager.SetForcedPlayerIndex(owner);
				stage.Pause();
			}
			if (PlatformInterface.Instance.IsGuideVisible)
			{
				guideCausedPause = true;
				stage.Pause();
			}
			break;
		}
		}
		layoutScene.Enabled = stage.IsPaused;
		shopLayoutScene.Enabled = stage.IsOnShop;
		shopLayout.Enabled = stage.IsOnShop;
	}

	public override void Dispose()
	{
		StorageManager.Instance.SaveEnabled = true;
		if (GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData && stage != null && stage.FarmData.IsLocal)
		{
			stage.FarmData.Save(now: true);
		}
		XACTJukebox.Instance.Volume = 1f;
		XACTJukebox.Instance.Stop();
		if (GameManager.CurrentGame != null)
		{
			GameManager.CurrentGame.OnStageFinished -= OnStageFinished;
		}
		if (stageScene != null)
		{
			stageScene.Dispose();
		}
		stageScene = null;
		if (HUDScene != null)
		{
			HUDScene.Dispose();
		}
		AvatarFarmOnline.AvatarFarmOnlineGame.AntialiasPass.Clear();
		HUDScene = null;
		stage = null;
		base.Dispose();
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
	}
}
