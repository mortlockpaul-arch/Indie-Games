using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_MCLICKINZONE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		short num = (short)rhPtr.rhEvtProg.rhCurParam0;
		if (((PARAM_SHORT)evtParams[0]).value == num)
		{
			PARAM_ZONE pARAM_ZONE = (PARAM_ZONE)evtParams[1];
			if (rhPtr.rh2MouseX >= pARAM_ZONE.x1 && rhPtr.rh2MouseX < pARAM_ZONE.x2 && rhPtr.rh2MouseY >= pARAM_ZONE.y1 && rhPtr.rh2MouseY < pARAM_ZONE.y2)
			{
				return true;
			}
		}
		return false;
	}

	public override bool eva2(CRun rhPtr)
	{
		if (((PARAM_SHORT)evtParams[0]).value == rhPtr.rhEvtProg.rh2CurrentClick)
		{
			PARAM_ZONE pARAM_ZONE = (PARAM_ZONE)evtParams[1];
			if (rhPtr.rh2MouseX >= pARAM_ZONE.x1 && rhPtr.rh2MouseX < pARAM_ZONE.x2 && rhPtr.rh2MouseY >= pARAM_ZONE.y1 && rhPtr.rh2MouseY < pARAM_ZONE.y2)
			{
				return true;
			}
		}
		return false;
	}
}
