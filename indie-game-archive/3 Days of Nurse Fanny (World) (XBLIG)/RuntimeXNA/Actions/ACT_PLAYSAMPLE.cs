using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_PLAYSAMPLE : CAct
{
	public override void execute(CRun rhPtr)
	{
		PARAM_SAMPLE pARAM_SAMPLE = (PARAM_SAMPLE)evtParams[0];
		bool bPrio = pARAM_SAMPLE.sndFlags != 0;
		rhPtr.rhApp.soundPlayer.play(pARAM_SAMPLE.sndHandle, 1, -1, bPrio);
	}
}
