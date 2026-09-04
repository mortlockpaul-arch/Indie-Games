using RuntimeXNA.OI;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_STRGETNUMBER : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		rhPtr.rh4CurToken++;
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceString("");
			return;
		}
		int num = rhPtr.get_ExpressionInt();
		CText cText = (CText)cObject;
		if (num < 0)
		{
			if (cText.rsTextBuffer != null)
			{
				rhPtr.getCurrentResult().forceString(cText.rsTextBuffer);
			}
			else
			{
				rhPtr.getCurrentResult().forceString("");
			}
			return;
		}
		if (num >= cText.rsMaxi)
		{
			num = cText.rsMaxi - 1;
		}
		CDefTexts cDefTexts = (CDefTexts)cObject.hoCommon.ocObject;
		rhPtr.getCurrentResult().forceString(cDefTexts.otTexts[num].tsText);
	}
}
