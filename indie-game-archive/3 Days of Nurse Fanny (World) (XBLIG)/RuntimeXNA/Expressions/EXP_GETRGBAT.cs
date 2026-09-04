using RuntimeXNA.Banks;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Expressions;

public class EXP_GETRGBAT : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
		rhPtr.rh4CurToken++;
		if (cObject == null)
		{
			rhPtr.getCurrentResult().forceInt(0);
			return;
		}
		int expressionInt = rhPtr.get_ExpressionInt();
		rhPtr.rh4CurToken++;
		int expressionInt2 = rhPtr.get_ExpressionInt();
		int value = 0;
		if (cObject.roc.rcImage != -1)
		{
			CImage imageFromHandle = rhPtr.rhApp.imageBank.getImageFromHandle(cObject.roc.rcImage);
			if (expressionInt > 0 && expressionInt < imageFromHandle.width && expressionInt2 > 0 && expressionInt2 < imageFromHandle.height)
			{
				int[] array = new int[imageFromHandle.width * imageFromHandle.height];
				imageFromHandle.image.GetData(array);
				value = array[expressionInt2 * imageFromHandle.width + expressionInt] & 0xFFFFFF;
				value = CServices.swapRGB(value);
			}
		}
		rhPtr.getCurrentResult().forceInt(value);
	}
}
