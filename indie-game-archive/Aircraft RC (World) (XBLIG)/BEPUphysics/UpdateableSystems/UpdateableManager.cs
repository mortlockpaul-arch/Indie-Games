using System;
using System.Collections.Generic;
using BEPUphysics.Threading;

namespace BEPUphysics.UpdateableSystems;

/// <summary>
///  Superclass of updateable managers.
/// </summary>
public abstract class UpdateableManager : MultithreadedProcessingStage
{
	protected Action<int> multithreadedUpdateDelegate;

	protected TimeStepSettings timeStepSettings;

	/// <summary>
	///  Gets the time step settings used by the updateable manager.
	/// </summary>
	public TimeStepSettings TimeStepSettings => timeStepSettings;

	/// <summary>
	///  Gets or sets the owning space.
	/// </summary>
	public ISpace Space { get; set; }

	protected UpdateableManager(TimeStepSettings timeStepSettings)
	{
		this.timeStepSettings = timeStepSettings;
		Enabled = true;
	}

	protected UpdateableManager(TimeStepSettings timeStepSettings, IThreadManager threadManager)
		: this(timeStepSettings)
	{
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	/// <summary>
	///  Notifies the manager that the updateable has changed sequential updating state.
	/// </summary>
	/// <param name="updateable">Updateable with changed state.</param>
	public abstract void SequentialUpdatingStateChanged(ISpaceUpdateable updateable);
}
/// <summary>
///  Superclass of updateable managers with a specific type.
/// </summary>
/// <typeparam name="T">Type of Updateable being managed.</typeparam>
public abstract class UpdateableManager<T> : UpdateableManager where T : class, ISpaceUpdateable
{
	protected List<T> sequentiallyUpdatedUpdateables = new List<T>();

	protected List<T> simultaneouslyUpdatedUpdateables = new List<T>();

	protected UpdateableManager(TimeStepSettings timeStepSettings)
		: base(timeStepSettings)
	{
		multithreadedUpdateDelegate = MultithreadedUpdate;
	}

	protected UpdateableManager(TimeStepSettings timeStepSettings, IThreadManager threadManager)
		: base(timeStepSettings, threadManager)
	{
		multithreadedUpdateDelegate = MultithreadedUpdate;
	}

	protected abstract void MultithreadedUpdate(int i);

	protected abstract void SequentialUpdate(int i);

	public override void SequentialUpdatingStateChanged(ISpaceUpdateable updateable)
	{
		if (updateable.Managers.Contains(this))
		{
			T item = updateable as T;
			if (updateable.IsUpdatedSequentially)
			{
				if (simultaneouslyUpdatedUpdateables.Remove(item))
				{
					sequentiallyUpdatedUpdateables.Add(item);
				}
			}
			else if (sequentiallyUpdatedUpdateables.Remove(item))
			{
				simultaneouslyUpdatedUpdateables.Add(item);
			}
			return;
		}
		throw new Exception("Updateable does not belong to this manager.");
	}

	/// <summary>
	///  Adds an updateable to the manager.
	/// </summary>
	/// <param name="updateable">Updateable to add.</param>
	/// <exception cref="T:System.Exception">Thrown if the manager already contains the updateable.</exception>
	public void Add(T updateable)
	{
		if (!updateable.Managers.Contains(this))
		{
			if (updateable.IsUpdatedSequentially)
			{
				sequentiallyUpdatedUpdateables.Add(updateable);
			}
			else
			{
				simultaneouslyUpdatedUpdateables.Add(updateable);
			}
			updateable.Managers.Add(this);
			return;
		}
		throw new Exception("Updateable already belongs to the manager, cannot re-add.");
	}

	/// <summary>
	///  Removes an updateable from the manager.
	/// </summary>
	/// <param name="updateable">Updateable to remove.</param>
	/// <exception cref="T:System.Exception">Thrown if the manager does not contain the updateable.</exception>
	public void Remove(T updateable)
	{
		if (updateable.Managers.Contains(this))
		{
			if (updateable.IsUpdatedSequentially)
			{
				sequentiallyUpdatedUpdateables.Remove(updateable);
			}
			else
			{
				simultaneouslyUpdatedUpdateables.Remove(updateable);
			}
			updateable.Managers.Remove(this);
			return;
		}
		throw new Exception("Updateable does not belong to this manager; cannot remove.");
	}

	protected override void UpdateMultithreaded()
	{
		for (int i = 0; i < sequentiallyUpdatedUpdateables.Count; i++)
		{
			SequentialUpdate(i);
		}
		base.ThreadManager.ForLoop(0, simultaneouslyUpdatedUpdateables.Count, multithreadedUpdateDelegate);
	}

	protected override void UpdateSingleThreaded()
	{
		for (int i = 0; i < sequentiallyUpdatedUpdateables.Count; i++)
		{
			SequentialUpdate(i);
		}
		for (int j = 0; j < simultaneouslyUpdatedUpdateables.Count; j++)
		{
			MultithreadedUpdate(j);
		}
	}
}
