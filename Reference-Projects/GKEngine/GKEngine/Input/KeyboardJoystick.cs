using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GKEngine.Input;

public class KeyboardJoystick
{
	public static float SPEED = 0.0001f;

	public static float SPEED_DECAY = 0.01f;

	public static float VALUE_DECAY = 0.05f;

	public static float MAX_SPEED = 0.1f;

	public bool active;

	public Vector2 value = new Vector2(0f, 0f);

	private Vector2 speed = new Vector2(0f, 0f);

	private bool decayXActive = true;

	private bool decayYActive = true;

	private Keys up;

	private Keys down;

	private Keys left;

	private Keys right;

	public KeyboardJoystick(Keys xUp, Keys xDown, Keys xLeft, Keys xRight)
	{
		up = xUp;
		down = xDown;
		left = xLeft;
		right = xRight;
	}

	public void Update(GameTime oGameTime)
	{
		float num = oGameTime.ElapsedGameTime.Milliseconds;
		decayXActive = false;
		if (UniversalInput.keyboardState.IsKeyDown(right))
		{
			speed.X = MathHelper.Clamp(SPEED * num + speed.X, 0f - MAX_SPEED, MAX_SPEED);
		}
		else if (UniversalInput.keyboardState.IsKeyDown(left))
		{
			speed.X = MathHelper.Clamp((0f - SPEED) * num + speed.X, 0f - MAX_SPEED, MAX_SPEED);
		}
		else
		{
			decayXActive = true;
			if (speed.X != 0f)
			{
				int num2 = Math.Sign(speed.X);
				speed.X += SPEED_DECAY * num * (float)(num2 * -1);
				if (num2 != Math.Sign(speed.X))
				{
					speed.X = 0f;
				}
			}
		}
		if (decayXActive)
		{
			int num3 = Math.Sign(value.X);
			value.X += VALUE_DECAY * num * (float)(num3 * -1);
			if (num3 != Math.Sign(value.X))
			{
				value.X = 0f;
			}
		}
		else
		{
			value.X = MathHelper.Clamp(value.X + speed.X * num, -1f, 1f);
		}
		decayYActive = false;
		if (UniversalInput.keyboardState.IsKeyDown(up))
		{
			speed.Y = MathHelper.Clamp(SPEED * num + speed.Y, 0f - MAX_SPEED, MAX_SPEED);
		}
		else if (UniversalInput.keyboardState.IsKeyDown(down))
		{
			speed.Y = MathHelper.Clamp((0f - SPEED) * num + speed.Y, 0f - MAX_SPEED, MAX_SPEED);
		}
		else
		{
			decayYActive = true;
			if (speed.Y != 0f)
			{
				int num4 = Math.Sign(speed.Y);
				speed.Y += SPEED_DECAY * num * (float)(num4 * -1);
				if (num4 != Math.Sign(speed.Y))
				{
					speed.Y = 0f;
				}
			}
		}
		if (decayYActive)
		{
			int num5 = Math.Sign(value.Y);
			value.Y += VALUE_DECAY * num * (float)(num5 * -1);
			if (num5 != Math.Sign(value.Y))
			{
				value.Y = 0f;
			}
		}
		else
		{
			value.Y = MathHelper.Clamp(value.Y + speed.Y * num, -1f, 1f);
		}
	}
}
