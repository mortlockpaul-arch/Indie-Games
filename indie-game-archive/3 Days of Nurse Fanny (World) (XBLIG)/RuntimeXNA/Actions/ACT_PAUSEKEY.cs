using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_PAUSEKEY : CAct
{
	public override void execute(CRun rhPtr)
	{
		rhPtr.rh4PauseKey = ((PARAM_KEY)evtParams[0]).key;
		rhPtr.bAnyKeyDown = true;
		rhPtr.bCheckResume = true;
		rhPtr.pause();
	}
}
