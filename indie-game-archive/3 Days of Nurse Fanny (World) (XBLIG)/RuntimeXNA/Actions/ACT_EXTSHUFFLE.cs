using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTSHUFFLE : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			rhPtr.rhEvtProg.rh2ShuffleBuffer.add(cObject);
		}
	}
}
