using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_CHOOSEALL : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		rhPtr.rhEvtProg.count_ObjectsFromType(0, -1);
		if (rhPtr.rhEvtProg.evtNSelectedObjects == 0)
		{
			return false;
		}
		int stop = rhPtr.random((short)rhPtr.rhEvtProg.evtNSelectedObjects);
		CObject pHo = rhPtr.rhEvtProg.count_ObjectsFromType(0, stop);
		rhPtr.rhEvtProg.evt_DeleteCurrent();
		rhPtr.rhEvtProg.evt_AddCurrentObject(pHo);
		return true;
	}
}
