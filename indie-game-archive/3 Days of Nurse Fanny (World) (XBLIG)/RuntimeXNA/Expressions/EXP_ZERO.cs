using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

internal class EXP_ZERO : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.getCurrentResult().forceInt(0);
	}
}
