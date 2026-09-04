using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_CDISPLAYY : CAct
{
	public override void execute(CRun rhPtr)
	{
		int y = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		rhPtr.setDisplay(0, y, -1, 2);
	}
}
