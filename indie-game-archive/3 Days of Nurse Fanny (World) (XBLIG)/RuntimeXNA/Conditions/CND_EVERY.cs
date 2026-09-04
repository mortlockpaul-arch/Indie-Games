using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EVERY : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		PARAM_EVERY pARAM_EVERY = (PARAM_EVERY)evtParams[0];
		pARAM_EVERY.compteur -= rhPtr.rhTimerDelta;
		if (pARAM_EVERY.compteur > 0)
		{
			return false;
		}
		pARAM_EVERY.compteur += pARAM_EVERY.delay;
		return true;
	}
}
