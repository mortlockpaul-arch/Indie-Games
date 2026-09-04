using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTNOMOREOBJECT : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		if (hoPtr == null)
		{
			return eva2(rhPtr);
		}
		if (evtOi >= 0)
		{
			if (hoPtr.hoOi != evtOi)
			{
				return false;
			}
			return true;
		}
		return evaNoMoreObject(rhPtr, 1);
	}

	public override bool eva2(CRun rhPtr)
	{
		return evaNoMoreObject(rhPtr, 0);
	}

	internal virtual bool evaNoMoreObject(CRun rhPtr, int sub)
	{
		short num = evtOiList;
		if (num >= 0)
		{
			CObjInfo cObjInfo = rhPtr.rhOiList[num];
			if (cObjInfo.oilNObjects == 0)
			{
				return true;
			}
			return false;
		}
		if (num == -1)
		{
			return false;
		}
		CQualToOiList cQualToOiList = rhPtr.rhEvtProg.qualToOiList[num & 0x7FFF];
		int num2 = 0;
		for (int i = 0; i < cQualToOiList.qoiList.Length; i += 2)
		{
			CObjInfo cObjInfo = rhPtr.rhOiList[cQualToOiList.qoiList[i + 1]];
			num2 += cObjInfo.oilNObjects;
		}
		if (num2 - sub == 0)
		{
			return true;
		}
		return false;
	}
}
