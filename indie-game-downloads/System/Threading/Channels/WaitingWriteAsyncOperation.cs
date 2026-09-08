namespace System.Threading.Channels;

internal sealed class WaitingWriteAsyncOperation : AsyncOperation<WaitingWriteAsyncOperation, bool>
{
	public WaitingWriteAsyncOperation(bool runContinuationsAsynchronously, CancellationToken cancellationToken = default(CancellationToken), bool pooled = false, Action<object, CancellationToken> cancellationCallback = null)
		: base(runContinuationsAsynchronously, cancellationToken, pooled, cancellationCallback)
	{
	}
}
