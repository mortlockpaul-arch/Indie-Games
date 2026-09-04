using RuntimeXNA.Application;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_GETINPUTKEY : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		int num = oi;
		rhPtr.rh4CurToken++;
		int expressionInt = rhPtr.get_ExpressionInt();
		string keyText = CKeyConvert.getKeyText(rhPtr.rhApp.pcCtrlKeys[num * 4 + expressionInt]);
		rhPtr.getCurrentResult().forceString(keyText);
	}
}
