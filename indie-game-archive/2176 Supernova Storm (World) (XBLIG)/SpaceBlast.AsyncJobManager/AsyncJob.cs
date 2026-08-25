namespace SpaceBlast.AsyncJobManager;

internal abstract class AsyncJob
{
	public bool IsComplete;

	public abstract void ExecuteJob();
}
