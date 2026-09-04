using System;
using RuntimeXNA.Banks;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements;

internal class CMovePlatform : CMove
{
	public const short MPJC_NOJUMP = 0;

	public const short MPJC_DIAGO = 1;

	public const short MPJC_BUTTON1 = 2;

	public const short MPJC_BUTTON2 = 3;

	public const short MPTYPE_WALK = 0;

	public const short MPTYPE_CLIMB = 1;

	public const short MPTYPE_JUMP = 2;

	public const short MPTYPE_FALL = 3;

	public const short MPTYPE_CROUCH = 4;

	public const short MPTYPE_UNCROUCH = 5;

	public int MP_Type;

	public int MP_Bounce;

	public int MP_BounceMu;

	public int MP_XSpeed;

	public int MP_Gravity;

	public int MP_Jump;

	public int MP_YSpeed;

	public int MP_XMB;

	public int MP_YMB;

	public int MP_HTFOOT;

	public int MP_JumpControl;

	public int MP_JumpStopped;

	public int MP_PreviousDir;

	public CObject MP_ObjectUnder;

	public int MP_XObjectUnder;

	public int MP_YObjectUnder;

	public bool MP_NoJump;

	public override void init(CObject ho, CMoveDef mvPtr)
	{
		hoPtr = ho;
		CMoveDefPlatform cMoveDefPlatform = (CMoveDefPlatform)mvPtr;
		hoPtr.hoCalculX = 0;
		hoPtr.hoCalculY = 0;
		MP_XSpeed = 0;
		hoPtr.roc.rcSpeed = 0;
		MP_Bounce = 0;
		hoPtr.roc.rcPlayer = mvPtr.mvControl;
		rmAcc = cMoveDefPlatform.mpAcc;
		rmAccValue = getAccelerator(rmAcc);
		rmDec = cMoveDefPlatform.mpDec;
		rmDecValue = getAccelerator(rmDec);
		hoPtr.roc.rcMaxSpeed = cMoveDefPlatform.mpSpeed;
		hoPtr.roc.rcMinSpeed = 0;
		MP_Gravity = cMoveDefPlatform.mpGravity;
		MP_Jump = cMoveDefPlatform.mpJump;
		int num = cMoveDefPlatform.mpJumpControl;
		if (num > 3)
		{
			num = 1;
		}
		MP_JumpControl = num;
		MP_YSpeed = 0;
		MP_JumpStopped = 0;
		MP_ObjectUnder = null;
		moveAtStart(mvPtr);
		MP_PreviousDir = hoPtr.roc.rcDir;
		hoPtr.roc.rcChanged = true;
		MP_Type = 0;
	}

