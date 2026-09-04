using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSPEED : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int speed = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			if (cObject.rom != null)
			{
				cObject.rom.rmMovement.setSpeed(speed);
			}
		}
	}
}
