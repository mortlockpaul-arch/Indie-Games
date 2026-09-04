using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_STRSETSTRING : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject == null)
		{
			return;
		}
		string text = rhPtr.get_EventExpressionString((CParamExpression)evtParams[0]);
		CText cText = (CText)cObject;
		if (cText.rsTextBuffer == null || (cText.rsTextBuffer != null && string.CompareOrdinal(text, cText.rsTextBuffer) != 0))
		{
			cText.txtSetString(text);
			cText.txtChange(-1);
			if ((cObject.ros.rsFlags & 1) == 0)
			{
				cObject.roc.rcChanged = true;
				cObject.display();
			}
		}
	}
}
