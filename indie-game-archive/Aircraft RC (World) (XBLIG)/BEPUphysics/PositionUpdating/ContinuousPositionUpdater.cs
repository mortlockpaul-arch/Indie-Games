using System;
using BEPUphysics.DataStructures;
using BEPUphysics.Threading;

namespace BEPUphysics.PositionUpdating;

/// <summary>
///  Updates objects according to the position update mode.
///  This allows continuous objects to avoid missing collisions.
/// </summary>
public class ContinuousPositionUpdater : PositionUpdater
{
	private RawList<IPositionUpdateable> discreteUpdateables = new RawList<IPositionUpdateable>();

	private RawList<ICCDPositionUpdateable> passiveUpdateables = new RawList<ICCDPositionUpdateable>();

	private RawList<ICCDPositionUpdateable> continuousUpdateables = new RawList<ICCDPositionUpdateable>();

	/// <summary>
	///  Number of objects in a list required to use multithreading.
	/// </summary>
	public static int MultithreadingThreshold = 100;

	private Action<int> preUpdate;

	private Action<int> updateTimeOfImpact;

	private Action<int> updateContinuous;

	/// <summary>
	///  Constructs the position updater.
	/// </summary>
	/// <param name="timeStepSettings">Time step settings to use.</param>
	public ContinuousPositionUpdater(TimeStepSettings timeStepSettings)
		: base(timeStepSettings)
	{
		preUpdate = PreUpdate;
		updateTimeOfImpact = UpdateTimeOfImpact;
		updateContinuous = UpdateContinuousItem;
	}

	/// <summary>
	///  Constructs the position updater.
	/// </summary>
	/// <param name="timeStepSettings">Time step settings to use.</param>
	///  <param name="threadManager">Thread manager to use.</param>
	public ContinuousPositionUpdater(TimeStepSettings timeStepSettings, IThreadManager threadManager)
		: base(timeStepSettings, threadManager)
	{
		preUpdate = PreUpdate;
		updateTimeOfImpact = UpdateTimeOfImpact;
		updateContinuous = UpdateContinuousItem;
	}

	private void PreUpdate(int i)
	{
		if (i >= discreteUpdateables.count)
		{
			i -= discreteUpdateables.count;
			if (i >= passiveUpdateables.count)
			{
				i -= passiveUpdateables.count;
				if (continuousUpdateables.Elements[i].IsActive)
				{
					continuousUpdateables.Elements[i].PreUpdatePosition(timeStepSettings.TimeStepDuration);
				}
			}
			else if (passiveUpdateables.Elements[i].IsActive)
			{
				passiveUpdateables.Elements[i].PreUpdatePosition(timeStepSettings.TimeStepDuration);
			}
		}
		else if (discreteUpdateables.Elements[i].IsActive)
		{
			discreteUpdateables.Elements[i].PreUpdatePosition(timeStepSettings.TimeStepDuration);
		}
	}

	private void UpdateTimeOfImpact(int i)
	{
		continuousUpdateables.Elements[i].UpdateTimeOfImpacts(timeStepSettings.TimeStepDuration);
	}

	private void UpdateContinuousItem(int i)
	{
		if (i < passiveUpdateables.count)
		{
			if (passiveUpdateables.Elements[i].IsActive)
			{
				passiveUpdateables.Elements[i].UpdatePositionContinuously(timeStepSettings.TimeStepDuration);
			}
		}
		else if (continuousUpdateables.Elements[i - passiveUpdateables.count].IsActive)
		{
			continuousUpdateables.Elements[i - passiveUpdateables.count].UpdatePositionContinuously(timeStepSettings.TimeStepDuration);
		}
	}

	protected override void UpdateMultithreaded()
	{
		int endIndex = discreteUpdateables.count + passiveUpdateables.count + continuousUpdateables.count;
		base.ThreadManager.ForLoop(0, endIndex, preUpdate);
		if (continuousUpdateables.count > MultithreadingThreshold)
		{
			base.ThreadManager.ForLoop(0, continuousUpdateables.count, updateTimeOfImpact);
		}
		else
		{
			for (int i = 0; i < continuousUpdateables.count; i++)
			{
				UpdateTimeOfImpact(i);
			}
		}
		endIndex = passiveUpdateables.count + continuousUpdateables.count;
		if (endIndex > MultithreadingThreshold)
		{
			base.ThreadManager.ForLoop(0, endIndex, updateContinuous);
			return;
		}
		for (int j = 0; j < endIndex; j++)
		{
			UpdateContinuousItem(j);
		}
	}

