using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_TIMER : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		if ((evtFlags & 2) != 0)
		{
			return false;
		}
		PARAM_TIME pARAM_TIME = (PARAM_TIME)evtParams[0];
		long num = pARAM_TIME.timer;
		if (rhPtr.rhTimer < num)
		{
			return false;
		}
		evtFlags |= 2;
		return true;
	}

	public override bool eva2(CRun rhPtr)
	{
		return false;
	}
}
