using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_NOTALWAYS : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		CEventGroup rhEventGroup = rhPtr.rhEvtProg.rhEventGroup;
		if ((rhEventGroup.evgFlags & 2) != 0)
		{
			return true;
		}
		if ((rhEventGroup.evgFlags & 8) != 0)
		{
			return false;
		}
		rhEventGroup.evgInhibit = 65534;
		rhEventGroup.evgFlags |= 2;
		return true;
	}
}
