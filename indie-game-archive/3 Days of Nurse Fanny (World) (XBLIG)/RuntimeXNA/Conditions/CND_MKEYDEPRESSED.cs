using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_MKEYDEPRESSED : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		switch (((PARAM_KEY)evtParams[0]).mouseKey)
		{
		case 1:
			if (rhPtr.mouseKey == 0)
			{
				return negaTRUE();
			}
			break;
		case 2:
			if (rhPtr.mouseKey == 2)
			{
				return negaTRUE();
			}
			break;
		case 3:
			if (rhPtr.mouseKey == 1)
			{
				return negaTRUE();
			}
			break;
		}
		return negaFALSE();
	}
}
