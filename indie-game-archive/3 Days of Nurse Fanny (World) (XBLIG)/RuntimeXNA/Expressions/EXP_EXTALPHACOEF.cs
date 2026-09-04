using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

internal class EXP_EXTALPHACOEF : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		if (cObject == null || cObject.ros == null)
		{
			rhPtr.getCurrentResult().forceInt(0);
			return;
		}
		int rsEffect = cObject.ros.rsEffect;
		int rsEffectParam = cObject.ros.rsEffectParam;
		int num = 0;
		int num2 = rsEffectParam;
		num = (((rsEffect & 0xFFF) == 13 || (rsEffect & 0x1000) != 0) ? (255 - ((num2 >> 24) & 0xFF)) : ((rsEffectParam != -1) ? (rsEffectParam * 2) : 0));
		rhPtr.getCurrentResult().forceInt(num);
	}
}
