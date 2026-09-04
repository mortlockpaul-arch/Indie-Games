using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSTOPANIM : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			cObject.roa.raAnimStopped = true;
		}
	}
}
