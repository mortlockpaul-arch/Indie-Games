namespace OluXNA;

internal class EnemyQueuePart
{
	public Enemy enem;

	public ConditionSet cond;

	public EnemyQueuePart()
	{
		cond = new ConditionSet();
	}

	public EnemyQueuePart(EnemyQueuePart other)
		: this()
	{
		enem = other.enem;
	}

	public EnemyQueuePart(Enemy _enem, float _time)
		: this()
	{
		enem = _enem;
		cond.set.Add(new TimeCondition(_time));
	}

	public EnemyQueuePart(Enemy _enem, ICondition _cond)
		: this()
	{
		enem = _enem;
		cond.set.Add(_cond);
	}

	public EnemyQueuePart(Enemy _enem, ICondition _cond, ICondition _cond2)
		: this(_enem, _cond)
	{
		cond.set.Add(_cond2);
	}
}
