using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_GETINPUT : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		int num = oi;
		int num2 = 5;
		if (num < 4)
		{
			num2 = rhPtr.rhApp.pcCtrlType[num];
		}
		if (num2 == 5)
		{
			num2 = 0;
		}
		rhPtr.getCurrentResult().forceInt(num2);
	}
}
