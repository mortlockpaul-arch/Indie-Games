using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions;

public class ACT_EXTSETBOLD : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			CFontInfo objectFont = CRun.getObjectFont(cObject);
			if (num != 0)
			{
				objectFont.lfWeight = 700;
			}
			else
			{
				objectFont.lfWeight = 400;
			}
			CRun.setObjectFont(cObject, objectFont, null);
		}
	}
}
