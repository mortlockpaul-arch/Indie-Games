using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_DELALLCREATEDBKD : CAct
{
	public override void execute(CRun rhPtr)
	{
		int nLayer = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]) - 1;
		rhPtr.deleteAllBackdrop2(nLayer);
	}
}
