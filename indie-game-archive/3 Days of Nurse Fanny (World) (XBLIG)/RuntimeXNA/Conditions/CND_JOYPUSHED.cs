using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_JOYPUSHED : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		short num = rhPtr.rhPlayer[evtOi];
		PARAM_SHORT pARAM_SHORT = (PARAM_SHORT)evtParams[0];
		num &= pARAM_SHORT.value;
		if (num != pARAM_SHORT.value)
		{
			return negaFALSE();
		}
		return negaTRUE();
	}
}
