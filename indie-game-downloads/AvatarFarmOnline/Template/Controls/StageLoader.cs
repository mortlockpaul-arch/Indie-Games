using System;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Sections;
using Quasar.GUI;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Template.Controls;
using Quasar.Global;

namespace AvatarFarmOnline.Template.Controls;

internal class StageLoader : Loader
{
	public new const string Type = "StageLoader";

	private AvatarFarmOnline.Sections.StageSection section;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	public override string ControlType => "StageLoader";

	public AvatarFarmOnline.Sections.StageSection LoadedSection => section;

	public AvatarFarmOnline.Logic.Stage.Stage LoadedStage => stage;

	public StageLoader(Layout layout)
		: base(layout)
	{
	}

	public override void startLoading()
	{
		try
		{
			GameManager.PrepareNextRound();
		}
		catch (Exception)
		{
			invokeLoadError();
		}
		base.startLoading();
	}

	protected override void Load()
	{
		((BaseGame)Engine.Game).WaitForPreloadFinish(stopPreload: false);
		stage = GameManager.CurrentGame.LoadStage() as AvatarFarmOnline.Logic.Stage.Stage;
		section = new AvatarFarmOnline.Sections.StageSection(stage);
		section.InitScenes();
	}

	protected override void PostLoad()
	{
		GameManager.CurrentGame.StartGame();
	}
}
