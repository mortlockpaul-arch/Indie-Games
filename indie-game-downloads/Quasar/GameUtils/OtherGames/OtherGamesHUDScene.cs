using System.Collections.Generic;
using Quasar.GameUtils.Template;
using Quasar.Input;
using Quasar.Language;
using Quasar.Scenes;

namespace Quasar.GameUtils.OtherGames;

internal class OtherGamesHUDScene : Scene2D
{
	private OtherGames otherGames;

	private ButtonInstructionsNode instructions;

	private List<KeyValuePair<InputManager.MenuInputCodes, string>> instructionsFront;

	private List<KeyValuePair<InputManager.MenuInputCodes, string>> instructionsBack;

	private bool lastState;

	public OtherGamesHUDScene(OtherGames otherGames)
	{
		this.otherGames = otherGames;
		instructions = new ButtonInstructionsNode();
		Add(instructions);
		instructionsFront = new List<KeyValuePair<InputManager.MenuInputCodes, string>>(2);
		instructionsFront.Add(new KeyValuePair<InputManager.MenuInputCodes, string>(InputManager.MenuInputCodes.Interact, "OTHER_GAMES_SHOW_DETAILS".Translate()));
		instructionsFront.Add(new KeyValuePair<InputManager.MenuInputCodes, string>(InputManager.MenuInputCodes.Cancel, "BACK".Translate()));
		instructionsBack = new List<KeyValuePair<InputManager.MenuInputCodes, string>>(2);
		instructionsBack.Add(new KeyValuePair<InputManager.MenuInputCodes, string>(InputManager.MenuInputCodes.Interact, "OTHER_GAMES_ZOOM".Translate()));
		instructionsBack.Add(new KeyValuePair<InputManager.MenuInputCodes, string>(InputManager.MenuInputCodes.Cancel, "BACK".Translate()));
		instructions.SetInstructions(otherGames.IsShowingFront ? instructionsFront : instructionsBack);
		lastState = otherGames.IsShowingFront;
	}

	public override void Update()
	{
		if (lastState != otherGames.IsShowingFront)
		{
			instructions.SetInstructions(otherGames.IsShowingFront ? instructionsFront : instructionsBack);
			lastState = otherGames.IsShowingFront;
		}
		base.Update();
	}
}
