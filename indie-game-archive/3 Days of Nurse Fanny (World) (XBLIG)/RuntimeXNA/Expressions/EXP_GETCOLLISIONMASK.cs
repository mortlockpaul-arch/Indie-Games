using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_GETCOLLISIONMASK : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		int expressionInt = rhPtr.get_ExpressionInt();
		rhPtr.rh4CurToken++;
		int expressionInt2 = rhPtr.get_ExpressionInt();
		int value = 0;
		if (rhPtr.y_GetLadderAt_Absolute(-1, expressionInt, expressionInt2) != null)
		{
			value = 2;
		}
		else if (rhPtr.rhFrame.bkdCol_TestPoint(expressionInt - rhPtr.rhWindowX, expressionInt2 - rhPtr.rhWindowY, -1, 0))
		{
			value = 1;
		}
		rhPtr.getCurrentResult().forceInt(value);
	}
}
