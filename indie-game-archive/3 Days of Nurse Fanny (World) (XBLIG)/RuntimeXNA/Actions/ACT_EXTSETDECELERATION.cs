using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSETDECELERATION : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int dec = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			cObject.rom.rmMovement.setDec(dec);
		}
	}
}
