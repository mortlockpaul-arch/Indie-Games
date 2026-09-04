using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Movements;

internal class CMoveBall : CMove
{
	public int MB_StartDir;

	public int MB_Angles;

	public int MB_Securite;

	public int MB_SecuCpt;

	public int MB_Bounce;

	public int MB_Speed;

	public int MB_MaskBounce;

	public int MB_LastBounce;

	public bool MB_Blocked;

	private static short[] rebond_List = new short[512]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
		10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
		20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
		30, 31, 30, 31, 0, 1, 4, 3, 2, 1,
		0, 31, 30, 29, 28, 27, 26, 25, 24, 23,
		22, 21, 20, 24, 25, 26, 27, 27, 28, 28,
		28, 28, 29, 29, 24, 23, 22, 21, 20, 19,
		18, 17, 16, 15, 14, 13, 12, 16, 17, 18,
		19, 19, 20, 20, 20, 20, 21, 21, 22, 23,
		24, 25, 28, 27, 26, 25, 0, 31, 30, 29,
		28, 27, 26, 25, 24, 23, 22, 21, 20, 19,
		18, 17, 16, 20, 21, 22, 22, 23, 24, 24,
		24, 24, 25, 26, 27, 28, 29, 30, 8, 7,
		6, 5, 4, 8, 9, 10, 11, 11, 12, 12,
		12, 12, 13, 13, 14, 15, 16, 17, 20, 19,
		18, 17, 16, 15, 14, 13, 12, 11, 10, 9,
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
		10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
		20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
		30, 31, 16, 15, 14, 13, 12, 11, 10, 9,
		8, 12, 13, 14, 15, 15, 16, 16, 16, 16,
		17, 17, 18, 19, 20, 21, 24, 23, 22, 21,
		20, 19, 18, 17, 16, 17, 18, 19, 20, 21,
		22, 23, 24, 23, 22, 21, 20, 19, 18, 17,
		16, 17, 18, 19, 20, 21, 22, 23, 24, 23,
		22, 21, 20, 19, 18, 17, 3, 3, 4, 4,
		4, 4, 5, 5, 6, 7, 8, 9, 12, 11,
		10, 9, 8, 7, 6, 5, 4, 3, 2, 1,
		0, 31, 30, 29, 28, 0, 1, 2, 0, 0,
		1, 1, 2, 3, 4, 5, 8, 7, 6, 5,
		4, 3, 2, 1, 0, 31, 30, 29, 28, 27,
		26, 25, 24, 28, 29, 30, 31, 31, 0, 0,
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
		10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
		20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
		30, 31, 0, 31, 30, 29, 28, 27, 26, 25,
		24, 25, 26, 27, 28, 29, 30, 31, 0, 31,
		30, 29, 28, 27, 25, 25, 24, 25, 26, 27,
		28, 29, 30, 31, 0, 4, 5, 6, 7, 7,
		8, 8, 8, 8, 9, 9, 10, 11, 12, 13,
		16, 15, 14, 13, 12, 11, 10, 9, 8, 7,
		6, 5, 4, 3, 2, 1, 0, 1, 2, 3,
		4, 5, 6, 7, 8, 7, 6, 5, 4, 3,
		2, 1, 0, 1, 2, 3, 4, 5, 6, 7,
		8, 7, 6, 5, 4, 3, 2, 1, 16, 15,
		14, 13, 12, 11, 10, 9, 8, 9, 10, 11,
		12, 13, 14, 15, 16, 15, 14, 13, 12, 11,
		10, 9, 8, 9, 10, 11, 12, 13, 14, 15,
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
		10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
		20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
		30, 31
	};

	private static uint[] MaskBounce = new uint[3] { 4294967292u, 4294967294u, 4294967295u };

	private static int[] PlusAngles = new int[6] { -4, 4, -2, 2, -1, 1 };

	private static int[] PlusAnglesTry = new int[6] { -4, 4, -4, 4, -4, 4 };

	public override void init(CObject ho, CMoveDef mvPtr)
	{
		hoPtr = ho;
		CMoveDefBall cMoveDefBall = (CMoveDefBall)mvPtr;
		hoPtr.hoCalculX = 0;
		hoPtr.hoCalculY = 0;
		hoPtr.roc.rcSpeed = cMoveDefBall.mbSpeed;
		hoPtr.roc.rcMaxSpeed = cMoveDefBall.mbSpeed;
		hoPtr.roc.rcMinSpeed = cMoveDefBall.mbSpeed;
		MB_Speed = cMoveDefBall.mbSpeed << 8;
		int num = cMoveDefBall.mbDecelerate;
		if (num != 0)
		{
			num = getAccelerator(num);
			hoPtr.roc.rcMinSpeed = 0;
		}
		rmDecValue = num;
		MB_Bounce = cMoveDefBall.mbBounce;
		MB_Angles = cMoveDefBall.mbAngles;
		MB_MaskBounce = (int)MaskBounce[MB_Angles];
		MB_Blocked = false;
		MB_LastBounce = -1;
		MB_Securite = (100 - cMoveDefBall.mbSecurity) / 8;
		MB_SecuCpt = MB_Securite;
		moveAtStart(mvPtr);
		hoPtr.roc.rcChanged = true;
	}

	public override void move()
	{
		hoPtr.rom.rmBouncing = false;
		hoPtr.hoAdRunHeader.rhVBLObjet = 1;
		hoPtr.roc.rcAnim = 1;
		if (hoPtr.roa != null)
		{
			hoPtr.roa.animate();
		}
		if (CRun.bMoveChanged)
		{
			return;
		}
		if (rmDecValue != 0)
		{
			int mB_Speed = MB_Speed;
			if (mB_Speed > 0)
			{
				int num = rmDecValue;
				if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
				{
					num = (int)((double)num * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
				}
				mB_Speed -= num;
				if (mB_Speed < 0)
				{
					mB_Speed = 0;
				}
				MB_Speed = mB_Speed;
				mB_Speed >>= 8;
				hoPtr.roc.rcSpeed = mB_Speed;
			}
		}
		newMake_Move(hoPtr.roc.rcSpeed, hoPtr.roc.rcDir);
	}

	public override void stop()
	{
		if (rmStopSpeed == 0)
		{
			rmStopSpeed = hoPtr.roc.rcSpeed | 0x8000;
			hoPtr.roc.rcSpeed = 0;
			MB_Speed = 0;
			hoPtr.rom.rmMoveFlag = true;
		}
	}

	public override void start()
	{
		int num = rmStopSpeed;
		if (num != 0)
		{
			num &= 0x7FFF;
			hoPtr.roc.rcSpeed = num;
			MB_Speed = num << 8;
			rmStopSpeed = 0;
			hoPtr.rom.rmMoveFlag = true;
		}
	}

	public override void bounce()
	{
		if (rmStopSpeed != 0 || hoPtr.hoAdRunHeader.rhLoopCount == MB_LastBounce)
		{
			return;
		}
		MB_LastBounce = hoPtr.hoAdRunHeader.rhLoopCount;
		if (rmCollisionCount == hoPtr.hoAdRunHeader.rh3CollisionCount)
		{
			mb_Approach(MB_Blocked);
		}
		int hoX = hoPtr.hoX;
		int hoY = hoPtr.hoY;
		int num = 0;
		hoX -= 8;
		hoY -= 8;
		if (!tst_Position(hoX, hoY, MB_Blocked))
		{
			num |= 1;
		}
		hoX += 16;
		if (!tst_Position(hoX, hoY, MB_Blocked))
		{
			num |= 2;
		}
		hoY += 16;
		if (!tst_Position(hoX, hoY, MB_Blocked))
		{
			num |= 4;
		}
		hoX -= 16;
		if (!tst_Position(hoX, hoY, MB_Blocked))
		{
			num |= 8;
		}
		int num2 = rebond_List[num * 32 + hoPtr.roc.rcDir];
		num2 &= MB_MaskBounce;
		if (!mvb_Test(num2))
		{
			int num3 = PlusAnglesTry[MB_Angles * 2 + 1];
			int num4 = num3;
			bool flag = false;
			do
			{
				num2 -= num3;
				num2 &= 0x1F;
				if (mvb_Test(num2))
				{
					flag = true;
					break;
				}
				num2 += 2 * num3;
				num2 &= 0x1F;
				if (mvb_Test(num2))
				{
					flag = true;
					break;
				}
				num2 -= num3;
				num2 &= 0x1F;
				num3 += num4;
			}
			while (num3 <= 16);
			if (!flag)
			{
				MB_Blocked = true;
				hoPtr.roc.rcDir = hoPtr.hoAdRunHeader.random(32) & MB_MaskBounce;
				hoPtr.rom.rmBouncing = true;
				hoPtr.rom.rmMoveFlag = true;
				return;
			}
		}
		MB_Blocked = false;
		hoPtr.roc.rcDir = num2;
		int num5 = hoPtr.hoAdRunHeader.random(100);
		if (num5 < MB_Bounce)
		{
			num5 >>= 2;
			if (num5 < 25)
			{
				num5 -= 12;
				num5 &= 0x1F;
				num5 &= MB_MaskBounce;
				if (mvb_Test(num5))
				{
					hoPtr.roc.rcDir = num5;
					hoPtr.rom.rmBouncing = true;
					hoPtr.rom.rmMoveFlag = true;
					return;
				}
			}
		}
		num2 = hoPtr.roc.rcDir & 7;
		if (MB_SecuCpt != 12)
		{
			if (num2 == 0)
			{
				MB_SecuCpt--;
				if (MB_SecuCpt < 0)
				{
					num2 = hoPtr.roc.rcDir + PlusAngles[hoPtr.hoAdRunHeader.random(2) + MB_Angles * 2];
					num2 &= 0x1F;
					if (mvb_Test(num2))
					{
						hoPtr.roc.rcDir = num2;
						MB_SecuCpt = MB_Securite;
					}
				}
			}
			else
			{
				MB_SecuCpt = MB_Securite;
			}
		}
		hoPtr.rom.rmBouncing = true;
		hoPtr.rom.rmMoveFlag = true;
	}

	private bool mvb_Test(int dir)
	{
		int num = (hoPtr.hoX << 16) | (hoPtr.hoCalculX & 0xFFFF);
		int num2 = (hoPtr.hoY << 16) | (hoPtr.hoCalculY & 0xFFFF);
		int num3 = (CMove.Cosinus32[dir] << 11) + num;
		int num4 = (CMove.Sinus32[dir] << 11) + num2;
		num3 = (num3 >> 16) & 0xFFFF;
		num4 = (num4 >> 16) & 0xFFFF;
		return tst_Position(num3, num4, flag: false);
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
		hoPtr.roc.rcSpeed = speed;
		MB_Speed = speed << 8;
		rmStopSpeed = 0;
		hoPtr.rom.rmMoveFlag = true;
	}

	public override void setMaxSpeed(int speed)
	{
		setSpeed(speed);
	}

	public override void reverse()
	{
		if (rmStopSpeed == 0)
		{
			hoPtr.rom.rmMoveFlag = true;
			hoPtr.roc.rcDir += 16;
			hoPtr.roc.rcDir &= 31;
		}
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
}
