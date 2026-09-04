using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_REPEAT : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		CEventGroup rhEventGroup = rhPtr.rhEvtProg.rhEventGroup;
		if ((rhEventGroup.evgFlags & 4) != 0)
		{
			return true;
		}
		if ((rhEventGroup.evgFlags & 8) != 0)
		{
			return false;
		}
		rhEventGroup.evgInhibitCpt = (short)rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		rhEventGroup.evgFlags |= 4;
		return true;
	}
}
