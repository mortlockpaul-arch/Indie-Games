using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Expressions;

internal class EXP_EXTRGBCOEF : CExpOi
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
		num = (((rsEffect & 0xFFF) != 13 && (rsEffect & 0x1000) == 0) ? 16777215 : CServices.swapRGB(num2 & 0xFFFFFF));
		rhPtr.getCurrentResult().forceInt(num);
	}
}
