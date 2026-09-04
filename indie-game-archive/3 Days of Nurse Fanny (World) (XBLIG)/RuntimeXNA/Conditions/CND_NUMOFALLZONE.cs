using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_NUMOFALLZONE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		rhPtr.rhEvtProg.count_ZoneTypeObjects((PARAM_ZONE)evtParams[0], -1, 0);
		return compareCondition(rhPtr, 1, rhPtr.rhEvtProg.evtNSelectedObjects);
	}
}
