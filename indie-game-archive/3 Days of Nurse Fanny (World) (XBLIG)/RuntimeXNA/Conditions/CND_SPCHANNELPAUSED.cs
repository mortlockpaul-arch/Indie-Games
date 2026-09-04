using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_SPCHANNELPAUSED : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		return rhPtr.rhApp.soundPlayer.isChannelPaused(num - 1);
	}
}
