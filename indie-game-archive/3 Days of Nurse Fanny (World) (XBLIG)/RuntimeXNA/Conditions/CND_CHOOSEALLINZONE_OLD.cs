using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_CHOOSEALLINZONE_OLD : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		PARAM_ZONE p = (PARAM_ZONE)evtParams[0];
		if (rhPtr.rhEvtProg.select_ZoneTypeObjects(p, 2) != 0)
		{
			return true;
		}
		return false;
	}
}
