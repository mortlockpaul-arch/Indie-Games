using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions;

public class ACT_STRSETCOLOUR : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int rsTextColor;
			if (evtParams[0].code == 24)
			{
				rsTextColor = ((PARAM_COLOUR)evtParams[0]).color;
			}
			else
			{
				rsTextColor = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
				rsTextColor = CServices.swapRGB(rsTextColor);
			}
			CText cText = (CText)cObject;
			cText.rsTextColor = rsTextColor;
			cObject.roc.rcChanged = true;
			cObject.display();
		}
	}
}
