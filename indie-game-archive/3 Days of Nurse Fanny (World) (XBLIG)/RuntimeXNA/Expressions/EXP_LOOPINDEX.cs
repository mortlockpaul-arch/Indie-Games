using System;
using RuntimeXNA.Actions;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_LOOPINDEX : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		string expressionString = rhPtr.get_ExpressionString();
		for (int i = 0; i < rhPtr.rh4FastLoops.size(); i++)
		{
			CLoop cLoop = (CLoop)rhPtr.rh4FastLoops.get(i);
			if (string.Compare(cLoop.name, expressionString, StringComparison.OrdinalIgnoreCase) == 0)
			{
				rhPtr.getCurrentResult().forceInt(cLoop.index);
				return;
			}
		}
		rhPtr.getCurrentResult().forceInt(0);
	}
}
