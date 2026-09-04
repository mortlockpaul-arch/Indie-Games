using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions;

public class ACT_EXTSETFONTSIZE : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject == null)
		{
			return;
		}
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		int num2 = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
		CFontInfo objectFont = CRun.getObjectFont(cObject);
		int lfHeight = objectFont.lfHeight;
		objectFont.lfHeight = num;
		if (num2 == 0)
		{
			CRun.setObjectFont(cObject, objectFont, null);
			return;
		}
		CRect cRect = new CRect();
		float num3 = 1f;
		if (lfHeight != 0)
		{
			num3 = (float)num / (float)lfHeight;
		}
		cRect.right = (int)((float)cObject.hoImgWidth * num3);
		cRect.bottom = (int)((float)cObject.hoImgHeight * num3);
		cRect.left = 0;
		cRect.top = 0;
		CRun.setObjectFont(cObject, objectFont, cRect);
	}
}
