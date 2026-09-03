namespace OluXNA;

internal class HitCorrectCondition : ICondition
{
	private bool _condMet;

	private int targetsHit;

	public bool ConditionMet()
	{
		if (_condMet)
		{
			return targetsHit == 8;
		}
		return false;
	}

	public void Update()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		targetsHit = 9 - BaseGame.Get().actualEnem;
		foreach (TargetEffect item in BaseGame.Get().targetFX)
		{
			if (item is TargetEffectBase && !item.activated && ((TargetEffectBase)item).fillMode == ((TargetEffectBase)item).eTarget.fillMode)
			{
				_condMet = false;
				break;
			}
		}
	}

	public void Start()
	{
		targetsHit = 0;
		_condMet = true;
	}
}