	protected override void UpdateSingleThreaded()
	{
		int num = discreteUpdateables.count + passiveUpdateables.count + continuousUpdateables.count;
		for (int i = 0; i < num; i++)
		{
			PreUpdate(i);
		}
		for (int j = 0; j < continuousUpdateables.count; j++)
		{
			UpdateTimeOfImpact(j);
		}
		num = passiveUpdateables.count + continuousUpdateables.count;
		for (int k = 0; k < num; k++)
		{
			UpdateContinuousItem(k);
		}
	}

	/// <summary>
	///  Notifies the position updater that an updateable has changed state.
	/// </summary>
	/// <param name="updateable">Updateable with changed state.</param>
	/// <param name="previousMode">Previous state the updateable was in.</param>
	public void UpdateableModeChanged(ICCDPositionUpdateable updateable, PositionUpdateMode previousMode)
	{
		switch (previousMode)
		{
		case PositionUpdateMode.Discrete:
			discreteUpdateables.Remove(updateable);
			break;
		case PositionUpdateMode.Passive:
			passiveUpdateables.Remove(updateable);
			break;
		case PositionUpdateMode.Continuous:
			continuousUpdateables.Remove(updateable);
			break;
		}
		switch (updateable.PositionUpdateMode)
		{
		case PositionUpdateMode.Discrete:
			discreteUpdateables.Add(updateable);
			break;
		case PositionUpdateMode.Passive:
			passiveUpdateables.Add(updateable);
			break;
		case PositionUpdateMode.Continuous:
			continuousUpdateables.Add(updateable);
			break;
		}
	}

	/// <summary>
	///  Adds an object to the position updater.
	/// </summary>
	/// <param name="updateable">Updateable to add.</param>
	/// <exception cref="T:System.Exception">Thrown if the updateable already belongs to a position updater.</exception>
	public override void Add(IPositionUpdateable updateable)
	{
		if (updateable.PositionUpdater == null)
		{
			updateable.PositionUpdater = this;
			if (updateable is ICCDPositionUpdateable iCCDPositionUpdateable)
			{
				switch (iCCDPositionUpdateable.PositionUpdateMode)
				{
				case PositionUpdateMode.Discrete:
					discreteUpdateables.Add(updateable);
					break;
				case PositionUpdateMode.Passive:
					passiveUpdateables.Add(iCCDPositionUpdateable);
					break;
				case PositionUpdateMode.Continuous:
					continuousUpdateables.Add(iCCDPositionUpdateable);
					break;
				}
			}
			else
			{
				discreteUpdateables.Add(updateable);
			}
			return;
		}
		throw new Exception("Cannot add object to Integrator; it already belongs to one.");
	}

	/// <summary>
	///  Removes an updateable from the updater.
	/// </summary>
	/// <param name="updateable">Item to remove.</param>
	/// <exception cref="T:System.Exception">Thrown if the updater does not own the updateable.</exception>
	public override void Remove(IPositionUpdateable updateable)
	{
		if (updateable.PositionUpdater == this)
		{
			updateable.PositionUpdater = null;
			if (updateable is ICCDPositionUpdateable iCCDPositionUpdateable)
			{
				switch (iCCDPositionUpdateable.PositionUpdateMode)
				{
				case PositionUpdateMode.Discrete:
					discreteUpdateables.Remove(updateable);
					break;
				case PositionUpdateMode.Passive:
					passiveUpdateables.Remove(iCCDPositionUpdateable);
					break;
				case PositionUpdateMode.Continuous:
					continuousUpdateables.Remove(iCCDPositionUpdateable);
					break;
				}
			}
			else
			{
				discreteUpdateables.Remove(updateable);
			}
			return;
		}
		throw new Exception("Cannot remove object from this Integrator.  The object doesn't belong to it.");
	}
}