	public override void move()
	{
		hoPtr.hoAdRunHeader.rhVBLObjet = 1;
		int num = hoPtr.hoAdRunHeader.rhPlayer[hoPtr.roc.rcPlayer - 1];
		calcMBFoot();
		int num2 = MP_XSpeed;
		if (MP_JumpStopped == 0)
		{
			if (num2 <= 0)
			{
				if ((num & 4) != 0)
				{
					int num3 = rmAccValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
					{
						num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					}
					num2 -= num3;
					int num4 = num2 / 256;
					if (num4 < -hoPtr.roc.rcMaxSpeed)
					{
						num2 = -hoPtr.roc.rcMaxSpeed * 256;
					}
				}
				else if (num2 < 0)
				{
					int num3 = rmDecValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
					{
						num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					}
					num2 += num3;
					if (num2 > 0)
					{
						num2 = 0;
					}
				}
				if ((num & 8) != 0)
				{
					num2 = -num2;
				}
			}
			if (num2 >= 0)
			{
				if ((num & 8) != 0)
				{
					int num3 = rmAccValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
					{
						num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					}
					num2 += num3;
					int num4 = num2 / 256;
					if (num4 > hoPtr.roc.rcMaxSpeed)
					{
						num2 = hoPtr.roc.rcMaxSpeed * 256;
					}
				}
				else if (num2 > 0)
				{
					int num3 = rmDecValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
					{
						num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					}
					num2 -= num3;
					if (num2 < 0)
					{
						num2 = 0;
					}
				}
				if ((num & 4) != 0)
				{
					num2 = -num2;
				}
			}
			MP_XSpeed = num2;
		}
		int num5 = MP_YSpeed;
		bool flag = false;
		while (true)
		{
			switch (MP_Type)
			{
			case 2:
			case 3:
			{
				int num3 = MP_Gravity << 5;
				if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
				{
					num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
				}
				num5 += num3;
				if (num5 > 64000)
				{
					num5 = 64000;
				}
				break;
			}
			case 0:
				if ((num & 1) != 0)
				{
					if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX + MP_XMB, hoPtr.hoY + MP_YMB - 4) != int.MaxValue)
					{
						MP_Type = 1;
						flag = true;
						continue;
					}
				}
				else if ((num & 2) != 0 && check_Ladder(hoPtr.hoLayer, hoPtr.hoX + MP_XMB, hoPtr.hoY + MP_YMB + 4) != int.MaxValue)
				{
					MP_Type = 1;
					flag = true;
					continue;
				}
				break;
			case 1:
				if (!flag)
				{
					MP_JumpStopped = 0;
					if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX + MP_XMB, hoPtr.hoY + MP_YMB) == int.MaxValue && check_Ladder(hoPtr.hoLayer, hoPtr.hoX + MP_XMB, hoPtr.hoY + MP_YMB - 4) == int.MaxValue)
					{
						break;
					}
				}
				if (num5 <= 0)
				{
					if ((num & 1) != 0)
					{
						int num3 = rmAccValue;
						if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
						{
							num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
						}
						num5 -= num3;
						int num4 = num5 / 256;
						if (num4 < -hoPtr.roc.rcMaxSpeed)
						{
							num5 = -hoPtr.roc.rcMaxSpeed * 256;
						}
					}
					else
					{
						int num3 = rmDecValue;
						if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
						{
							num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
						}
						num5 += num3;
						if (num5 > 0)
						{
							num5 = 0;
						}
					}
					if ((num & 2) != 0)
					{
						num5 = -num5;
					}
				}
				if (num5 < 0)
				{
					break;
				}
				if ((num & 2) != 0)
				{
					int num3 = rmAccValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
					{
						num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					}
					num5 += num3;
					int num4 = num5 / 256;
					if (num4 > hoPtr.roc.rcMaxSpeed)
					{
						num5 = hoPtr.roc.rcMaxSpeed * 256;
					}
				}
				else
				{
					int num3 = rmDecValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
					{
						num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					}
					num5 -= num3;
					if (num5 < 0)
					{
						num5 = 0;
					}
				}
				if ((num & 1) != 0)
				{
					num5 = -num5;
				}
				break;
			}
			break;
		}
		MP_YSpeed = num5;
		int num6 = 0;
		if (num2 < 0)
		{
			num6 = 16;
		}
		int num7 = num2;
		int num8 = num5;
		if (num8 != 0)
		{
			int num9 = 0;
			if (num7 < 0)
			{
				num9 |= 1;
				num7 = -num7;
			}
			if (num8 < 0)
			{
				num9 |= 2;
				num8 = -num8;
			}
			num7 <<= 8;
			num7 /= num8;
			int i;
			for (i = 0; num7 < CMove.CosSurSin32[i]; i += 2)
			{
			}
			num6 = CMove.CosSurSin32[i + 1];
			if ((num9 & 2) != 0)
			{
				num6 = -num6 + 32;
				num6 &= 0x1F;
			}
			if ((num9 & 1) != 0)
			{
				num6 -= 8;
				num6 &= 0x1F;
				num6 = -num6;
				num6 &= 0x1F;
				num6 += 8;
				num6 &= 0x1F;
			}
		}
		num7 = num2;
		int num10 = CMove.Cosinus32[num6];
		int num11 = CMove.Sinus32[num6];
		if (num10 < 0)
		{
			num10 = -num10;
		}
		if (num11 < 0)
		{
			num11 = -num11;
		}
		if (num10 < num11)
		{
			num10 = num11;
			num7 = num5;
		}
		if (num7 < 0)
		{
			num7 = -num7;
		}
		num7 /= num10;
		if (num7 > 250)
		{
			num7 = 250;
		}
		hoPtr.roc.rcSpeed = num7;
		switch (MP_Type)
		{
		case 1:
			if (num5 < 0)
			{
				hoPtr.roc.rcDir = 8;
			}
			else if (num5 > 0)
			{
				hoPtr.roc.rcDir = 24;
			}
			break;
		case 3:
			hoPtr.roc.rcDir = num6;
			break;
		default:
			if (num2 < 0)
			{
				hoPtr.roc.rcDir = 16;
			}
			else if (num2 > 0)
			{
				hoPtr.roc.rcDir = 0;
			}
			break;
		}
		switch (MP_Type)
		{
		case 4:
			hoPtr.roc.rcAnim = 10;
			break;
		case 5:
			hoPtr.roc.rcAnim = 11;
			break;
		case 3:
			hoPtr.roc.rcAnim = 8;
			break;
		case 2:
			hoPtr.roc.rcAnim = 7;
			break;
		case 1:
			hoPtr.roc.rcAnim = 9;
			break;
		default:
			hoPtr.roc.rcAnim = 1;
			break;
		}
		if (hoPtr.roa != null)
		{
			hoPtr.roa.animate();
			if (CRun.bMoveChanged)
			{
				return;
			}
		}
		calcMBFoot();
		newMake_Move(hoPtr.roc.rcSpeed, num6);
		if (CRun.bMoveChanged)
		{
			return;
		}
		if ((MP_Type == 0 || MP_Type == 1) && !MP_NoJump)
		{
			bool flag2 = false;
			int mP_JumpControl = MP_JumpControl;
			if (mP_JumpControl != 0)
			{
				mP_JumpControl--;
				if (mP_JumpControl == 0)
				{
					if ((num & 5) == 5)
					{
						flag2 = true;
					}
					if ((num & 9) == 9)
					{
						flag2 = true;
					}
				}
				else
				{
					mP_JumpControl <<= 4;
					if ((num & mP_JumpControl) != 0)
					{
						flag2 = true;
					}
				}
			}
			if (flag2)
			{
				MP_YSpeed = -MP_Jump << 8;
				MP_Type = 2;
			}
		}
		switch (MP_Type)
		{
		case 2:
			if (MP_YSpeed >= 0)
			{
				MP_Type = 3;
			}
			break;
		case 3:
			if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX + MP_XMB, hoPtr.hoY + MP_YMB) != int.MaxValue)
			{
				MP_YSpeed = 0;
				MP_Type = 1;
				hoPtr.roc.rcDir = 8;
			}
			break;
		case 0:
			if ((num & 3) != 0 && (num & 0xC) == 0 && check_Ladder(hoPtr.hoLayer, hoPtr.hoX + MP_XMB, hoPtr.hoY + MP_YMB) != int.MaxValue)
			{
				MP_Type = 1;
				MP_XSpeed = 0;
				break;
			}
			if ((num & 2) != 0 && hoPtr.roa != null && hoPtr.roa.anim_Exist(10))
			{
				MP_XSpeed = 0;
				MP_Type = 4;
			}
			if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX + MP_XMB, hoPtr.hoY + MP_YMB) == int.MaxValue)
			{
				if (!tst_SpritePosition(hoPtr.hoX, hoPtr.hoY + 10, (short)MP_HTFOOT, 1, flag: true))
				{
					int num12 = hoPtr.hoX - hoPtr.hoAdRunHeader.rhWindowX;
					int num13 = hoPtr.hoY - hoPtr.hoAdRunHeader.rhWindowY;
					int destY = num13 + MP_HTFOOT - 1;
					CPoint cPoint = new CPoint();
					mpApproachSprite(num12, destY, num12, num13, (short)MP_HTFOOT, 1, cPoint);
					hoPtr.hoX = cPoint.x + hoPtr.hoAdRunHeader.rhWindowX;
					hoPtr.hoY = cPoint.y + hoPtr.hoAdRunHeader.rhWindowY;
					MP_NoJump = false;
				}
				else
				{
					MP_Type = 3;
				}
			}
			break;
		case 1:
			if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX + MP_XMB, hoPtr.hoY + MP_YMB) == int.MaxValue)
			{
				if (MP_YSpeed < 0)
				{
					for (num8 = 0; num8 < 32; num8++)
					{
						if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX + MP_XMB, hoPtr.hoY + MP_YMB + num8) != int.MaxValue)
						{
							hoPtr.hoY += num8;
							break;
						}
					}
				}
				MP_YSpeed = 0;
			}
			if ((num & 0xC) != 0)
			{
				MP_Type = 0;
				MP_YSpeed = 0;
			}
			break;
		case 4:
			if ((num & 2) == 0)
			{
				if (hoPtr.roa != null && hoPtr.roa.anim_Exist(11))
				{
					MP_Type = 5;
					hoPtr.roc.rcAnim = 11;
					hoPtr.roa.animate();
					hoPtr.roa.raAnimRepeat = 1;
				}
				else
				{
					MP_Type = 0;
				}
			}
			break;
		case 5:
			if (hoPtr.roa != null && hoPtr.roa.raAnimNumberOfFrame == 0)
			{
				MP_Type = 0;
			}
			break;
		}
		if (MP_Type == 0 || MP_Type == 4 || MP_Type == 5)
		{
			if (hoPtr.hoAdRunHeader.objectAllCol_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, hoPtr.hoX, hoPtr.hoY, hoPtr.hoOiList.oilColList) == null)
			{
				CArrayList cArrayList = hoPtr.hoAdRunHeader.objectAllCol_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, hoPtr.hoX, hoPtr.hoY + 1, hoPtr.hoOiList.oilColList);
				if (cArrayList != null && cArrayList.size() == 1)
				{
					CObject cObject = (CObject)cArrayList.get(0);
					if ((MP_ObjectUnder == null || MP_ObjectUnder != cObject) && hoPtr.hoOi != cObject.hoOi)
					{
						MP_ObjectUnder = cObject;
						MP_XObjectUnder = cObject.hoX;
						MP_YObjectUnder = cObject.hoY;
						return;
					}
					int num14 = cObject.hoX - MP_XObjectUnder;
					int num15 = cObject.hoY - MP_YObjectUnder;
					MP_XObjectUnder = cObject.hoX;
					MP_YObjectUnder = cObject.hoY;
					hoPtr.hoX += num14;
					hoPtr.hoY += num15;
					hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
					hoPtr.roc.rcChanged = true;
					return;
				}
			}
			MP_ObjectUnder = null;
		}
		else
		{
			MP_ObjectUnder = null;
		}
	}

	private void mpStopIt()
	{
		hoPtr.roc.rcSpeed = 0;
		MP_XSpeed = 0;
		MP_YSpeed = 0;
	}

	public override void stop()
	{
		MP_Bounce = 0;
		if (rmCollisionCount != hoPtr.hoAdRunHeader.rh3CollisionCount)
		{
			mpStopIt();
			return;
		}
		hoPtr.rom.rmMoveFlag = true;
		int num = hoPtr.hoX - hoPtr.hoAdRunHeader.rhWindowX;
		int num2 = hoPtr.hoY - hoPtr.hoAdRunHeader.rhWindowY;
		switch (hoPtr.hoAdRunHeader.rhEvtProg.rhCurCode >> 16)
		{
		case -12:
		{
			int num3 = hoPtr.hoX - hoPtr.hoImgXSpot;
			int num4 = hoPtr.hoY - hoPtr.hoImgYSpot;
			int num5 = hoPtr.hoAdRunHeader.quadran_Out(num3, num4, num3 + hoPtr.hoImgWidth, num4 + hoPtr.hoImgHeight);
			num3 = hoPtr.hoX;
			num4 = hoPtr.hoY;
			if ((num5 & 1) != 0)
			{
				num3 = hoPtr.hoImgXSpot;
				MP_XSpeed = 0;
				MP_NoJump = true;
			}
			if ((num5 & 2) != 0)
			{
				num3 = hoPtr.hoAdRunHeader.rhLevelSx - hoPtr.hoImgWidth + hoPtr.hoImgXSpot;
				MP_XSpeed = 0;
				MP_NoJump = true;
			}
			if ((num5 & 4) != 0)
			{
				num4 = hoPtr.hoImgYSpot;
				MP_YSpeed = 0;
				MP_NoJump = false;
			}
			if ((num5 & 8) != 0)
			{
				num4 = hoPtr.hoAdRunHeader.rhLevelSy - hoPtr.hoImgHeight + hoPtr.hoImgYSpot;
				MP_YSpeed = 0;
				MP_NoJump = false;
			}
			hoPtr.hoX = num3;
			hoPtr.hoY = num4;
			if (MP_Type == 2)
			{
				MP_Type = 3;
			}
			else
			{
				MP_Type = 0;
			}
			MP_JumpStopped = 0;
			break;
		}
		case -14:
		case -13:
		{
			MP_NoJump = false;
			CPoint cPoint = new CPoint();
			if (MP_Type == 3)
			{
				mpApproachSprite(num, num2, hoPtr.roc.rcOldX - hoPtr.hoAdRunHeader.rhWindowX, hoPtr.roc.rcOldY - hoPtr.hoAdRunHeader.rhWindowY, (short)MP_HTFOOT, 1, cPoint);
				hoPtr.hoX = cPoint.x + hoPtr.hoAdRunHeader.rhWindowX;
				hoPtr.hoY = cPoint.y + hoPtr.hoAdRunHeader.rhWindowY;
				MP_Type = 0;
				hoPtr.roc.rcChanged = true;
				if (tst_SpritePosition(hoPtr.hoX, hoPtr.hoY + 1, 0, 1, flag: true))
				{
					hoPtr.roc.rcSpeed = 0;
					MP_XSpeed = 0;
				}
				else
				{
					MP_JumpStopped = 0;
					hoPtr.roc.rcSpeed = Math.Abs(MP_XSpeed / 256);
					MP_YSpeed = 0;
				}
				break;
			}
			if (MP_Type == 0)
			{
				if (mpApproachSprite(num, num2, num, num2 - MP_HTFOOT, 0, 1, cPoint))
				{
					hoPtr.hoX = cPoint.x + hoPtr.hoAdRunHeader.rhWindowX;
					hoPtr.hoY = cPoint.y + hoPtr.hoAdRunHeader.rhWindowY;
					hoPtr.roc.rcChanged = true;
					break;
				}
				if (mpApproachSprite(num, num2, hoPtr.roc.rcOldX - hoPtr.hoAdRunHeader.rhWindowX, hoPtr.roc.rcOldY - hoPtr.hoAdRunHeader.rhWindowY, 0, 1, cPoint))
				{
					hoPtr.hoX = cPoint.x + hoPtr.hoAdRunHeader.rhWindowX;
					hoPtr.hoY = cPoint.y + hoPtr.hoAdRunHeader.rhWindowY;
					hoPtr.roc.rcChanged = true;
					mpStopIt();
					break;
				}
			}
			if (MP_Type == 2)
			{
				if (mpApproachSprite(num, num2, num, num2 - MP_HTFOOT, 0, 1, cPoint))
				{
					hoPtr.hoX = cPoint.x + hoPtr.hoAdRunHeader.rhWindowX;
					hoPtr.hoY = cPoint.y + hoPtr.hoAdRunHeader.rhWindowY;
					hoPtr.roc.rcChanged = true;
					break;
				}
				MP_JumpStopped = 1;
				MP_XSpeed = 0;
			}
			if (MP_Type == 1 && mpApproachSprite(num, num2, hoPtr.roc.rcOldX - hoPtr.hoAdRunHeader.rhWindowX, hoPtr.roc.rcOldY - hoPtr.hoAdRunHeader.rhWindowY, 0, 1, cPoint))
			{
				hoPtr.hoX = cPoint.x + hoPtr.hoAdRunHeader.rhWindowX;
				hoPtr.hoY = cPoint.y + hoPtr.hoAdRunHeader.rhWindowY;
				hoPtr.roc.rcChanged = true;
				mpStopIt();
				break;
			}
			hoPtr.roc.rcImage = hoPtr.roc.rcOldImage;
			hoPtr.roc.rcAngle = hoPtr.roc.rcOldAngle;
			if (!tst_SpritePosition(hoPtr.hoX, hoPtr.hoY, 0, 1, flag: true))
			{
				hoPtr.hoX = hoPtr.roc.rcOldX;
				hoPtr.hoY = hoPtr.roc.rcOldY;
				hoPtr.roc.rcChanged = true;
			}
			break;
		}
		}
	}

	public override void bounce()
	{
		stop();
	}

	public override void setXPosition(int x)
	{
		if (hoPtr.hoX != x)
		{
			hoPtr.hoX = x;
			hoPtr.rom.rmMoveFlag = true;
			hoPtr.roc.rcChanged = true;
			hoPtr.roc.rcCheckCollides = true;
		}
	}

	public override void setYPosition(int y)
	{
		if (hoPtr.hoY != y)
		{
			hoPtr.hoY = y;
			hoPtr.rom.rmMoveFlag = true;
			hoPtr.roc.rcChanged = true;
			hoPtr.roc.rcCheckCollides = true;
		}
	}

	public override void setSpeed(int speed)
	{
		if (speed < 0)
		{
			speed = 0;
		}
		if (speed > 250)
		{
			speed = 250;
		}
		if (speed > hoPtr.roc.rcMaxSpeed)
		{
			speed = hoPtr.roc.rcMaxSpeed;
		}
		hoPtr.roc.rcSpeed = speed;
		MP_XSpeed = hoPtr.roc.rcSpeed * CMove.Cosinus32[hoPtr.roc.rcDir];
		MP_YSpeed = hoPtr.roc.rcSpeed * CMove.Sinus32[hoPtr.roc.rcDir];
		hoPtr.rom.rmMoveFlag = true;
	}

	public override void setMaxSpeed(int speed)
	{
		if (speed < 0)
		{
			speed = 0;
		}
		if (speed > 250)
		{
			speed = 250;
		}
		hoPtr.roc.rcMaxSpeed = speed;
		speed <<= 8;
		if (MP_XSpeed > speed)
		{
			MP_XSpeed = speed;
		}
		hoPtr.rom.rmMoveFlag = true;
	}

	public void MPSetGravity(int gravity)
	{
		MP_Gravity = gravity;
	}

	public override void setDir(int dir)
	{
		hoPtr.roc.rcDir = dir;
		MP_XSpeed = hoPtr.roc.rcSpeed * CMove.Cosinus32[dir];
		MP_YSpeed = hoPtr.roc.rcSpeed * CMove.Sinus32[dir];
	}

	private void calcMBFoot()
	{
		CImage cImage;
		if (hoPtr.roc.rcImage != 0)
		{
			cImage = hoPtr.hoAdRunHeader.rhApp.imageBank.getImageInfoEx(hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY);
		}
		else
		{
			cImage = new CImage();
			cImage.width = (short)hoPtr.hoImgWidth;
			cImage.height = (short)hoPtr.hoImgHeight;
			cImage.xSpot = (short)hoPtr.hoImgXSpot;
			cImage.ySpot = (short)hoPtr.hoImgYSpot;
		}
		MP_XMB = -hoPtr.hoAdRunHeader.rhWindowX;
		MP_YMB = cImage.height - hoPtr.hoAdRunHeader.rhWindowY - cImage.ySpot;
		MP_HTFOOT = cImage.height * 2 + cImage.height >> 3;
	}

	private int check_Ladder(int nLayer, int x, int y)
	{
		return hoPtr.hoAdRunHeader.y_GetLadderAt(nLayer, x, y)?.top ?? int.MaxValue;
	}

	public void mpHandle_Background()
	{
		calcMBFoot();
		if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX + MP_XMB, hoPtr.hoY + MP_YMB) == int.MaxValue && (hoPtr.hoAdRunHeader.colMask_TestObject_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, hoPtr.hoX, hoPtr.hoY, 0, 0) != 0 || ((MP_Type != 2 || MP_YSpeed >= 0) && hoPtr.hoAdRunHeader.colMask_TestObject_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, hoPtr.hoX, hoPtr.hoY, (short)MP_HTFOOT, 1) != 0)))
		{
			hoPtr.hoAdRunHeader.rhEvtProg.handle_Event(hoPtr, -851968 | (hoPtr.hoType & 0xFFFF));
		}
	}
}
