using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_STRGETCURRENT : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceString("");
			return;
		}
		CText cText = (CText)cObject;
		if (cText.rsTextBuffer != null)
		{
			rhPtr.getCurrentResult().forceString(cText.rsTextBuffer);
		}
		else
		{
			rhPtr.getCurrentResult().forceString("");
		}
	}
}
