using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTLOOKAT : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject == null)
		{
			return;
		}
		CPosition cPosition = (CPosition)evtParams[0];
		CPositionInfo cPositionInfo = new CPositionInfo();
		if (cPosition.read_Position(rhPtr, 0, cPositionInfo))
		{
			int x = cPositionInfo.x;
			int y = cPositionInfo.y;
			x -= cObject.hoX;
			y -= cObject.hoY;
			int num = CRun.get_DirFromPente(x, y);
			num &= 0x1F;
			if (cObject.roc.rcDir != num)
			{
				cObject.roc.rcDir = num;
				cObject.roc.rcChanged = true;
				cObject.rom.rmMovement.setDir(num);
			}
		}
	}
}
