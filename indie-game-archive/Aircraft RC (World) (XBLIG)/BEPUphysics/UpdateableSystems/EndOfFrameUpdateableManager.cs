using BEPUphysics.Threading;

namespace BEPUphysics.UpdateableSystems;

/// <summary>
///  Manages updateables that update at the end of a frame.
/// </summary>
public class EndOfFrameUpdateableManager : UpdateableManager<IEndOfFrameUpdateable>
{
	/// <summary>
	///  Constructs a manager.
	/// </summary>
	/// <param name="timeStepSettings">Time step settings to use.</param>
	public EndOfFrameUpdateableManager(TimeStepSettings timeStepSettings)
		: base(timeStepSettings)
	{
	}

	/// <summary>
	///  Constructs a manager.
	/// </summary>
	/// <param name="timeStepSettings">Time step settings to use.</param>
	///  <param name="threadManager">Thread manager to use.</param>
	public EndOfFrameUpdateableManager(TimeStepSettings timeStepSettings, IThreadManager threadManager)
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
