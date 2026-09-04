using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_PLAYYTOP : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		int num = rhPtr.rhWindowY;
		if ((rhPtr.rh3Scrolling & 1) != 0)
		{
			num = rhPtr.rh3DisplayY;
		}
		if (num < 0)
		{
			num = 0;
		}
		rhPtr.getCurrentResult().forceInt(num);
	}
}
