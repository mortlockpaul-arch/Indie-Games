using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_PLAYYBOTTOM : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		int num = rhPtr.rhWindowY;
		if ((rhPtr.rh3Scrolling & 1) != 0)
		{
			num = rhPtr.rh3DisplayY;
		}
		num += rhPtr.rh3WindowSy;
		if (num > rhPtr.rhLevelSy)
		{
			num = rhPtr.rhLevelSy;
		}
		rhPtr.getCurrentResult().forceInt(num);
	}
}
