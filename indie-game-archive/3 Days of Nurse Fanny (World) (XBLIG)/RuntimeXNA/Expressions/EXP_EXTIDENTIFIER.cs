using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_EXTIDENTIFIER : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceInt(0);
			return;
		}
		int value = (cObject.hoCreationId << 16) | (cObject.hoNumber & 0xFFFF);
		rhPtr.getCurrentResult().forceInt(value);
	}
}
