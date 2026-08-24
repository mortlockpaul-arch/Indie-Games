using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace ZP2K9.characters;

public class CharKeys
{
	public bool keyLeft;

	public bool keyRight;

	public bool keyJump;

	public bool keyUp;

	public bool keyDown;

	public bool keyBack;

	public float jumpPower;

	public bool keyDUp;

	public bool keyDRight;

	public bool keyDDown;

	public bool keyDLeft;

	public bool keyA;

	public bool keyX;

	public bool keyB;

	public bool keyY;

	public bool keyPickup;

	public bool keyReload;

	private float xFrame;

	public bool keyGrenade;

	public bool keyGren2;

	public bool keyStart;

	public Vector2 shootVec;

	public Vector2 runVec;

	public bool keyShoot;

	public bool keyLeftShoulder;

	public bool keyRightShoulder;

	private GamePadState pgs;

	public bool keyKick;

	public bool keyRoll;

	public bool keyFloat;

	public bool keyJetpack;

	public bool keySquat;

	public bool keySuicide;

	private float suicideFrame;

	public float runSpeed;

	public bool KeyPickup()
	{
		if (keyX)
		{
			return xFrame > 0.15f;
		}
		return false;
	}

	public void ClearKeys()
	{
		keyLeft = false;
		keyRight = false;
		keyJump = false;
		keyUp = false;
		keyDown = false;
		keyBack = false;
		keyStart = false;
		keyDUp = false;
		keyDRight = false;
		keyDDown = false;
		keyDLeft = false;
		keyJetpack = false;
		keyKick = false;
		keyRoll = false;
		keyShoot = false;
		keyGren2 = false;
		keyGrenade = false;
		keyFloat = false;
		keyA = false;
		keyB = false;
		keyY = false;
		keyPickup = false;
		keyReload = false;
		keyLeftShoulder = false;
		keyRightShoulder = false;
		shootVec.X = 0f;
		shootVec.Y = 0f;
		runVec.X = 0f;
		runVec.Y = 0f;
		keySquat = false;
		jumpPower = 0f;
		keySuicide = false;
	}

