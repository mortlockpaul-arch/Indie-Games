using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_REVERSEFIND : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		string expressionString = rhPtr.get_ExpressionString();
		rhPtr.rh4CurToken++;
		string expressionString2 = rhPtr.get_ExpressionString();
		rhPtr.rh4CurToken++;
		int num = rhPtr.get_ExpressionInt();
		if (num > expressionString.Length)
		{
			num = expressionString.Length;
		}
		int num2 = -1;
		int value;
		do
		{
			value = num2;
			int num3 = expressionString.IndexOf(expressionString2, num2 + 1);
			if (num3 == -1)
			{
				break;
			}
			num2 = num3;
		}
		while (num2 <= num);
		rhPtr.getCurrentResult().forceInt(value);
	}
}
