using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Quasar.Input;

public class DefaultXboxGamepadGroup : InputGroup
{
	public enum InputCodes
	{
		ButtonA,
		ButtonB,
		ButtonX,
		ButtonY,
		Start,
		Back,
		LeftTrigger,
		RightTrigger,
		LeftShoulder,
		RightShoulder,
		Movement,
		Aim,
		LeftStick,
		RightStick
	}

	public const int BaseCharGlyph = 1000;

	private GamepadAxis2Input leftStickAction;

	private GamepadAxis2Input rightStickAction;

	public static char GetInputGlyph(InputCodes code)
	{
		return (char)(1000 + code);
	}

	public void SetDeadZone(Gamepad.DeadZoneSetting setting)
	{
		leftStickAction.SetDeadZone(setting);
		rightStickAction.SetDeadZone(setting);
	}

	public DefaultXboxGamepadGroup(int id, PlayerIndex playerIndex)
		: base(id)
	{
		CreateInputList(0, playerIndex, new GamepadButtonInput(Buttons.A));
		base.BaseGlyph = 1000;
		leftStickAction = new GamepadAxis2Input(Stick.LeftStick);
		CreateInputList(10, playerIndex, leftStickAction);
		AddInput(10, new GamepadButtonAxis2Input(Buttons.DPadLeft, Buttons.DPadRight, Buttons.DPadDown, Buttons.DPadUp));
		rightStickAction = new GamepadAxis2Input(Stick.RightStick);
		CreateInputList(11, playerIndex, rightStickAction);
		CreateInputList(1, playerIndex, new GamepadButtonInput(Buttons.B));
		CreateInputList(2, playerIndex, new GamepadButtonInput(Buttons.X));
		CreateInputList(3, playerIndex, new GamepadButtonInput(Buttons.Y));
		CreateInputList(8, playerIndex, new GamepadButtonInput(Buttons.LeftShoulder));
		CreateInputList(9, playerIndex, new GamepadButtonInput(Buttons.RightShoulder));
		CreateInputList(6, playerIndex, new GamepadTriggerInput(Trigger.LeftTrigger));
		CreateInputList(7, playerIndex, new GamepadTriggerInput(Trigger.RightTrigger));
		CreateInputList(4, playerIndex, new GamepadButtonInput(Buttons.Start));
		CreateInputList(5, playerIndex, new GamepadButtonInput(Buttons.Back));
		CreateInputList(12, playerIndex, new GamepadButtonInput(Buttons.LeftStick));
		CreateInputList(13, playerIndex, new GamepadButtonInput(Buttons.RightStick));
	}

	public Vector2 Movement()
	{
		return CompatHooks.GetCompatMovement();
	}

	public Vector2 Aim()
	{
		return CompatHooks.GetCompatAim();
	}

	public bool Up()
	{
		return GetInputState<Axis2InputState>(10).PressedUp;
	}

	public bool UpState()
	{
		return GetInputState<Axis2InputState>(10).Up;
	}

	public bool UpRepeat()
	{
		return GetInputState<Axis2InputState>(10).RepeatUp;
	}

	public bool Down()
	{
		return GetInputState<Axis2InputState>(10).PressedDown;
	}

	public bool DownState()
	{
		return GetInputState<Axis2InputState>(10).Down;
	}

	public bool DownRepeat()
	{
		return GetInputState<Axis2InputState>(10).RepeatDown;
	}

	public bool Left()
	{
		return GetInputState<Axis2InputState>(10).PressedLeft;
	}

	public bool LeftState()
	{
		return GetInputState<Axis2InputState>(10).Left;
	}

	public bool LeftRepeat()
	{
		return GetInputState<Axis2InputState>(10).RepeatLeft;
	}

	public bool Right()
	{
		return GetInputState<Axis2InputState>(10).PressedRight;
	}

	public bool RightState()
	{
		return GetInputState<Axis2InputState>(10).Right;
	}

	public bool RightRepeat()
	{
		return GetInputState<Axis2InputState>(10).RepeatRight;
	}

	public bool ButtonA()
	{
		return IsPressed(0);
	}

	public bool ButtonAState()
	{
		return GetInputState<ButtonInputState>(0).Value.Value;
	}

	public bool ButtonAReleased()
	{
		return GetInputState<ButtonInputState>(0).Released;
	}

	public bool ButtonARepeat()
	{
		return GetInputState<ButtonInputState>(0).Repeat;
	}

