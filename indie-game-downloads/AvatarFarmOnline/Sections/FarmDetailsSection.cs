using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GUI.Controls.GroupControls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Sections;
using Quasar.Input;
using Quasar.Language;

namespace AvatarFarmOnline.Sections;

internal class FarmDetailsSection : GUISection
{
	private Selector modeSelector;

	private Selector friendSelector;

	private Selector publicSelector;

	private Selector simulationSpeedSelector;

	private Group group;

	private AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader currentFarm;

	public FarmDetailsSection(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader header)
		: base(24, new Layout("", LanguageManager.Texts["FARM_DETAILS_TITLE"], AvatarFarmOnline.Template.ExtendedGameTemplate.Template))
	{
		currentFarm = header;
		group = new Group(base.Layout);
		group.SetId("MainMenuGroup");
		modeSelector = base.Layout.AddSelector(group, "Mode", "MODE".Translate(), loop: true);
		modeSelector.OnInteraction += OnInteraction;
		simulationSpeedSelector = base.Layout.AddSelector(group, "SimulationSpeed", "SIMULATION_SPEED".Translate(), loop: false);
		simulationSpeedSelector.OnInteraction += OnInteraction;
		friendSelector = base.Layout.AddSelector(group, "FriendPermissions", "FRIEND_PERMISSIONS".Translate(), loop: true);
		friendSelector.OnInteraction += OnInteraction;
		publicSelector = base.Layout.AddSelector(group, "PublicPermissions", "PUBLIC_PERMISSIONS".Translate(), loop: true);
		publicSelector.OnInteraction += OnInteraction;
		fillSelectors();
		group.OnCancel += GroupCancel;
		base.Layout.AddControl(group);
		ButtonInstructions buttonInstructions = new ButtonInstructions(base.Layout);
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Interact, "OK".Translate());
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
		base.Layout.AddControl(buttonInstructions);
		base.Layout.OnCancel += LayoutCancel;
	}

	private void fillSelectors()
	{
		modeSelector.addOption(0, "OFFLINE".Translate());
		modeSelector.addOption(1, "PRIVATE".Translate());
		modeSelector.addOption(2, "PUBLIC".Translate());
		modeSelector.OnChange += modeSelector_OnChange;
		modeSelector.CurrentOption = (int)currentFarm.PlayMode;
		friendSelector.addOption(0, "FULL".Translate());
		friendSelector.addOption(1, "PLANT".Translate());
		friendSelector.addOption(2, "HARVEST".Translate());
		friendSelector.addOption(3, "GUEST".Translate());
		friendSelector.CurrentOption = (int)currentFarm.FriendPermissions;
		publicSelector.addOption(0, "FULL".Translate());
		publicSelector.addOption(1, "PLANT".Translate());
		publicSelector.addOption(2, "HARVEST".Translate());
		publicSelector.addOption(3, "GUEST".Translate());
		publicSelector.CurrentOption = (int)currentFarm.PublicPermissions;
		modeSelector_OnChange(modeSelector, modeSelector.CurrentOption, modeSelector.CurrentOptionValue);
		simulationSpeedSelector.addOption(0, "0%");
		simulationSpeedSelector.addOption(1, "25%");
		simulationSpeedSelector.addOption(2, "50%");
		simulationSpeedSelector.addOption(3, "75%");
		simulationSpeedSelector.addOption(4, "100%");
		simulationSpeedSelector.CurrentOption = (int)currentFarm.SimulationSpeed;
	}

	private void modeSelector_OnChange(Selector selectorChanged, int selection, string value)
	{
		group.removeControl(friendSelector);
		group.removeControl(publicSelector);
		switch ((PlayMode)modeSelector.CurrentOption)
		{
		case PlayMode.Public:
			group.addControl(friendSelector);
			group.addControl(publicSelector);
			break;
		case PlayMode.Private:
			group.addControl(friendSelector);
			group.addControl(publicSelector);
			break;
		case PlayMode.Local:
			break;
		}
	}

	private void OnInteraction(Button button, PlayerIndex whoPressed)
	{
		Back();
	}

	public bool LayoutCancel(Layout layout, PlayerIndex whoPressed)
	{
		Back();
		return true;
	}

	public bool GroupCancel(Group group, PlayerIndex whoPressed)
	{
		Back();
		return true;
	}

	private void Back()
	{
		currentFarm.SetPermissions((PlayMode)modeSelector.CurrentOption, (AvatarFarmOnline.Logic.PlayerPermissions)friendSelector.CurrentOption, (AvatarFarmOnline.Logic.PlayerPermissions)publicSelector.CurrentOption);
		currentFarm.SimulationSpeed = (AvatarFarmOnline.Logic.SimulationSpeeds)simulationSpeedSelector.CurrentOption;
		currentFarm.Manager.Save();
		BaseGame.Instance.NextGameSection = new AvatarFarmOnline.Sections.FarmListSection(currentFarm);
	}

	public override void MainLoop()
	{
		Player.SetPresence(GamerPresenceMode.AtMenu);
		base.MainLoop();
	}
}
