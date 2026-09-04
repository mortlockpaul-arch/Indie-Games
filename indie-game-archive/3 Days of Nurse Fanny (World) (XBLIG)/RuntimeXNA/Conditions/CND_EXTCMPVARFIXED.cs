using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTCMPVARFIXED : CCnd, IEvaExpObject
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return evaExpObject(rhPtr, this);
	}

	public override bool eva2(CRun rhPtr)
	{
		return evaExpObject(rhPtr, this);
	}

	public virtual bool evaExpRoutine(CObject hoPtr, int value_Renamed, short comp)
	{
		int value = (hoPtr.hoCreationId << 16) | (hoPtr.hoNumber & 0xFFFF);
		return CRun.compareTer(value, value_Renamed, comp);
	}
}
