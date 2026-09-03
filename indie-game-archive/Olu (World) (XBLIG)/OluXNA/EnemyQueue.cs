using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class EnemyQueue
{
	private List<EnemyQueuePart> toBeDeployed;

	public int EnemCount()
	{
		return toBeDeployed.Count;
	}

	public void Update(GameTime gametime)
	{
		if (toBeDeployed.Count > 0)
		{
			toBeDeployed[0].cond.Update();
		}
	}

	public bool enemyReady()
	{
		if (toBeDeployed.Count > 0)
		{
			return toBeDeployed[0].cond.ConditionsMet();
		}
		return false;
	}

	public void Push(EnemyQueuePart toAdd)
	{
		toBeDeployed.Add(toAdd);
	}

	public void PushAtFront(EnemyQueuePart toAdd)
	{
		toBeDeployed.Insert(0, toAdd);
	}

	public void Start()
	{
		if (toBeDeployed.Count >= 1)
		{
			toBeDeployed[0].cond.Start();
		}
	}

	public Enemy Peek()
	{
		if (enemyReady())
		{
			return toBeDeployed[0].enem;
		}
		return null;
	}

	public void Popoff()
	{
		toBeDeployed.RemoveAt(0);
		if (toBeDeployed.Count > 0)
		{
			toBeDeployed[0].cond.Start();
		}
	}

	public void Clear()
	{
		toBeDeployed.Clear();
	}

	public EnemyQueue()
	{
		toBeDeployed = new List<EnemyQueuePart>();
	}

	public EnemyQueue(float _baseline)
		: this()
	{
	}
}
