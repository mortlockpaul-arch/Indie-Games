using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTRESTSPEED : CAct
{
	public override void execute(CRun rhPtr)
	{
		rhPtr.rhEvtProg.get_ActionObjects(this)?.roa.animSpeed_Restore();
	}
}
