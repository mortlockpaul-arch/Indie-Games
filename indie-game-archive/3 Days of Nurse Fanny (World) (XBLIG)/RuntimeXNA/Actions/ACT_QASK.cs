using RuntimeXNA.Events;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_QASK : CAct
{
	public override void execute(CRun rhPtr)
	{
		if (evtOiList >= 0)
		{
			qstCreate(rhPtr, evtOi);
		}
		else if (evtOiList != -1)
		{
			CQualToOiList cQualToOiList = rhPtr.rhEvtProg.qualToOiList[evtOiList & 0x7FFF];
			for (int i = 0; i < cQualToOiList.qoiList.Length; i += 2)
			{
				qstCreate(rhPtr, cQualToOiList.qoiList[i]);
			}
		}
	}

	internal virtual void qstCreate(CRun rhPtr, short oi)
	{
		CCreate cCreate = (CCreate)evtParams[0];
		CPositionInfo cPositionInfo = new CPositionInfo();
		if (cCreate.read_Position(rhPtr, 16, cPositionInfo))
		{
			rhPtr.f_CreateObject(cCreate.cdpHFII, oi, cPositionInfo.x, cPositionInfo.y, cPositionInfo.dir, 0, rhPtr.rhFrame.nLayers - 1, -1);
		}
	}
}
