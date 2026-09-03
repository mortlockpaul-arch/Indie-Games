using Microsoft.Xna.Framework;

namespace OluXNA;

internal class EnemyState
{
	public delegate void t_Update(GameTime gametime);

	public delegate void t_Draw(GameTime gametime);

	public delegate bool t_Remove(ConditionSet cS);

	public delegate EnemyState t_GetNewState();

	private t_Update uFunc;

	private t_Draw dFunc;

	private t_Remove rFunc;

	private t_GetNewState newState;

	public ConditionSet condSet;

	public EnemyState(t_Update _uFunc, t_Draw _dFunc, t_Remove _rFunc, t_GetNewState _newState)
	{
		uFunc = _uFunc;
		dFunc = _dFunc;
		rFunc = _rFunc;
		newState = _newState;
	}

	public void Update(GameTime gametime)
	{
		if (uFunc != null)
		{
			uFunc(gametime);
		}
	}

	public void Draw(GameTime gametime)
	{
		if (dFunc != null)
		{
			dFunc(gametime);
		}
	}

	public bool Remove()
	{
		if (rFunc != null)
		{
			return rFunc(condSet);
		}
		return condSet.ConditionsMet();
	}

	public EnemyState GetNewState()
	{
		if (newState != null)
		{
			return newState();
		}
		return null;
	}
}
