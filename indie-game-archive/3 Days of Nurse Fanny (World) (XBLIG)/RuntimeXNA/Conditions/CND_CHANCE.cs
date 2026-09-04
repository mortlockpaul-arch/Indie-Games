using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_CHANCE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		int num2 = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
		if (num2 >= 1 && num > 0 && num <= num2)
		{
			int num3 = rhPtr.random((short)num2);
			if (num3 <= num)
			{
				return true;
			}
		}
		return false;
	}
}
