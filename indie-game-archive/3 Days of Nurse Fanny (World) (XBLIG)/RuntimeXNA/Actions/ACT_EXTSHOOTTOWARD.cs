using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSHOOTTOWARD : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject == null)
		{
			return;
		}
		PARAM_SHOOT pARAM_SHOOT = (PARAM_SHOOT)evtParams[0];
		CPositionInfo cPositionInfo = new CPositionInfo();
		if (pARAM_SHOOT.read_Position(rhPtr, 17, cPositionInfo))
		{
			CPositionInfo cPositionInfo2 = new CPositionInfo();
			if (((CPosition)evtParams[1]).read_Position(rhPtr, 0, cPositionInfo2))
			{
				int x = cPositionInfo2.x;
				int y = cPositionInfo2.y;
				int dir = CRun.get_DirFromPente(x - cPositionInfo.x, y - cPositionInfo.y);
				cObject.shtCreate(pARAM_SHOOT, cPositionInfo.x, cPositionInfo.y, dir);
			}
		}
	}
}
