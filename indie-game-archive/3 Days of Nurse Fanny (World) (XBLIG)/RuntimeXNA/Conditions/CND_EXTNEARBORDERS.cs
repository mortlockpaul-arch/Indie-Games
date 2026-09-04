using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTNEARBORDERS : CCnd, IEvaExpObject
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return evaExpObject(rhPtr, this);
	}

	public override bool eva2(CRun rhPtr)
	{
		return evaExpObject(rhPtr, this);
	}

	public virtual bool evaExpRoutine(CObject hoPtr, int bord, short comp)
	{
		int num = hoPtr.hoAdRunHeader.rhWindowX + bord;
		int num2 = hoPtr.hoX - hoPtr.hoImgXSpot;
		if (num2 <= num)
		{
			return negaTRUE();
		}
		num = hoPtr.hoAdRunHeader.rhWindowX + hoPtr.hoAdRunHeader.rh3WindowSx - bord;
		num2 += hoPtr.hoImgWidth;
		if (num2 >= num)
		{
			return negaTRUE();
		}
		int num3 = hoPtr.hoAdRunHeader.rhWindowY + bord;
		int num4 = hoPtr.hoY - hoPtr.hoImgYSpot;
		if (num4 <= num3)
		{
			return negaTRUE();
		}
		num3 = hoPtr.hoAdRunHeader.rhWindowY + hoPtr.hoAdRunHeader.rh3WindowSy - bord;
		num4 += hoPtr.hoImgHeight;
		if (num4 >= num3)
		{
			return negaTRUE();
		}
		return negaFALSE();
	}
}
