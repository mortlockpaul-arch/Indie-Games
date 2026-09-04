using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETSCORE : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		int num2 = evtOi;
		int[] scores = rhPtr.rhApp.scores;
		scores[num2] = num;
		rhPtr.update_PlayerObjects(num2, 5, scores[num2]);
	}
}
