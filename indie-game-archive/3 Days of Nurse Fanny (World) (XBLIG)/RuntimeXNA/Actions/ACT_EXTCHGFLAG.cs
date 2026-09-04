using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTCHGFLAG : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null && cObject.rov != null)
		{
			int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			int num2 = 1 << num;
			if ((cObject.rov.rvValueFlags & num2) != 0)
			{
				cObject.rov.rvValueFlags &= ~num2;
			}
			else
			{
				cObject.rov.rvValueFlags |= num2;
			}
		}
	}
}
