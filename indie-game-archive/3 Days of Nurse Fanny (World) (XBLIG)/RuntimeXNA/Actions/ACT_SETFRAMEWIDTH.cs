using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETFRAMEWIDTH : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		int leWidth = rhPtr.rhFrame.leWidth;
		rhPtr.rhFrame.leWidth = num;
		if (leWidth == rhPtr.rhFrame.leVirtualRect.right)
		{
			rhPtr.rhFrame.leVirtualRect.right = (rhPtr.rhLevelSx = num);
		}
		rhPtr.ohRedrawLevel(bRedrawTotalColMask: true);
	}
}
