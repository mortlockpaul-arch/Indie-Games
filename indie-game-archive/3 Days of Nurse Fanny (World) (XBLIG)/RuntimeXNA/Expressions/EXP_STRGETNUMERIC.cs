using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Expressions;

public class EXP_STRGETNUMERIC : CExpOi
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
		if (cText.rsTextBuffer != null)
		{
			CFuncVal cFuncVal = new CFuncVal();
			switch (cFuncVal.parse(cText.rsTextBuffer))
			{
			case 0:
				rhPtr.getCurrentResult().forceInt(cFuncVal.intValue);
				return;
			case 1:
				rhPtr.getCurrentResult().forceDouble(cFuncVal.doubleValue);
				return;
			}
		}
		rhPtr.getCurrentResult().forceInt(0);
	}
}
