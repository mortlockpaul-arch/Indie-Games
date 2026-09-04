using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_EXTVARSTRINGBYINDEX : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		rhPtr.rh4CurToken++;
		int expressionInt = rhPtr.get_ExpressionInt();
		if (cObject == null || expressionInt < 0 || expressionInt >= 10)
		{
			rhPtr.getCurrentResult().forceString("");
		}
		else
		{
			rhPtr.getCurrentResult().forceString(cObject.rov.getString(expressionInt));
		}
	}
}
