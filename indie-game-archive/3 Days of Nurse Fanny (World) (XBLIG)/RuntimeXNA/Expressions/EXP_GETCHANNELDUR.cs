using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_GETCHANNELDUR : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		int expressionInt = rhPtr.get_ExpressionInt();
		rhPtr.getCurrentResult().forceInt(rhPtr.rhApp.soundPlayer.getDurationChannel(expressionInt - 1));
	}
}
