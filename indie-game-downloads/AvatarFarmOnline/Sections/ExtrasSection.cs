using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GUI.Controls.GroupControls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.OtherGames;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Sections;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace AvatarFarmOnline.Sections;

internal class ExtrasSection : GUISection
{
	public ExtrasSection()
		: base(14, new Layout("", LanguageManager.Texts["EXTRAS_TITLE"], AvatarFarmOnline.Template.ExtendedGameTemplate.Template))
	{
		Group obj = new Group(base.Layout);
		obj.SetId("MainMenuGroup");
		base.Layout.AddButton(obj, "Settings", "SETTINGS".Translate(), SettingsButtonClick);
		base.Layout.AddButton(obj, "HighScores", "BEST_FARMERS".Translate(), HighScoresButtonClick);
		base.Layout.AddButton(obj, "Credits", "CREDITS".Translate(), CreditsButtonClick);
		base.Layout.AddButton(obj, "OtherGames", "OTHER_GAMES".Translate(), OtherGamesButtonClick);
		obj.OnCancel += GroupCancel;
		base.Layout.AddControl(obj);
		ButtonInstructions buttonInstructions = new ButtonInstructions(base.Layout);
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Interact, "OK".Translate());
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
		base.Layout.AddControl(buttonInstructions);
		base.Layout.OnCancel += LayoutCancel;
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

	public void BackButtonClick(Button button, PlayerIndex whoPressed)
	{
		Back();
	}

	private void Back()
	{
		BaseGame.Instance.NextGameSectionId = 0;
	}

	private void SettingsButtonClick(Button button, PlayerIndex whoPressed)
	{
		((BaseGame)Engine.Game).NextGameSectionId = 13;
	}

	private void HighScoresButtonClick(Button button, PlayerIndex whoPressed)
	{
		((BaseGame)Engine.Game).NextGameSection = new AvatarFarmOnline.Sections.HighscoresSection();
	}

	public void OtherGamesButtonClick(Button button, PlayerIndex whoPressed)
	{
		((BaseGame)Engine.Game).NextGameSection = new OtherGamesSection(base.Id, AvatarFarmOnline.AvatarFarmOnlineGame.AntialiasPass, "Avatar Farm Online");
	}

	public void CreditsButtonClick(Button button, PlayerIndex whoPressed)
	{
		((BaseGame)Engine.Game).NextGameSection = new CreditsSection(base.Id, AvatarFarmOnline.Template.ExtendedGameTemplate.Template);
	}

	public override void MainLoop()
	{
		Player.SetPresence(GamerPresenceMode.AtMenu);
		base.MainLoop();
	}
}
