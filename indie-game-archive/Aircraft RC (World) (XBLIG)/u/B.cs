using System;
using System.Collections.Generic;
using SynapseGaming.LightingSystem.Core;

namespace u;

internal class B<TManager, TObject> where TManager : ISubmit<TObject>
{
	private TManager HCB;

	private List<TObject> HC_0002 = new List<TObject>(16);

	public TManager ContainingManager => HCB;

	public void SetManager(TManager manager)
	{
		if (HCB != null)
		{
			if (manager != null)
			{
				throw new Exception("Unable to assign scene to more than one " + typeof(TManager).Name + ". Remove from the previous manager first.");
			}
			RemoveSubmittedObjects();
		}
		HCB = manager;
	}

	public void Submit(TObject obj)
	{
		if (HCB != null)
		{
			HCB.Submit(obj);
			HC_0002.Add(obj);
		}
	}

	public void RemoveSubmittedObjects()
	{
		if (HCB == null)
		{
			return;
		}
		foreach (TObject item in HC_0002)
		{
			HCB.Remove(item);
		}
		HC_0002.Clear();
	}

	public void Optimize()
	{
		if (HCB is IWorldRenderableManager)
		{
			(HCB as IWorldRenderableManager).Optimize();
		}
	}
}
