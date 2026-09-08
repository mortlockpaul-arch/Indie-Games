using System;
using System.Collections.Generic;
using Loot.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Eyehook.Framework;

public class GamePadManager
{
	private static readonly float deadZone = 0.4f;

	private static readonly PlayerIndex[] playerIndexes = new PlayerIndex[4]
	{
		PlayerIndex.One,
		PlayerIndex.Two,
		PlayerIndex.Three,
		PlayerIndex.Four
	};

	public readonly PlayerIndex PlayerIndex;

	private GamePadState previousGamePadState;

	private GamePadState gamePadState;

	private TimeSpan vibrateTimer = TimeSpan.Zero;

	private bool isDisconnected = true;

	private static List<GamePadManager> managers = new List<GamePadManager>();

	private static readonly double diagTolerance = MathHelper.ToRadians(30f);

	private static readonly double nsewTolerance = MathHelper.ToRadians(60f);

	private static readonly double rad45 = MathHelper.ToRadians(45f);

	private static readonly double rad90 = MathHelper.ToRadians(90f);

	private static readonly double rad135 = MathHelper.ToRadians(135f);

	private static readonly double rad180 = MathHelper.ToRadians(180f);

	public GamePadState PreviousGamePadState => previousGamePadState;

	public GamePadState GamePadState => gamePadState;

	public GamePadDPad DPad => gamePadState.DPad;

	~GamePadManager()
	{
	}

	public GamePadManager(PlayerIndex playerIndex)
	{
		PlayerIndex = playerIndex;
		managers.Add(this);
		beginUpdate();
	}

	public void dispose()
	{
		managers.Remove(this);
		stopVibrating();
	}

	public bool IsVibrating()
	{
		return vibrateTimer > TimeSpan.Zero;
	}

	public void vibrate(TimeSpan duration, float leftMotor, float rightMotor)
	{
		vibrateTimer = duration;
		GamePad.SetVibration(PlayerIndex, leftMotor, rightMotor);
	}

	public void stopVibrating()
	{
		if (GamePad.SetVibration(PlayerIndex, 0f, 0f))
		{
			vibrateTimer = TimeSpan.Zero;
		}
		else
		{
			vibrateTimer = TimeSpan.FromMilliseconds(100.0);
		}
	}

	public GamePadDir get4Dir()
	{
		return get4Dir(GamePadState);
	}

	public GamePadDir get8Dir()
	{
		return get8Dir(gamePadState);
	}

	private static bool isNear(double value, double target, double tolerance)
	{
		return Math.Abs(target - value) <= tolerance / 2.0;
	}

	public static GamePadDir getDPad4Dir(GamePadState state)
	{
		if (state.DPad.Up == ButtonState.Pressed)
		{
			return GamePadDir.N;
		}
		if (state.DPad.Down == ButtonState.Pressed)
		{
			return GamePadDir.S;
		}
		if (state.DPad.Right == ButtonState.Pressed)
		{
			return GamePadDir.E;
		}
		return (state.DPad.Left == ButtonState.Pressed) ? GamePadDir.W : GamePadDir.NONE;
	}

