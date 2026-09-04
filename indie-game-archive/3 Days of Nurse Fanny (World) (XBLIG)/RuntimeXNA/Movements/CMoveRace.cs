using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Movements;

internal class CMoveRace : CMove
{
	public int MR_Bounce;

	public int MR_BounceMu;

	public int MR_Speed;

	public int MR_RotSpeed;

	public int MR_RotCpt;

	public int MR_RotPos;

	public int MR_RotMask;

	public int MR_OkReverse;

	public int MR_OldJoy;

	public int MR_LastBounce;

	public static uint[] RaceMask = new uint[4] { 4294967288u, 4294967292u, 4294967294u, 4294967295u };

	public override void init(CObject ho, CMoveDef mvPtr)
	{
		hoPtr = ho;
		CMoveDefRace cMoveDefRace = (CMoveDefRace)mvPtr;
		MR_Speed = 0;
		hoPtr.roc.rcSpeed = 0;
		MR_Bounce = 0;
		MR_LastBounce = -1;
		hoPtr.roc.rcPlayer = cMoveDefRace.mvControl;
		rmAcc = cMoveDefRace.mrAcc;
		rmAccValue = getAccelerator(cMoveDefRace.mrAcc);
		rmDec = cMoveDefRace.mrDec;
		rmDecValue = getAccelerator(cMoveDefRace.mrDec);
		hoPtr.roc.rcMaxSpeed = cMoveDefRace.mrSpeed;
		hoPtr.roc.rcMinSpeed = 0;
		MR_BounceMu = cMoveDefRace.mrBounceMult;
		MR_OkReverse = cMoveDefRace.mrOkReverse;
		hoPtr.rom.rmReverse = 0;
		MR_OldJoy = 0;
		rmOpt = cMoveDefRace.mvOpt;
		MR_RotMask = (int)RaceMask[cMoveDefRace.mrAngles];
		MR_RotSpeed = cMoveDefRace.mrRot;
		MR_RotCpt = 0;
		MR_RotPos = hoPtr.roc.rcDir;
		hoPtr.hoCalculX = 0;
		hoPtr.hoCalculY = 0;
		moveAtStart(mvPtr);
		hoPtr.roc.rcChanged = true;
	}

