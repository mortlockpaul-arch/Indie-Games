using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETSAMPLEMALNPAN : CAct
{
	public override void execute(CRun rhPtr)
	{
		int mainPan = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		rhPtr.rhApp.soundPlayer.setMainPan(mainPan);
	}
}
