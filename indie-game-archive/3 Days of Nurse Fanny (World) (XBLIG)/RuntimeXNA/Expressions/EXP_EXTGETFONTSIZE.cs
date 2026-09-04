using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Expressions;

public class EXP_EXTGETFONTSIZE : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceInt(0);
			return;
		}
		CFontInfo objectFont = CRun.getObjectFont(cObject);
		rhPtr.getCurrentResult().forceInt(objectFont.lfHeight);
	}
}
