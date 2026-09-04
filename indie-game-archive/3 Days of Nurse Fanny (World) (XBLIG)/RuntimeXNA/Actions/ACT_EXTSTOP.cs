using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSTOP : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null && cObject.rom != null)
		{
			cObject.rom.rmMovement.stop();
		}
	}
}
