using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETCHANNELVOL : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		int volume = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
		rhPtr.rhApp.soundPlayer.setVolumeChannel(num - 1, volume);
	}
}
