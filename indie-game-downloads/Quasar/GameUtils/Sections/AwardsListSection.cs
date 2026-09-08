using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Template.Controls;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace Quasar.GameUtils.Sections;

public class AwardsListSection : GUISection
{
	private const string MainGroup = "MainGroup";

	private int previousSectionId;

	public AwardsListSection(PlayerIndex playerIndex, int previousSectionId, string template)
		: base(5, new Layout("", LanguageManager.Texts["AWARDS_TITLE"], template))
	{
		this.previousSectionId = previousSectionId;
		base.Layout.OnCancel += LayoutCancel;
		Quasar.GameUtils.Template.Controls.Awards awards = new Quasar.GameUtils.Template.Controls.Awards(3, base.Layout, playerIndex);
		awards.OnCancel += AwardsCancel;
		base.Layout.AddControl(awards);
		ButtonInstructions buttonInstructions = new ButtonInstructions(base.Layout);
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Interact, "NEXT_PAGE".Translate());
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
		base.Layout.AddControl(buttonInstructions);
	}

	private bool AwardsCancel()
	{
		Back();
		return true;
	}

	public bool LayoutCancel(Layout layout, PlayerIndex whoPressed)
	{
		Back();
		return true;
	}

	private void Back()
	{
		((BaseGame)Engine.Game).NextGameSectionId = previousSectionId;
	}

	public override void MainLoop()
	{
		Quasar.GameUtils.Player.Player.SetPresence(GamerPresenceMode.FoundSecret);
		base.MainLoop();
	}
}
