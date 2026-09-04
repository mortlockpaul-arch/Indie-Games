using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTDESTROY : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject == null)
		{
			return;
		}
		if (cObject.hoType == 3)
		{
			CText cText = (CText)cObject;
			if ((cText.rsHidden & 4) != 0)
			{
				cObject.ros.obHide();
				cObject.ros.rsFlags &= -33;
				cObject.hoFlags |= 8192;
			}
			else
			{
				cObject.hoFlags |= 1;
				rhPtr.destroy_Add(cObject.hoNumber);
			}
		}
		else if ((cObject.hoFlags & 1) == 0)
		{
			cObject.hoFlags |= 1;
			if ((cObject.hoOEFlags & 0x20) != 0 || (cObject.hoOEFlags & 0x200) != 0)
			{
				rhPtr.init_Disappear(cObject);
				return;
			}
			cObject.hoCallRoutine = false;
			rhPtr.destroy_Add(cObject.hoNumber);
		}
	}
}
