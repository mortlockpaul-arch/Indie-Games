using BEPUphysics.Threading;

namespace BEPUphysics.UpdateableSystems;

/// <summary>
///  Manages updateables that update during the forces stage.
/// </summary>
public class DuringForcesUpdateableManager : UpdateableManager<IDuringForcesUpdateable>
{
	/// <summary>
	///  Constructs a manager.
	/// </summary>
	/// <param name="timeStepSettings">Time step settings to use.</param>
	public DuringForcesUpdateableManager(TimeStepSettings timeStepSettings)
		: base(timeStepSettings)
	{
	}

	/// <summary>
	///  Constructs a manager.
	/// </summary>
	/// <param name="timeStepSettings">Time step settings to use.</param>
	///  <param name="threadManager">Thread manager to use.</param>
	public DuringForcesUpdateableManager(TimeStepSettings timeStepSettings, IThreadManager threadManager)
		: base(timeStepSettings, threadManager)
	{
	}

	protected override void MultithreadedUpdate(int i)
	{
		if (simultaneouslyUpdatedUpdateables[i].IsUpdating)
		{
			simultaneouslyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
		}
	}

	protected override void SequentialUpdate(int i)
	{
		if (sequentiallyUpdatedUpdateables[i].IsUpdating)
		{
			sequentiallyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
		}
	}
}
