using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SPRADDBKD : CAct
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
			rhPtr.activeToBackdrop(cObject, ((PARAM_SHORT)evtParams[0]).value, bTrueObject: true);
		}
	}
}
