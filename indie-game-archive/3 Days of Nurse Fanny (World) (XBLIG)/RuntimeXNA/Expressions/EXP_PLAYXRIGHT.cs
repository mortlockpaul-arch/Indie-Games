using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_PLAYXRIGHT : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		int num = rhPtr.rhWindowX;
		if ((rhPtr.rh3Scrolling & 1) != 0)
		{
			num = rhPtr.rh3DisplayX;
		}
		num += rhPtr.rh3WindowSx;
		if (num > rhPtr.rhLevelSx)
		{
			num = rhPtr.rhLevelSx;
		}
		rhPtr.getCurrentResult().forceInt(num);
	}
}
