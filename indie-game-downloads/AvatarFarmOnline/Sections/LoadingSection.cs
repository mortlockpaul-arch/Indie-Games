using System;
using AvatarFarmOnline.Items;
using AvatarFarmOnline.Template;
using AvatarFarmOnline.Template.Controls;
using Quasar;
using Quasar.GUI;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Sections;
using Quasar.GameUtils.Template.Controls;
using Quasar.Global;
using Quasar.Language;
using Quasar.Scenes;

namespace AvatarFarmOnline.Sections;

internal class LoadingSection : GUISection
{
	private AvatarFarmOnline.Template.Controls.StageLoader loader;

	private Scene loadingScene;

	private AvatarFarmOnline.Items.LoadingItem loadingItem;

	public LoadingSection()
		: base(12, new Layout("", "", AvatarFarmOnline.Template.ExtendedGameTemplate.Template))
	{
		loader = new AvatarFarmOnline.Template.Controls.StageLoader(base.Layout);
		loader.LoadFinished += OnFinishLoad;
		loader.LoadError += OnLoadError;
		base.Layout.AddControl(loader);
	}

	protected override void initRenderProcesses()
	{
		base.initRenderProcesses();
		mainRenderPass.addSource(loadingScene);
	}

	private void OnLoadError()
	{
		AvatarFarmOnline.Sections.MainMenuSection mainMenuSection = new AvatarFarmOnline.Sections.MainMenuSection();
		mainMenuSection.Layout.ShowMessage("FARM_LOAD_ERROR_TITLE".Translate(), "FARM_LOAD_ERROR".Translate());
		BaseGame.Instance.NextGameSection = mainMenuSection;
	}

	private void OnFinishLoad()
	{
		((BaseGame)Engine.Game).NextGameSection = loader.LoadedSection;
	}

	protected override void initScenes()
	{
		loadingItem = new AvatarFarmOnline.Items.LoadingItem();
		loadingScene = new Scene2D();
		loadingScene.Add(loadingItem);
		AddScene(loadingScene, isDefault: true);
		base.initScenes();
		loader.startLoading();
	}

	public override void MainLoop()
	{
		Player.SetPresence(GamerPresenceMode.StartingGame);
		base.MainLoop();
	}

	public override void Dispose()
	{
		if (loader.LoadState == ELoadState.Loading || loader.LoadState == ELoadState.Error)
		{
			try
			{
				if (loader.LoadedSection != null)
				{
					((Section)loader.LoadedSection).Dispose();
				}
			}
			catch (Exception)
			{
			}
		}
		base.Dispose();
	}
}
