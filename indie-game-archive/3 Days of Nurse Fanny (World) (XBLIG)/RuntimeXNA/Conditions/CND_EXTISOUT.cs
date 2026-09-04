using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTISOUT : CCnd, IEvaObject
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
		int num = pHo.hoX - pHo.hoImgXSpot;
		int x = num + pHo.hoImgWidth;
		int num2 = pHo.hoY - pHo.hoImgYSpot;
		int y = num2 + pHo.hoImgHeight;
		if (pHo.hoAdRunHeader.quadran_In(num, num2, x, y) != 0)
		{
			return negaTRUE();
		}
		return negaFALSE();
	}
}
