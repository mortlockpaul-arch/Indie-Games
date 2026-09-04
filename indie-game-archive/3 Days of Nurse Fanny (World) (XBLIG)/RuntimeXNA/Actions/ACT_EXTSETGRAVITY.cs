using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSETGRAVITY : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int gravity = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			cObject.rom.rmMovement.setGravity(gravity);
		}
	}
}
