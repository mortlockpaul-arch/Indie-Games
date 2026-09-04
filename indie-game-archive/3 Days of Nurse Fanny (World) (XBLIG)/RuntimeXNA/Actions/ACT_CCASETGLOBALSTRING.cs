using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_CCASETGLOBALSTRING : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int number = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			string value_Renamed = rhPtr.get_EventExpressionString((CParamExpression)evtParams[1]);
			((CCCA)cObject).setGlobalString(number, value_Renamed);
		}
	}
}
