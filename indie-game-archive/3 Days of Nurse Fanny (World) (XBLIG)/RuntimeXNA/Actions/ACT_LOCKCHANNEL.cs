using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_LOCKCHANNEL : CAct
{
	public override void execute(CRun rhPtr)
	{
		rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
	}
}
