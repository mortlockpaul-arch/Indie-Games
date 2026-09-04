using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_INT : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		double expressionDouble = rhPtr.get_ExpressionDouble();
		rhPtr.getCurrentResult().forceInt((int)expressionDouble);
	}
}
