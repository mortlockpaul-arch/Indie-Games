using RuntimeXNA.Banks;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_EXTYAP : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		int num = 0;
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		if (cObject != null)
		{
			num = cObject.hoY;
			if (cObject.roa != null)
			{
				CImage imageInfoEx = rhPtr.rhApp.imageBank.getImageInfoEx(cObject.roc.rcImage, cObject.roc.rcAngle, cObject.roc.rcScaleX, cObject.roc.rcScaleY);
				if (imageInfoEx != null)
				{
					num += imageInfoEx.yAP - imageInfoEx.ySpot;
				}
			}
		}
		rhPtr.getCurrentResult().forceInt(num);
	}
}
