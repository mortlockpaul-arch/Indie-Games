using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETVIRTUALWIDTH : CAct
{
	public override void execute(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		if (num < rhPtr.rhFrame.leWidth)
		{
			num = rhPtr.rhFrame.leWidth;
		}
		if (num > 2147479552)
		{
			num = 2147479552;
		}
		if (rhPtr.rhFrame.leVirtualRect.right != num)
		{
			rhPtr.rhFrame.leVirtualRect.right = (rhPtr.rhLevelSx = num);
		}
	}
}
