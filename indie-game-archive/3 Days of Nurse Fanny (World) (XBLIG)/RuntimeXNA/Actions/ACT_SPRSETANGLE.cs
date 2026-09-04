using RuntimeXNA.Banks;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SPRSETANGLE : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject == null)
		{
			return;
		}
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		bool flag = false;
		if (rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]) != 0)
		{
			flag = true;
		}
		num %= 360;
		if (num < 0)
		{
			num += 360;
		}
		bool flag2 = false;
		if ((cObject.ros.rsFlags & 0x10) != 0)
		{
			flag2 = true;
		}
		if (cObject.roc.rcAngle != num || flag2 != flag)
		{
			cObject.roc.rcAngle = num;
			cObject.ros.rsFlags &= -17;
			if (flag)
			{
				cObject.ros.rsFlags |= 16;
			}
			cObject.roc.rcChanged = true;
			CImage imageInfoEx = cObject.hoAdRunHeader.rhApp.imageBank.getImageInfoEx(cObject.roc.rcImage, cObject.roc.rcAngle, cObject.roc.rcScaleX, cObject.roc.rcScaleY);
			cObject.hoImgWidth = imageInfoEx.width;
			cObject.hoImgHeight = imageInfoEx.height;
			cObject.hoImgXSpot = imageInfoEx.xSpot;
			cObject.hoImgYSpot = imageInfoEx.ySpot;
		}
	}
}
