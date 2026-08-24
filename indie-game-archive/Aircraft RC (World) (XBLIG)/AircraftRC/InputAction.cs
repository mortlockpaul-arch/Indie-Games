using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AircraftRC;

public class InputAction
{
	private delegate bool ButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex player);

	private readonly Buttons[] buttons;

	private readonly bool newPressOnly;

	public InputAction(Buttons[] buttons, bool newPressOnly)
	{
		this.buttons = ((buttons != null) ? (buttons.Clone() as Buttons[]) : new Buttons[0]);
		this.newPressOnly = newPressOnly;
	}

	public bool Evaluate(InputState state, PlayerIndex? controllingPlayer, out PlayerIndex player)
	{
		ButtonPress buttonPress = ((!newPressOnly) ? new ButtonPress(state.IsButtonPressed) : new ButtonPress(state.IsNewButtonPress));
		Buttons[] array = buttons;
		foreach (Buttons button in array)
		{
			if (buttonPress(button, controllingPlayer, out player))
			{
				return true;
			}
		}
		player = PlayerIndex.One;
		return false;
	}
}
