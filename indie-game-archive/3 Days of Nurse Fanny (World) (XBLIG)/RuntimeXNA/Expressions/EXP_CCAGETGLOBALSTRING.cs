using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_CCAGETGLOBALSTRING : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		rhPtr.rh4CurToken++;
		rhPtr.get_ExpressionInt();
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceString("");
		}
	}
}
