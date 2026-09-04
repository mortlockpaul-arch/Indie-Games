using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_CHOOSEZONE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		PARAM_ZONE pZone = (PARAM_ZONE)evtParams[0];
		rhPtr.rhEvtProg.count_ZoneTypeObjects(pZone, -1, 0);
		if (rhPtr.rhEvtProg.evtNSelectedObjects == 0)
		{
			return false;
		}
		int stop = rhPtr.random((short)rhPtr.rhEvtProg.evtNSelectedObjects);
		CObject pHo = rhPtr.rhEvtProg.count_ZoneTypeObjects(pZone, stop, 0);
		rhPtr.rhEvtProg.evt_DeleteCurrent();
		rhPtr.rhEvtProg.evt_AddCurrentObject(pHo);
		return true;
	}
}