	public override void move()
	{
		hoPtr.hoAdRunHeader.rhVBLObjet = 1;
		if (MR_Bounce == 0)
		{
			hoPtr.rom.rmBouncing = false;
			int num = hoPtr.hoAdRunHeader.rhPlayer[hoPtr.roc.rcPlayer - 1] & 0xF;
			int num2 = 0;
			if ((num & 8) != 0)
			{
				num2 = -1;
			}
			if ((num & 4) != 0)
			{
				num2 = 1;
			}
			if (num2 != 0)
			{
				int num3 = MR_RotSpeed;
				if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
				{
					num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
				}
				MR_RotCpt += num3;
				while (MR_RotCpt > 100)
				{
					MR_RotCpt -= 100;
					MR_RotPos += num2;
					MR_RotPos &= 31;
					hoPtr.roc.rcDir = MR_RotPos & MR_RotMask;
				}
				hoPtr.roc.rcChanged = true;
			}
			int num4 = 0;
			if (hoPtr.rom.rmReverse != 0)
			{
				if ((num & 1) != 0)
				{
					num4 = 1;
				}
				if ((num & 2) != 0)
				{
					num4 = 2;
				}
			}
			else
			{
				if ((num & 1) != 0)
				{
					num4 = 2;
				}
				if ((num & 2) != 0)
				{
					num4 = 1;
				}
			}
			int mR_Speed = MR_Speed;
			if ((num4 & 1) != 0)
			{
				if (MR_Speed == 0)
				{
					if (MR_OkReverse != 0 && (MR_OldJoy & 3) == 0)
					{
						hoPtr.rom.rmReverse ^= 1;
						int num3 = rmAccValue;
						if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
						{
							num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
						}
						mR_Speed += num3;
						int num5 = mR_Speed >> 8;
						if (num5 > hoPtr.roc.rcMaxSpeed)
						{
							mR_Speed = (MR_Speed = hoPtr.roc.rcMaxSpeed << 8);
						}
						MR_Speed = mR_Speed;
					}
				}
				else
				{
					int num3 = rmDecValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
					{
						num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					}
					mR_Speed -= num3;
					if (mR_Speed < 0)
					{
						mR_Speed = 0;
					}
					MR_Speed = mR_Speed;
				}
			}
			else if ((num4 & 2) != 0)
			{
				int num3 = rmAccValue;
				if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
				{
					num3 = (int)((double)num3 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
				}
				mR_Speed += num3;
				int num5 = mR_Speed >> 8;
				if (num5 > hoPtr.roc.rcMaxSpeed)
				{
					mR_Speed = (MR_Speed = hoPtr.roc.rcMaxSpeed << 8);
				}
				MR_Speed = mR_Speed;
			}
			MR_OldJoy = num;
			hoPtr.roc.rcSpeed = MR_Speed >> 8;
			hoPtr.roc.rcAnim = 1;
			if (hoPtr.roa != null)
			{
				hoPtr.roa.animate();
				if (CRun.bMoveChanged)
				{
					return;
				}
			}
			int num6 = hoPtr.roc.rcDir;
			if (hoPtr.rom.rmReverse != 0)
			{
				num6 += 16;
				num6 &= 0x1F;
			}
			if (!newMake_Move(hoPtr.roc.rcSpeed, num6) || CRun.bMoveChanged)
			{
				return;
			}
		}
		while (!CRun.bMoveChanged && MR_Bounce != 0 && hoPtr.hoAdRunHeader.rhVBLObjet != 0)
		{
			int mR_Speed = MR_Speed;
			mR_Speed -= rmDecValue;
			if (mR_Speed <= 0)
			{
				MR_Speed = 0;
				MR_Bounce = 0;
				break;
			}
			MR_Speed = mR_Speed;
			mR_Speed >>= 8;
			int num6 = hoPtr.roc.rcDir;
			if (MR_Bounce != 0)
			{
				num6 += 16;
				num6 &= 0x1F;
			}
			if (!newMake_Move(mR_Speed, num6))
			{
				break;
			}
		}
	}

	public override void stop()
	{
		MR_Bounce = 0;
		MR_Speed = 0;
		hoPtr.rom.rmReverse = 0;
		if (rmCollisionCount == hoPtr.hoAdRunHeader.rh3CollisionCount)
		{
			mv_Approach((rmOpt & 1) != 0);
			hoPtr.rom.rmMoveFlag = true;
		}
	}

	public override void start()
	{
		rmStopSpeed = 0;
		hoPtr.rom.rmMoveFlag = true;
	}

	public override void bounce()
	{
		if (rmCollisionCount == hoPtr.hoAdRunHeader.rh3CollisionCount)
		{
			mv_Approach((rmOpt & 1) != 0);
		}
		if (hoPtr.hoAdRunHeader.rhLoopCount != MR_LastBounce)
		{
			MR_Bounce = hoPtr.rom.rmReverse;
			hoPtr.rom.rmReverse = 0;
			MR_Bounce++;
			if (MR_Bounce >= 16)
			{
				stop();
				return;
			}
			hoPtr.rom.rmMoveFlag = true;
			hoPtr.rom.rmBouncing = true;
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
		speed <<= 8;
		MR_Speed = speed;
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
		if (MR_Speed > speed)
		{
			MR_Speed = speed;
		}
		hoPtr.rom.rmMoveFlag = true;
	}

	public void MRSetRotSpeed(int speed)
	{
		MR_RotSpeed = speed;
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

	public override void setDir(int dir)
	{
		MR_RotPos = dir;
		hoPtr.roc.rcDir = dir & MR_RotMask;
	}
}
