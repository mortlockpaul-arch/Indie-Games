using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_STRDESTROY : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
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
	}
}
