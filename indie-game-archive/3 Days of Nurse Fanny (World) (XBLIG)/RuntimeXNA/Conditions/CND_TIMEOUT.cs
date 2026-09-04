using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_TIMEOUT : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		long num = ((evtParams[0].code != 22) ? ((PARAM_TIME)evtParams[0]).timer : rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]));
		if (rhPtr.rh4TimeOut > num)
		{
			return true;
		}
		return false;
	}
}
