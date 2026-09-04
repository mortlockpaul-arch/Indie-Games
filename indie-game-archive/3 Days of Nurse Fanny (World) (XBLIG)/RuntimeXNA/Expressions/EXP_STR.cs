using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_STR : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		CValue expression = rhPtr.getExpression();
		string value = "";
		switch (expression.getType())
		{
		case 0:
			value = expression.getInt().ToString();
			break;
		case 1:
			value = expression.getDouble().ToString();
			break;
		}
		rhPtr.getCurrentResult().forceString(value);
	}
}
