using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>A type for HTTP handlers that delegate the processing of HTTP response messages to another handler, called the inner handler.</summary>
public abstract class DelegatingHandler : HttpMessageHandler
{
	private HttpMessageHandler _innerHandler;

	private volatile bool _operationStarted;

	private volatile bool _disposed;

	/// <summary>Gets or sets the inner handler which processes the HTTP response messages.</summary>
	/// <returns>The inner handler for HTTP response messages.</returns>
	public HttpMessageHandler? InnerHandler
	{
		get
		{
			return _innerHandler;
		}
		[param: DisallowNull]
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			CheckDisposedOrStarted();
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Associate(this, value, "InnerHandler");
			}
			_innerHandler = value;
		}
	}

	/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.DelegatingHandler" /> class.</summary>
	protected DelegatingHandler()
	{
	}

	/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.DelegatingHandler" /> class with a specific inner handler.</summary>
	/// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
	protected DelegatingHandler(HttpMessageHandler innerHandler)
	{
		InnerHandler = innerHandler;
	}

	protected internal override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request, "request");
		SetOperationStarted();
		return _innerHandler.Send(request, cancellationToken);
	}

	/// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
	/// <param name="request">The HTTP request message to send to the server.</param>
	/// <param name="cancellationToken">A cancellation token to cancel operation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
	protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request, "request");
		SetOperationStarted();
		return _innerHandler.SendAsync(request, cancellationToken);
	}

	/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.DelegatingHandler" />, and optionally disposes of the managed resources.</summary>
	/// <param name="disposing">
	///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && !_disposed)
		{
			_disposed = true;
			_innerHandler?.Dispose();
		}
		base.Dispose(disposing);
	}

	private void CheckDisposedOrStarted()
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		if (_operationStarted)
		{
			throw new InvalidOperationException(System.SR.net_http_operation_started);
		}
	}

	private void SetOperationStarted()
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		if (_innerHandler == null)
		{
			throw new InvalidOperationException(System.SR.net_http_handler_not_assigned);
		}
		if (!_operationStarted)
		{
			_operationStarted = true;
		}
	}
}
