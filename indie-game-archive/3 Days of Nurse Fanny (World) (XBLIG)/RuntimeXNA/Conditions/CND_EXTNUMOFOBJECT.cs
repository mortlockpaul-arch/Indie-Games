using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTNUMOFOBJECT : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		int num = 0;
		short num2 = evtOiList;
		if (num2 >= 0)
		{
			CObjInfo cObjInfo = rhPtr.rhOiList[num2];
			num = cObjInfo.oilNObjects;
		}
		else if (num2 != -1)
		{
			CQualToOiList cQualToOiList = rhPtr.rhEvtProg.qualToOiList[num2 & 0x7FFF];
			for (int i = 0; i < cQualToOiList.qoiList.Length; i += 2)
			{
				CObjInfo cObjInfo = rhPtr.rhOiList[cQualToOiList.qoiList[i + 1]];
				num += cObjInfo.oilNObjects;
			}
		}
		int value = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		return CRun.compareTer(num, value, ((CParamExpression)evtParams[0]).comparaison);
	}
}
