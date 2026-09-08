using System;
using Microsoft.Xna.Framework;
using Quasar.Input;

namespace Quasar.Elements.Cameras;

public class BasicOrbitCamera : OrbitCamera
{
	private int playerIndex = -1;

	public float Radius
	{
		get
		{
			return radius;
		}
		set
		{
			radius = Math.Max(float.Epsilon, value);
		}
	}

	public float Pitch
	{
		get
		{
			return pitch;
		}
		set
		{
			pitch = value;
		}
	}

	public float Heading
	{
		get
		{
			return heading;
		}
		set
		{
			heading = value;
		}
	}

	public BasicOrbitCamera(Transform targetMotion)
	{
		base.Target = targetMotion;
	}

	public BasicOrbitCamera(Transform targetMotion, int playerIndex)
		: this(targetMotion)
	{
		this.playerIndex = playerIndex;
	}

	protected override void DoUpdate()
	{
		if (playerIndex != -1)
		{
			Gamepad gamepad = Gamepad.Instance((PlayerIndex)playerIndex);
			Vector2 vector = gamepad.StickPosition(Stick.RightStick);
			gamepad.StickPosition(Stick.LeftStick);
			heading += vector.X * 0.02f;
			pitch -= vector.Y * 0.02f;
		}
		base.DoUpdate();
	}
}
