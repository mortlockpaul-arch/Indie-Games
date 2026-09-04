using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_JOYPRESSED : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		int num = evtOi;
		if (num != rhPtr.rhEvtProg.rhCurOi)
		{
			return false;
		}
		short num2 = (short)rhPtr.rhEvtProg.rhCurParam0;
		PARAM_SHORT pARAM_SHORT = (PARAM_SHORT)evtParams[0];
		num2 &= pARAM_SHORT.value;
		if (num2 != pARAM_SHORT.value)
		{
			return false;
		}
		return true;
	}

	public override bool eva2(CRun rhPtr)
	{
		int num = evtOi;
		sbyte b = (sbyte)(rhPtr.rh2NewPlayer[num] & rhPtr.rhPlayer[num]);
		short num2 = b;
		PARAM_SHORT pARAM_SHORT = (PARAM_SHORT)evtParams[0];
		num2 &= pARAM_SHORT.value;
		if (pARAM_SHORT.value != num2)
		{
			return false;
		}
		return true;
	}
}
