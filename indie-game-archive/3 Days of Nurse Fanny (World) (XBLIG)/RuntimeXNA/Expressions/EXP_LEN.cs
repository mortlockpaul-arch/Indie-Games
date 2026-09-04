using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_LEN : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		string expressionString = rhPtr.get_ExpressionString();
		rhPtr.getCurrentResult().forceInt(expressionString.Length);
	}
}
