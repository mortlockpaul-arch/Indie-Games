using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_STRNEXT : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			CText cText = (CText)cObject;
			int num = cText.rsMini + 1;
			if (cText.txtChange(num))
			{
				cObject.roc.rcChanged = true;
				cObject.display();
			}
		}
	}
}
