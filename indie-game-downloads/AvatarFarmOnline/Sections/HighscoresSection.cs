using AvatarFarmOnline.Scores;
using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Sections;
using Quasar.GameUtils.Template.Controls;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace AvatarFarmOnline.Sections;

internal class HighscoresSection : GUISection
{
	private ProHighscores<AvatarFarmOnline.Scores.GameHighscore> highScores;

	public HighscoresSection()
		: base(22, new Layout("", LanguageManager.Texts["BEST_FARMERS"], AvatarFarmOnline.Template.ExtendedGameTemplate.Template))
	{
		highScores = new ProHighscores<AvatarFarmOnline.Scores.GameHighscore>(AvatarFarmOnline.Scores.GameScoreManager.Instance, base.Layout);
		highScores.OnCancel += highScores_OnCancel;
		base.Layout.AddControl(highScores);
		ButtonInstructions buttonInstructions = new ButtonInstructions(base.Layout);
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
		base.Layout.AddControl(buttonInstructions);
		base.Layout.OnCancel += OnCancel;
	}

	private void highScores_OnCancel(ProHighscores<AvatarFarmOnline.Scores.GameHighscore> arg1, PlayerIndex arg2)
	{
		GoBack();
	}

	private bool OnCancel(Layout layout, PlayerIndex whoPressed)
	{
		GoBack();
		return true;
	}

	private void GoBack()
	{
		((BaseGame)Engine.Game).NextGameSectionId = 14;
	}

	public override void MainLoop()
	{
		Player.SetPresence(GamerPresenceMode.AtMenu);
		base.MainLoop();
	}

	public override void Dispose()
	{
		base.Dispose();
	}
}
