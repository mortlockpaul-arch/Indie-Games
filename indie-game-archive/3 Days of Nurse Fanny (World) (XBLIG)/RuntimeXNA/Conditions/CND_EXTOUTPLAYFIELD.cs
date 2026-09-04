using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTOUTPLAYFIELD : CCnd, IEvaObject
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		PARAM_SHORT pARAM_SHORT = (PARAM_SHORT)evtParams[0];
		if ((pARAM_SHORT.value & (short)rhPtr.rhEvtProg.rhCurParam0) == 0)
		{
			return false;
		}
		if (compute_NoRepeat(hoPtr))
		{
			rhPtr.rhEvtProg.evt_AddCurrentObject(hoPtr);
			return true;
		}
		CEventGroup rhEventGroup = rhPtr.rhEvtProg.rhEventGroup;
		if ((rhEventGroup.evgFlags & 0x800) == 0)
		{
			return false;
		}
		rhPtr.rhEvtProg.rh3DoStop = true;
		return true;
	}

	public override bool eva2(CRun rhPtr)
	{
		return evaObject(rhPtr, this);
	}

	public virtual bool evaObjectRoutine(CObject pHo)
	{
		if ((pHo.rom.rmEventFlags & 2) != 0)
		{
			return negaTRUE();
		}
		return negaFALSE();
	}
}
