using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_MINZONE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		PARAM_ZONE pARAM_ZONE = (PARAM_ZONE)evtParams[0];
		if (rhPtr.rh2MouseX >= pARAM_ZONE.x1 && rhPtr.rh2MouseX < pARAM_ZONE.x2 && rhPtr.rh2MouseY >= pARAM_ZONE.y1 && rhPtr.rh2MouseY < pARAM_ZONE.y2)
		{
			return negaTRUE();
		}
		return negaFALSE();
	}
}
