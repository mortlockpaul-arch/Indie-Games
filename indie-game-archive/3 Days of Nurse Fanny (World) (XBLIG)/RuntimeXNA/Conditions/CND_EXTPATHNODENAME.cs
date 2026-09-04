using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTPATHNODENAME : CCnd, IEvaObject
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		string strB = rhPtr.get_EventExpressionString((CParamExpression)evtParams[0]);
		if (hoPtr.hoMT_NodeName != null && string.CompareOrdinal(hoPtr.hoMT_NodeName, strB) == 0)
		{
			return true;
		}
		return false;
	}

	public override bool eva2(CRun rhPtr)
	{
		return evaObject(rhPtr, this);
	}

	public virtual bool evaObjectRoutine(CObject hoPtr)
	{
		if (hoPtr.roc.rcMovementType != 5)
		{
			return false;
		}
		if (checkMark(hoPtr.hoAdRunHeader, hoPtr.hoMark1))
		{
			string strB = hoPtr.hoAdRunHeader.get_EventExpressionString((CParamExpression)evtParams[0]);
			if (hoPtr.hoMT_NodeName != null && string.CompareOrdinal(hoPtr.hoMT_NodeName, strB) == 0)
			{
				return true;
			}
		}
		return false;
	}
}
