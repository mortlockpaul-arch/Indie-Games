using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_TIMMINITS : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		int num = (int)(rhPtr.rhTimer / 60000);
		rhPtr.getCurrentResult().forceInt(num % 60);
	}
}
