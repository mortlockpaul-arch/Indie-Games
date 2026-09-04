using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETPLAYERNAME : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = evtOi;
		if (num < 4)
		{
			string text = rhPtr.get_EventExpressionString((CParamExpression)evtParams[0]);
			rhPtr.rhApp.playerNames[num] = text;
		}
	}
}
