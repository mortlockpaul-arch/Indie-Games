using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Input;

public class GamepadAxis2Input : BaseInput<Axis2InputState>
{
	private Stick stick;

	private Gamepad.DeadZoneSetting? deadZoneSetting;

	public GamepadAxis2Input(Stick stick)
	{
		this.stick = stick;
	}

	public void SetDeadZone(Gamepad.DeadZoneSetting setting)
	{
		deadZoneSetting = setting;
	}

	public void Update(List<PlayerIndex> playerIndices, ref Axis2InputState result)
	{
		if (deadZoneSetting.HasValue)
		{
			Gamepad.PushDeadZone(playerIndices, deadZoneSetting.Value);
		}
		result.Value.Value += Gamepad.SumStickPositions(playerIndices, stick, ref result.Value.Player);
		if (deadZoneSetting.HasValue)
		{
			Gamepad.PopDeadZone(playerIndices);
		}
	}

	public void Update(PlayerIndex playerIndex, ref Axis2InputState result)
	{
		Gamepad gamepad = Gamepad.Instance(playerIndex);
		if (deadZoneSetting.HasValue)
		{
			gamepad.PushDeadZone(deadZoneSetting.Value);
		}
		Vector2 vector = gamepad.StickPosition(stick);
		if (deadZoneSetting.HasValue)
		{
			gamepad.PopDeadZone();
		}
		result.Value.Value += vector;
		if (!GameMath.ZeroLength(vector))
		{
			result.Value.Player = playerIndex;
		}
	}
}
