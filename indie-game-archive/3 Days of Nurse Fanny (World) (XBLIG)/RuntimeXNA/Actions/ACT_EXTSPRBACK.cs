using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSPRBACK : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null && cObject.roc.rcSprite != null)
		{
			rhPtr.rhApp.spriteGen.moveSpriteToBack(cObject.roc.rcSprite);
		}
	}
}
