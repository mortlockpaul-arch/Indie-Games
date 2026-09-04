using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SPRPASTE : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			if (cObject.roa != null)
			{
				cObject.roa.animIn(0);
			}
			if (cObject.hoLayer == 0 && cObject.roc.rcSprite != null)
			{
				rhPtr.rhApp.spriteGen.activeSprite(cObject.roc.rcSprite, 1, null);
			}
			rhPtr.activeToBackdrop(cObject, ((PARAM_SHORT)evtParams[0]).value, bTrueObject: false);
		}
	}
}
