using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements;

public class CMove
{
	public const int MVTOPT_8DIR_STICK = 1;

	public static int[] Cosinus32 = new int[32]
	{
		256, 251, 236, 212, 181, 142, 97, 49, 0, -49,
		-97, -142, -181, -212, -236, -251, -256, -251, -236, -212,
		-181, -142, -97, -49, 0, 49, 97, 142, 181, 212,
		236, 251
	};

	public static int[] Sinus32 = new int[32]
	{
		0, -49, -97, -142, -181, -212, -236, -251, -256, -251,
		-236, -212, -181, -142, -97, -49, 0, 49, 97, 142,
		181, 212, 236, 251, 256, 251, 236, 212, 181, 142,
		97, 49
	};

	public static short[] accelerators = new short[101]
	{
		2, 3, 4, 6, 8, 10, 12, 16, 20, 24,
		48, 56, 64, 72, 80, 88, 96, 104, 112, 120,
		144, 160, 176, 192, 208, 224, 240, 256, 272, 288,
		320, 336, 352, 368, 384, 400, 416, 432, 448, 480,
		512, 544, 560, 592, 624, 640, 672, 688, 720, 736,
		768, 784, 816, 848, 864, 896, 928, 944, 976, 992,
		1024, 1120, 1216, 1312, 1440, 1536, 1632, 1728, 1824, 1952,
		2048, 2240, 2432, 2688, 2880, 3072, 3264, 3456, 3712, 3904,
		4096, 6544, 4914, 5216, 5732, 6144, 6553, 6962, 7366, 7780,
		8192, 9836, 11672, 13316, 14960, 16604, 18248, 19892, 21504, 25600,
		25600
	};

	public static sbyte[] Joy2Dir = new sbyte[16]
	{
		-1, 8, 24, -1, 16, 12, 20, 16, 0, 4,
		28, 0, -1, 8, 24, -1
	};

	public static int[] CosSurSin32 = new int[18]
	{
		2599, 0, 844, 31, 479, 30, 312, 29, 210, 28,
		137, 27, 78, 26, 25, 25, 0, 24
	};

	public static int[] mvap_TableDirs = new int[144]
	{
		0, -2, 0, 2, 0, -4, 0, 4, 0, -8,
		0, 8, -4, 0, -8, 0, 0, 0, -2, -2,
		2, 2, -4, -4, 4, 4, -8, -8, 8, 8,
		-4, 4, -8, 8, 0, 0, -2, 0, 2, 0,
		-4, 0, 4, 0, -8, 0, 8, 0, 0, 4,
		0, 8, 0, 0, -2, 2, 2, -2, -4, 4,
		4, -4, -8, 8, 8, -8, 4, 4, 8, 8,
		0, 0, 0, 2, 0, -2, 0, 4, 0, -4,
		0, 8, 0, -8, 4, 0, 8, 0, 0, 0,
		2, 2, -2, -2, 4, 4, -4, -4, 8, 8,
		-8, -8, 4, -4, 8, -8, 0, 0, 2, 0,
		-2, 0, 4, 0, -4, 0, 8, 0, -8, 0,
		0, -4, 0, -8, 0, 0, 2, -2, -2, 2,
		4, -4, -4, 4, 8, -8, -8, 8, -4, -4,
		-8, -8, 0, 0
	};

	public CObject hoPtr;

	public int rmAcc;

	public int rmDec;

	public short rmCollisionCount;

	public int rmStopSpeed;

	public int rmAccValue;

	public int rmDecValue;

	public byte rmOpt;

