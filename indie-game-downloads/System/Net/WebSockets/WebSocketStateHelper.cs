namespace System.Net.WebSockets;

internal static class WebSocketStateHelper
{
	internal static bool IsValidSendState(WebSocketState state)
	{
		return (ManagedWebSocketStates.Open | ManagedWebSocketStates.CloseReceived).HasFlag(ToFlag(state));
	}

	internal static void ThrowIfInvalidState(WebSocketState currentState, bool isDisposed, Exception innerException, ManagedWebSocketStates validStates)
	{
		if ((ToFlag(currentState) & validStates) == 0)
		{
			string message = System.SR.Format(System.SR.net_WebSockets_InvalidState, currentState, validStates);
			throw new WebSocketException(WebSocketError.InvalidState, message, innerException);
		}
		if (innerException != null)
		{
			throw new OperationCanceledException("Aborted", innerException);
		}
		ObjectDisposedException.ThrowIf(isDisposed, typeof(WebSocket));
	}

	private static ManagedWebSocketStates ToFlag(WebSocketState value)
	{
		return (ManagedWebSocketStates)(1 << (int)value);
	}
}
