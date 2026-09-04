using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSHOW : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null && cObject.ros != null)
		{
			cObject.ros.obShow();
			cObject.ros.rsFlags |= 32;
			cObject.ros.rsFlash = 0;
		}
	}
}
