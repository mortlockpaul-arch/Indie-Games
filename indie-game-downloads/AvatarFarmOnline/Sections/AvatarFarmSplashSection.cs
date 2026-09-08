using System;
using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GUI;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Sections;
using Quasar.GameUtils.Template;
using Quasar.GameUtils.Template.Controls;
using Quasar.Input;
using Quasar.Language;
using Quasar.Scenes;
using Quasar.Xml;

namespace AvatarFarmOnline.Sections;

internal class AvatarFarmSplashSection : GUISection
{
	private PressAToStart pressAToStart;

	private RenderItem logo;

	public AvatarFarmSplashSection()
		: base(3, new Layout("", "", AvatarFarmOnline.Template.ExtendedGameTemplate.Template))
	{
		GameManager.Clear();
		InputManager.ClearIndices();
		BaseGame.Instance.ClearStorageDevices();
		pressAToStart = new PressAToStart(base.Layout);
		pressAToStart.NeedsSignIn = true;
		pressAToStart.CheckStorage = true;
		pressAToStart.MandatoryStorage = true;
		pressAToStart.OnSelected += OnSelected;
		base.Layout.AddControl(pressAToStart);
		Layout obj = base.Layout;
		Func<Layout, PlayerIndex, bool> value = delegate(Layout l, PlayerIndex p)
		{
			PromptExit(p);
			return true;
		};
		obj.OnCancel += value;
	}

	private void PromptExit(PlayerIndex whoPressed)
	{
		base.Layout.ShowDialog(LanguageManager.Texts["EXIT"], LanguageManager.Texts["EXIT_PROMPT"], DialogOptions.YesNo, OnExitDialog);
	}

	private void OnExitDialog(DialogResult result, PlayerIndex whoPressed)
	{
		if (result == DialogResult.OkYes)
		{
			BaseGame.Instance.FinishGame();
		}
	}

	protected override void initRenderProcesses()
	{
		base.initRenderProcesses();
		Scene2D scene2D = new Scene2D();
		AddScene(scene2D, isDefault: false);
		logo = ModelLoader.LoadModelDefinition("Backgrounds/LogoModel");
		logo.Transform.Scale = new Vector3(2f);
		logo.Transform.Translation = new Vector3(0f, 100f, 0f);
		scene2D.Add(logo);
		base.MainRenderPass.insertSource(0, scene2D);
	}

	protected virtual void OnSelected(PlayerIndex obj)
	{
		GameTemplate.InteractionAudio.Start();
		AvatarFarmOnline.Logic.Mode.Farm.FarmGameSetup setup = new AvatarFarmOnline.Logic.Mode.Farm.FarmGameSetup(obj);
		GameManager.SetSetup(setup);
		GameManager.StartGame();
		InputManager.SetIndex(obj);
		BaseGame.Instance.NextGameSection = new AvatarFarmOnline.Sections.MainMenuSection();
	}

	public override void MainLoop()
	{
		base.MainLoop();
	}
}
