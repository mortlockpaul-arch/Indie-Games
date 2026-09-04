using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Actions;

public class ACT_EXTMOVEBEFORE : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject == null || cObject.ros == null)
		{
			return;
		}
		CObject cObject2 = rhPtr.rhEvtProg.get_ParamActionObjects(((PARAM_OBJECT)evtParams[0]).oiList, this);
		if (cObject2 != null)
		{
			CSprite rcSprite = cObject.roc.rcSprite;
			CSprite rcSprite2 = cObject2.roc.rcSprite;
			if (rcSprite != null && rcSprite2 != null)
			{
				rhPtr.rhApp.spriteGen.moveSpriteBefore(rcSprite, rcSprite2);
			}
		}
	}
}
