using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTNUMBERZONE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		int value = rhPtr.rhEvtProg.count_ZoneOneObject(evtOiList, (PARAM_ZONE)evtParams[0]);
		int value2 = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
		return CRun.compareTer(value, value2, ((CParamExpression)evtParams[1]).comparaison);
	}
}
