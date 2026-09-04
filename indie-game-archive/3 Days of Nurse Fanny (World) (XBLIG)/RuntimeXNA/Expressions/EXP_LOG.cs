using System;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_LOG : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		double expressionDouble = rhPtr.get_ExpressionDouble();
		rhPtr.getCurrentResult().forceDouble(Math.Log10(expressionDouble));
	}
}
