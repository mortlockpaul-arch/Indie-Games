namespace System.IO.Pipelines;

internal readonly struct PipeCompletionCallback(Action<Exception?, object?> callback, object? state)
{
	public readonly Action<Exception?, object?> Callback = callback;

	public readonly object? State = state;
}
