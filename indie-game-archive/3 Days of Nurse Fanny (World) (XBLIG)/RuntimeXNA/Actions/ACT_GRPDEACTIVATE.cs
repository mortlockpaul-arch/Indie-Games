using RuntimeXNA.Events;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_GRPDEACTIVATE : CAct
{
	public override void execute(CRun rhPtr)
	{
		PARAM_GROUPOINTER pARAM_GROUPOINTER = (PARAM_GROUPOINTER)evtParams[0];
		int pointer = pARAM_GROUPOINTER.pointer;
		CEventGroup cEventGroup = rhPtr.rhEvtProg.events[pointer];
		CEvent cEvent = cEventGroup.evgEvents[0];
		PARAM_GROUP pARAM_GROUP = (PARAM_GROUP)cEvent.evtParams[0];
		bool flag = (pARAM_GROUP.grpFlags & 8) == 0;
		pARAM_GROUP.grpFlags |= 8;
		if (flag && (pARAM_GROUP.grpFlags & 4) == 0)
		{
			grpDeactivate(rhPtr, pointer);
		}
	}

	internal virtual int grpDeactivate(CRun rhPtr, int evg)
	{
		CEventGroup cEventGroup = rhPtr.rhEvtProg.events[evg];
		CEvent cEvent = cEventGroup.evgEvents[0];
		PARAM_GROUP pARAM_GROUP = (PARAM_GROUP)cEvent.evtParams[0];
		cEventGroup.evgFlags |= 16384;
		evg++;
		bool flag = false;
		int num = 1;
		while (true)
		{
			cEventGroup = rhPtr.rhEvtProg.events[evg];
			cEvent = cEventGroup.evgEvents[0];
			switch (cEvent.evtCode)
			{
			case -589825:
			{
				pARAM_GROUP = (PARAM_GROUP)cEvent.evtParams[0];
				bool flag2 = (pARAM_GROUP.grpFlags & 4) == 0;
				if (num == 1)
				{
					pARAM_GROUP.grpFlags |= 4;
				}
				if (flag2 && (pARAM_GROUP.grpFlags & 8) == 0)
				{
					evg = grpDeactivate(rhPtr, evg);
					continue;
				}
				num++;
				break;
			}
			case -655361:
				num--;
				if (num == 0)
				{
					cEventGroup.evgFlags |= 16384;
					flag = true;
					evg++;
				}
				break;
			default:
				if (num == 1)
				{
					cEventGroup.evgFlags |= 16384;
				}
				break;
			}
			if (flag)
			{
				break;
			}
			evg++;
		}
		return evg;
	}
}
