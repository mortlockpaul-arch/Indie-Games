using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Movements;

internal class CMoveGeneric : CMove
{
	public int MG_Bounce;

	public int MG_OkDirs;

	public int MG_BounceMu;

	public int MG_Speed;

	public int MG_LastBounce;

	public int MG_DirMask;

	public override void init(CObject ho, CMoveDef mvPtr)
	{
		hoPtr = ho;
		CMoveDefGeneric cMoveDefGeneric = (CMoveDefGeneric)mvPtr;
		hoPtr.hoCalculX = 0;
		hoPtr.hoCalculY = 0;
		MG_Speed = 0;
		hoPtr.roc.rcSpeed = 0;
		MG_Bounce = 0;
		MG_LastBounce = -1;
		hoPtr.roc.rcPlayer = mvPtr.mvControl;
		rmAcc = cMoveDefGeneric.mgAcc;
		rmAccValue = getAccelerator(rmAcc);
		rmDec = cMoveDefGeneric.mgDec;
		rmDecValue = getAccelerator(rmDec);
		hoPtr.roc.rcMaxSpeed = cMoveDefGeneric.mgSpeed;
		hoPtr.roc.rcMinSpeed = 0;
		MG_BounceMu = cMoveDefGeneric.mgBounceMult;
		MG_OkDirs = cMoveDefGeneric.mgDir;
		rmOpt = cMoveDefGeneric.mvOpt;
		hoPtr.roc.rcChanged = true;
	}

	public override void move()
	{
		hoPtr.hoAdRunHeader.rhVBLObjet = 1;
		int num = hoPtr.roc.rcDir;
		hoPtr.roc.rcOldDir = num;
		if (MG_Bounce == 0)
		{
			hoPtr.rom.rmBouncing = false;
			int num2 = 0;
			int num3 = hoPtr.hoAdRunHeader.rhPlayer[hoPtr.roc.rcPlayer - 1] & 0xF;
			if (num3 != 0)
			{
				int num4 = CMove.Joy2Dir[num3];
				if (num4 != -1)
				{
					int num5 = 1 << num4;
					if ((num5 & MG_OkDirs) != 0)
					{
						num2 = 1;
						num = num4;
					}
				}
			}
			int num6 = MG_Speed;
			if (num2 == 0)
			{
				if (num6 != 0)
				{
					int num7 = rmDecValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
					{
						num7 = (int)((double)num7 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					}
					num6 -= num7;
					if (num6 <= 0)
					{
						num6 = 0;
					}
				}
			}
			else
			{
				int num8 = num6 >> 8;
				if (num8 < hoPtr.roc.rcMaxSpeed)
				{
					int num7 = rmAccValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
					{
						num7 = (int)((double)num7 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					}
					num6 += num7;
					num8 = num6 >> 8;
					if (num8 > hoPtr.roc.rcMaxSpeed)
					{
						num6 = hoPtr.roc.rcMaxSpeed << 8;
					}
				}
			}
			MG_Speed = num6;
			hoPtr.roc.rcSpeed = num6 >> 8;
			hoPtr.roc.rcDir = num;
			hoPtr.roc.rcAnim = 1;
			if (hoPtr.roa != null)
			{
				hoPtr.roa.animate();
				if (CRun.bMoveChanged)
				{
					return;
				}
			}
			if (!newMake_Move(hoPtr.roc.rcSpeed, hoPtr.roc.rcDir) || CRun.bMoveChanged)
			{
				return;
			}
			if (hoPtr.roc.rcSpeed == 0)
			{
				num6 = MG_Speed;
				if (num6 == 0 || hoPtr.roc.rcOldDir == hoPtr.roc.rcDir)
				{
					return;
				}
				hoPtr.roc.rcSpeed = num6 >> 8;
				hoPtr.roc.rcDir = hoPtr.roc.rcOldDir;
				if (!newMake_Move(hoPtr.roc.rcSpeed, hoPtr.roc.rcDir) || CRun.bMoveChanged)
				{
					return;
				}
			}
		}
		while (true)
		{
			if (MG_Bounce == 0 || hoPtr.hoAdRunHeader.rhVBLObjet == 0)
			{
				return;
			}
			int num6 = MG_Speed;
			num6 -= rmDecValue;
			if (num6 <= 0)
			{
				break;
			}
			MG_Speed = num6;
			num6 >>= 8;
			hoPtr.roc.rcSpeed = num6;
			int num4 = hoPtr.roc.rcDir;
			if (MG_Bounce != 0)
			{
				num4 += 16;
				num4 &= 0x1F;
			}
			if (!newMake_Move(num6, num4) || CRun.bMoveChanged)
			{
				return;
			}
		}
		MG_Speed = 0;
		hoPtr.roc.rcSpeed = 0;
		MG_Bounce = 0;
	}

	public override void bounce()
	{
		if (rmCollisionCount == hoPtr.hoAdRunHeader.rh3CollisionCount)
		{
			mv_Approach((rmOpt & 1) != 0);
		}
		if (hoPtr.hoAdRunHeader.rhLoopCount != MG_LastBounce)
		{
			MG_LastBounce = hoPtr.hoAdRunHeader.rhLoopCount;
			MG_Bounce++;
			if (MG_Bounce >= 12)
			{
				stop();
				return;
			}
			hoPtr.rom.rmBouncing = true;
			hoPtr.rom.rmMoveFlag = true;
		}
	}

	public override void stop()
	{
		hoPtr.roc.rcSpeed = 0;
		MG_Bounce = 0;
		MG_Speed = 0;
		hoPtr.rom.rmMoveFlag = true;
		if (rmCollisionCount == hoPtr.hoAdRunHeader.rh3CollisionCount)
		{
			mv_Approach((rmOpt & 1) != 0);
			MG_Bounce = 0;
		}
	}

	public override void start()
	{
		hoPtr.rom.rmMoveFlag = true;
		rmStopSpeed = 0;
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
		if (hoPtr.roc.rcSpeed > speed)
		{
			hoPtr.roc.rcSpeed = speed;
			MG_Speed = speed << 8;
		}
		hoPtr.rom.rmMoveFlag = true;
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
		MG_Speed = speed << 8;
		hoPtr.rom.rmMoveFlag = true;
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

	public void set8Dir(int dirs)
	{
		MG_OkDirs = dirs;
	}
}
