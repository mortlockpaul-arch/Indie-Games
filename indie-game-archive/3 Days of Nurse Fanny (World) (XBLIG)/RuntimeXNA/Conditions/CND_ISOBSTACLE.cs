using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_ISOBSTACLE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		int num2 = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
		if (rhPtr.rhFrame.bkdCol_TestPoint(num - rhPtr.rhWindowX, num2 - rhPtr.rhWindowY, -1, 0))
		{
			return negaTRUE();
		}
		return negaFALSE();
	}
}
