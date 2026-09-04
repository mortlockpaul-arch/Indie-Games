using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_NOMOREALLZONE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		PARAM_ZONE pZone = (PARAM_ZONE)evtParams[0];
		rhPtr.rhEvtProg.count_ZoneTypeObjects(pZone, -1, 0);
		if (rhPtr.rhEvtProg.evtNSelectedObjects != 0)
		{
			return false;
		}
		return true;
	}
}
