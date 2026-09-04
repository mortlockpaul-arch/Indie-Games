using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTNOMOREZONE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		int num = rhPtr.rhEvtProg.count_ZoneOneObject(evtOiList, (PARAM_ZONE)evtParams[0]);
		return num == 0;
	}
}
