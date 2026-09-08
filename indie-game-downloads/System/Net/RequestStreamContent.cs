using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net;

internal sealed class RequestStreamContent(TaskCompletionSource<Stream> getStreamTcs, TaskCompletionSource completeTcs) : HttpContent
{
	protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
	{
		return SerializeToStreamAsync(stream, context, default(CancellationToken));
	}

	protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context, CancellationToken cancellationToken)
	{
		getStreamTcs.TrySetResult(stream);
		await completeTcs.Task.WaitAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	protected override bool TryComputeLength(out long length)
	{
		length = -1L;
		return false;
	}
}
