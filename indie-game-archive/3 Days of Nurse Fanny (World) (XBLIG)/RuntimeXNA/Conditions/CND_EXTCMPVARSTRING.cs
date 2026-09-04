using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTCMPVARSTRING : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
		if (cObject == null)
		{
			return false;
		}
		int num = rhPtr.rhEvtProg.evtNSelectedObjects;
		CValue cValue = new CValue();
		CParamExpression cParamExpression = (CParamExpression)evtParams[1];
		do
		{
			int num2 = ((evtParams[0].code != 62) ? ((PARAM_SHORT)evtParams[0]).value : rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]));
			if (num2 >= 0 && num2 < 10 && cObject.rov != null)
			{
				cValue.forceString(cObject.rov.getString(num2));
				CValue pValue = rhPtr.get_EventExpressionAny(cParamExpression);
				if (!CRun.compareTo(cValue, pValue, cParamExpression.comparaison))
				{
					num--;
					rhPtr.rhEvtProg.evt_DeleteCurrentObject();
				}
			}
			else
			{
				num--;
				rhPtr.rhEvtProg.evt_DeleteCurrentObject();
			}
			cObject = rhPtr.rhEvtProg.evt_NextObject();
		}
		while (cObject != null);
		return num != 0;
	}
}
