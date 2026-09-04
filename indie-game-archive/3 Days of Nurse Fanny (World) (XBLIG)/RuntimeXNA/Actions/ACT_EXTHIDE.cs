using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTHIDE : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null && cObject.ros != null)
		{
			cObject.ros.obHide();
			cObject.ros.rsFlags &= -33;
			cObject.ros.rsFlash = 0;
		}
	}
}
