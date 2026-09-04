using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_NOT : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		int expressionInt = rhPtr.get_ExpressionInt();
		rhPtr.getCurrentResult().forceInt(expressionInt ^ -1);
	}
}
