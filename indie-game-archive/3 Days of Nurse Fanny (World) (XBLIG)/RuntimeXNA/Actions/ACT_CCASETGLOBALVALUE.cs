using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_CCASETGLOBALVALUE : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int number = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			CValue value_Renamed = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[1]);
			((CCCA)cObject).setGlobalValue(number, value_Renamed);
		}
	}
}
