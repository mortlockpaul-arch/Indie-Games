using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_RESUMESAMPLE : CAct
{
	public override void execute(CRun rhPtr)
	{
		PARAM_SAMPLE pARAM_SAMPLE = (PARAM_SAMPLE)evtParams[0];
		rhPtr.rhApp.soundPlayer.resumeSample(pARAM_SAMPLE.sndHandle);
	}
}
