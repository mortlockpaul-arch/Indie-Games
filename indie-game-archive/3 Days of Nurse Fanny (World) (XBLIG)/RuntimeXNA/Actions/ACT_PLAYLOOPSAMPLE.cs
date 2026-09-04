using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_PLAYLOOPSAMPLE : CAct
{
	public override void execute(CRun rhPtr)
	{
		PARAM_SAMPLE pARAM_SAMPLE = (PARAM_SAMPLE)evtParams[0];
		int nLoops = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
		bool bPrio = pARAM_SAMPLE.sndFlags != 0;
		rhPtr.rhApp.soundPlayer.play(pARAM_SAMPLE.sndHandle, nLoops, -1, bPrio);
	}
}
