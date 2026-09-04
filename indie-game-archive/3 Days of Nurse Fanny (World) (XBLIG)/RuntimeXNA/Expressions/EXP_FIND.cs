using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_FIND : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		string expressionString = rhPtr.get_ExpressionString();
		rhPtr.rh4CurToken++;
		string expressionString2 = rhPtr.get_ExpressionString();
		rhPtr.rh4CurToken++;
		int expressionInt = rhPtr.get_ExpressionInt();
		if (expressionInt >= expressionString.Length)
		{
			rhPtr.getCurrentResult().forceInt(-1);
		}
		else
		{
			rhPtr.getCurrentResult().forceInt(expressionString.IndexOf(expressionString2, expressionInt));
		}
	}
}
