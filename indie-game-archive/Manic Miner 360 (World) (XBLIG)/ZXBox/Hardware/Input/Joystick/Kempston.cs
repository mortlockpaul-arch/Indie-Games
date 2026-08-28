using Microsoft.Xna.Framework.Input;
using ZXBox.Hardware.Interfaces;

namespace ZXBox.Hardware.Input.Joystick;

public class Kempston : IInput
{
	private GamePadState currentState;

	private bool _Enabled;

	public bool Enabled
	{
		get
		{
			return _Enabled;
		}
		set
		{
			_Enabled = value;
		}
	}

	public void UpdateState(GamePadState state)
	{
		currentState = state;
	}

	public int Input(int Port, int tact)
	{
		int num = 255;
		if ((Port & 0xFF) == 31)
		{
			num = 0;
			if (currentState.Buttons.A == ButtonState.Pressed)
			{
				num |= 0x10;
			}
			if (currentState.DPad.Up == ButtonState.Pressed)
			{
				num |= 8;
			}
			if (currentState.DPad.Down == ButtonState.Pressed)
			{
				num |= 4;
			}
			if (currentState.DPad.Left == ButtonState.Pressed)
			{
				num |= 2;
			}
			if (currentState.DPad.Right == ButtonState.Pressed)
			{
				num |= 1;
			}
		}
		return num;
	}
}
