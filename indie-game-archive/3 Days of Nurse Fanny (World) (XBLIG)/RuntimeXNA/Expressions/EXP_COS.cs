using System;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_COS : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		double expressionDouble = rhPtr.get_ExpressionDouble();
		double value = Math.Cos(expressionDouble / (180.0 / Math.PI));
		rhPtr.getCurrentResult().forceDouble(value);
	}
}
