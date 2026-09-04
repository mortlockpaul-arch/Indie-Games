using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_STRNUMBER : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceInt(0);
			return;
		}
		CText cText = (CText)cObject;
		rhPtr.getCurrentResult().forceInt(cText.rsMini + 1);
	}
}
