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

public class StatsSection : GUISection
{
	private const string MainGroup = "MainGroup";

	private Quasar.GameUtils.Template.Controls.Stats stats;

	private ButtonInstructions instructions;

	private int previousSectionId;

	public StatsSection(PlayerIndex playerIndex, int previousSectionId, string template)
		: base(7, new Layout("", LanguageManager.Texts["STATS_TITLE"], template))
	{
		this.previousSectionId = previousSectionId;
		base.Layout.OnCancel += LayoutCancel;
		stats = new Quasar.GameUtils.Template.Controls.Stats(10, base.Layout, playerIndex);
		stats.OnCancel += StatsCancel;
		stats.OnChange += st_OnChange;
		base.Layout.AddControl(stats);
		instructions = new ButtonInstructions(base.Layout);
		instructions.AddInstructions(InputManager.MenuInputCodes.Interact, "NEXT_PAGE".Translate());
		instructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
		base.Layout.AddControl(instructions);
		st_OnChange();
	}

	private void st_OnChange()
	{
		instructions.ClearInstructions();
		if (stats.CurrentPage < stats.NumPages - 1)
		{
			instructions.AddInstructions(InputManager.MenuInputCodes.Interact, "NEXT_PAGE".Translate());
		}
		instructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
	}

	private bool StatsCancel()
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
		Quasar.GameUtils.Player.Player.SetPresence(GamerPresenceMode.WastingTime);
		base.MainLoop();
	}
}
