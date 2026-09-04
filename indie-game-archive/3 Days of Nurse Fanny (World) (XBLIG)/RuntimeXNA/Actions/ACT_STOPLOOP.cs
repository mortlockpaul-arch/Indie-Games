using System;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_STOPLOOP : CAct
{
	public override void execute(CRun rhPtr)
	{
		string text = rhPtr.get_EventExpressionString((CParamExpression)evtParams[0]);
		if (text.Length == 0)
		{
			return;
		}
		for (int i = 0; i < rhPtr.rh4FastLoops.size(); i++)
		{
			CLoop cLoop = (CLoop)rhPtr.rh4FastLoops.get(i);
			if (string.Compare(cLoop.name, text, StringComparison.OrdinalIgnoreCase) == 0)
			{
				cLoop.flags |= 1;
			}
		}
	}
}
