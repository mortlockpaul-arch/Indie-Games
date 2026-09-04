using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

internal class ACT_EXTSETEFFECT : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject == null)
		{
			return;
		}
		string pEffect = ((PARAM_EFFECT)evtParams[0]).pEffect;
		int effect = 0;
		if (pEffect != null && pEffect.Length != 0)
		{
			switch (pEffect)
			{
			case "Add":
				effect = 9;
				break;
			case "Invert":
				effect = 2;
				break;
			case "Sub":
				effect = 11;
				break;
			case "Mono":
				effect = 10;
				break;
			case "Blend":
				effect = 1;
				break;
			case "XOR":
				effect = 3;
				break;
			case "OR":
				effect = 5;
				break;
			case "AND":
				effect = 4;
				break;
			}
			cObject.ros.modifSpriteEffect(effect, cObject.ros.rsEffectParam);
		}
	}
}
