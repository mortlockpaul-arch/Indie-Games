using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

internal class CND_EVERY2 : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		PARAM_INT pARAM_INT = (PARAM_INT)evtParams[1];
		if (pARAM_INT.value2 == 0)
		{
			int value_Renamed = ((evtParams[0].code != 22) ? ((PARAM_TIME)evtParams[0]).timer : rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]));
			pARAM_INT.value_Renamed = value_Renamed;
			pARAM_INT.value2 = -1;
		}
		else
		{
			pARAM_INT.value_Renamed -= rhPtr.rhTimerDelta;
			if (pARAM_INT.value_Renamed <= 0)
			{
				int value_Renamed = ((evtParams[0].code != 22) ? ((PARAM_TIME)evtParams[0]).timer : rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]));
				pARAM_INT.value_Renamed += value_Renamed;
				return true;
			}
		}
		return false;
	}
}
