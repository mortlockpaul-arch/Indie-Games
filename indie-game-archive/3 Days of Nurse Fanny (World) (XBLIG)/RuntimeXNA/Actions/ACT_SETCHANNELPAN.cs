using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETCHANNELPAN : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		int pan = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
		rhPtr.rhApp.soundPlayer.setPanChannel(num - 1, pan);
	}
}
