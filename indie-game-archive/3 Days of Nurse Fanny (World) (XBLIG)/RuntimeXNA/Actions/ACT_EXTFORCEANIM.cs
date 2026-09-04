using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTFORCEANIM : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int anim = ((evtParams[0].code != 10) ? rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]) : ((PARAM_SHORT)evtParams[0]).value);
			cObject.roa.animation_Force(anim);
			cObject.roc.rcChanged = true;
		}
	}
}
