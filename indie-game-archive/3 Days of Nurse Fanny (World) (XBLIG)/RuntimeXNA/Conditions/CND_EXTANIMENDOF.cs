using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTANIMENDOF : CCnd, IEvaExpObject, IEvaObject
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		int num = ((evtParams[0].code != 10) ? rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]) : ((PARAM_SHORT)evtParams[0]).value);
		if (num != rhPtr.rhEvtProg.rhCurParam0)
		{
			return false;
		}
		rhPtr.rhEvtProg.evt_AddCurrentObject(hoPtr);
		return true;
	}

	public override bool eva2(CRun rhPtr)
	{
		if (evtParams[0].code == 10)
		{
			return evaObject(rhPtr, this);
		}
		return evaExpObject(rhPtr, this);
	}

	public virtual bool evaExpRoutine(CObject hoPtr, int value_Renamed, short comp)
	{
		if (value_Renamed != hoPtr.roa.raAnimOn)
		{
			return false;
		}
		if (hoPtr.roa.raAnimNumberOfFrame == 0)
		{
			return true;
		}
		return false;
	}

	public virtual bool evaObjectRoutine(CObject hoPtr)
	{
		short value = ((PARAM_SHORT)evtParams[0]).value;
		if (value != hoPtr.roa.raAnimOn)
		{
			return false;
		}
		if (hoPtr.roa.raAnimNumberOfFrame == 0)
		{
			return true;
		}
		return false;
	}
}
