using System;
using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_ONLOOP : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		CParamExpression cParamExpression = (CParamExpression)evtParams[0];
		if (cParamExpression.tokens.Length == 2 && cParamExpression.tokens[0].code == 262143 && cParamExpression.tokens[1].code == 0)
		{
			if (string.Compare(rhPtr.rh4CurrentFastLoop, ((EXP_STRING)cParamExpression.tokens[0]).pString, StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				return true;
			}
			return false;
		}
		string strB = rhPtr.get_EventExpressionString(cParamExpression);
		if (string.Compare(rhPtr.rh4CurrentFastLoop, strB, StringComparison.CurrentCultureIgnoreCase) != 0)
		{
			return false;
		}
		rhPtr.rhEvtProg.rh2ActionOn = false;
		return true;
	}

	public override bool eva2(CRun rhPtr)
	{
		return false;
	}
}
