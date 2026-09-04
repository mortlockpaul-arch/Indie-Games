using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTFORCESPEED : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int speed = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			cObject.roa.animSpeed_Force(speed);
		}
	}
}
