using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public class ACT_EXTINKEFFECT : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null && cObject.ros != null)
		{
			PARAM_2SHORTS pARAM_2SHORTS = (PARAM_2SHORTS)evtParams[0];
			int value = pARAM_2SHORTS.value1;
			int effectParam = pARAM_2SHORTS.value2;
			if (value != 1)
			{
				effectParam = 0;
			}
			cObject.ros.modifSpriteEffect(value, effectParam);
		}
	}
}
