using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_NOMORE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		CEventGroup rhEventGroup = rhPtr.rhEvtProg.rhEventGroup;
		if ((rhEventGroup.evgFlags & 8) != 0)
		{
			return true;
		}
		if ((rhEventGroup.evgFlags & 6) != 0)
		{
			return false;
		}
		if (evtParams[0].code == 22)
		{
			rhEventGroup.evgInhibit = (ushort)(rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]) / 10);
		}
		else
		{
			rhEventGroup.evgInhibit = (ushort)(((PARAM_TIME)evtParams[0]).timer / 10);
		}
		rhEventGroup.evgInhibitCpt = 0;
		rhEventGroup.evgFlags |= 8;
		return true;
	}
}
