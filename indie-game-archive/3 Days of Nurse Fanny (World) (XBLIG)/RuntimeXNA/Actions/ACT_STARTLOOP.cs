using System;
using RuntimeXNA.Events;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_STARTLOOP : CAct
{
	public override void execute(CRun rhPtr)
	{
		string text = rhPtr.get_EventExpressionString((CParamExpression)evtParams[0]);
		if (text.Length == 0)
		{
			return;
		}
		int num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
		bool flag = false;
		int i;
		CLoop cLoop;
		for (i = 0; i < rhPtr.rh4FastLoops.size(); i++)
		{
			cLoop = (CLoop)rhPtr.rh4FastLoops.get(i);
			if (string.Compare(cLoop.name, text, StringComparison.OrdinalIgnoreCase) == 0)
			{
				break;
			}
		}
		if (i == rhPtr.rh4FastLoops.size())
		{
			CLoop cLoop2 = new CLoop();
			rhPtr.rh4FastLoops.add(cLoop2);
			i = rhPtr.rh4FastLoops.size() - 1;
			cLoop2.name = text;
			cLoop2.flags = 0;
		}
		cLoop = (CLoop)rhPtr.rh4FastLoops.get(i);
		cLoop.flags &= -2;
		flag = false;
		if (num < 0)
		{
			flag = true;
			num = 10;
		}
		string rh4CurrentFastLoop = rhPtr.rh4CurrentFastLoop;
		bool rh2ActionLoop = rhPtr.rhEvtProg.rh2ActionLoop;
		int rh2ActionLoopCount = rhPtr.rhEvtProg.rh2ActionLoopCount;
		CEventGroup rhEventGroup = rhPtr.rhEvtProg.rhEventGroup;
		cLoop.index = 0;
		while (cLoop.index < num)
		{
			rhPtr.rh4CurrentFastLoop = cLoop.name;
			rhPtr.rhEvtProg.rh2ActionOn = false;
			rhPtr.rhEvtProg.handle_GlobalEvents(-983041);
			if ((cLoop.flags & 1) != 0)
			{
				break;
			}
			if (flag)
			{
				num = cLoop.index + 10;
			}
			cLoop.index++;
		}
		rhPtr.rhEvtProg.rhEventGroup = rhEventGroup;
		rhPtr.rhEvtProg.rh2ActionLoopCount = rh2ActionLoopCount;
		rhPtr.rhEvtProg.rh2ActionLoop = rh2ActionLoop;
		rhPtr.rh4CurrentFastLoop = rh4CurrentFastLoop;
		rhPtr.rhEvtProg.rh2ActionOn = true;
		rhPtr.rh4FastLoops.remove(i);
	}
}
