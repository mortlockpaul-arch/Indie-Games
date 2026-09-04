using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETTIMER : CAct
{
	public override void execute(CRun rhPtr)
	{
		long rhTimer = ((evtParams[0].code != 22) ? ((PARAM_TIME)evtParams[0]).timer : rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]));
		rhPtr.rhTimer = rhTimer;
		rhPtr.rhTimerOld = rhPtr.rhApp.timer - rhPtr.rhTimer;
		rhPtr.rhEvtProg.restartTimerEvents();
	}
}
