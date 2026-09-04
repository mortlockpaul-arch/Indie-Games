using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_RIGHT : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		string expressionString = rhPtr.get_ExpressionString();
		rhPtr.rh4CurToken++;
		int num = rhPtr.get_ExpressionInt();
		if (num < 0)
		{
			num = 0;
		}
		if (num > expressionString.Length)
		{
			num = expressionString.Length;
		}
		rhPtr.getCurrentResult().forceString(expressionString.Substring(expressionString.Length - num, expressionString.Length - (expressionString.Length - num)));
	}
}
