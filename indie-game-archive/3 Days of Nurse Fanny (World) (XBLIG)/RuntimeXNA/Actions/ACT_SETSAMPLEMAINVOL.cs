using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETSAMPLEMAINVOL : CAct
{
	public override void execute(CRun rhPtr)
	{
		int mainVolume = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		rhPtr.rhApp.soundPlayer.setMainVolume(mainVolume);
	}
}
