using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Input;

public class GamepadAxisInput : BaseInput<AxisInputState>
{
	private Stick stick;

	private Axis axis;

	private Gamepad.DeadZoneSetting? deadZoneSetting;

	public GamepadAxisInput(Stick stick, Axis axis)
	{
		this.axis = axis;
		this.stick = stick;
	}

	public void SetDeadZone(Gamepad.DeadZoneSetting setting)
	{
		deadZoneSetting = setting;
	}

	public void Update(List<PlayerIndex> playerIndices, ref AxisInputState result)
	{
		if (deadZoneSetting.HasValue)
		{
			Gamepad.PushDeadZone(playerIndices, deadZoneSetting.Value);
		}
		Vector2 vector = Gamepad.SumStickPositions(playerIndices, stick, ref result.Value.Player);
		result.Value.Value += ((axis == Axis.X) ? vector.X : vector.Y);
		if (deadZoneSetting.HasValue)
		{
			Gamepad.PopDeadZone(playerIndices);
		}
	}

	public void Update(PlayerIndex playerIndex, ref AxisInputState result)
	{
		Gamepad gamepad = Gamepad.Instance(playerIndex);
		if (deadZoneSetting.HasValue)
		{
			gamepad.PushDeadZone(deadZoneSetting.Value);
		}
		Vector2 v = gamepad.StickPosition(stick);
		if (deadZoneSetting.HasValue)
		{
			gamepad.PopDeadZone();
		}
		result.Value.Value += ((axis == Axis.X) ? v.X : v.Y);
		if (!GameMath.ZeroLength(v))
		{
			result.Value.Player = playerIndex;
		}
	}
}
