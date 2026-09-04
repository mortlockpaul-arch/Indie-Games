using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSETDIRECTIONS : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int value_Renamed = ((PARAM_INT)evtParams[0]).value_Renamed;
			cObject.rom.rmMovement.set8Dirs(value_Renamed);
		}
	}
}
