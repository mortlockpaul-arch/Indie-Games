using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTCHOOSE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		rhPtr.rhEvtProg.count_ObjectsFromOiList(evtOiList, -1);
		if (rhPtr.rhEvtProg.evtNSelectedObjects == 0)
		{
			return false;
		}
		short stop = rhPtr.random((short)rhPtr.rhEvtProg.evtNSelectedObjects);
		CObject pHo = rhPtr.rhEvtProg.count_ObjectsFromOiList(evtOiList, stop);
		rhPtr.rhEvtProg.evt_ForceOneObject(pHo);
		return true;
	}
}
