using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace ZP2K9.hud;

public class InterfaceKeys
{
	public bool keyLeft;

	public bool keyRight;

	public bool keyUp;

	public bool keyDown;

	public bool keyAccept;

	public bool keyCancel;

	public bool keySelect;

	public bool keyStart;

	public bool keyDrawA;

	public bool keyDrawB;

	public bool keyDrawX;

	public bool keyDrawY;

	public Vector2 leftAnalog;

	public Vector2 rightAnalog;

	public bool keyDLeft;

	public bool keyDRight;

	public bool keyDUp;

	public bool keyDDown;

	public bool keyY;

	private GamePadState pgs;

	public float keyLeftTrig;

	public float keyRightTrig;

	public bool keyRightShoulder;

	public bool keyLeftShoulder;

	public void Reset()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		keyLeft = false;
		keyRight = false;
		keyUp = false;
		keyDown = false;
		keyDLeft = false;
		keyDRight = false;
		keyDUp = false;
		keyDDown = false;
		keyAccept = false;
		keyCancel = false;
		keySelect = false;
		keyStart = false;
		keyY = false;
		keyDrawA = false;
		keyDrawB = false;
		keyDrawX = false;
		keyDrawY = false;
		leftAnalog = default(Vector2);
		keyLeftTrig = 0f;
		keyRightTrig = 0f;
		keyLeftShoulder = false;
		keyRightShoulder = false;
	}

	public void Update(GamePadState gs)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Invalid comparison between Unknown and I4
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Invalid comparison between Unknown and I4
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Invalid comparison between Unknown and I4
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Invalid comparison between Unknown and I4
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Invalid comparison between Unknown and I4
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Invalid comparison between Unknown and I4
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Invalid comparison between Unknown and I4
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Invalid comparison between Unknown and I4
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Invalid comparison between Unknown and I4
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Invalid comparison between Unknown and I4
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Invalid comparison between Unknown and I4
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Invalid comparison between Unknown and I4
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Invalid comparison between Unknown and I4
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Invalid comparison between Unknown and I4
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Invalid comparison between Unknown and I4
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		Reset();
		if (Guide.IsVisible)
		{
			return;
		}
		GamePadThumbSticks thumbSticks = ((GamePadState)(ref gs)).ThumbSticks;
		if (((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.3f)
		{
			GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref pgs)).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks2)).Left.X >= -0.3f)
			{
				keyLeft = true;
			}
		}
		GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref gs)).ThumbSticks;
		if (((GamePadThumbSticks)(ref thumbSticks3)).Left.X > 0.3f)
		{
			GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref pgs)).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks4)).Left.X <= 0.3f)
			{
				keyRight = true;
			}
		}
		GamePadThumbSticks thumbSticks5 = ((GamePadState)(ref gs)).ThumbSticks;
		if (((GamePadThumbSticks)(ref thumbSticks5)).Left.Y < -0.3f)
		{
			GamePadThumbSticks thumbSticks6 = ((GamePadState)(ref pgs)).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks6)).Left.Y >= -0.3f)
			{
				keyDown = true;
			}
		}
		GamePadThumbSticks thumbSticks7 = ((GamePadState)(ref gs)).ThumbSticks;
		if (((GamePadThumbSticks)(ref thumbSticks7)).Left.Y > 0.3f)
		{
			GamePadThumbSticks thumbSticks8 = ((GamePadState)(ref pgs)).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks8)).Left.Y <= 0.3f)
			{
				keyUp = true;
			}
		}
		GamePadThumbSticks thumbSticks9 = ((GamePadState)(ref gs)).ThumbSticks;
		leftAnalog = ((GamePadThumbSticks)(ref thumbSticks9)).Left;
		GamePadThumbSticks thumbSticks10 = ((GamePadState)(ref gs)).ThumbSticks;
		rightAnalog = ((GamePadThumbSticks)(ref thumbSticks10)).Right;
		GamePadDPad dPad = ((GamePadState)(ref gs)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Left == 1)
		{
			GamePadDPad dPad2 = ((GamePadState)(ref pgs)).DPad;
			if ((int)((GamePadDPad)(ref dPad2)).Left == 0)
			{
				keyLeft = true;
				keyDLeft = true;
			}
		}
		GamePadDPad dPad3 = ((GamePadState)(ref gs)).DPad;
		if ((int)((GamePadDPad)(ref dPad3)).Right == 1)
		{
			GamePadDPad dPad4 = ((GamePadState)(ref pgs)).DPad;
			if ((int)((GamePadDPad)(ref dPad4)).Right == 0)
			{
				keyRight = true;
				keyDRight = true;
			}
		}
		GamePadDPad dPad5 = ((GamePadState)(ref gs)).DPad;
		if ((int)((GamePadDPad)(ref dPad5)).Up == 1)
		{
			GamePadDPad dPad6 = ((GamePadState)(ref pgs)).DPad;
			if ((int)((GamePadDPad)(ref dPad6)).Up == 0)
			{
				keyUp = true;
				keyDUp = true;
			}
		}
		GamePadDPad dPad7 = ((GamePadState)(ref gs)).DPad;
		if ((int)((GamePadDPad)(ref dPad7)).Down == 1)
		{
			GamePadDPad dPad8 = ((GamePadState)(ref pgs)).DPad;
			if ((int)((GamePadDPad)(ref dPad8)).Down == 0)
			{
				keyDown = true;
				keyDDown = true;
			}
		}
		GamePadButtons buttons = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).A == 1)
		{
			GamePadButtons buttons2 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).A == 0)
			{
				keyAccept = true;
			}
		}
		GamePadButtons buttons3 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons3)).Y == 1)
		{
			GamePadButtons buttons4 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons4)).Y == 0)
			{
				keyY = true;
			}
		}
		GamePadButtons buttons5 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons5)).Start == 1)
		{
			GamePadButtons buttons6 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons6)).Start == 0)
			{
				keyStart = true;
			}
		}
		GamePadButtons buttons7 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons7)).Back == 1)
		{
			GamePadButtons buttons8 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons8)).Back == 0)
			{
				keySelect = true;
			}
		}
		GamePadButtons buttons9 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons9)).B == 1)
		{
			GamePadButtons buttons10 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons10)).B == 0)
			{
				keyCancel = true;
			}
		}
		GamePadButtons buttons11 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons11)).A == 1)
		{
			keyDrawA = true;
		}
		GamePadButtons buttons12 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons12)).B == 1)
		{
			keyDrawB = true;
		}
		GamePadButtons buttons13 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons13)).X == 1)
		{
			keyDrawX = true;
		}
		GamePadButtons buttons14 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons14)).Y == 1)
		{
			keyDrawY = true;
		}
		GamePadTriggers triggers = ((GamePadState)(ref gs)).Triggers;
		keyLeftTrig = ((GamePadTriggers)(ref triggers)).Left;
		GamePadTriggers triggers2 = ((GamePadState)(ref gs)).Triggers;
		keyRightTrig = ((GamePadTriggers)(ref triggers2)).Right;
		GamePadButtons buttons15 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons15)).LeftShoulder == 1)
		{
			GamePadButtons buttons16 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons16)).LeftShoulder == 0)
			{
				keyLeftShoulder = true;
			}
		}
		GamePadButtons buttons17 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons17)).RightShoulder == 1)
		{
			GamePadButtons buttons18 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons18)).RightShoulder == 0)
			{
				keyRightShoulder = true;
			}
		}
		pgs = gs;
	}
}
