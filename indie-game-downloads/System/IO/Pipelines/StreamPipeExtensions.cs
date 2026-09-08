using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines;

public static class StreamPipeExtensions
{
	public static Task CopyToAsync(this Stream source, PipeWriter destination, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (destination == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
		}
		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled(cancellationToken);
		}
		return destination.CopyFromAsync(source, cancellationToken);
	}
}