	public static GamePadDir get4Dir(GamePadState state)
	{
		if ((double)new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y).Length() < (double)deadZone)
		{
			return getDPad4Dir(state);
		}
		double value = Math.Atan2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
		return (!isNear(value, 0.0, rad90)) ? (isNear(value, rad90, rad90) ? GamePadDir.E : ((!isNear(value, 0.0 - rad90, rad90)) ? GamePadDir.S : GamePadDir.W)) : GamePadDir.N;
	}

	public static GamePadDir getDPad8Dir(GamePadState state)
	{
		if (state.DPad.Up == ButtonState.Pressed && state.DPad.Right == ButtonState.Pressed)
		{
			return GamePadDir.NE;
		}
		if (state.DPad.Up == ButtonState.Pressed && state.DPad.Left == ButtonState.Pressed)
		{
			return GamePadDir.NW;
		}
		if (state.DPad.Down == ButtonState.Pressed && state.DPad.Right == ButtonState.Pressed)
		{
			return GamePadDir.SE;
		}
		if (state.DPad.Down == ButtonState.Pressed && state.DPad.Left == ButtonState.Pressed)
		{
			return GamePadDir.SW;
		}
		if (state.DPad.Up == ButtonState.Pressed)
		{
			return GamePadDir.N;
		}
		if (state.DPad.Down == ButtonState.Pressed)
		{
			return GamePadDir.S;
		}
		if (state.DPad.Right == ButtonState.Pressed)
		{
			return GamePadDir.E;
		}
		return (state.DPad.Left == ButtonState.Pressed) ? GamePadDir.W : GamePadDir.NONE;
	}

	public static GamePadDir get8Dir(GamePadState state)
	{
		if ((double)new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y).Length() < (double)deadZone)
		{
			return getDPad8Dir(state);
		}
		double value = Math.Atan2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
		return isNear(value, rad45, diagTolerance) ? GamePadDir.NE : (isNear(value, rad135, diagTolerance) ? GamePadDir.SE : (isNear(value, 0.0 - rad135, diagTolerance) ? GamePadDir.SW : (isNear(value, 0.0 - rad45, diagTolerance) ? GamePadDir.NW : ((!isNear(value, 0.0, nsewTolerance)) ? (isNear(value, rad90, nsewTolerance) ? GamePadDir.E : ((!isNear(value, 0.0 - rad90, nsewTolerance)) ? GamePadDir.S : GamePadDir.W)) : GamePadDir.N))));
	}

	public bool isNewDirRight()
	{
		return isDirRight(GamePadState) && !isDirRight(PreviousGamePadState);
	}

	public bool isDirRight()
	{
		return isDirRight(gamePadState);
	}

	public static bool isDirRight(GamePadState state)
	{
		return state.DPad.Right == ButtonState.Pressed || (double)state.ThumbSticks.Left.X > (double)deadZone || (double)state.ThumbSticks.Right.X > (double)deadZone;
	}

	public bool isNewDirLeft()
	{
		return isDirLeft(GamePadState) && !isDirLeft(PreviousGamePadState);
	}

	public bool isDirLeft()
	{
		return isDirLeft(gamePadState);
	}

	public static bool isDirLeft(GamePadState state)
	{
		return state.DPad.Left == ButtonState.Pressed || (double)state.ThumbSticks.Left.X < 0.0 - (double)deadZone || (double)state.ThumbSticks.Right.X < 0.0 - (double)deadZone;
	}

	public bool isNewDirUp()
	{
		return isDirUp(GamePadState) && !isDirUp(PreviousGamePadState);
	}

	public bool isDirUp()
	{
		return isDirUp(gamePadState);
	}

	public static bool isDirUp(GamePadState state)
	{
		return state.DPad.Up == ButtonState.Pressed || (double)state.ThumbSticks.Left.Y > (double)deadZone || (double)state.ThumbSticks.Right.Y > (double)deadZone;
	}

	public bool isNewDirDown()
	{
		return isDirDown(GamePadState) && !isDirDown(PreviousGamePadState);
	}

	public bool isDirDown()
	{
		return isDirDown(gamePadState);
	}

	public static bool isDirDown(GamePadState state)
	{
		return state.DPad.Down == ButtonState.Pressed || (double)state.ThumbSticks.Left.Y < 0.0 - (double)deadZone || (double)state.ThumbSticks.Right.Y < 0.0 - (double)deadZone;
	}

	public bool isButtonDown(Buttons button)
	{
		return gamePadState.IsButtonDown(button);
	}

	public bool isNewButtonDown(Buttons button)
	{
		return gamePadState.IsButtonDown(button) && previousGamePadState.IsButtonUp(button);
	}

	public bool isButtonUp(Buttons button)
	{
		return gamePadState.IsButtonUp(button);
	}

	public bool isNewButtonUp(Buttons button)
	{
		return gamePadState.IsButtonUp(button) && previousGamePadState.IsButtonDown(button);
	}

	public static PlayerIndex? scanForButtonDown(Buttons button)
	{
		PlayerIndex[] array = playerIndexes;
		foreach (PlayerIndex playerIndex in array)
		{
			if (GamePad.GetState(playerIndex).IsButtonDown(button))
			{
				return playerIndex;
			}
		}
		return null;
	}

	public static void beginUpdate()
	{
		for (int i = 0; i < managers.Count; i++)
		{
			managers[i].gamePadState = GamePad.GetState(managers[i].PlayerIndex);
		}
	}

	public static void endUpdate(GameTime gameTime)
	{
		for (int i = 0; i < managers.Count; i++)
		{
			managers[i].endManagerUpdate(gameTime);
		}
	}

	private void endManagerUpdate(GameTime gameTime)
	{
		if (isDisconnected)
		{
			if (!GamePadState.IsConnected)
			{
				return;
			}
			isDisconnected = false;
		}
		if (!GamePadState.IsConnected)
		{
			isDisconnected = true;
			gamePadDisconnect();
			return;
		}
		if (vibrateTimer > TimeSpan.Zero)
		{
			vibrateTimer -= gameTime.ElapsedGameTime;
			if (vibrateTimer <= TimeSpan.Zero)
			{
				stopVibrating();
			}
		}
		previousGamePadState = gamePadState;
	}

	public void gamePadDisconnect()
	{
		MC.ScreenManager.addScreen(new GamePadDisconnectScreen());
	}
}
