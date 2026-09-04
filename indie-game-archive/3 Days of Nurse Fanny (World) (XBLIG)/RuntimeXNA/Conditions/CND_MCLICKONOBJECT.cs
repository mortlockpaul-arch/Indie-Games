using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_MCLICKONOBJECT : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		PARAM_SHORT pARAM_SHORT = (PARAM_SHORT)evtParams[0];
		if (rhPtr.rhEvtProg.rhCurParam0 != pARAM_SHORT.value)
		{
			return false;
		}
		short num = (short)rhPtr.rhEvtProg.rhCurParam1;
		PARAM_OBJECT pARAM_OBJECT = (PARAM_OBJECT)evtParams[1];
		if (num == pARAM_OBJECT.oi)
		{
			rhPtr.rhEvtProg.evt_AddCurrentObject(rhPtr.rhEvtProg.rh4_2ndObject);
			return true;
		}
		short oiList = pARAM_OBJECT.oiList;
		if (oiList >= 0)
		{
			return false;
		}
		CQualToOiList cQualToOiList = rhPtr.rhEvtProg.qualToOiList[oiList & 0x7FFF];
		for (int i = 0; i < cQualToOiList.qoiList.Length; i += 2)
		{
			if (cQualToOiList.qoiList[i] == num)
			{
				rhPtr.rhEvtProg.evt_AddCurrentQualifier(oiList);
				rhPtr.rhEvtProg.evt_AddCurrentObject(rhPtr.rhEvtProg.rh4_2ndObject);
				return true;
			}
		}
		return false;
	}

	public override bool eva2(CRun rhPtr)
	{
		PARAM_SHORT pARAM_SHORT = (PARAM_SHORT)evtParams[0];
		if (rhPtr.rhEvtProg.rh2CurrentClick != pARAM_SHORT.value)
		{
			return false;
		}
		PARAM_OBJECT pARAM_OBJECT = (PARAM_OBJECT)evtParams[1];
		return rhPtr.getMouseOnObjectsEDX(pARAM_OBJECT.oiList, nega: false);
	}
}
