using System;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_ATAN2 : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		double expressionDouble = rhPtr.get_ExpressionDouble();
		rhPtr.rh4CurToken++;
		double expressionDouble2 = rhPtr.get_ExpressionDouble();
		rhPtr.getCurrentResult().forceDouble(Math.Atan2(expressionDouble, expressionDouble2) * 180.0 / Math.PI);
	}
}
