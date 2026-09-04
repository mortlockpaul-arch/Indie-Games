using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_CDISPLAY : CAct
{
	public override void execute(CRun rhPtr)
	{
		CPosition cPosition = (CPosition)evtParams[0];
		CPositionInfo cPositionInfo = new CPositionInfo();
		cPosition.read_Position(rhPtr, 0, cPositionInfo);
		rhPtr.setDisplay(cPositionInfo.x, cPositionInfo.y, cPositionInfo.layer, 3);
	}
}
