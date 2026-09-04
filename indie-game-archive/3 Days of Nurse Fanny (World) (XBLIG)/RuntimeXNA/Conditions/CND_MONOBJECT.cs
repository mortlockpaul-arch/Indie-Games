using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_MONOBJECT : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		bool nega = (evtFlags2 & 1) != 0;
		PARAM_OBJECT pARAM_OBJECT = (PARAM_OBJECT)evtParams[0];
		return rhPtr.getMouseOnObjectsEDX(pARAM_OBJECT.oiList, nega);
	}
}
