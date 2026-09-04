using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTCLRFLAG : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null && cObject.rov != null)
		{
			int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			cObject.rov.rvValueFlags &= ~(1 << num);
		}
	}
}
