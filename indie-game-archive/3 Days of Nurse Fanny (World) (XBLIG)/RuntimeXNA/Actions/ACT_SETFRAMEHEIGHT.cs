using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETFRAMEHEIGHT : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		int leHeight = rhPtr.rhFrame.leHeight;
		rhPtr.rhFrame.leHeight = num;
		if (leHeight == rhPtr.rhFrame.leVirtualRect.bottom)
		{
			rhPtr.rhFrame.leVirtualRect.bottom = (rhPtr.rhLevelSy = num);
		}
		rhPtr.ohRedrawLevel(bRedrawTotalColMask: true);
	}
}
