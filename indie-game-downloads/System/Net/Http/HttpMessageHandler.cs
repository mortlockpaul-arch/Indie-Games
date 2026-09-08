using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>A base type for HTTP message handlers.</summary>
public abstract class HttpMessageHandler : IDisposable
{
	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpMessageHandler" /> class.</summary>
	protected HttpMessageHandler()
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, null, ".ctor");
		}
	}

	protected internal virtual HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		throw new NotSupportedException(System.SR.Format(System.SR.net_http_missing_sync_implementation, GetType(), "HttpMessageHandler", "Send"));
	}

	/// <summary>Send an HTTP request as an asynchronous operation.</summary>
	/// <param name="request">The HTTP request message to send.</param>
	/// <param name="cancellationToken">The cancellation token to cancel operation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
	protected internal abstract Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);

	/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpMessageHandler" /> and optionally disposes of the managed resources.</summary>
	/// <param name="disposing">
	///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
	}

	/// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:System.Net.Http.HttpMessageHandler" />.</summary>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
