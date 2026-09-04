using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSETPOS : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			CPosition cPosition = (CPosition)evtParams[0];
			CPositionInfo cPositionInfo = new CPositionInfo();
			if (cPosition.read_Position(rhPtr, 0, cPositionInfo))
			{
				CRun.setXPosition(cObject, cPositionInfo.x);
				CRun.setYPosition(cObject, cPositionInfo.y);
			}
		}
	}
}
