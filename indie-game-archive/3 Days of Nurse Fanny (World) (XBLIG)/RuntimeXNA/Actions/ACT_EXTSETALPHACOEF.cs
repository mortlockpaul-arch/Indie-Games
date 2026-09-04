using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions;

internal class ACT_EXTSETALPHACOEF : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null && cObject.ros != null)
		{
			byte b = (byte)CServices.clamp(255 - rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]), 0, 255);
			bool flag = (cObject.ros.rsEffect & 0x1000) == 0;
			cObject.ros.rsEffect = (cObject.ros.rsEffect & 0xFFF) | 0x1000;
			int num = 16777215;
			if (!flag)
			{
				num = cObject.ros.rsEffectParam;
			}
			int num2 = b << 24;
			int num3 = num & 0xFFFFFF;
			cObject.ros.rsEffectParam = num2 | num3;
			cObject.roc.rcChanged = true;
			if (cObject.roc.rcSprite != null)
			{
				cObject.hoAdRunHeader.rhApp.spriteGen.modifSpriteEffect(cObject.roc.rcSprite, cObject.ros.rsEffect, cObject.ros.rsEffectParam);
			}
		}
	}
}
