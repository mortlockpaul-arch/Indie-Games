using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSUBVAR : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int num = ((evtParams[0].code != 53) ? ((PARAM_SHORT)evtParams[0]).value : rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]));
			if (num >= 0 && num < 26 && cObject.rov != null)
			{
				CValue value = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[1]);
				cObject.rov.getValue(num).sub(value);
			}
		}
	}
}
