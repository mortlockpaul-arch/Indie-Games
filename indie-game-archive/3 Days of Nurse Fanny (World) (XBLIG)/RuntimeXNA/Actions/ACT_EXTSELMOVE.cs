using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSELMOVE : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int mvt = ((evtParams[0].code != 22) ? ((PARAM_SHORT)evtParams[0]).value : rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]));
			if (cObject.rom != null)
			{
				cObject.rom.selectMovement(cObject, mvt);
			}
		}
	}
}
