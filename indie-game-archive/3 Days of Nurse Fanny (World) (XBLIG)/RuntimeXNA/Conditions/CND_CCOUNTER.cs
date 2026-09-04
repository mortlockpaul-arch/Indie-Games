using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_CCOUNTER : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
		int num = rhPtr.rhEvtProg.evtNSelectedObjects;
		CValue cValue = new CValue();
		while (cObject != null)
		{
			cValue.forceValue(((CCounter)cObject).cpt_GetValue());
			CValue pValue = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[0]);
			if (!CRun.compareTo(cValue, pValue, ((CParamExpression)evtParams[0]).comparaison))
			{
				num--;
				rhPtr.rhEvtProg.evt_DeleteCurrentObject();
			}
			cObject = rhPtr.rhEvtProg.evt_NextObject();
		}
		while (cObject != null)
		{
		}
		return num != 0;
	}
}
