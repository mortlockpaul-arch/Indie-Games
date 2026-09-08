namespace System.Threading.Channels;

internal sealed class BlockedWriteAsyncOperation<T> : AsyncOperation<BlockedWriteAsyncOperation<T>, VoidResult>
{
	public T Item { get; set; }

	public BlockedWriteAsyncOperation(bool runContinuationsAsynchronously, CancellationToken cancellationToken = default(CancellationToken), bool pooled = false, Action<object, CancellationToken> cancellationCallback = null)
		: base(runContinuationsAsynchronously, cancellationToken, pooled, cancellationCallback)
	{
	}
}
