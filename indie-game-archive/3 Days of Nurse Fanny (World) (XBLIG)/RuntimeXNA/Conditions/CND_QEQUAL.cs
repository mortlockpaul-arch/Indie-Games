using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_QEQUAL : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		if (rhPtr.rhEvtProg.rhCurParam0 == num)
		{
			return true;
		}
		return false;
	}

	public override bool eva2(CRun rhPtr)
	{
		return false;
	}
}
