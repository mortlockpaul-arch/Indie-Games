namespace Microsoft.XboxLive.Avatars.Internal;

public class DebugLog : Log
{
	public DebugLog()
	{
	}

	public DebugLog(object sender, string message)
		: base(sender, message)
	{
	}
}
