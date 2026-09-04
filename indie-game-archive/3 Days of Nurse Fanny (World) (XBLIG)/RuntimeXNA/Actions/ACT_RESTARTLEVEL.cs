using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_RESTARTLEVEL : CAct
{
	public override void execute(CRun rhPtr)
	{
		rhPtr.rhQuit = 101;
	}
}
