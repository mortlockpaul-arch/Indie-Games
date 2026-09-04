using RuntimeXNA.Expressions;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_ADDVARG : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = ((evtParams[0].code != 52) ? ((PARAM_SHORT)evtParams[0]).value : (rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]) - 1));
		CValue value = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[1]);
		rhPtr.rhApp.getGlobalValueAt(num).add(value);
	}
}
