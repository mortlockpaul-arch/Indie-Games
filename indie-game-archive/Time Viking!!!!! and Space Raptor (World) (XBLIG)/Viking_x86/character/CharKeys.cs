using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Viking_x86.director;
using Yuki_Win;

namespace Viking_x86.character;

public class CharKeys
{
	public Vector2 runVec;

	public Vector2 shootVec;

	private GamePadState pgs;

	private bool autoMode;

	public bool keyUp;

	public bool keyDown;

	public bool keyAccept;

	public bool keyCancel;

	public void Update(int IDX)
	{
		GamePadState state = GamePad.GetState((PlayerIndex)IDX);
		keyUp = (keyDown = (keyCancel = (keyAccept = false)));
		runVec = state.ThumbSticks.Left;
		shootVec = state.ThumbSticks.Right;
		runVec.Y = 0f - runVec.Y;
		float angle = Trig.GetAngle(runVec, default(Vector2));
		angle -= VScroll.angle;
		runVec = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
		shootVec.Y = 0f - shootVec.Y;
		if (state.Buttons.RightStick == ButtonState.Pressed && state.Buttons.LeftStick == ButtonState.Pressed && pgs.Buttons.RightStick == ButtonState.Released && pgs.Buttons.LeftStick == ButtonState.Released)
		{
			autoMode = !autoMode;
		}
		if (autoMode)
		{
			shootVec = Rand.GetRandomVec2(-1f, 1f, -1f, 0f);
			Game1.vgame.charMgr.character[IDX].lives = 5;
		}
		if (state.Buttons.Start == ButtonState.Pressed && pgs.Buttons.Start == ButtonState.Released)
		{
			TimeMgr.CurTMgr().Pause(IDX);
		}
		if (pgs.ThumbSticks.Left.Y < 0.3f && state.ThumbSticks.Left.Y >= 0.3f)
		{
			keyUp = true;
		}
		if (pgs.ThumbSticks.Left.Y > -0.3f && state.ThumbSticks.Left.Y <= -0.3f)
		{
			keyDown = true;
		}
		if (pgs.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed)
		{
			keyAccept = true;
		}
		if (pgs.Buttons.B == ButtonState.Released && pgs.Buttons.B == ButtonState.Pressed)
		{
			keyCancel = true;
		}
		pgs = state;
	}

	internal void Clear()
	{
	}
}
