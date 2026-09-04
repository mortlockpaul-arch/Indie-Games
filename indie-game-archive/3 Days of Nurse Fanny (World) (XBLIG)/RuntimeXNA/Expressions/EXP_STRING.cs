using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_STRING : CExp
{
	public string pString;

	public override void evaluate(CRun rhPtr)
	{
		rhPtr.getCurrentResult().forceString(pString);
	}
}
