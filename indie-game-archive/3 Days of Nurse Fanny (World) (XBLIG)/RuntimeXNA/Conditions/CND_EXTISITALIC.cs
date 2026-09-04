using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Conditions;

public class CND_EXTISITALIC : CCnd, IEvaObject
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return evaObject(rhPtr, this);
	}

	public override bool eva2(CRun rhPtr)
	{
		return evaObject(rhPtr, this);
	}

	public virtual bool evaObjectRoutine(CObject pHo)
	{
		CFontInfo objectFont = CRun.getObjectFont(pHo);
		if (objectFont.lfItalic != 0)
		{
			return true;
		}
		return false;
	}
}