	public void Update(GamePadState gs, Character c)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Invalid comparison between Unknown and I4
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Invalid comparison between Unknown and I4
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Invalid comparison between Unknown and I4
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Invalid comparison between Unknown and I4
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Invalid comparison between Unknown and I4
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Invalid comparison between Unknown and I4
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Invalid comparison between Unknown and I4
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Invalid comparison between Unknown and I4
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Invalid comparison between Unknown and I4
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Invalid comparison between Unknown and I4
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Invalid comparison between Unknown and I4
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Invalid comparison between Unknown and I4
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Invalid comparison between Unknown and I4
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Invalid comparison between Unknown and I4
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Invalid comparison between Unknown and I4
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Invalid comparison between Unknown and I4
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Invalid comparison between Unknown and I4
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		ClearKeys();
		if (Game1.menu.IsActive() || Game1.netSession.postLobby || Guide.IsVisible)
		{
			return;
		}
		GamePadButtons buttons = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 1)
		{
			GamePadButtons buttons2 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).LeftShoulder == 0)
			{
				keyJump = true;
				keyLeftShoulder = true;
			}
		}
		GamePadButtons buttons3 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons3)).LeftShoulder == 1)
		{
			keyJetpack = true;
		}
		GamePadButtons buttons4 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons4)).Start == 1)
		{
			GamePadButtons buttons5 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons5)).Start == 0)
			{
				keyStart = true;
			}
		}
		GamePadButtons buttons6 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons6)).RightStick == 1)
		{
			GamePadButtons buttons7 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons7)).RightStick == 0)
			{
				keyKick = true;
			}
		}
		GamePadButtons buttons8 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons8)).LeftStick == 1)
		{
			GamePadButtons buttons9 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons9)).LeftStick == 0)
			{
				keySquat = true;
			}
		}
		GamePadTriggers triggers = ((GamePadState)(ref gs)).Triggers;
		if (((GamePadTriggers)(ref triggers)).Left > 0.3f)
		{
			GamePadTriggers triggers2 = ((GamePadState)(ref pgs)).Triggers;
			if (((GamePadTriggers)(ref triggers2)).Left <= 0.3f)
			{
				keyRoll = true;
			}
		}
		GamePadTriggers triggers3 = ((GamePadState)(ref gs)).Triggers;
		if (((GamePadTriggers)(ref triggers3)).Left > 0.3f)
		{
			keyFloat = true;
		}
		GamePadButtons buttons10 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons10)).A == 1)
		{
			GamePadButtons buttons11 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons11)).A == 0)
			{
				keyA = true;
			}
		}
		GamePadTriggers triggers4 = ((GamePadState)(ref gs)).Triggers;
		if (((GamePadTriggers)(ref triggers4)).Left > 0.5f)
		{
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref gs)).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.5f)
			{
				GamePadButtons buttons12 = ((GamePadState)(ref gs)).Buttons;
				if ((int)((GamePadButtons)(ref buttons12)).X == 1)
				{
					suicideFrame += Game1.frameTime;
					if (suicideFrame > 0.5f)
					{
						keySuicide = true;
					}
					goto IL_0208;
				}
			}
		}
		suicideFrame = 0f;
		goto IL_0208;
		IL_0208:
		GamePadButtons buttons13 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons13)).B == 1)
		{
			GamePadButtons buttons14 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons14)).B == 0)
			{
				keyB = true;
			}
		}
		GamePadButtons buttons15 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons15)).X == 1)
		{
			keyX = true;
			xFrame += Game1.frameTime;
		}
		else
		{
			if (keyX)
			{
				keyX = false;
				if (xFrame < 0.35f)
				{
					keyReload = true;
				}
				else
				{
					keyPickup = true;
				}
			}
			xFrame = 0f;
		}
		GamePadButtons buttons16 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons16)).Y == 1)
		{
			GamePadButtons buttons17 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons17)).Y == 0)
			{
				keyY = true;
			}
		}
		GamePadButtons buttons18 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons18)).RightShoulder == 1)
		{
			GamePadButtons buttons19 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons19)).RightShoulder == 0)
			{
				keyRightShoulder = true;
			}
		}
		GamePadButtons buttons20 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons20)).Back == 1)
		{
			GamePadButtons buttons21 = ((GamePadState)(ref pgs)).Buttons;
			if ((int)((GamePadButtons)(ref buttons21)).Back == 0)
			{
				keyBack = true;
			}
		}
		GamePadDPad dPad = ((GamePadState)(ref gs)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Right == 1)
		{
			GamePadDPad dPad2 = ((GamePadState)(ref pgs)).DPad;
			if ((int)((GamePadDPad)(ref dPad2)).Right == 0)
			{
				keyDRight = true;
			}
		}
		GamePadDPad dPad3 = ((GamePadState)(ref gs)).DPad;
		if ((int)((GamePadDPad)(ref dPad3)).Right == 1)
		{
			GamePadDPad dPad4 = ((GamePadState)(ref pgs)).DPad;
			if ((int)((GamePadDPad)(ref dPad4)).Right == 0)
			{
				keyDRight = true;
			}
		}
		GamePadDPad dPad5 = ((GamePadState)(ref gs)).DPad;
		if ((int)((GamePadDPad)(ref dPad5)).Left == 1)
		{
			GamePadDPad dPad6 = ((GamePadState)(ref pgs)).DPad;
			if ((int)((GamePadDPad)(ref dPad6)).Left == 0)
			{
				keyDLeft = true;
			}
		}
		GamePadDPad dPad7 = ((GamePadState)(ref gs)).DPad;
		if ((int)((GamePadDPad)(ref dPad7)).Up == 1)
		{
			GamePadDPad dPad8 = ((GamePadState)(ref pgs)).DPad;
			if ((int)((GamePadDPad)(ref dPad8)).Up == 0)
			{
				keyDUp = true;
			}
		}
		GamePadDPad dPad9 = ((GamePadState)(ref gs)).DPad;
		if ((int)((GamePadDPad)(ref dPad9)).Down == 1)
		{
			GamePadDPad dPad10 = ((GamePadState)(ref pgs)).DPad;
			if ((int)((GamePadDPad)(ref dPad10)).Down == 0)
			{
				keyDDown = true;
			}
		}
		GamePadTriggers triggers5 = ((GamePadState)(ref gs)).Triggers;
		if (((GamePadTriggers)(ref triggers5)).Right > 0.3f)
		{
			keyGrenade = true;
		}
		GamePadButtons buttons22 = ((GamePadState)(ref gs)).Buttons;
		if ((int)((GamePadButtons)(ref buttons22)).RightShoulder == 1)
		{
			keyGren2 = true;
		}
		GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref gs)).ThumbSticks;
		if (((GamePadThumbSticks)(ref thumbSticks2)).Left.X < -0.2f)
		{
			keyLeft = true;
		}
		GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref gs)).ThumbSticks;
		if (((GamePadThumbSticks)(ref thumbSticks3)).Left.X > 0.2f)
		{
			keyRight = true;
		}
		GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref gs)).ThumbSticks;
		if (((GamePadThumbSticks)(ref thumbSticks4)).Left.Y < -0.2f)
		{
			keyDown = true;
		}
		GamePadThumbSticks thumbSticks5 = ((GamePadState)(ref gs)).ThumbSticks;
		if (((GamePadThumbSticks)(ref thumbSticks5)).Left.Y > 0.3f)
		{
			keyUp = true;
		}
		GamePadThumbSticks thumbSticks6 = ((GamePadState)(ref gs)).ThumbSticks;
		jumpPower = ((GamePadThumbSticks)(ref thumbSticks6)).Left.Y;
		if (keyJump || keyA)
		{
			jumpPower = 1f;
		}
		if (GameState.gameType == 4 && c.team == 1)
		{
			jumpPower = 1f;
		}
		GamePadThumbSticks thumbSticks7 = ((GamePadState)(ref gs)).ThumbSticks;
		runVec = ((GamePadThumbSticks)(ref thumbSticks7)).Left;
		runSpeed = ((Vector2)(ref runVec)).Length();
		runSpeed *= 1.2f;
		if (runSpeed > 1f)
		{
			runSpeed = 1f;
		}
		GamePadThumbSticks thumbSticks8 = ((GamePadState)(ref gs)).ThumbSticks;
		shootVec = ((GamePadThumbSticks)(ref thumbSticks8)).Right;
		shootVec.Y = 0f - shootVec.Y;
		if (!Game1.settings.twinStickShooter)
		{
			bool flag = keyGrenade;
			bool flag2 = keyGren2;
			if (flag || flag2)
			{
				Vector2 val = default(Vector2);
				if (shootVec.X == 0f && shootVec.Y == 0f)
				{
					if (runVec.X == 0f && runVec.Y == 0f)
					{
						((Vector2)(ref val))._002Ector((float)Math.Cos(c.angle), (float)Math.Sin(c.angle));
						val.Y = 0f - val.Y;
						if (c.face == 0)
						{
							val = -val;
						}
					}
					else
					{
						val = runVec;
						val.Y = 0f - val.Y;
					}
				}
				else
				{
					val = shootVec;
				}
				if (!flag2 || ((Vector2)(ref val)).Length() < 0.61f)
				{
					((Vector2)(ref val)).Normalize();
					val *= 0.61f;
				}
				if (c.grenAmmo[0] <= 0 && !flag)
				{
					((Vector2)(ref val)).Normalize();
					val *= 0.59f;
				}
				shootVec = val;
			}
			else if ((shootVec.X != 0f || shootVec.Y != 0f) && ((Vector2)(ref shootVec)).Length() > 0.59f)
			{
				((Vector2)(ref shootVec)).Normalize();
				shootVec *= 0.59f;
			}
			keyGren2 = false;
			keyGrenade = flag2;
		}
		if (c.spawnFrame > 0f || c.dyingFrame > 0f)
		{
			shootVec = default(Vector2);
		}
		if (Game1.menu.menuLevel[9].alpha > 0f)
		{
			ClearKeys();
		}
		pgs = gs;
	}

	internal void SetKeyPickup()
	{
		keyX = true;
		xFrame = 0.2f;
	}
}
