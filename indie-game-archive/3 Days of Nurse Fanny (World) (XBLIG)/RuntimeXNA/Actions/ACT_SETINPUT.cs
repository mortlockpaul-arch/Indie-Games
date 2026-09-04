using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETINPUT : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		if (num <= 5)
		{
			if (num == 0)
			{
				num = 5;
			}
			int num2 = evtOi;
			if (num2 < 4)
			{
				rhPtr.rhApp.pcCtrlType[num2] = (short)num;
			}
		}
	}
}
