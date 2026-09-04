using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSETSEMITRANSPARENCY : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null && cObject.ros != null)
		{
			int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			if (num < 0)
			{
				num = 0;
			}
			if (num > 128)
			{
				num = 128;
			}
			cObject.ros.rsEffect &= -4096;
			cObject.ros.rsEffect |= 1;
			cObject.ros.rsEffectParam = num;
			cObject.roc.rcChanged = true;
			if (cObject.roc.rcSprite != null)
			{
				cObject.hoAdRunHeader.rhApp.spriteGen.modifSpriteEffect(cObject.roc.rcSprite, cObject.ros.rsEffect, cObject.ros.rsEffectParam);
			}
		}
	}
}
