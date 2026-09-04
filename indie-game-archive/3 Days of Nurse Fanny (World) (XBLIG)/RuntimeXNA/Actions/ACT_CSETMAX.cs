using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_CSETMAX : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			CValue value = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[0]);
			((CCounter)cObject).cpt_SetMax(value);
		}
	}
}
