using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTCOLLISION : CCnd
{
	public override bool eva1(CRun rhPtr, CObject pHo)
	{
		CObject cObject = rhPtr.rhObjectList[rhPtr.rhEvtProg.rh1stObjectNumber];
		short num = evtOi;
		PARAM_OBJECT pARAM_OBJECT = (PARAM_OBJECT)evtParams[0];
		short oi = pARAM_OBJECT.oi;
		if (num == pHo.hoOi)
		{
			if (oi != cObject.hoOi)
			{
				if (oi >= 0)
				{
					return false;
				}
				if (!colGetList(rhPtr, pARAM_OBJECT.oiList, cObject.hoOi))
				{
					return false;
				}
			}
		}
		else if (oi == pHo.hoOi)
		{
			if (num != cObject.hoOi)
			{
				if (num >= 0)
				{
					return false;
				}
				if (!colGetList(rhPtr, evtOiList, cObject.hoOi))
				{
					return false;
				}
			}
		}
		else if (num < 0)
		{
			if (oi < 0)
			{
				if (colGetList(rhPtr, evtOiList, pHo.hoOi))
				{
					if (!colGetList(rhPtr, pARAM_OBJECT.oiList, cObject.hoOi))
					{
						if (!colGetList(rhPtr, pARAM_OBJECT.oiList, pHo.hoOi))
						{
							return false;
						}
						if (!colGetList(rhPtr, evtOiList, cObject.hoOi))
						{
							return false;
						}
					}
				}
				else if (!colGetList(rhPtr, evtOiList, cObject.hoOi))
				{
					return false;
				}
			}
			else if (oi != cObject.hoOi)
			{
				return false;
			}
		}
		else
		{
			if (oi >= 0)
			{
				return false;
			}
			if (num != cObject.hoOi)
			{
				return false;
			}
		}
		int identifier = (cObject.hoCreationId << 16) | (evtIdentifier & 0xFFFF);
		if (!compute_NoRepeatCol(identifier, pHo))
		{
			if ((rhPtr.rhEvtProg.rhEventGroup.evgFlags & 0x800) == 0)
			{
				return false;
			}
			rhPtr.rhEvtProg.rh3DoStop = true;
		}
		identifier = (pHo.hoCreationId << 16) | (evtIdentifier & 0xFFFF);
		if (!compute_NoRepeatCol(identifier, cObject))
		{
			if ((rhPtr.rhEvtProg.rhEventGroup.evgFlags & 0x800) == 0)
			{
				return false;
			}
			rhPtr.rhEvtProg.rh3DoStop = true;
		}
		rhPtr.rhEvtProg.evt_AddCurrentObject(pHo);
		rhPtr.rhEvtProg.evt_AddCurrentObject(cObject);
		if (cObject.rom.rmMovement.rmCollisionCount == rhPtr.rh3CollisionCount)
		{
			pHo.rom.rmMovement.rmCollisionCount = rhPtr.rh3CollisionCount;
		}
		else if (pHo.rom.rmMovement.rmCollisionCount == rhPtr.rh3CollisionCount)
		{
			cObject.rom.rmMovement.rmCollisionCount = rhPtr.rh3CollisionCount;
		}
		return true;
	}

	public override bool eva2(CRun rhPtr)
	{
		return isColliding(rhPtr);
	}

	internal virtual bool colGetList(CRun rhPtr, short oiList, short lookFor)
	{
		if (oiList == -1)
		{
			return false;
		}
		CQualToOiList cQualToOiList = rhPtr.rhEvtProg.qualToOiList[oiList & 0x7FFF];
		for (int i = 0; i < cQualToOiList.qoiList.Length; i += 2)
		{
			if (cQualToOiList.qoiList[i] == lookFor)
			{
				return true;
			}
		}
		return false;
	}
}
