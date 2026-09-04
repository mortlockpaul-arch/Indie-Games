using System;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_NEWLINE : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		string newLine = Environment.NewLine;
		rhPtr.getCurrentResult().forceString(newLine);
	}
}
