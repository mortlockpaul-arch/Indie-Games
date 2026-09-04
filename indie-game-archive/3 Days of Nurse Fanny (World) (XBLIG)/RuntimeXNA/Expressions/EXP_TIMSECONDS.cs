using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_TIMSECONDS : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		int num = (int)(rhPtr.rhTimer / 1000);
		rhPtr.getCurrentResult().forceInt(num % 60);
	}
}
