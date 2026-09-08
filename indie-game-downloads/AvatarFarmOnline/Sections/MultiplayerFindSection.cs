using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Logic.Mode.Farm.Online;
using AvatarFarmOnline.Template;
using AvatarFarmOnline.Template.Controls;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Sections;
using Quasar.Input;
using Quasar.Language;

namespace AvatarFarmOnline.Sections;

internal class MultiplayerFindSection : GUISection
{
	private AvatarFarmOnline.Template.Controls.FindGames findGames;

	public MultiplayerFindSection()
		: base(25, new Layout("FindGames", LanguageManager.Texts["FIND_GAMES_TITLE"], AvatarFarmOnline.Template.ExtendedGameTemplate.Template))
	{
		findGames = new AvatarFarmOnline.Template.Controls.FindGames(base.Layout);
		findGames.OnJoined += OnJoined;
		findGames.OnCancel += OnCancel;
		base.Layout.AddControl(findGames);
		ButtonInstructions buttonInstructions = new ButtonInstructions(base.Layout);
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Secondary, "SEARCH_AGAIN".Translate());
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Interact, "JOIN".Translate());
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
		base.Layout.AddControl(buttonInstructions);
		base.Layout.OnCancel += LayoutCancel;
	}

	public bool LayoutCancel(Layout layout, PlayerIndex whoPressed)
	{
		Back();
		return true;
	}

	private void OnJoined(AvatarFarmOnline.Template.Controls.FindGames findGames)
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		if (farmPersistentGameData.Online.MultiplayerState == AvatarFarmOnline.Logic.Mode.Farm.Online.Online.MultiplayerStates.Loading)
		{
			BaseGame.Instance.NextGameSectionId = 12;
		}
		else
		{
			BaseGame.Instance.NextGameSection = new AvatarFarmOnline.Sections.ReceivingDataSection();
		}
	}

	private void OnCancel(AvatarFarmOnline.Template.Controls.FindGames findGames, PlayerIndex whoPressed)
	{
		Back();
	}

	private void Back()
	{
		switch (findGames.State)
		{
		case AvatarFarmOnline.Template.Controls.FindGames.FindGamesState.Found:
		case AvatarFarmOnline.Template.Controls.FindGames.FindGamesState.ErrorJoining:
		case AvatarFarmOnline.Template.Controls.FindGames.FindGamesState.NotFound:
			BaseGame.Instance.NextGameSectionId = 0;
			break;
		default:
			base.Layout.ShowMessage("WARNING".Translate(), "SEARCH_IN_PROGRESS".Translate());
			break;
		}
	}

	public override void MainLoop()
	{
		Player.SetPresence(GamerPresenceMode.AtMenu);
		base.MainLoop();
	}
}
