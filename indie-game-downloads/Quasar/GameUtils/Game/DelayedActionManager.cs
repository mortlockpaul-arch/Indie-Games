using System;
using System.Collections.Generic;
using Quasar.Global;

namespace Quasar.GameUtils.Game;

public class DelayedActionManager
{
	private static DelayedActionManager instance;

	private List<KeyValuePair<long, Action>> delayedActions = new List<KeyValuePair<long, Action>>(10);

	public static DelayedActionManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new DelayedActionManager();
			}
			return instance;
		}
	}

	private DelayedActionManager()
	{
	}

	public void Update()
	{
		long totalTime = Timer.DefaultTimer.TotalTime;
		while (delayedActions.Count > 0 && delayedActions[0].Key <= totalTime)
		{
			KeyValuePair<long, Action> keyValuePair = delayedActions[0];
			delayedActions.RemoveAt(0);
			keyValuePair.Value();
		}
	}

	public static void AddAction(int activationTime, Action action)
	{
		long num = Timer.DefaultTimer.TotalTime + activationTime;
		int count = instance.delayedActions.Count;
		int i;
		for (i = 0; i < count && num >= instance.delayedActions[i].Key; i++)
		{
		}
		instance.delayedActions.Insert(i, new KeyValuePair<long, Action>(num, action));
	}
}
