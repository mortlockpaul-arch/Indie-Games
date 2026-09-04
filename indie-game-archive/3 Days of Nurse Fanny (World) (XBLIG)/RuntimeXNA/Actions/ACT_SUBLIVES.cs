using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SUBLIVES : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		int num2 = evtOi;
		num = rhPtr.rhApp.lives[num2] - num;
		rhPtr.actPla_FinishLives(num2, num);
	}
}
