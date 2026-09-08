using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>A specialty class that allows applications to call the <see cref="M:System.Net.Http.HttpMessageInvoker.SendAsync(System.Net.Http.HttpRequestMessage,System.Threading.CancellationToken)" /> method on an HTTP handler chain.</summary>
public class HttpMessageInvoker : IDisposable
{
	private volatile bool _disposed;

	private readonly bool _disposeHandler;

	private readonly HttpMessageHandler _handler;

	/// <summary>Initializes an instance of a <see cref="T:System.Net.Http.HttpMessageInvoker" /> class with a specific <see cref="T:System.Net.Http.HttpMessageHandler" />.</summary>
	/// <param name="handler">The <see cref="T:System.Net.Http.HttpMessageHandler" /> responsible for processing the HTTP response messages.</param>
	public HttpMessageInvoker(HttpMessageHandler handler)
		: this(handler, disposeHandler: true)
	{
	}

	/// <summary>Initializes an instance of a <see cref="T:System.Net.Http.HttpMessageInvoker" /> class with a specific <see cref="T:System.Net.Http.HttpMessageHandler" />.</summary>
	/// <param name="handler">The <see cref="T:System.Net.Http.HttpMessageHandler" /> responsible for processing the HTTP response messages.</param>
	/// <param name="disposeHandler">
	///   <see langword="true" /> if the inner handler should be disposed of by Dispose(), <see langword="false" /> if you intend to reuse the inner handler.</param>
	public HttpMessageInvoker(HttpMessageHandler handler, bool disposeHandler)
	{
		ArgumentNullException.ThrowIfNull(handler, "handler");
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Associate(this, handler, ".ctor");
		}
		_handler = handler;
		_disposeHandler = disposeHandler;
	}

	[UnsupportedOSPlatform("browser")]
	public virtual HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request, "request");
		ObjectDisposedException.ThrowIf(_disposed, this);
		if (ShouldSendWithTelemetry(request))
		{
			HttpTelemetry.Log.RequestStart(request);
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				httpResponseMessage = _handler.Send(request, cancellationToken);
				return httpResponseMessage;
			}
			catch (Exception exception) when (LogRequestFailed(exception, telemetryStarted: true))
			{
				throw;
			}
			finally
			{
				HttpTelemetry.Log.RequestStop(httpResponseMessage);
			}
		}
		return _handler.Send(request, cancellationToken);
	}

	/// <summary>Send an HTTP request as an asynchronous operation.</summary>
	/// <param name="request">The HTTP request message to send.</param>
	/// <param name="cancellationToken">The cancellation token to cancel operation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
	public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request, "request");
		ObjectDisposedException.ThrowIf(_disposed, this);
		if (ShouldSendWithTelemetry(request))
		{
			return SendAsyncWithTelemetry(_handler, request, cancellationToken);
		}
		return _handler.SendAsync(request, cancellationToken);
		static async Task<HttpResponseMessage> SendAsyncWithTelemetry(HttpMessageHandler handler, HttpRequestMessage request2, CancellationToken cancellationToken2)
		{
			HttpTelemetry.Log.RequestStart(request2);
			HttpResponseMessage response = null;
			try
			{
				response = await handler.SendAsync(request2, cancellationToken2).ConfigureAwait(continueOnCapturedContext: false);
				return response;
			}
			catch (Exception exception) when (LogRequestFailed(exception, telemetryStarted: true))
			{
				throw;
			}
			finally
			{
				HttpTelemetry.Log.RequestStop(response);
			}
		}
	}

	private static bool ShouldSendWithTelemetry(HttpRequestMessage request)
	{
		if (HttpTelemetry.Log.IsEnabled() && !request.WasSentByHttpClient())
		{
			Uri requestUri = request.RequestUri;
			if ((object)requestUri != null)
			{
				return requestUri.IsAbsoluteUri;
			}
		}
		return false;
	}

	internal static bool LogRequestFailed(Exception exception, bool telemetryStarted)
	{
		if (HttpTelemetry.Log.IsEnabled() & telemetryStarted)
		{
			HttpTelemetry.Log.RequestFailed(exception);
		}
		return false;
	}

	/// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:System.Net.Http.HttpMessageInvoker" />.</summary>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpMessageInvoker" /> and optionally disposes of the managed resources.</summary>
	/// <param name="disposing">
	///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing && !_disposed)
		{
			_disposed = true;
			if (_disposeHandler)
			{
				_handler.Dispose();
			}
		}
	}
}
