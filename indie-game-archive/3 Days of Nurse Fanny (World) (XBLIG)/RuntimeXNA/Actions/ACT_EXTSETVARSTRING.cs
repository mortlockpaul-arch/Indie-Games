using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSETVARSTRING : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int num = ((evtParams[0].code != 62) ? ((PARAM_SHORT)evtParams[0]).value : rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]));
			if (num >= 0 && num < 10)
			{
				string s = rhPtr.get_EventExpressionString((CParamExpression)evtParams[1]);
				cObject.rov.setString(num, s);
			}
		}
	}
}
