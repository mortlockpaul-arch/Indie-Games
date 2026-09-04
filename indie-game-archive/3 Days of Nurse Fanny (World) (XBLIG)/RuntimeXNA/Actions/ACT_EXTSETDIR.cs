using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSETDIR : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject == null)
		{
			return;
		}
		int num = ((evtParams[0].code != 29) ? rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]) : rhPtr.get_Direction(((PARAM_INT)evtParams[0]).value_Renamed));
		num &= 0x1F;
		if (cObject.roc.rcDir != num)
		{
			cObject.roc.rcDir = num;
			cObject.roc.rcChanged = true;
			cObject.rom.rmMovement.setDir(num);
			if (cObject.hoType == 2)
			{
				cObject.roa.animIn(0);
			}
		}
	}
}
