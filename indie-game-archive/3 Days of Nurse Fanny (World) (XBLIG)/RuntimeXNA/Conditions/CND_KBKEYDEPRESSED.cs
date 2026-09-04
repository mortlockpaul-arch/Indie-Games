using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_KBKEYDEPRESSED : CCnd
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
		return negaTRUE();
	}
}
