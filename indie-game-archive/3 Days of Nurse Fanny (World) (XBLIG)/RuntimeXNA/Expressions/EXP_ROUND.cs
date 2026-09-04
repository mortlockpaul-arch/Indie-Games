using System;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_ROUND : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		double expressionDouble = rhPtr.get_ExpressionDouble();
		double num = Math.Floor(expressionDouble);
		if (expressionDouble - num > 0.5)
		{
			num++;
		}
		rhPtr.getCurrentResult().forceInt((int)num);
	}
}
