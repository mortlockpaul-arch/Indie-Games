using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_EXTFLAG : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		rhPtr.rh4CurToken++;
		int expressionInt = rhPtr.get_ExpressionInt();
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceInt(0);
			return;
		}
		expressionInt &= 0x1F;
		if (cObject.rov != null)
		{
			int value = 0;
			if (((1 << expressionInt) & cObject.rov.rvValueFlags) != 0)
			{
				value = 1;
			}
			rhPtr.getCurrentResult().forceInt(value);
		}
		else
		{
			rhPtr.getCurrentResult().forceInt(0);
		}
	}
}
