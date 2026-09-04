using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETINPUTKEY : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		if (num < 8)
		{
			int num2 = evtOi;
			if (num2 < 4)
			{
				rhPtr.rhApp.pcCtrlKeys[num2 * 4 + num] = ((PARAM_KEY)evtParams[1]).key;
			}
		}
	}
}
