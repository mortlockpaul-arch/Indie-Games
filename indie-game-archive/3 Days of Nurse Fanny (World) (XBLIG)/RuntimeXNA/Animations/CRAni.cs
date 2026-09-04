using RuntimeXNA.Banks;
using RuntimeXNA.OI;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Animations;

public class CRAni
{
	private static short[] anim_Defined = new short[15]
	{
		0, 1, 2, 5, 6, 7, 8, 9, 10, 11,
		12, 13, 14, 15, -1
	};

	public CObject hoPtr;

	public int raAnimForced;

	public int raAnimDirForced;

	public int raAnimSpeedForced;

	public bool raAnimStopped;

	public int raAnimOn;

	public CAnim raAnimOffset;

	public int raAnimDir;

	public int raAnimPreviousDir;

	public CAnimDir raAnimDirOffset;

	public int raAnimSpeed;

	public int raAnimMinSpeed;

	public int raAnimMaxSpeed;

	public int raAnimDeltaSpeed;

	public int raAnimCounter;

	public int raAnimDelta;

	public int raAnimRepeat;

	public int raAnimRepeatLoop;

	public int raAnimFrame;

	public int raAnimNumberOfFrame;

	public int raAnimFrameForced;

	public int raRoutineAnimation;

	public int raOldAngle = -1;

	public void init(CObject ho)
	{
		hoPtr = ho;
		raRoutineAnimation = 0;
		init_Animation(1);
		if (anim_Exist(3))
		{
			raRoutineAnimation = 1;
			animation_Force(3);
			animation_OneLoop();
			animations();
			return;
		}
		int i;
		for (i = 0; anim_Defined[i] >= 0 && !anim_Exist(anim_Defined[i]); i++)
		{
		}
		if (anim_Defined[i] < 0 && anim_Exist(4))
		{
			raRoutineAnimation = 2;
			animation_Force(4);
			animation_OneLoop();
			animations();
		}
	}

	public void init_Animation(int anim)
	{
		hoPtr.roc.rcAnim = anim;
		raAnimStopped = false;
		raAnimForced = 0;
		raAnimDirForced = 0;
		raAnimSpeedForced = 0;
		raAnimFrameForced = 0;
		raAnimCounter = 0;
		raAnimFrame = 0;
		raAnimOffset = null;
		raAnimDirOffset = null;
		raAnimOn = -1;
		raAnimMinSpeed = -1;
		raAnimPreviousDir = -1;
		raAnimOffset = null;
		raAnimDirOffset = null;
		animations();
	}

	private void check_Animate()
	{
		animIn(0);
	}

	public void extAnimations(int anim)
	{
		hoPtr.roc.rcAnim = anim;
		animate();
	}

	public bool animate()
	{
		switch (raRoutineAnimation)
		{
		case 0:
			return animations();
		case 1:
			anim_Appear();
			break;
		case 2:
			anim_Disappear();
			break;
		}
		return false;
	}

	public bool animations()
	{
		int hoX = hoPtr.hoX;
		hoPtr.roc.rcOldX = hoX;
		hoX -= hoPtr.hoImgXSpot;
		hoPtr.roc.rcOldX1 = hoX;
		hoX += hoPtr.hoImgWidth;
		hoPtr.roc.rcOldX2 = hoX;
		int hoY = hoPtr.hoY;
		hoPtr.roc.rcOldY = hoY;
		hoY -= hoPtr.hoImgYSpot;
		hoPtr.roc.rcOldY1 = hoY;
		hoY += hoPtr.hoImgHeight;
		hoPtr.roc.rcOldY2 = hoY;
		hoPtr.roc.rcOldImage = hoPtr.roc.rcImage;
		hoPtr.roc.rcOldAngle = hoPtr.roc.rcAngle;
		return animIn(1);
	}

