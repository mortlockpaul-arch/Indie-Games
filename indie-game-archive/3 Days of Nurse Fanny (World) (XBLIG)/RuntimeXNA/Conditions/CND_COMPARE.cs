using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_COMPARE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		CValue cValue = new CValue();
		cValue.forceValue(rhPtr.get_EventExpressionAny((CParamExpression)evtParams[0]));
		CParamExpression cParamExpression = (CParamExpression)evtParams[1];
		CValue pValue = rhPtr.get_EventExpressionAny(cParamExpression);
		return CRun.compareTo(cValue, pValue, cParamExpression.comparaison);
	}
}
