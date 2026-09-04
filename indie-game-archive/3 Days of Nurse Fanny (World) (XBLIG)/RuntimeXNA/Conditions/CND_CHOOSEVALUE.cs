using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_CHOOSEVALUE : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		int num = 0;
		for (CObject cObject = rhPtr.rhEvtProg.evt_FirstObjectFromType(-1); cObject != null; cObject = rhPtr.rhEvtProg.evt_NextObjectFromType())
		{
			num++;
			int n = ((evtParams[0].code != 53) ? ((PARAM_SHORT)evtParams[0]).value : rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]));
			CValue pValue = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[1]);
			if (cObject.rov != null)
			{
				CValue pValue2 = new CValue(cObject.rov.getValue(n));
				short comparaison = ((CParamExpression)evtParams[1]).comparaison;
				if (!CRun.compareTo(pValue2, pValue, comparaison))
				{
					rhPtr.rhEvtProg.evt_DeleteCurrentObject();
					num--;
				}
			}
		}
		if (num != 0)
		{
			return true;
		}
		return false;
	}
}
