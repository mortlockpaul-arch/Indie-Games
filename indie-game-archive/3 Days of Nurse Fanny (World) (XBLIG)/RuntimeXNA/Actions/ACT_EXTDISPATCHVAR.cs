using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTDISPATCHVAR : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int n = ((evtParams[0].code != 53) ? ((PARAM_SHORT)evtParams[0]).value : rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]));
			PARAM_INT pARAM_INT = (PARAM_INT)evtParams[2];
			if (rhPtr.rhEvtProg.rh2ActionLoopCount == 0)
			{
				pARAM_INT.value_Renamed = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
			}
			else
			{
				pARAM_INT.value_Renamed++;
			}
			if (cObject.rov != null)
			{
				cObject.rov.getValue(n).forceInt(pARAM_INT.value_Renamed);
			}
		}
	}
}
