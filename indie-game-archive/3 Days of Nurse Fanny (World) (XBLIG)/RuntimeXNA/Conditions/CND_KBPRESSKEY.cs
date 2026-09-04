using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_KBPRESSKEY : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		if (!rhPtr.isKeyDown(((PARAM_KEY)evtParams[0]).key))
		{
			return negaFALSE();
		}
		if (compute_GlobalNoRepeat(rhPtr))
		{
			return negaTRUE();
		}
		return negaFALSE();
	}
}
