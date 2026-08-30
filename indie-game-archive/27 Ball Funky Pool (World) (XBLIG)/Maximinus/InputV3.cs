using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Maximinus;

public class InputV3 : InputV2
{
	public delegate void EndFindPadDelegate(GameTime gameTime);

	public delegate void ButtonActionDelegate(GameTime gameTime, Buttons button, PressOrRelease pressOrRelease);

	private EndFindPadDelegate endFindPadCB;

	private ButtonActionDelegate buttonActionCB;

	private bool called1stFrameCB;

	private static Vector2 stickLeft;

	private static Vector2 stickRight;

	private static Vector2 dpad;

	private static Vector2 triggers;

	public static readonly List<Buttons> ButtonList = new List<Buttons>
	{
		Buttons.A,
		Buttons.B,
		Buttons.X,
		Buttons.Y,
		Buttons.Start,
		Buttons.Back,
		Buttons.LeftShoulder,
		Buttons.RightShoulder
	};

	public static Vector2 Triggers => triggers;

	public static Vector2 StickLeft => stickLeft;

	public static Vector2 StickRight => stickRight;

	public static Vector2 DPad => dpad;

	public static Vector2 DirectionAllSticksCombined => MergeSticks(StickLeft, StickRight, DPad);

	public static Vector2 MergeSticks(Vector2 stick1, Vector2 stick2)
	{
		return new Vector2((Math.Abs(stick1.X) > Math.Abs(stick2.X)) ? stick1.X : stick2.X, (Math.Abs(stick1.Y) > Math.Abs(stick2.Y)) ? stick1.Y : stick2.Y);
	}

	public static Vector2 MergeSticks(Vector2 stick1, Vector2 stick2, Vector2 stick3)
	{
		return MergeSticks(stick1, MergeSticks(stick2, stick3));
	}

	public InputV3(EndFindPadDelegate endFindPadCB, ButtonActionDelegate buttonActionCB)
	{
		this.endFindPadCB = endFindPadCB;
		this.buttonActionCB = buttonActionCB;
	}

	public virtual void Update(GameTime gameTime)
	{
		if (!UpdatePre(gameTime))
		{
			return;
		}
		if (!called1stFrameCB)
		{
			called1stFrameCB = true;
			endFindPadCB(gameTime);
		}
		bool flag = MaximinusGame.Instance.Camera != null && MaximinusGame.Instance.Camera.DoHandleInput;
		GetCurrentState();
		triggers = new Vector2(Multiplayer.CurrentState(base.PlayerIndex).Triggers.Left, Multiplayer.CurrentState(base.PlayerIndex).Triggers.Right);
		stickLeft = Multiplayer.CurrentState(base.PlayerIndex).ThumbSticks.Left;
		stickRight = Multiplayer.CurrentState(base.PlayerIndex).ThumbSticks.Right;
		dpad = DPadVectorStatic(Multiplayer.CurrentState(base.PlayerIndex));
		foreach (Buttons button in ButtonList)
		{
			if (isPressedV3(button))
			{
				buttonActionCB(gameTime, button, PressOrRelease.Press);
				if (flag)
				{
					MaximinusGame.Instance.Camera.HandleInputDigital(button, PressOrRelease.Press);
				}
			}
			if (justReleasedV3(button))
			{
				buttonActionCB(gameTime, button, PressOrRelease.Release);
				if (flag)
				{
					MaximinusGame.Instance.Camera.HandleInputDigital(button, PressOrRelease.Release);
				}
			}
		}
		UpdatePreviousStates();
		if (flag)
		{
			MaximinusGame.Instance.Camera.HandleInputAnalog(stickLeft, stickRight, dpad, triggers);
		}
	}

	private bool isPressedV3(Buttons b)
	{
		return isPressed(Multiplayer.CurrentState(base.PlayerIndex), b);
	}

	private bool justReleasedV3(Buttons b)
	{
		return justReleased(Multiplayer.CurrentState(base.PlayerIndex), Multiplayer.PreviousState(base.PlayerIndex), b);
	}
}
