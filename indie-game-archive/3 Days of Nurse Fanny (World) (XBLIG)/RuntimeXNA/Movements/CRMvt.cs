using RuntimeXNA.OI;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Movements;

public class CRMvt
{
	public const short EF_GOESINPLAYFIELD = 1;

	public const short EF_GOESOUTPLAYFIELD = 2;

	public const short EF_WRAP = 4;

	public int rmMvtNum;

	public CMove rmMovement;

	public byte rmWrapping;

	public bool rmMoveFlag;

	public int rmReverse;

	public bool rmBouncing;

	public short rmEventFlags;

	public void init(int nMove, CObject hoPtr, CObjectCommon ocPtr, CCreateObjectInfo cob, int forcedType)
	{
		if (rmMovement != null)
		{
			rmMovement.kill();
		}
		if (cob != null)
		{
			hoPtr.roc.rcDir = cob.cobDir;
		}
		rmWrapping = hoPtr.hoOiList.oilWrap;
		CMoveDef cMoveDef = null;
		hoPtr.roc.rcMovementType = -1;
		if (ocPtr.ocMovements != null && nMove < ocPtr.ocMovements.nMovements)
		{
			cMoveDef = ocPtr.ocMovements.moveList[nMove];
			rmMvtNum = nMove;
			if (forcedType == -1)
			{
				forcedType = cMoveDef.mvType;
			}
			hoPtr.roc.rcMovementType = forcedType;
			switch (forcedType)
			{
			case 0:
				rmMovement = new CMoveStatic();
				break;
			case 1:
				rmMovement = new CMoveMouse();
				break;
			case 2:
				rmMovement = new CMoveRace();
				break;
			case 3:
				rmMovement = new CMoveGeneric();
				break;
			case 4:
				rmMovement = new CMoveBall();
				break;
			case 5:
				rmMovement = new CMovePath();
				break;
			case 9:
				rmMovement = new CMovePlatform();
				break;
			case 14:
				rmMovement = loadMvtExtension(hoPtr, (CMoveDefExtension)cMoveDef);
				if (rmMovement == null)
				{
					rmMovement = new CMoveStatic();
				}
				break;
			}
			hoPtr.roc.rcDir = dirAtStart(hoPtr, cMoveDef.mvDirAtStart, hoPtr.roc.rcDir);
			rmMovement.init(hoPtr, cMoveDef);
		}
		if (hoPtr.roc.rcMovementType == -1)
		{
			hoPtr.roc.rcMovementType = 0;
			rmMovement = new CMoveStatic();
			rmMovement.init(hoPtr, null);
			hoPtr.roc.rcDir = 0;
		}
	}

	public CMove loadMvtExtension(CObject hoPtr, CMoveDefExtension mvDef)
	{
		CRunMvtExtension cRunMvtExtension = null;
		if (string.Compare(mvDef.moduleName, "clickteam-simple_ellipse") == 0)
		{
			cRunMvtExtension = new CRunMvtclickteam_simple_ellipse();
		}
		if (cRunMvtExtension != null)
		{
			cRunMvtExtension.init(hoPtr);
			return new CMoveExtension(cRunMvtExtension);
		}
		return null;
	}

	public void initSimple(CObject hoPtr, int forcedType, bool bRestore)
	{
		if (rmMovement != null)
		{
			rmMovement.kill();
		}
		hoPtr.roc.rcMovementType = forcedType;
		switch (forcedType)
		{
		case 11:
			rmMovement = new CMoveDisappear();
			CRun.bMoveChanged = true;
			break;
		case 13:
			rmMovement = new CMoveBullet();
			break;
		}
		rmMovement.hoPtr = hoPtr;
		if (!bRestore)
		{
			rmMovement.init(hoPtr, null);
		}
	}

	public void kill(bool bFast)
	{
		rmMovement.kill();
	}

	public void move()
	{
		rmMovement.move();
	}

	public void nextMovement(CObject hoPtr)
	{
		CObjectCommon hoCommon = hoPtr.hoCommon;
		if (hoCommon.ocMovements != null && rmMvtNum + 1 < hoCommon.ocMovements.nMovements)
		{
			kill(bFast: false);
			init(rmMvtNum + 1, hoPtr, hoCommon, null, -1);
		}
	}

	public void previousMovement(CObject hoPtr)
	{
		CObjectCommon hoCommon = hoPtr.hoCommon;
		if (hoCommon.ocMovements != null && rmMvtNum - 1 >= 0)
		{
			kill(bFast: false);
			init(rmMvtNum - 1, hoPtr, hoCommon, null, -1);
		}
	}

	public void selectMovement(CObject hoPtr, int mvt)
	{
		CObjectCommon hoCommon = hoPtr.hoCommon;
		if (hoCommon.ocMovements != null && mvt >= 0 && mvt < hoCommon.ocMovements.nMovements)
		{
			kill(bFast: false);
			init(mvt, hoPtr, hoCommon, null, -1);
		}
	}

	public int dirAtStart(CObject hoPtr, int dirAtStart, int dir)
	{
		if (dir < 0 || dir >= 32)
		{
			int num = 0;
			int num2 = dirAtStart;
			for (int i = 0; i < 32; i++)
			{
				int num3 = num2;
				num2 >>= 1;
				if ((num3 & 1) != 0)
				{
					num++;
				}
			}
			if (num == 0)
			{
				dir = 0;
			}
			else
			{
				num = hoPtr.hoAdRunHeader.random((short)num);
				num2 = dirAtStart;
				dir = 0;
				while (true)
				{
					int num3 = num2;
					num2 >>= 1;
					if ((num3 & 1) != 0)
					{
						num--;
						if (num < 0)
						{
							break;
						}
					}
					dir++;
				}
			}
		}
		return dir;
	}
}
