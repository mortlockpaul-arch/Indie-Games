namespace System.Threading.Channels;

internal sealed class WaitingReadAsyncOperation : AsyncOperation<WaitingReadAsyncOperation, bool>
{
	public WaitingReadAsyncOperation(bool runContinuationsAsynchronously, CancellationToken cancellationToken = default(CancellationToken), bool pooled = false, Action<object, CancellationToken> cancellationCallback = null)
		: base(runContinuationsAsynchronously, cancellationToken, pooled, cancellationCallback)
	{
	}
}
