namespace SpaceBlast.AsyncJobManager;

internal class TestJob : AsyncJob
{
	private string m_Msg;

	public TestJob(string message)
	{
		m_Msg = message;
	}

	public override void ExecuteJob()
	{
	}
}