	public bool newMake_Move(int speed, int angle)
	{
		hoPtr.hoAdRunHeader.rh3CollisionCount++;
		rmCollisionCount = hoPtr.hoAdRunHeader.rh3CollisionCount;
		hoPtr.rom.rmMoveFlag = false;
		if (speed == 0)
		{
			hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
			return false;
		}
		int num;
		for (num = (((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) == 0) ? (speed << 5) : ((int)((double)speed * hoPtr.hoAdRunHeader.rh4MvtTimerCoef * 32.0))); num > 2048; num -= 2048)
		{
			int num2 = (hoPtr.hoX << 16) | (hoPtr.hoCalculX & 0xFFFF);
			int num3 = (hoPtr.hoY << 16) | (hoPtr.hoCalculY & 0xFFFF);
			num2 += Cosinus32[angle] * 2048;
			num3 += Sinus32[angle] * 2048;
			hoPtr.hoCalculX = num2 & 0xFFFF;
			hoPtr.hoX = (short)(num2 >> 16);
			hoPtr.hoCalculY = num3 & 0xFFFF;
			hoPtr.hoY = (short)(num3 >> 16);
			if (hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr))
			{
				return true;
			}
			if (hoPtr.rom.rmMoveFlag)
			{
				break;
			}
		}
		if (!hoPtr.rom.rmMoveFlag)
		{
			int num2 = (hoPtr.hoX << 16) | (hoPtr.hoCalculX & 0xFFFF);
			int num3 = (hoPtr.hoY << 16) | (hoPtr.hoCalculY & 0xFFFF);
			num2 += Cosinus32[angle] * num;
			num3 += Sinus32[angle] * num;
			hoPtr.hoCalculX = num2 & 0xFFFF;
			hoPtr.hoX = (short)(num2 >> 16);
			hoPtr.hoCalculY = num3 & 0xFFFF;
			hoPtr.hoY = (short)(num3 >> 16);
			if (hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr))
			{
				return true;
			}
		}
		hoPtr.roc.rcChanged = true;
		if (!hoPtr.rom.rmMoveFlag)
		{
			hoPtr.hoAdRunHeader.rhVBLObjet = 0;
		}
		return hoPtr.rom.rmMoveFlag;
	}

	public void moveAtStart(CMoveDef mvPtr)
	{
		if (mvPtr.mvMoveAtStart == 0)
		{
			stop();
		}
	}

	public int getAccelerator(int acceleration)
	{
		if (acceleration <= 100)
		{
			return accelerators[acceleration];
		}
		return acceleration << 8;
	}

	public void mv_Approach(bool bStickToObject)
	{
		if (bStickToObject)
		{
			mb_Approach(flag: false);
			return;
		}
		bool flag = false;
		switch (hoPtr.hoAdRunHeader.rhEvtProg.rhCurCode >> 16)
		{
		case -12:
		{
			int num2 = hoPtr.hoX - hoPtr.hoImgXSpot;
			int num3 = hoPtr.hoY - hoPtr.hoImgYSpot;
			int num4 = hoPtr.hoAdRunHeader.quadran_Out(num2, num3, num2 + hoPtr.hoImgWidth, num3 + hoPtr.hoImgHeight);
			num2 = hoPtr.hoX;
			num3 = hoPtr.hoY;
			if ((num4 & 1) != 0)
			{
				num2 = hoPtr.hoImgXSpot;
			}
			if ((num4 & 2) != 0)
			{
				num2 = hoPtr.hoAdRunHeader.rhLevelSx - hoPtr.hoImgWidth + hoPtr.hoImgXSpot;
			}
			if ((num4 & 4) != 0)
			{
				num3 = hoPtr.hoImgYSpot;
			}
			if ((num4 & 8) != 0)
			{
				num3 = hoPtr.hoAdRunHeader.rhLevelSy - hoPtr.hoImgHeight + hoPtr.hoImgYSpot;
			}
			hoPtr.hoX = num2;
			hoPtr.hoY = num3;
			break;
		}
		case -14:
		case -13:
		{
			int num = (hoPtr.roc.rcDir >> 2) * 18;
			do
			{
				if (tst_Position(hoPtr.hoX + mvap_TableDirs[num], hoPtr.hoY + mvap_TableDirs[num + 1], flag))
				{
					hoPtr.hoX += mvap_TableDirs[num];
					hoPtr.hoY += mvap_TableDirs[num + 1];
					return;
				}
				num += 2;
			}
			while (mvap_TableDirs[num] != 0 || mvap_TableDirs[num + 1] != 0);
			if (!flag)
			{
				hoPtr.hoX = hoPtr.roc.rcOldX;
				hoPtr.hoY = hoPtr.roc.rcOldY;
				hoPtr.roc.rcImage = hoPtr.roc.rcOldImage;
				hoPtr.roc.rcAngle = hoPtr.roc.rcOldAngle;
			}
			break;
		}
		}
	}

	public void mb_Approach(bool flag)
	{
		switch (hoPtr.hoAdRunHeader.rhEvtProg.rhCurCode >> 16)
		{
		case -12:
		{
			int num2 = hoPtr.hoX - hoPtr.hoImgXSpot;
			int num3 = hoPtr.hoY - hoPtr.hoImgYSpot;
			int num4 = hoPtr.hoAdRunHeader.quadran_Out(num2, num3, num2 + hoPtr.hoImgWidth, num3 + hoPtr.hoImgHeight);
			num2 = hoPtr.hoX;
			num3 = hoPtr.hoY;
			if ((num4 & 1) != 0)
			{
				num2 = hoPtr.hoImgXSpot;
			}
			if ((num4 & 2) != 0)
			{
				num2 = hoPtr.hoAdRunHeader.rhLevelSx - hoPtr.hoImgWidth + hoPtr.hoImgXSpot;
			}
			if ((num4 & 4) != 0)
			{
				num3 = hoPtr.hoImgYSpot;
			}
			if ((num4 & 8) != 0)
			{
				num3 = hoPtr.hoAdRunHeader.rhLevelSy - hoPtr.hoImgHeight + hoPtr.hoImgYSpot;
			}
			hoPtr.hoX = num2;
			hoPtr.hoY = num3;
			break;
		}
		case -14:
		case -13:
		{
			CPoint cPoint = new CPoint();
			if (mbApproachSprite(hoPtr.hoX, hoPtr.hoY, hoPtr.roc.rcOldX, hoPtr.roc.rcOldY, flag, cPoint))
			{
				hoPtr.hoX = cPoint.x;
				hoPtr.hoY = cPoint.y;
				break;
			}
			int num = (hoPtr.roc.rcDir >> 2) * 18;
			do
			{
				if (tst_Position(hoPtr.hoX + mvap_TableDirs[num], hoPtr.hoY + mvap_TableDirs[num + 1], flag))
				{
					hoPtr.hoX += mvap_TableDirs[num];
					hoPtr.hoY += mvap_TableDirs[num + 1];
					return;
				}
				num += 2;
			}
			while (mvap_TableDirs[num] != 0 || mvap_TableDirs[num + 1] != 0);
			if (!flag)
			{
				hoPtr.hoX = hoPtr.roc.rcOldX;
				hoPtr.hoY = hoPtr.roc.rcOldY;
				hoPtr.roc.rcImage = hoPtr.roc.rcOldImage;
				hoPtr.roc.rcAngle = hoPtr.roc.rcOldAngle;
			}
			break;
		}
		}
	}

	public bool tst_SpritePosition(int x, int y, short htFoot, short planCol, bool flag)
	{
		short num = -1;
		if (flag)
		{
			num = hoPtr.hoOi;
		}
		CObjInfo hoOiList = hoPtr.hoOiList;
		if ((hoOiList.oilLimitFlags & 0xF) != 0)
		{
			int num2 = x - hoPtr.hoImgXSpot;
			int num3 = y - hoPtr.hoImgYSpot;
			if ((hoPtr.hoAdRunHeader.quadran_Out(num2, num3, num2 + hoPtr.hoImgWidth, num3 + hoPtr.hoImgHeight) & hoOiList.oilLimitFlags) != 0)
			{
				return false;
			}
		}
		if ((hoOiList.oilLimitFlags & 0x10) != 0 && hoPtr.hoAdRunHeader.colMask_TestObject_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, x, y, htFoot, planCol) != 0)
		{
			return false;
		}
		if (hoOiList.oilLimitList == -1)
		{
			return true;
		}
		CArrayList cArrayList = hoPtr.hoAdRunHeader.objectAllCol_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, x, y, hoOiList.oilColList);
		if (cArrayList == null)
		{
			return true;
		}
		short[] limitBuffer = hoPtr.hoAdRunHeader.rhEvtProg.limitBuffer;
		for (int i = 0; i < cArrayList.size(); i++)
		{
			CObject cObject = (CObject)cArrayList.get(i);
			short hoOi = cObject.hoOi;
			if (hoOi == num)
			{
				continue;
			}
			for (int j = hoOiList.oilLimitList; limitBuffer[j] >= 0; j++)
			{
				if (limitBuffer[j] == hoOi)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool tst_Position(int x, int y, bool flag)
	{
		short num = -1;
		if (flag)
		{
			num = hoPtr.hoOi;
		}
		CObjInfo hoOiList = hoPtr.hoOiList;
		if ((hoOiList.oilLimitFlags & 0xF) != 0)
		{
			int num2 = x - hoPtr.hoImgXSpot;
			int num3 = y - hoPtr.hoImgYSpot;
			int num4 = hoPtr.hoAdRunHeader.quadran_Out(num2, num3, num2 + hoPtr.hoImgWidth, num3 + hoPtr.hoImgHeight);
			if ((num4 & hoOiList.oilLimitFlags) != 0)
			{
				return false;
			}
		}
		if ((hoOiList.oilLimitFlags & 0x10) != 0 && hoPtr.hoAdRunHeader.colMask_TestObject_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, x, y, 0, 1) != 0)
		{
			return false;
		}
		if (hoOiList.oilLimitList == -1)
		{
			return true;
		}
		CArrayList cArrayList = hoPtr.hoAdRunHeader.objectAllCol_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, x, y, hoOiList.oilColList);
		if (cArrayList == null)
		{
			return true;
		}
		short[] limitBuffer = hoPtr.hoAdRunHeader.rhEvtProg.limitBuffer;
		for (int i = 0; i < cArrayList.size(); i++)
		{
			CObject cObject = (CObject)cArrayList.get(i);
			short hoOi = cObject.hoOi;
			if (hoOi == num)
			{
				continue;
			}
			for (int j = hoOiList.oilLimitList; limitBuffer[j] >= 0; j++)
			{
				if (limitBuffer[j] == hoOi)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool mpApproachSprite(int destX, int destY, int maxX, int maxY, short htFoot, short planCol, CPoint ptFinal)
	{
		int num = destX;
		int num2 = destY;
		int num3 = maxX;
		int num4 = maxY;
		int num5 = (num + num3) / 2;
		int num6 = (num2 + num4) / 2;
		while (true)
		{
			if (tst_SpritePosition(num5 + hoPtr.hoAdRunHeader.rhWindowX, num6 + hoPtr.hoAdRunHeader.rhWindowY, htFoot, planCol, flag: false))
			{
				num3 = num5;
				num4 = num6;
				int num7 = num5;
				int num8 = num6;
				num5 = (num3 + num) / 2;
				num6 = (num4 + num2) / 2;
				if (num5 == num7 && num6 == num8)
				{
					if ((num3 != num || num4 != num2) && tst_SpritePosition(num + hoPtr.hoAdRunHeader.rhWindowX, num2 + hoPtr.hoAdRunHeader.rhWindowY, htFoot, planCol, flag: false))
					{
						num5 = num;
						num6 = num2;
					}
					ptFinal.x = num5;
					ptFinal.y = num6;
					return true;
				}
			}
			else
			{
				num = num5;
				num2 = num6;
				int num7 = num5;
				int num8 = num6;
				num5 = (num3 + num) / 2;
				num6 = (num4 + num2) / 2;
				if (num5 == num7 && num6 == num8)
				{
					break;
				}
			}
		}
		if ((num3 != num || num4 != num2) && tst_SpritePosition(num3 + hoPtr.hoAdRunHeader.rhWindowX, num4 + hoPtr.hoAdRunHeader.rhWindowY, htFoot, planCol, flag: false))
		{
			ptFinal.x = num3;
			ptFinal.y = num4;
			return true;
		}
		ptFinal.x = num5;
		ptFinal.y = num6;
		return false;
	}

	private bool mbApproachSprite(int destX, int destY, int maxX, int maxY, bool flag, CPoint ptFinal)
	{
		int num = destX;
		int num2 = destY;
		int num3 = maxX;
		int num4 = maxY;
		int num5 = (num + num3) / 2;
		int num6 = (num2 + num4) / 2;
		while (true)
		{
			if (tst_Position(num5, num6, flag))
			{
				num3 = num5;
				num4 = num6;
				int num7 = num5;
				int num8 = num6;
				num5 = (num3 + num) / 2;
				num6 = (num4 + num2) / 2;
				if (num5 == num7 && num6 == num8)
				{
					if ((num3 != num || num4 != num2) && tst_Position(num, num2, flag))
					{
						num5 = num;
						num6 = num2;
					}
					ptFinal.x = num5;
					ptFinal.y = num6;
					return true;
				}
			}
			else
			{
				num = num5;
				num2 = num6;
				int num7 = num5;
				int num8 = num6;
				num5 = (num3 + num) / 2;
				num6 = (num4 + num2) / 2;
				if (num5 == num7 && num6 == num8)
				{
					break;
				}
			}
		}
		if ((num3 != num || num4 != num2) && tst_Position(num3, num4, flag))
		{
			ptFinal.x = num3;
			ptFinal.y = num4;
			return true;
		}
		ptFinal.x = num5;
		ptFinal.y = num6;
		return false;
	}

	public static int getDeltaX(int pente, int angle)
	{
		return pente * Cosinus32[angle] / 256;
	}

	public static int getDeltaY(int pente, int angle)
	{
		return pente * Sinus32[angle] / 256;
	}

	public void setAcc(int acc)
	{
		if (acc > 250)
		{
			acc = 250;
		}
		if (acc < 0)
		{
			acc = 0;
		}
		rmAcc = acc;
		rmAccValue = getAccelerator(acc);
		if (hoPtr.roc.rcMovementType == 14)
		{
			CMoveExtension cMoveExtension = (CMoveExtension)this;
			cMoveExtension.movement.setAcc(acc);
		}
	}

	public void setDec(int dec)
	{
		if (dec > 250)
		{
			dec = 250;
		}
		if (dec < 0)
		{
			dec = 0;
		}
		rmDec = dec;
		rmDecValue = getAccelerator(dec);
		if (hoPtr.roc.rcMovementType == 14)
		{
			CMoveExtension cMoveExtension = (CMoveExtension)this;
			cMoveExtension.movement.setDec(dec);
		}
	}

	public void setRotSpeed(int speed)
	{
		if (speed > 250)
		{
			speed = 250;
		}
		if (speed < 0)
		{
			speed = 0;
		}
		if (hoPtr.roc.rcMovementType == 2)
		{
			CMoveRace cMoveRace = (CMoveRace)this;
			cMoveRace.MRSetRotSpeed(speed);
		}
		if (hoPtr.roc.rcMovementType == 14)
		{
			CMoveExtension cMoveExtension = (CMoveExtension)this;
			cMoveExtension.movement.setRotSpeed(speed);
		}
	}

	public void set8Dirs(int dirs)
	{
		if (hoPtr.roc.rcMovementType == 3)
		{
			CMoveGeneric cMoveGeneric = (CMoveGeneric)this;
			cMoveGeneric.set8Dir(dirs);
		}
		if (hoPtr.roc.rcMovementType == 14)
		{
			CMoveExtension cMoveExtension = (CMoveExtension)this;
			cMoveExtension.movement.set8Dirs(dirs);
		}
	}

	public void setGravity(int gravity)
	{
		if (gravity > 250)
		{
			gravity = 250;
		}
		if (gravity < 0)
		{
			gravity = 0;
		}
		if (hoPtr.roc.rcMovementType == 9)
		{
			CMovePlatform cMovePlatform = (CMovePlatform)this;
			cMovePlatform.MPSetGravity(gravity);
		}
		if (hoPtr.roc.rcMovementType == 14)
		{
			CMoveExtension cMoveExtension = (CMoveExtension)this;
			cMoveExtension.movement.setGravity(gravity);
		}
	}

	public int getSpeed()
	{
		if (hoPtr.roc.rcMovementType == 14)
		{
			CMoveExtension cMoveExtension = (CMoveExtension)this;
			return cMoveExtension.movement.getSpeed();
		}
		return hoPtr.roc.rcSpeed;
	}

	public int getAcc()
	{
		if (hoPtr.roc.rcMovementType == 14)
		{
			CMoveExtension cMoveExtension = (CMoveExtension)this;
			return cMoveExtension.movement.getAcceleration();
		}
		return rmAcc;
	}

	public int getDec()
	{
		if (hoPtr.roc.rcMovementType == 14)
		{
			CMoveExtension cMoveExtension = (CMoveExtension)this;
			return cMoveExtension.movement.getDeceleration();
		}
		return rmDec;
	}

	public int getGravity()
	{
		if (hoPtr.roc.rcMovementType == 9)
		{
			CMovePlatform cMovePlatform = (CMovePlatform)this;
			return cMovePlatform.MP_Gravity;
		}
		if (hoPtr.roc.rcMovementType == 14)
		{
			CMoveExtension cMoveExtension = (CMoveExtension)this;
			return cMoveExtension.movement.getGravity();
		}
		return 0;
	}

	public virtual void init(CObject hoPtr, CMoveDef mvPtr)
	{
	}

	public virtual void kill()
	{
	}

	public virtual void move()
	{
	}

	public virtual void stop()
	{
	}

	public virtual void start()
	{
	}

	public virtual void bounce()
	{
	}

	public virtual void reverse()
	{
	}

	public virtual void setXPosition(int x)
	{
	}

	public virtual void setYPosition(int u)
	{
	}

	public virtual void setSpeed(int speed)
	{
	}

	public virtual void setMaxSpeed(int speed)
	{
	}

	public virtual void setDir(int dir)
	{
	}
}
