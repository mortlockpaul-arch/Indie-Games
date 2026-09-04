using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_GROUPSTART : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		CEventGroup rhEventGroup = rhPtr.rhEvtProg.rhEventGroup;
		if ((rhEventGroup.evgFlags & 1) != 0)
		{
			return false;
		}
		rhEventGroup.evgFlags |= 1;
		return true;
	}
}
