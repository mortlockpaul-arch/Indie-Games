using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_NEXTLEVEL : CAct
{
	public override void execute(CRun rhPtr)
	{
		rhPtr.rhQuit = 1;
	}
}
