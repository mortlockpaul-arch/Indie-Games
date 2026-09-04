using RuntimeXNA.Movements;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTBRANCHNODE : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			string pName = rhPtr.get_EventExpressionString((CParamExpression)evtParams[0]);
			if (cObject.roc.rcMovementType == 5)
			{
				CMovePath cMovePath = (CMovePath)cObject.rom.rmMovement;
				cMovePath.mtBranchNode(pName);
			}
		}
	}
}
