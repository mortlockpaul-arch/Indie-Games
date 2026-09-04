using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSHOOT : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			PARAM_SHOOT pARAM_SHOOT = (PARAM_SHOOT)evtParams[0];
			CPositionInfo cPositionInfo = new CPositionInfo();
			if (pARAM_SHOOT.read_Position(rhPtr, 17, cPositionInfo))
			{
				cObject.shtCreate(pARAM_SHOOT, cPositionInfo.x, cPositionInfo.y, cPositionInfo.dir);
			}
		}
	}
}
