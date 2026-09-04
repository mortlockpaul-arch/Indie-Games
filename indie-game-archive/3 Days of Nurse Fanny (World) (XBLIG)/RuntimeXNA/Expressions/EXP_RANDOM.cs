using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_RANDOM : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		int expressionInt = rhPtr.get_ExpressionInt();
		rhPtr.getCurrentResult().forceInt(rhPtr.random((short)expressionInt));
	}
}
