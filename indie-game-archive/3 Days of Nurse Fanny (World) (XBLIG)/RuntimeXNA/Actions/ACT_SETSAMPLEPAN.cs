using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETSAMPLEPAN : CAct
{
	public override void execute(CRun rhPtr)
	{
		PARAM_SAMPLE pARAM_SAMPLE = (PARAM_SAMPLE)evtParams[0];
		int pan = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
		rhPtr.rhApp.soundPlayer.setPanSample(pARAM_SAMPLE.sndHandle, pan);
	}
}
