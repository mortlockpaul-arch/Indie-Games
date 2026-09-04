using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_STRSET : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int num = ((evtParams[0].code != 31) ? (rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]) - 1) : ((PARAM_SHORT)evtParams[0]).value);
			CText cText = (CText)cObject;
			if (cText.txtChange(num))
			{
				cObject.roc.rcChanged = true;
				cObject.display();
			}
		}
	}
}
