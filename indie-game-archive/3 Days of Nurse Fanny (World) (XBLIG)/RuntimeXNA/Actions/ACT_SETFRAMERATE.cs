using RuntimeXNA.Application;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETFRAMERATE : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		if (num >= 1 && num <= 1000)
		{
			CRunApp cRunApp = rhPtr.rhApp;
			while (cRunApp.parentApp != null)
			{
				cRunApp = cRunApp.parentApp;
			}
			cRunApp.gaFrameRate = num;
			cRunApp.setFrameRate(num);
		}
	}
}
