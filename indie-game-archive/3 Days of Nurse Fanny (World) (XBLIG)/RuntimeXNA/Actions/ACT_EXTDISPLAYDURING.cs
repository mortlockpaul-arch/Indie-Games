using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTDISPLAYDURING : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null && cObject.ros != null)
		{
			cObject.ros.obHide();
			cObject.ros.rsFlags &= -33;
			cObject.ros.rsFlash = ((PARAM_TIME)evtParams[0]).timer;
			cObject.ros.rsFlashCpt = ((PARAM_TIME)evtParams[0]).timer;
		}
	}
}
