using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_COMPAREGSTRING : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		int num = ((evtParams[0].code != 52) ? ((PARAM_SHORT)evtParams[0]).value : (rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]) - 1));
		string globalStringAt = rhPtr.rhApp.getGlobalStringAt(num);
		CValue pValue = new CValue(globalStringAt);
		CValue pValue2 = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[1]);
		short comparaison = ((CParamExpression)evtParams[1]).comparaison;
		return CRun.compareTo(pValue, pValue2, comparaison);
	}
}
