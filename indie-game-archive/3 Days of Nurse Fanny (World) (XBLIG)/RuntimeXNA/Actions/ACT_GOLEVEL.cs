using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_GOLEVEL : CAct
{
	public override void execute(CRun rhPtr)
	{
		short num;
		if (evtParams[0].code == 26)
		{
			num = ((PARAM_SHORT)evtParams[0]).value;
			if (rhPtr.rhApp.HCellToNCell(num) == -1)
			{
				return;
			}
		}
		else
		{
			num = (short)(rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]) - 1);
			if (num < 0 || num >= 4096)
			{
				return;
			}
			num |= -32768;
		}
		rhPtr.rhQuit = 3;
		rhPtr.rhQuitParam = num;
	}
}
