using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_MCLICK : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		short num = (short)rhPtr.rhEvtProg.rhCurParam0;
		if (((PARAM_SHORT)evtParams[0]).value != num)
		{
			return false;
		}
		return true;
	}

	public override bool eva2(CRun rhPtr)
	{
		if (((PARAM_SHORT)evtParams[0]).value == rhPtr.rhEvtProg.rh2CurrentClick)
		{
			return true;
		}
		return false;
	}
}
