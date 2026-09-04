using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_MID : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		string expressionString = rhPtr.get_ExpressionString();
		rhPtr.rh4CurToken++;
		int num = rhPtr.get_ExpressionInt();
		rhPtr.rh4CurToken++;
		int num2 = rhPtr.get_ExpressionInt();
		if (num < 0)
		{
			num = 0;
		}
		if (num > expressionString.Length)
		{
			num = expressionString.Length;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num + num2 > expressionString.Length)
		{
			num2 = expressionString.Length - num;
		}
		rhPtr.getCurrentResult().forceString(expressionString.Substring(num, num + num2 - num));
	}
}
