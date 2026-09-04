using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_SETLIVES : CAct
{
	public override void execute(CRun rhPtr)
	{
		int live = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		int joueur = evtOi;
		rhPtr.actPla_FinishLives(joueur, live);
	}
}
