using RuntimeXNA.Frame;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_CREATE : CAct
{
	public override void execute(CRun rhPtr)
	{
		PARAM_CREATE pARAM_CREATE = (PARAM_CREATE)evtParams[0];
		CPositionInfo cPositionInfo = new CPositionInfo();
		if (pARAM_CREATE.read_Position(rhPtr, 17, cPositionInfo))
		{
			if (cPositionInfo.bRepeat)
			{
				evtFlags |= 1;
				rhPtr.rhEvtProg.rh2ActionLoop = true;
			}
			else
			{
				evtFlags &= 254;
			}
		}
		int num = rhPtr.f_CreateObject(pARAM_CREATE.cdpHFII, pARAM_CREATE.cdpOi, cPositionInfo.x, cPositionInfo.y, cPositionInfo.dir, 0, cPositionInfo.layer, -1);
		if (num < 0)
		{
			return;
		}
		CObject cObject = rhPtr.rhObjectList[num];
		rhPtr.rhEvtProg.evt_AddCurrentObject(cObject);
		if (cPositionInfo.layer != -1 && (cObject.hoOEFlags & 0x200) != 0)
		{
			CLayer cLayer = rhPtr.rhFrame.layers[cPositionInfo.layer];
			if ((cLayer.dwOptions & 0x20010) != 16)
			{
				cObject.ros.obHide();
			}
		}
	}
}
