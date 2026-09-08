namespace System.Threading.Channels;

internal sealed class BlockedReadAsyncOperation<TResult> : AsyncOperation<BlockedReadAsyncOperation<TResult>, TResult>
{
	public BlockedReadAsyncOperation(bool runContinuationsAsynchronously, CancellationToken cancellationToken = default(CancellationToken), bool pooled = false, Action<object, CancellationToken> cancellationCallback = null)
		: base(runContinuationsAsynchronously, cancellationToken, pooled, cancellationCallback)
	{
	}
}
