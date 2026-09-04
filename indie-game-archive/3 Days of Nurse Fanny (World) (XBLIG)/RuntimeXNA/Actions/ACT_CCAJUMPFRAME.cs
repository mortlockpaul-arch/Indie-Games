using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_CCAJUMPFRAME : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int frame = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			((CCCA)cObject).jumpFrame(frame);
		}
	}
}
