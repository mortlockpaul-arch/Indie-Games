using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTFORCEDIR : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int dir = ((evtParams[0].code != 29) ? rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]) : rhPtr.get_Direction(((PARAM_INT)evtParams[0]).value_Renamed));
			cObject.roa.animDir_Force(dir);
			cObject.roc.rcChanged = true;
		}
	}
}
