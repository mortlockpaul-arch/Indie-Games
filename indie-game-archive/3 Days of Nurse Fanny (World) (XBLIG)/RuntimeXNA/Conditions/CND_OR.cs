using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_OR : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return false;
	}

	public override bool eva2(CRun rhPtr)
	{
		return false;
	}
}
