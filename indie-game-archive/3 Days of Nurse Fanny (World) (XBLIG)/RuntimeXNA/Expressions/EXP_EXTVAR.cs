using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_EXTVAR : CExpOi
{
	public short number;

	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceInt(0);
		}
		else if (cObject.rov != null)
		{
			rhPtr.getCurrentResult().forceValue(cObject.rov.getValue(number));
		}
		else
		{
			rhPtr.getCurrentResult().forceInt(0);
		}
	}
}
