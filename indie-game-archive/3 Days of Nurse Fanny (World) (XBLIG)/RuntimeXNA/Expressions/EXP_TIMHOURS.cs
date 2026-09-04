using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_TIMHOURS : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		int value = (int)(rhPtr.rhTimer / 3600000);
		rhPtr.getCurrentResult().forceInt(value);
	}
}
