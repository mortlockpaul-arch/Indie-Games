using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_EXTDIR : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceInt(0);
		}
		else
		{
			rhPtr.getCurrentResult().forceInt(cObject.roc.rcDir);
		}
	}
}
