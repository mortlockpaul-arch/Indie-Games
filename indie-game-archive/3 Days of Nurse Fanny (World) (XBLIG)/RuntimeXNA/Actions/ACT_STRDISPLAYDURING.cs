using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_STRDISPLAYDURING : CAct
{
	public override void execute(CRun rhPtr)
	{
		PARAM_SHORT pARAM_SHORT = (PARAM_SHORT)evtParams[1];
		int num = rhPtr.txtDoDisplay(this, pARAM_SHORT.value);
		if (num >= 0)
		{
			PARAM_TIME pARAM_TIME = (PARAM_TIME)evtParams[2];
			CObject cObject = rhPtr.rhObjectList[num];
			cObject.ros.rsFlash = pARAM_TIME.timer;
			cObject.ros.rsFlashCpt = pARAM_TIME.timer;
		}
	}
}
