using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_NUMOFALLZONE_OLD : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		rhPtr.rhEvtProg.count_ZoneTypeObjects((PARAM_ZONE)evtParams[0], -1, 2);
		CValue pValue = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[1]);
		short comparaison = ((CParamExpression)evtParams[1]).comparaison;
		CValue pValue2 = new CValue(rhPtr.rhEvtProg.evtNSelectedObjects);
		return CRun.compareTo(pValue2, pValue, comparaison);
	}
}
