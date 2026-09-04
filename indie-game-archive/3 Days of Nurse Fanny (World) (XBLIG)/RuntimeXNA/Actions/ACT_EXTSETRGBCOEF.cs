using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions;

internal class ACT_EXTSETRGBCOEF : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null && cObject.ros != null)
		{
			uint num = (uint)rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			bool flag = (cObject.ros.rsEffect & 0x1000) == 0;
			cObject.ros.rsEffect = (cObject.ros.rsEffect & 0xFFF) | 0x1000;
			uint rsEffectParam = (uint)cObject.ros.rsEffectParam;
			uint num2 = ((!flag) ? (rsEffectParam & 0xFF000000u) : ((cObject.ros.rsEffectParam != -1) ? ((uint)(255 - cObject.ros.rsEffectParam * 2 << 24)) : 4278190080u));
			uint num3 = (uint)CServices.swapRGB((int)(num & 0xFFFFFF));
			uint rsEffectParam2 = num2 | num3;
			cObject.ros.rsEffectParam = (int)rsEffectParam2;
			cObject.roc.rcChanged = true;
			if (cObject.roc.rcSprite != null)
			{
				cObject.hoAdRunHeader.rhApp.spriteGen.modifSpriteEffect(cObject.roc.rcSprite, cObject.ros.rsEffect, cObject.ros.rsEffectParam);
			}
		}
	}
}
