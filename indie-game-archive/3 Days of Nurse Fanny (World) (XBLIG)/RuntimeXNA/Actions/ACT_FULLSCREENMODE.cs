using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_FULLSCREENMODE : CAct
{
	public override void execute(CRun rhPtr)
	{
		rhPtr.rhApp.setFullScreen(flag: true);
	}
}
