using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Expressions;

public class EXP_VAL : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		string expressionString = rhPtr.get_ExpressionString();
		CFuncVal cFuncVal = new CFuncVal();
		switch (cFuncVal.parse(expressionString))
		{
		case 0:
			rhPtr.getCurrentResult().forceInt(cFuncVal.intValue);
			break;
		case 1:
			rhPtr.getCurrentResult().forceDouble(cFuncVal.doubleValue);
			break;
		}
	}
}
