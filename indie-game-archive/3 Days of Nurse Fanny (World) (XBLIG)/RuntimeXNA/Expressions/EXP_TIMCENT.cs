using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_TIMCENT : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		int num = (int)(rhPtr.rhTimer / 10);
		rhPtr.getCurrentResult().forceInt(num % 100);
	}
}
