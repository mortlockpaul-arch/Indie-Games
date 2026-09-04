using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_GETSAMPLEPAN : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		string expressionString = rhPtr.get_ExpressionString();
		rhPtr.getCurrentResult().forceInt(rhPtr.rhApp.soundPlayer.getPanSample(expressionString));
	}
}
