using System;
using BEPUphysics.DataStructures;
using BEPUphysics.Threading;
using Microsoft.Xna.Framework;

namespace BEPUphysics.OtherSpaceStages;

/// <summary>
///  Applies forces to managed objects.
/// </summary>
public class ForceUpdater : MultithreadedProcessingStage
{
	private RawList<IForceUpdateable> dynamicObjects = new RawList<IForceUpdateable>();

	protected internal Vector3 gravity;

	internal Vector3 gravityDt;

	protected TimeStepSettings timeStepSettings;

	private Action<int> multithreadedLoopBodyDelegate;

	/// <summary>
	///  Gets or sets the gravity applied by the force updater.
	/// </summary>
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

	/// <summary>
	///  Gets or sets the time step settings used by the force updater.
	/// </summary>
	public TimeStepSettings TimeStepSettings
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

	/// <summary>
	///  Constructs the force updater.
	/// </summary>
	/// <param name="timeStepSettings">Time step settings to use.</param>
	public ForceUpdater(TimeStepSettings timeStepSettings)
	{
		TimeStepSettings = timeStepSettings;
		Enabled = true;
		multithreadedLoopBodyDelegate = UpdateObject;
	}

	/// <summary>
	///  Constructs the force updater.
	/// </summary>
	/// <param name="timeStepSettings">Time step settings to use.</param>
	///  <param name="threadManager">Thread manager to use.</param>
	public ForceUpdater(TimeStepSettings timeStepSettings, IThreadManager threadManager)
		: this(timeStepSettings)
	{
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	private void UpdateObject(int i)
	{
		if (dynamicObjects.Elements[i].IsActive)
		{
			dynamicObjects.Elements[i].UpdateForForces(timeStepSettings.TimeStepDuration);
		}
	}

	protected override void UpdateMultithreaded()
	{
		Vector3.Multiply(ref gravity, timeStepSettings.TimeStepDuration, out gravityDt);
		base.ThreadManager.ForLoop(0, dynamicObjects.Count, multithreadedLoopBodyDelegate);
	}

	protected override void UpdateSingleThreaded()
	{
		Vector3.Multiply(ref gravity, timeStepSettings.TimeStepDuration, out gravityDt);
		for (int i = 0; i < dynamicObjects.count; i++)
		{
			UpdateObject(i);
		}
	}

	/// <summary>
	///  Adds a force updateable to the force updater.
	/// </summary>
	/// <param name="forceUpdateable">Item to add.</param>
	/// <exception cref="T:System.Exception">Thrown when the item already belongs to a force updater.</exception>
	public void Add(IForceUpdateable forceUpdateable)
	{
		if (forceUpdateable.ForceUpdater == null)
		{
			forceUpdateable.ForceUpdater = this;
			if (forceUpdateable.IsDynamic)
			{
				dynamicObjects.Add(forceUpdateable);
			}
			return;
		}
		throw new Exception("Cannot add updateable; it already belongs to another manager.");
	}

	/// <summary>
	///  Removes a force updateable from the force updater.
	/// </summary>
	/// <param name="forceUpdateable">Item to remove.</param>
	/// <exception cref="T:System.Exception">Thrown when the item does not belong to this force updater or its state is corrupted.</exception>
	public void Remove(IForceUpdateable forceUpdateable)
	{
		if (forceUpdateable.ForceUpdater == this)
		{
			if (forceUpdateable.IsDynamic && !dynamicObjects.Remove(forceUpdateable))
			{
				throw new Exception("Dynamic object not present in dynamic objects list; ensure that the IForceUpdateable was never removed from the list improperly by using ForceUpdateableBecomingKinematic.");
			}
			forceUpdateable.ForceUpdater = null;
			return;
		}
		throw new Exception("Cannot remove updateable; it does not belong to this manager.");
	}

	/// <summary>
	/// Notifies the system that a force updateable is becoming dynamic.
	/// </summary>
	/// <param name="updateable">Updateable changing state.</param>
	public void ForceUpdateableBecomingDynamic(IForceUpdateable updateable)
	{
		if (updateable.ForceUpdater == this)
		{
			dynamicObjects.Add(updateable);
			return;
		}
		throw new Exception("Updateable does not belong to this manager.");
	}

	/// <summary>
	/// Notifies the system that a force updateable is becoming kinematic.
	/// </summary>
	/// <param name="updateable">Updateable changing state.</param>
	public void ForceUpdateableBecomingKinematic(IForceUpdateable updateable)
	{
		if (updateable.ForceUpdater == this)
		{
			if (!dynamicObjects.Remove(updateable))
			{
				throw new Exception("Dynamic object not present in dynamic objects list; ensure that the IVelocityUpdateable was never removed from the list improperly by using VelocityUpdateableBecomingKinematic.");
			}
			return;
		}
		throw new Exception("Updateable does not belong to this manager.");
	}
}
