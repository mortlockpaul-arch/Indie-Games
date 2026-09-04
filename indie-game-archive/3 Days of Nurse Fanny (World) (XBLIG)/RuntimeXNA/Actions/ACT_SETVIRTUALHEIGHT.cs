using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETVIRTUALHEIGHT : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		if (num < rhPtr.rhFrame.leHeight)
		{
			num = rhPtr.rhFrame.leHeight;
		}
		if (num > 2147479552)
		{
			num = 2147479552;
		}
		if (rhPtr.rhFrame.leVirtualRect.bottom != num)
		{
			rhPtr.rhFrame.leVirtualRect.bottom = (rhPtr.rhLevelSy = num);
		}
	}
}
