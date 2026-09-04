using System;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_BIN : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		int expressionInt = rhPtr.get_ExpressionInt();
		string value = "0b" + Convert.ToString(expressionInt, 2);
		rhPtr.getCurrentResult().forceString(value);
	}
}
