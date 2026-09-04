using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

internal class EXP_EMPTY : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.getCurrentResult().forceString("");
	}
}
