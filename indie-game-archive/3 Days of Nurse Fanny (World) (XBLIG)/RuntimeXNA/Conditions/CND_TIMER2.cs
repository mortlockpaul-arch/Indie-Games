using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

internal class CND_TIMER2 : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		int num = ((evtParams[0].code != 22) ? ((PARAM_TIME)evtParams[0]).timer : rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]));
		PARAM_INT pARAM_INT = (PARAM_INT)evtParams[1];
		if (rhPtr.rhTimer >= num)
		{
			if (pARAM_INT.value_Renamed == rhPtr.rhLoopCount)
			{
				pARAM_INT.value_Renamed = rhPtr.rhLoopCount + 1;
				return false;
			}
			pARAM_INT.value_Renamed = rhPtr.rhLoopCount + 1;
			return true;
		}
		return false;
	}
}
