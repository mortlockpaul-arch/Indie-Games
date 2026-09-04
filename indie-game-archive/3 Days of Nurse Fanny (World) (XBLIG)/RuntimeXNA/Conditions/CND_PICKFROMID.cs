using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_PICKFROMID : CCnd
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);
	}

	public override bool eva2(CRun rhPtr)
	{
		int val = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		return rhPtr.rhEvtProg.pickFromId(val);
	}
}
