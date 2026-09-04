using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_NOMOREALLZONE_OLD : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		rhPtr.rhEvtProg.count_ZoneTypeObjects((PARAM_ZONE)evtParams[0], -1, 2);
		if (rhPtr.rhEvtProg.evtNSelectedObjects != 0)
		{
			return false;
		}
		return true;
	}
}