	public bool ButtonB()
	{
		return IsPressed(1);
	}

	public bool ButtonBState()
	{
		return GetInputState<ButtonInputState>(1).Value.Value;
	}

	public bool ButtonBReleased()
	{
		return GetInputState<ButtonInputState>(1).Released;
	}

	public bool ButtonBRepeat()
	{
		return GetInputState<ButtonInputState>(1).Repeat;
	}

	public bool ButtonX()
	{
		return IsPressed(2);
	}

	public bool ButtonXState()
	{
		return GetInputState<ButtonInputState>(2).Value.Value;
	}

	public bool ButtonXReleased()
	{
		return GetInputState<ButtonInputState>(2).Released;
	}

	public bool ButtonXRepeat()
	{
		return GetInputState<ButtonInputState>(2).Repeat;
	}

	public bool ButtonY()
	{
		return IsPressed(3);
	}

	public bool ButtonYState()
	{
		return GetInputState<ButtonInputState>(3).Value.Value;
	}

	public bool ButtonYReleased()
	{
		return GetInputState<ButtonInputState>(3).Released;
	}

	public bool ButtonYRepeat()
	{
		return GetInputState<ButtonInputState>(3).Repeat;
	}

	public bool LeftShoulder()
	{
		return IsPressed(8);
	}

	public bool LeftShoulderState()
	{
		return GetInputState<ButtonInputState>(8).Value.Value;
	}

	public bool LeftShoulderReleased()
	{
		return GetInputState<ButtonInputState>(8).Released;
	}

	public bool LeftShoulderRepeat()
	{
		return GetInputState<ButtonInputState>(8).Repeat;
	}

	public bool RightShoulder()
	{
		return IsPressed(9);
	}

	public bool RightShoulderState()
	{
		return GetInputState<ButtonInputState>(9).Value.Value;
	}

	public bool RightShoulderReleased()
	{
		return GetInputState<ButtonInputState>(9).Released;
	}

	public bool RightShoulderRepeat()
	{
		return GetInputState<ButtonInputState>(9).Repeat;
	}

	public bool Start()
	{
		return IsPressed(4);
	}

	public bool StartState()
	{
		return GetInputState<ButtonInputState>(4).Value.Value;
	}

	public bool StartReleased()
	{
		return GetInputState<ButtonInputState>(4).Released;
	}

	public bool StartRepeat()
	{
		return GetInputState<ButtonInputState>(4).Repeat;
	}

	public bool Back()
	{
		return IsPressed(5);
	}

	public bool BackState()
	{
		return GetInputState<ButtonInputState>(5).Value.Value;
	}

	public bool BackReleased()
	{
		return GetInputState<ButtonInputState>(5).Released;
	}

	public bool BackRepeat()
	{
		return GetInputState<ButtonInputState>(5).Repeat;
	}

	public bool LeftTrigger()
	{
		return GetInputState<TriggerInputState>(6).Pressed;
	}

	public bool LeftTriggerReleased()
	{
		return GetInputState<TriggerInputState>(6).Released;
	}

	public bool LeftTriggerRepeat()
	{
		return GetInputState<TriggerInputState>(6).Repeat;
	}

	public bool LeftTriggerState()
	{
		return GetInputState<TriggerInputState>(6).Triggered;
	}

	public bool RightTrigger()
	{
		return GetInputState<TriggerInputState>(7).Pressed;
	}

	public bool RightTriggerReleased()
	{
		return GetInputState<TriggerInputState>(7).Released;
	}

	public bool RightTriggerRepeat()
	{
		return GetInputState<TriggerInputState>(7).Repeat;
	}

	public bool RightTriggerState()
	{
		return GetInputState<TriggerInputState>(7).Triggered;
	}

	public bool LeftStickButton()
	{
		return IsPressed(12);
	}

	public bool LeftStickButtonState()
	{
		return GetInputState<ButtonInputState>(12).Value.Value;
	}

	public bool LeftStickButtonReleased()
	{
		return GetInputState<ButtonInputState>(12).Released;
	}

	public bool RightStickButton()
	{
		return IsPressed(13);
	}

	public bool RightStickButtonState()
	{
		return GetInputState<ButtonInputState>(13).Value.Value;
	}

	public bool RightStickButtonReleased()
	{
		return GetInputState<ButtonInputState>(13).Released;
	}
}
