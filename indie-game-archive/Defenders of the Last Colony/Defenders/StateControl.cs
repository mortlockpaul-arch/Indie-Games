using System.Collections.Generic;

namespace Defenders;

internal class StateControl
{
	private List<StateList> states = new List<StateList>(100);

	private uint life = 0u;

	private ushort index = 0;

	public StateControl()
	{
		states.Add(new StateList(EnemState.normal, 50));
		states.Add(new StateList(EnemState.spawn, 200));
		states.Add(new StateList(EnemState.prepareRing, 100));
		states.Add(new StateList(EnemState.ring, 220));
		states.Add(new StateList(EnemState.snakes, 200));
		states.Add(new StateList(EnemState.prepare, 30));
		states.Add(new StateList(EnemState.shoot, 100));
	}

	public EnemState Update()
	{
		life++;
		if (life > states[index].next)
		{
			life = 0u;
			index++;
			if (index >= states.Count)
			{
				index = 0;
			}
		}
		return states[index].state;
	}
}
