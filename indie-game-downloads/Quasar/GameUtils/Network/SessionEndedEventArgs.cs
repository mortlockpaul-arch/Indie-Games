namespace Quasar.GameUtils.Network;

public class SessionEndedEventArgs
{
	public SessionEndReason EndReason { get; private set; }

	public SessionEndedEventArgs(SessionEndReason endReason)
	{
		EndReason = endReason;
	}
}
