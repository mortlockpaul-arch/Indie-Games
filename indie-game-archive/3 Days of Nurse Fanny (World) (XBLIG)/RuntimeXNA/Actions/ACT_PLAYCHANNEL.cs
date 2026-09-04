using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_PLAYCHANNEL : CAct
{
	public override void execute(CRun rhPtr)
	{
		PARAM_SAMPLE pARAM_SAMPLE = (PARAM_SAMPLE)evtParams[0];
		bool bPrio = pARAM_SAMPLE.sndFlags != 0;
		int channel = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
		rhPtr.rhApp.soundPlayer.play(pARAM_SAMPLE.sndHandle, 1, channel, bPrio);
	}
}
