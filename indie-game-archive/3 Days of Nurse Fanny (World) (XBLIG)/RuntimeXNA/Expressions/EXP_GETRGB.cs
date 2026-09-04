using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_GETRGB : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		int expressionInt = rhPtr.get_ExpressionInt();
		rhPtr.rh4CurToken++;
		int expressionInt2 = rhPtr.get_ExpressionInt();
		rhPtr.rh4CurToken++;
		int expressionInt3 = rhPtr.get_ExpressionInt();
		int value = ((expressionInt3 & 0xFF) << 16) + ((expressionInt2 & 0xFF) << 8) + (expressionInt & 0xFF);
		rhPtr.getCurrentResult().forceInt(value);
	}
}
