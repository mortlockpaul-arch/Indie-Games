using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_PLAYLOOPCHANNEL : CAct
{
	public override void execute(CRun rhPtr)
	{
		PARAM_SAMPLE pARAM_SAMPLE = (PARAM_SAMPLE)evtParams[0];
		bool bPrio = pARAM_SAMPLE.sndFlags != 0;
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
		int nLoops = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[2]);
		rhPtr.rhApp.soundPlayer.play(pARAM_SAMPLE.sndHandle, nLoops, num - 1, bPrio);
	}
}
