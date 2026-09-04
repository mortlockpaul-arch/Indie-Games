using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions;

public class ACT_SETFRAMEBDKCOLOR : CAct
{
	public override void execute(CRun rhPtr)
	{
		int leBackground;
		if (evtParams[0].code == 24)
		{
			leBackground = ((PARAM_COLOUR)evtParams[0]).color;
		}
		else
		{
			leBackground = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			leBackground = CServices.swapRGB(leBackground);
		}
		rhPtr.rhFrame.leBackground = leBackground;
		rhPtr.ohRedrawLevel(bRedrawTotalColMask: false);
	}
}
