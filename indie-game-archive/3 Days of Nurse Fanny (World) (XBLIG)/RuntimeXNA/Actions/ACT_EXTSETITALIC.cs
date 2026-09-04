using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions;

public class ACT_EXTSETITALIC : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			CFontInfo objectFont = CRun.getObjectFont(cObject);
			objectFont.lfItalic = (byte)num;
			CRun.setObjectFont(cObject, objectFont, null);
		}
	}
}
