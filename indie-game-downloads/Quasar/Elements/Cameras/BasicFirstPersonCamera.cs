using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Quasar.Global;
using Quasar.Input;

namespace Quasar.Elements.Cameras;

public class BasicFirstPersonCamera : FirstPersonCamera
{
	private int playerIndex = -1;

	private float speed = 1f;

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
		}
	}

	public BasicFirstPersonCamera(int playerIndex)
	{
		this.playerIndex = playerIndex;
	}

	public BasicFirstPersonCamera()
	{
	}

	protected override void DoUpdate()
	{
		float num = speed;
		if (Quasar.Input.Keyboard.Instance.KeyState(Keys.LeftShift))
		{
			num *= 50f;
		}
		if (Quasar.Input.Keyboard.Instance.KeyState(Keys.LeftControl))
		{
			num *= 0.05f;
		}
		if (playerIndex != -1)
		{
			Gamepad gamepad = Gamepad.Instance((PlayerIndex)playerIndex);
			Vector2 vector = gamepad.StickPosition(Stick.RightStick);
			Vector2 vector2 = gamepad.StickPosition(Stick.LeftStick);
			heading -= vector.X * 0.02f;
			pitch += vector.Y * 0.02f;
			roll += (gamepad.TriggerPosition(Trigger.LeftTrigger) - gamepad.TriggerPosition(Trigger.RightTrigger)) * 0.02f;
			transform.Translation -= transform.ZVector * num * Timer.DefaultTimer.LastInterval * vector2.Y / 500f;
			transform.Translation -= -transform.XVector * num * Timer.DefaultTimer.LastInterval * vector2.X / 500f;
		}
		else
		{
			Vector2 vector3 = Gamepad.SumStickPositions(Stick.RightStick);
			Vector2 vector4 = Gamepad.SumStickPositions(Stick.LeftStick);
			heading -= vector3.X * 0.02f;
			pitch += vector3.Y * 0.02f;
			roll += (Gamepad.SumTriggerPositions(Trigger.LeftTrigger) - Gamepad.SumTriggerPositions(Trigger.RightTrigger)) * 0.02f;
			transform.Translation -= transform.ZVector * Timer.DefaultTimer.LastInterval * vector4.Y / 500f;
			transform.Translation -= -transform.XVector * Timer.DefaultTimer.LastInterval * vector4.X / 500f;
		}
		if (Quasar.Input.Keyboard.Instance.KeyState(Keys.W))
		{
			transform.Translation -= transform.ZVector * Timer.DefaultTimer.LastInterval * num / 500f;
		}
		if (Quasar.Input.Keyboard.Instance.KeyState(Keys.S))
		{
			transform.Translation += transform.ZVector * Timer.DefaultTimer.LastInterval * num / 500f;
		}
		if (Quasar.Input.Keyboard.Instance.KeyState(Keys.A))
		{
			transform.Translation += -transform.XVector * Timer.DefaultTimer.LastInterval * num / 500f;
		}
		if (Quasar.Input.Keyboard.Instance.KeyState(Keys.D))
		{
			transform.Translation -= -transform.XVector * Timer.DefaultTimer.LastInterval * num / 500f;
		}
		base.DoUpdate();
	}
}
