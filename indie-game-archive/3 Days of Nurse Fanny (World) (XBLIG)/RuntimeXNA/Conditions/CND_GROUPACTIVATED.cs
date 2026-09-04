using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_GROUPACTIVATED : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		CEventGroup cEventGroup = rhPtr.rhEvtProg.events[((PARAM_GROUPOINTER)evtParams[0]).pointer];
		if ((cEventGroup.evgFlags & 0x4000) != 0)
		{
			return negaFALSE();
		}
		return negaTRUE();
	}
}
