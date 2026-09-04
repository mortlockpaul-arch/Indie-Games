using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions;

public class ACT_EXTSETFONTNAME : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			string lfFaceName = rhPtr.get_EventExpressionString((CParamExpression)evtParams[0]);
			CFontInfo objectFont = CRun.getObjectFont(cObject);
			objectFont.lfFaceName = lfFaceName;
			CRun.setObjectFont(cObject, objectFont, null);
		}
	}
}
