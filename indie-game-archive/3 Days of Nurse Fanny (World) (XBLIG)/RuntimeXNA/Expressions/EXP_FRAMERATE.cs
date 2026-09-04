using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_FRAMERATE : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		int num = 0;
		for (int i = 0; i < 10; i++)
		{
			num += rhPtr.rh4FrameRateArray[i];
		}
		if (num != 0)
		{
			rhPtr.getCurrentResult().forceInt(10000 / num);
		}
		else
		{
			rhPtr.getCurrentResult().forceInt(0);
		}
	}
}
