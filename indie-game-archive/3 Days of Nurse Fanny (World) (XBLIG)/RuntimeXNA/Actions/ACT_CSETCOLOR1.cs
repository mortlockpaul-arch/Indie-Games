using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions;

public class ACT_CSETCOLOR1 : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int rgb;
			if (evtParams[0].code == 22)
			{
				rgb = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
				rgb = CServices.swapRGB(rgb);
			}
			else
			{
				rgb = ((PARAM_COLOUR)evtParams[0]).color;
			}
			((CCounter)cObject).cpt_SetColor1(rgb);
		}
	}
}
