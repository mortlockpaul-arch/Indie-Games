using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_EXTVARSTRING : CExpOi
{
	public short number;

	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceString("");
		}
		else
		{
			rhPtr.getCurrentResult().forceString(cObject.rov.getString(number));
		}
	}
}
