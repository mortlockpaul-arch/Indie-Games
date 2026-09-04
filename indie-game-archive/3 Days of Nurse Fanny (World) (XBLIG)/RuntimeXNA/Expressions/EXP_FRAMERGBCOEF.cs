using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

internal class EXP_FRAMERGBCOEF : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.getCurrentResult().forceInt(16777215);
	}
}
