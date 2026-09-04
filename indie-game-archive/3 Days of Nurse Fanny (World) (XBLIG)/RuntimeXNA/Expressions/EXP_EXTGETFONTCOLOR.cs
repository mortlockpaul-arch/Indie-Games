using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Expressions;

public class EXP_EXTGETFONTCOLOR : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceInt(0);
			return;
		}
		int objectTextColor = CRun.getObjectTextColor(cObject);
		objectTextColor = CServices.swapRGB(objectTextColor);
		rhPtr.getCurrentResult().forceInt(objectTextColor);
	}
}
