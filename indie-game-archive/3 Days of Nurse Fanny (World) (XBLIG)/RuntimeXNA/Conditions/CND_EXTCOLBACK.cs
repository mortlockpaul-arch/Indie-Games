using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public class CND_EXTCOLBACK : CCnd, IEvaObject
{
	public override bool eva1(CRun rhPtr, CObject hoPtr)
	{
		if (compute_NoRepeat(hoPtr))
		{
			rhPtr.rhEvtProg.evt_AddCurrentObject(hoPtr);
			return true;
		}
		CEventGroup rhEventGroup = rhPtr.rhEvtProg.rhEventGroup;
		if ((rhEventGroup.evgFlags & 0x800) == 0)
		{
			return false;
		}
		rhPtr.rhEvtProg.rh3DoStop = true;
		return true;
	}

	public override bool eva2(CRun rhPtr)
	{
		return evaObject(rhPtr, this);
	}

	public virtual bool evaObjectRoutine(CObject hoPtr)
	{
		if (hoPtr.hoAdRunHeader.colMask_TestObject_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, hoPtr.hoX, hoPtr.hoY, 0, 1) != 0)
		{
			return negaTRUE();
		}
		return negaFALSE();
	}
}
