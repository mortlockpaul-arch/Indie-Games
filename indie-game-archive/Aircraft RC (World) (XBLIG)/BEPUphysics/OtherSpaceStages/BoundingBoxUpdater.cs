using System;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.Threading;

namespace BEPUphysics.OtherSpaceStages;

/// <summary>
///  Updates the bounding box of managed objects.
/// </summary>
public class BoundingBoxUpdater : MultithreadedProcessingStage
{
	private RawList<MobileCollidable> entries = new RawList<MobileCollidable>();

	private TimeStepSettings timeStepSettings;

	private Action<int> multithreadedLoopBodyDelegate;

	/// <summary>
	///  Gets or sets the time step settings used by the updater.
	/// </summary>
	public TimeStepSettings TimeStepSettings { get; set; }

	/// <summary>
	///  Constructs the bounding box updater.
	/// </summary>
	/// <param name="timeStepSettings">Time step setttings to be used by the updater.</param>
	public BoundingBoxUpdater(TimeStepSettings timeStepSettings)
	{
		multithreadedLoopBodyDelegate = LoopBody;
		Enabled = true;
		this.timeStepSettings = timeStepSettings;
	}

	/// <summary>
	///  Constructs the bounding box updater.
	/// </summary>
	/// <param name="timeStepSettings">Time step setttings to be used by the updater.</param>
	///  <param name="threadManager">Thread manager to be used by the updater.</param>
	public BoundingBoxUpdater(TimeStepSettings timeStepSettings, IThreadManager threadManager)
		: this(timeStepSettings)
	{
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	private void LoopBody(int i)
	{
		MobileCollidable mobileCollidable = entries.Elements[i];
		if (mobileCollidable.IsActive)
		{
			mobileCollidable.UpdateBoundingBox(timeStepSettings.TimeStepDuration);
		}
	}

	/// <summary>
	///  Adds an entry to the updater.
	/// </summary>
	/// <param name="entry">Entry to add.</param>
	public void Add(MobileCollidable entry)
	{
		entries.Add(entry);
	}

	/// <summary>
	///  Removes an entry from the updater.
	/// </summary>
	/// <param name="entry">Entry to remove.</param>
	public void Remove(MobileCollidable entry)
	{
		entries.Remove(entry);
	}

	protected override void UpdateMultithreaded()
	{
		base.ThreadManager.ForLoop(0, entries.Count, multithreadedLoopBodyDelegate);
	}

	protected override void UpdateSingleThreaded()
	{
		for (int i = 0; i < entries.count; i++)
		{
			LoopBody(i);
		}
	}
}
