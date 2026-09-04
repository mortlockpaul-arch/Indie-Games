using RuntimeXNA.Events;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_EXTNOBJECTS : CExpOi
{
	public override void evaluate(CRun rhPtr)
	{
		short num = oiList;
		if (num >= 0)
		{
			CObjInfo cObjInfo = rhPtr.rhOiList[num];
			rhPtr.getCurrentResult().forceInt(cObjInfo.oilNObjects);
			return;
		}
		int num2 = 0;
		if (num != -1)
		{
			CQualToOiList cQualToOiList = rhPtr.rhEvtProg.qualToOiList[num & 0x7FFF];
			for (int i = 0; i < cQualToOiList.qoiList.Length; i += 2)
			{
				CObjInfo cObjInfo = rhPtr.rhOiList[cQualToOiList.qoiList[i + 1]];
				num2 += cObjInfo.oilNObjects;
			}
		}
		rhPtr.getCurrentResult().forceInt(num2);
	}
}