	public bool animIn(int vbl)
	{
		CRun.bMoveChanged = false;
		CObjectCommon hoCommon = hoPtr.hoCommon;
		int num = hoPtr.roc.rcSpeed;
		int num2 = hoPtr.roc.rcAnim;
		if (raAnimSpeedForced != 0)
		{
			num = raAnimSpeedForced - 1;
		}
		if (num2 == 1)
		{
			if (num == 0)
			{
				num2 = 0;
			}
			if (num >= 75)
			{
				num2 = 2;
			}
		}
		if (raAnimForced != 0)
		{
			num2 = raAnimForced - 1;
		}
		if (num2 != raAnimOn)
		{
			raAnimOn = num2;
			if (num2 >= hoCommon.ocAnimations.ahAnimMax)
			{
				num2 = hoCommon.ocAnimations.ahAnimMax - 1;
			}
			CAnim cAnim = hoCommon.ocAnimations.ahAnims[num2];
			if (cAnim != raAnimOffset)
			{
				raAnimOffset = cAnim;
				raAnimDir = -1;
				raAnimFrame = 0;
			}
		}
		int num3 = hoPtr.roc.rcDir;
		if (raAnimDirForced != 0)
		{
			num3 = raAnimDirForced - 1;
		}
		bool flag = false;
		CAnimDir cAnimDir;
		if (raAnimDir != num3)
		{
			raAnimDir = num3;
			cAnimDir = raAnimOffset.anDirs[num3];
			if (cAnimDir == null)
			{
				if ((raAnimOffset.anAntiTrigo[num3] & 0x40) != 0)
				{
					num3 = raAnimOffset.anAntiTrigo[num3] & 0x3F;
				}
				else if ((raAnimOffset.anTrigo[num3] & 0x40) != 0)
				{
					num3 = raAnimOffset.anTrigo[num3] & 0x3F;
				}
				else
				{
					int num4 = num3;
					if (raAnimPreviousDir < 0)
					{
						num3 = raAnimOffset.anTrigo[num3] & 0x3F;
					}
					else
					{
						num3 -= raAnimPreviousDir;
						num3 &= 0x1F;
						num3 = ((num3 <= 15) ? (raAnimOffset.anAntiTrigo[num4] & 0x3F) : (raAnimOffset.anTrigo[num4] & 0x3F));
					}
				}
				cAnimDir = raAnimOffset.anDirs[num3];
			}
			else
			{
				raAnimPreviousDir = num3;
				cAnimDir = raAnimOffset.anDirs[num3];
			}
			if (raAnimOffset.anDirs[0] != null && (hoPtr.hoCommon.ocFlags2 & 0x40) != 0)
			{
				hoPtr.roc.rcAngle = raAnimDir * 360 / 32;
				cAnimDir = raAnimOffset.anDirs[0];
				raAnimDirOffset = null;
				flag = true;
			}
			if (raAnimDirOffset != cAnimDir)
			{
				raAnimDirOffset = cAnimDir;
				raAnimRepeat = cAnimDir.adRepeat;
				raAnimRepeatLoop = cAnimDir.adRepeatFrame;
				int adMinSpeed = cAnimDir.adMinSpeed;
				int adMaxSpeed = cAnimDir.adMaxSpeed;
				if (adMinSpeed != raAnimMinSpeed || adMaxSpeed != raAnimMaxSpeed)
				{
					raAnimMinSpeed = adMinSpeed;
					raAnimMaxSpeed = adMaxSpeed;
					adMaxSpeed -= adMinSpeed;
					raAnimDeltaSpeed = adMaxSpeed;
					raAnimDelta = adMinSpeed;
					raAnimSpeed = -1;
				}
				raAnimNumberOfFrame = cAnimDir.adNumberOfFrame;
				if (raAnimFrameForced != 0 && raAnimFrameForced - 1 >= raAnimNumberOfFrame)
				{
					raAnimFrameForced = 0;
				}
				if (raAnimFrame >= raAnimNumberOfFrame)
				{
					raAnimFrame = 0;
				}
				short num5 = cAnimDir.adFrames[raAnimFrame];
				if (!raAnimStopped)
				{
					hoPtr.roc.rcImage = num5;
					CImage imageInfoEx = hoPtr.hoAdRunHeader.rhApp.imageBank.getImageInfoEx(num5, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY);
					hoPtr.hoImgWidth = imageInfoEx.width;
					hoPtr.hoImgHeight = imageInfoEx.height;
					hoPtr.hoImgXSpot = imageInfoEx.xSpot;
					hoPtr.hoImgYSpot = imageInfoEx.ySpot;
					hoPtr.roc.rcChanged = true;
					hoPtr.roc.rcCheckCollides = true;
				}
				if (raAnimNumberOfFrame == 1)
				{
					if (raAnimMinSpeed == 0)
					{
						raAnimNumberOfFrame = 0;
					}
					num5 = hoPtr.roc.rcImage;
					if (num5 == 0)
					{
						return false;
					}
					CImage imageInfoEx2 = hoPtr.hoAdRunHeader.rhApp.imageBank.getImageInfoEx(num5, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY);
					hoPtr.hoImgWidth = imageInfoEx2.width;
					hoPtr.hoImgHeight = imageInfoEx2.height;
					hoPtr.hoImgXSpot = imageInfoEx2.xSpot;
					hoPtr.hoImgYSpot = imageInfoEx2.ySpot;
					return false;
				}
			}
		}
		if (vbl == 0 && raAnimFrameForced == 0)
		{
			return false;
		}
		if (!flag && raAnimNumberOfFrame == 0)
		{
			return false;
		}
		int num6 = raAnimDeltaSpeed;
		if (num != raAnimSpeed)
		{
			raAnimSpeed = num;
			if (num6 == 0)
			{
				raAnimDelta = raAnimMinSpeed;
				if (raAnimSpeedForced != 0)
				{
					raAnimDelta = raAnimSpeedForced - 1;
				}
			}
			else
			{
				int num7 = hoPtr.roc.rcMaxSpeed - hoPtr.roc.rcMinSpeed;
				if (num7 == 0)
				{
					if (raAnimSpeedForced != 0)
					{
						num6 *= num;
						num6 /= 100;
						num6 += raAnimMinSpeed;
						if (num6 > raAnimMaxSpeed)
						{
							num6 = raAnimMaxSpeed;
						}
						raAnimDelta = num6;
					}
					else
					{
						num6 /= 2;
						num6 += raAnimMinSpeed;
						raAnimDelta = num6;
					}
				}
				else
				{
					num6 *= num;
					num6 /= num7;
					num6 += raAnimMinSpeed;
					if (num6 > raAnimMaxSpeed)
					{
						num6 = raAnimMaxSpeed;
					}
					raAnimDelta = num6;
				}
			}
		}
		cAnimDir = raAnimDirOffset;
		int num8 = raAnimFrameForced;
		if (num8 == 0)
		{
			if (raAnimDelta == 0)
			{
				return false;
			}
			if (raAnimStopped)
			{
				return false;
			}
			int num9 = raAnimCounter;
			num8 = raAnimFrame;
			int num10 = raAnimDelta;
			if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
			{
				num10 = (int)((double)num10 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
			}
			num9 += num10;
			while (num9 > 100)
			{
				num9 -= 100;
				num8++;
				if (num8 < raAnimNumberOfFrame)
				{
					continue;
				}
				num8 = raAnimRepeatLoop;
				if (raAnimRepeat == 0)
				{
					continue;
				}
				raAnimRepeat--;
				if (raAnimRepeat == 0)
				{
					raAnimFrame = raAnimNumberOfFrame;
					raAnimNumberOfFrame = 0;
					if (raAnimForced != 0)
					{
						raAnimForced = 0;
						raAnimDirForced = 0;
						raAnimSpeedForced = 0;
					}
					if ((hoPtr.hoAdRunHeader.rhGameFlags & 0x200) != 0)
					{
						return false;
					}
					if (flag)
					{
						hoPtr.roc.rcChanged = true;
						hoPtr.roc.rcCheckCollides = true;
						CImage imageInfoEx3 = hoPtr.hoAdRunHeader.rhApp.imageBank.getImageInfoEx(hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY);
						hoPtr.hoImgWidth = imageInfoEx3.width;
						hoPtr.hoImgHeight = imageInfoEx3.height;
						hoPtr.hoImgXSpot = imageInfoEx3.xSpot;
						hoPtr.hoImgYSpot = imageInfoEx3.ySpot;
					}
					int num11 = -131072;
					num11 |= hoPtr.hoType & 0xFFFF;
					hoPtr.hoAdRunHeader.rhEvtProg.rhCurParam0 = hoPtr.roa.raAnimOn;
					return hoPtr.hoAdRunHeader.rhEvtProg.handle_Event(hoPtr, num11);
				}
			}
			raAnimCounter = num9;
		}
		else
		{
			num8--;
			if (num8 < 0)
			{
				num8 = 0;
			}
		}
		raAnimFrame = num8;
		short num12 = cAnimDir.adFrames[num8];
		hoPtr.roc.rcChanged = true;
		hoPtr.roc.rcCheckCollides = true;
		if (num12 != hoPtr.roc.rcImage || raOldAngle != hoPtr.roc.rcAngle)
		{
			hoPtr.roc.rcImage = num12;
			raOldAngle = hoPtr.roc.rcAngle;
			if (num12 < 0)
			{
				return false;
			}
			CImage imageInfoEx4 = hoPtr.hoAdRunHeader.rhApp.imageBank.getImageInfoEx(num12, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY);
			hoPtr.hoImgWidth = imageInfoEx4.width;
			hoPtr.hoImgHeight = imageInfoEx4.height;
			hoPtr.hoImgXSpot = imageInfoEx4.xSpot;
			hoPtr.hoImgYSpot = imageInfoEx4.ySpot;
		}
		return false;
	}

	public bool anim_Exist(int animId)
	{
		CAnimHeader ocAnimations = hoPtr.hoCommon.ocAnimations;
		if (ocAnimations.ahAnimExists[animId] == 0)
		{
			return false;
		}
		return true;
	}

	public void animation_OneLoop()
	{
		if (raAnimRepeat == 0)
		{
			raAnimRepeat = 1;
		}
	}

	public void animation_Force(int anim)
	{
		raAnimForced = anim + 1;
		animIn(0);
	}

	public void animation_Restore()
	{
		raAnimForced = 0;
		animIn(0);
	}

	public void animDir_Force(int dir)
	{
		dir &= 0x1F;
		raAnimDirForced = dir + 1;
		animIn(0);
	}

	public void animDir_Restore()
	{
		raAnimDirForced = 0;
		animIn(0);
	}

	public void animSpeed_Force(int speed)
	{
		if (speed < 0)
		{
			speed = 0;
		}
		if (speed > 100)
		{
			speed = 100;
		}
		raAnimSpeedForced = speed + 1;
		animIn(0);
	}

	public void animSpeed_Restore()
	{
		raAnimSpeedForced = 0;
		animIn(0);
	}

	public void anim_Restart()
	{
		raAnimOn = -1;
		animIn(0);
	}

	public void animFrame_Force(int frame)
	{
		if (frame >= raAnimNumberOfFrame)
		{
			frame = raAnimNumberOfFrame - 1;
		}
		if (frame < 0)
		{
			frame = 0;
		}
		raAnimFrameForced = frame + 1;
		animIn(0);
	}

	public void animFrame_Restore()
	{
		raAnimFrameForced = 0;
		animIn(0);
	}

	public void anim_Appear()
	{
		animIn(1);
		if (raAnimForced != 4)
		{
			if (anim_Exist(0) || anim_Exist(1) || anim_Exist(2))
			{
				raRoutineAnimation = 0;
				animation_Restore();
			}
			else
			{
				raRoutineAnimation = 2;
				hoPtr.hoAdRunHeader.init_Disappear(hoPtr);
			}
		}
	}

	private void anim_Disappear()
	{
		if ((hoPtr.hoFlags & 0x10) == 0)
		{
			animIn(1);
			if (raAnimForced != 5)
			{
				hoPtr.hoAdRunHeader.destroy_Add(hoPtr.hoNumber);
			}
		}
	}
}
