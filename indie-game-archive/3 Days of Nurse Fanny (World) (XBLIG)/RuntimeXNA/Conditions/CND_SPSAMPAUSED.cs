using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_SPSAMPAUSED : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		PARAM_SAMPLE pARAM_SAMPLE = (PARAM_SAMPLE)evtParams[0];
		return !rhPtr.rhApp.soundPlayer.isSamplePaused(pARAM_SAMPLE.sndHandle);
	}
}
