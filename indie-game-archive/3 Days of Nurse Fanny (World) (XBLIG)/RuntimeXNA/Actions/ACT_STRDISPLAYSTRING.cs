using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_STRDISPLAYSTRING : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			CText cText = (CText)cObject;
			if (cText.txtChange(-1))
			{
				cObject.roc.rcChanged = true;
				cObject.display();
			}
		}
	}
}
