using RuntimeXNA.Frame;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Actions;

public class ACT_EXTMOVETOLAYER : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject == null || cObject.ros == null)
		{
			return;
		}
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		if (num <= 0 || num > rhPtr.rhFrame.nLayers)
		{
			return;
		}
		num--;
		cObject.hoLayer = (short)num;
		if (cObject.ros == null)
		{
			return;
		}
		CSprite rcSprite = cObject.roc.rcSprite;
		if (rcSprite != null)
		{
			rhPtr.rhApp.spriteGen.setSpriteLayer(rcSprite, num);
			CLayer cLayer = rhPtr.rhFrame.layers[num];
			cLayer.nZOrderMax++;
			rcSprite.sprZOrder = cLayer.nZOrderMax;
			cObject.ros.rsZOrder = rcSprite.sprZOrder;
			if ((cLayer.dwOptions & 0x20010) != 16)
			{
				rhPtr.rhApp.spriteGen.activeSprite(rcSprite, 1, null);
				cObject.ros.obHide();
			}
			else if ((cObject.ros.rsFlags & 0x20) != 0 && (cObject.ros.rsFlags & 1) != 0 && (cLayer.dwOptions & 0x20010) == 16)
			{
				cObject.ros.obShow();
			}
		}
	}
}
