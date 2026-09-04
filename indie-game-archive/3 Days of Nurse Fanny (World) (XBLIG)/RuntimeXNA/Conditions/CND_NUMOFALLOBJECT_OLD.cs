using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_NUMOFALLOBJECT_OLD : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		rhPtr.rhEvtProg.count_ObjectsFromType(2, -1);
		return compareCondition(rhPtr, 0, rhPtr.rhEvtProg.evtNSelectedObjects);
	}
}
