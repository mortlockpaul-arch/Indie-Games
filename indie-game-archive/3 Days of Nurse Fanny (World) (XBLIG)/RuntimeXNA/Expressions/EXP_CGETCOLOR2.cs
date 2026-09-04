using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_CGETCOLOR2 : CExpOi
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
			rhPtr.getCurrentResult().forceInt(((CCounter)cObject).cpt_GetColor1());
		}
	}
}
