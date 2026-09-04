using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_CSUBVALUE : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			CValue pValue = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[0]);
			((CCounter)cObject).cpt_Sub(pValue);
		}
	}
}
