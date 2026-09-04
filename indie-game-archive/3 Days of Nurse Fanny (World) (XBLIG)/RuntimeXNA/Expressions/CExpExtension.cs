using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class CExpExtension : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceInt(0);
			return;
		}
		CExtension cExtension = (CExtension)cObject;
		int num = ((code >> 16) & 0xFFFF) - 80;
		CValue value = cExtension.expression(num);
		rhPtr.getCurrentResult().forceValue(value);
	}
}
