using System;
using Microsoft.Xna.Framework;
using d;
using l;
using r;

namespace W;

internal class _0006 : r.b
{
	private l._7<_6> a5h = new l._7<_6>();

	protected internal Vector3 gravity;

	internal Vector3 a5b;

	protected r.B timeStepSettings;

	private Action<int> a56;

	public Vector3 Gravity
	{
		get
		{
			return gravity;
		}
		set
		{
			gravity = value;
		}
	}

	public r.B TimeStepSettings
	{
		get
		{
			return timeStepSettings;
		}
		set
		{
			timeStepSettings = value;
		}
	}

	public _0006(r.B timeStepSettings)
	{
		TimeStepSettings = timeStepSettings;
		Enabled = true;
		a56 = a7;
	}

	public _0006(r.B timeStepSettings, d.b threadManager)
		: this(timeStepSettings)
	{
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	private void a7(int P_0)
	{
		if (a5h.Elements[P_0].IsActive)
		{
			a5h.Elements[P_0].UpdateForForces(timeStepSettings.TimeStepDuration);
		}
	}

	protected override void UpdateMultithreaded()
	{
		Vector3.Multiply(ref gravity, timeStepSettings.TimeStepDuration, out a5b);
		base.ThreadManager.ForLoop(0, a5h.Count, a56);
	}

	protected override void UpdateSingleThreaded()
	{
		Vector3.Multiply(ref gravity, timeStepSettings.TimeStepDuration, out a5b);
		for (int i = 0; i < a5h.a5h; i++)
		{
			a7(i);
		}
	}

	public void Add(_6 forceUpdateable)
	{
		if (forceUpdateable.ForceUpdater == null)
		{
			forceUpdateable.ForceUpdater = this;
			if (forceUpdateable.IsDynamic)
			{
				a5h.Add(forceUpdateable);
			}
			return;
		}
		throw new Exception("Cannot add updateable; it already belongs to another manager.");
	}

	public void Remove(_6 forceUpdateable)
	{
		if (forceUpdateable.ForceUpdater == this)
		{
			if (forceUpdateable.IsDynamic && !a5h.Remove(forceUpdateable))
			{
				throw new Exception("Dynamic object not present in dynamic objects list; ensure that the IForceUpdateable was never removed from the list improperly by using ForceUpdateableBecomingKinematic.");
			}
			forceUpdateable.ForceUpdater = null;
			return;
		}
		throw new Exception("Cannot remove updateable; it does not belong to this manager.");
	}

	public void ForceUpdateableBecomingDynamic(_6 updateable)
	{
		if (updateable.ForceUpdater == this)
		{
			a5h.Add(updateable);
			return;
		}
		throw new Exception("Updateable does not belong to this manager.");
	}

	public void ForceUpdateableBecomingKinematic(_6 updateable)
	{
		if (updateable.ForceUpdater == this)
		{
			if (!a5h.Remove(updateable))
			{
				throw new Exception("Dynamic object not present in dynamic objects list; ensure that the IVelocityUpdateable was never removed from the list improperly by using VelocityUpdateableBecomingKinematic.");
			}
			return;
		}
		throw new Exception("Updateable does not belong to this manager.");
	}
}
