using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BureauNewPDA;

public class GamePadControl
{
	public enum direction
	{
		NotSet,
		N,
		S,
		E,
		W,
		NW,
		NE,
		SW,
		SE
	}

	public bool padAPressed;

	public bool padBPressed;

	public bool padYPressed;

	public bool padXPressed;

	public bool pressStart;

	public bool shoulderPadLeft;

	public bool shoulderPadRight;

	public direction joyLeftDirection;

	public direction joyRightDirection;

	public direction padDPadDirection;

	public direction anyDirection;

	public float joyLeftDirectionAmount;

	private bool padARelease;

	private bool padBRelease;

	private bool padYRelease;

	private bool padXRelease;

	private bool startRelease;

	private GamePadState myGamePadState;

	public bool vibrate;

	public float leftAmount;

	public float rightAmount;

	public float rightTrigger;

	public float leftTrigger;

	public Vector2 joyRightVector;

	public Vector2 joyBothVector;

	public PlayerIndex myPlayer;

	private double changeProgressJoy = 1.0;

	public TimeSpan changeFrameControl = TimeSpan.FromMilliseconds(175.0);

	public float vibrateA;

	public float vibrateB;

	public void initiate(GamePadState myState)
	{
		myGamePadState = myState;
	}

	public void turnOnVibrate()
	{
		vibrate = true;
		vibrateA = 0.5f;
		vibrateB = 0.8f;
	}

	public void turnOffVibrate()
	{
		vibrate = false;
		vibrateA = 0f;
		vibrateB = 0f;
	}

	public void getCurrentGamePad(GamePadButtons myPad, GamePadThumbSticks myStick, GamePadDPad myDPad, PlayerIndex player, GamePadTriggers myTriggers, double elapsedTime)
	{
		shoulderPadLeft = false;
		shoulderPadRight = false;
		myPlayer = player;
		padAPressed = false;
		padBPressed = false;
		padYPressed = false;
		padXPressed = false;
		pressStart = false;
		joyLeftDirection = direction.NotSet;
		joyRightDirection = direction.NotSet;
		padDPadDirection = direction.NotSet;
		anyDirection = direction.NotSet;
		rightTrigger = 0f;
		leftTrigger = 0f;
		rightAmount = 0f;
		if ((myPad.A == ButtonState.Released) & padARelease)
		{
			padAPressed = true;
			padARelease = false;
		}
		else if ((myPad.A == ButtonState.Pressed) & !padARelease)
		{
			padARelease = true;
		}
		if ((myPad.B == ButtonState.Released) & padBRelease)
		{
			padBPressed = true;
			padBRelease = false;
		}
		else if ((myPad.B == ButtonState.Pressed) & !padBRelease)
		{
			padBRelease = true;
		}
		if ((myPad.Y == ButtonState.Released) & padYRelease)
		{
			padYPressed = true;
			padYRelease = false;
		}
		else if ((myPad.Y == ButtonState.Pressed) & !padYRelease)
		{
			padYRelease = true;
		}
		if ((myPad.X == ButtonState.Released) & padXRelease)
		{
			padXPressed = true;
			padXRelease = false;
		}
		else if ((myPad.X == ButtonState.Pressed) & !padXRelease)
		{
			padXRelease = true;
		}
		if ((myPad.Start == ButtonState.Released) & startRelease)
		{
			pressStart = true;
			startRelease = false;
		}
		else if ((myPad.Start == ButtonState.Pressed) & !startRelease)
		{
			startRelease = true;
		}
		if (myTriggers.Right > 0f)
		{
			rightTrigger = myTriggers.Right;
		}
		if (myTriggers.Left > 0f)
		{
			leftTrigger = myTriggers.Left;
		}
		if (myPad.LeftShoulder == ButtonState.Pressed)
		{
			shoulderPadLeft = true;
		}
		if (myPad.RightShoulder == ButtonState.Pressed)
		{
			shoulderPadRight = true;
		}
		joyRightVector = myStick.Right;
		joyLeftDirection = getJoystickDirection(myStick.Left);
		joyRightDirection = getJoystickDirection(myStick.Right);
		padDPadDirection = getGamePad(myDPad);
		joyLeftDirectionAmount = myStick.Left.X;
		joyBothVector = Vector2.Zero;
		if (joyLeftDirection != direction.NotSet)
		{
			joyBothVector = myStick.Left;
			anyDirection = joyLeftDirection;
		}
		else if (joyRightDirection != direction.NotSet)
		{
			joyBothVector = myStick.Right;
			anyDirection = joyRightDirection;
		}
		else if (padDPadDirection != direction.NotSet)
		{
			anyDirection = padDPadDirection;
		}
	}

	private direction getGamePad(GamePadDPad myDPad)
	{
		if (myDPad.Left == ButtonState.Pressed)
		{
			return direction.W;
		}
		if (myDPad.Right == ButtonState.Pressed)
		{
			return direction.E;
		}
		if (myDPad.Up == ButtonState.Pressed)
		{
			return direction.N;
		}
		if (myDPad.Down == ButtonState.Pressed)
		{
			return direction.S;
		}
		return direction.NotSet;
	}

	private direction getJoystickDirection(Vector2 myVector)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		if ((double)myVector.X >= 0.2)
		{
			flag2 = true;
		}
		else if ((double)myVector.X <= -0.2)
		{
			flag = true;
		}
		if ((double)myVector.Y >= 0.2)
		{
			flag3 = true;
		}
		else if ((double)myVector.Y <= -0.2)
		{
			flag4 = true;
		}
		if (flag3)
		{
			if (flag2)
			{
				return direction.NE;
			}
			if (flag)
			{
				return direction.NW;
			}
			return direction.N;
		}
		if (flag4)
		{
			if (flag2)
			{
				return direction.SE;
			}
			if (flag)
			{
				return direction.SW;
			}
			return direction.S;
		}
		if (flag)
		{
			return direction.W;
		}
		if (flag2)
		{
			return direction.E;
		}
		return direction.NotSet;
	}
}
