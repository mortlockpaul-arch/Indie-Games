using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTFORCEFRAME : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int frame = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			cObject.roa.animFrame_Force(frame);
			cObject.roc.rcChanged = true;
		}
	}
}
