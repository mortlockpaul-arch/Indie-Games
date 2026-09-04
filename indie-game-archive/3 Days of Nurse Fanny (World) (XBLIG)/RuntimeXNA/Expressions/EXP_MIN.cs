using System;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_MIN : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		CValue expressionAny = rhPtr.get_ExpressionAny();
		rhPtr.rh4CurToken++;
		CValue expressionAny2 = rhPtr.get_ExpressionAny();
		if (expressionAny.type == 0 && expressionAny2.type == 0)
		{
			int intValue = expressionAny.intValue;
			int intValue2 = expressionAny2.intValue;
			if (intValue < intValue2)
			{
				rhPtr.getCurrentResult().forceInt(intValue);
			}
			else
			{
				rhPtr.getCurrentResult().forceInt(intValue2);
			}
		}
		else
		{
			rhPtr.getCurrentResult().forceDouble(Math.Min(expressionAny.getDouble(), expressionAny2.getDouble()));
		}
	}
}
