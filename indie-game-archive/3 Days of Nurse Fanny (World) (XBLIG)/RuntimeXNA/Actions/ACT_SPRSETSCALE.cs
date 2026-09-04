using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SPRSETSCALE : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			float num = (float)rhPtr.get_EventExpressionDouble((CParamExpression)evtParams[0]);
			bool bResample = false;
			if (rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]) != 0)
			{
				bResample = true;
			}
			cObject.setScale(num, num, bResample);
		}
	}
}
