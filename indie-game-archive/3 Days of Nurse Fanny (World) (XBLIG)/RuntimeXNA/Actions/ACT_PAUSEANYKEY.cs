using Microsoft.Xna.Framework.Input;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_PAUSEANYKEY : CAct
{
	public override void execute(CRun rhPtr)
	{
		rhPtr.rh4PauseKey = Keys.None;
		rhPtr.bAnyKeyDown = true;
		rhPtr.bCheckResume = true;
		rhPtr.pause();
	}
}
