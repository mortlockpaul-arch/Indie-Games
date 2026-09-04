using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_PLAYXLEFT : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		int num = rhPtr.rhWindowX;
		if ((rhPtr.rh3Scrolling & 1) != 0)
		{
			num = rhPtr.rh3DisplayX;
		}
		if (num < 0)
		{
			num = 0;
		}
		rhPtr.getCurrentResult().forceInt(num);
	}
}
