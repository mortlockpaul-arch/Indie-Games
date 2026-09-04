using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_STRDISPLAY : CAct
{
	public override void execute(CRun rhPtr)
	{
		PARAM_SHORT pARAM_SHORT = (PARAM_SHORT)evtParams[1];
		rhPtr.txtDoDisplay(this, pARAM_SHORT.value);
	}
}
