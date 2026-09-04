using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_HEX : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		string value = rhPtr.get_ExpressionInt().ToString("X");
		rhPtr.getCurrentResult().forceString(value);
	}
}
