using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTFACING : CCnd, IEvaExpObject, IEvaObject
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		if (evtParams[0].code == 29)
		{
			return evaObject(rhPtr, this);
		}
		return evaExpObject(rhPtr, this);
	}

	public virtual bool evaObjectRoutine(CObject hoPtr)
	{
		int value_Renamed = ((PARAM_INT)evtParams[0]).value_Renamed;
		for (int i = 0; i < 32; i++)
		{
			if (((1 << i) & value_Renamed) != 0 && hoPtr.roc.rcDir == i)
			{
				return negaTRUE();
			}
		}
		return negaFALSE();
	}

	public virtual bool evaExpRoutine(CObject hoPtr, int value_Renamed, short comp)
	{
		value_Renamed &= 0x1F;
		if (hoPtr.roc.rcDir == value_Renamed)
		{
			return negaTRUE();
		}
		return negaFALSE();
	}
}
