using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>A base type for handlers which only do some small processing of request and/or response messages.</summary>
public abstract class MessageProcessingHandler : DelegatingHandler
{
	private sealed class SendState : TaskCompletionSource<HttpResponseMessage>
	{
		internal readonly MessageProcessingHandler _handler;

		internal readonly CancellationToken _token;

		public SendState(MessageProcessingHandler handler, CancellationToken token)
		{
			_handler = handler;
			_token = token;
		}
	}

	/// <summary>Creates an instance of a <see cref="T:System.Net.Http.MessageProcessingHandler" /> class.</summary>
	protected MessageProcessingHandler()
	{
	}

	/// <summary>Creates an instance of a <see cref="T:System.Net.Http.MessageProcessingHandler" /> class with a specific inner handler.</summary>
	/// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
	protected MessageProcessingHandler(HttpMessageHandler innerHandler)
		: base(innerHandler)
	{
	}

	/// <summary>Performs processing on each request sent to the server.</summary>
	/// <param name="request">The HTTP request message to process.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The HTTP request message that was processed.</returns>
	protected abstract HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken);

	/// <summary>Perform processing on each response from the server.</summary>
	/// <param name="response">The HTTP response message to process.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The HTTP response message that was processed.</returns>
	protected abstract HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken);

	protected internal sealed override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request, "request");
		HttpRequestMessage request2 = ProcessRequest(request, cancellationToken);
		HttpResponseMessage response = base.Send(request2, cancellationToken);
		return ProcessResponse(response, cancellationToken);
	}

	/// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
	/// <param name="request">The HTTP request message to send to the server.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
	protected internal sealed override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request, "request");
		SendState sendState = new SendState(this, cancellationToken);
		try
		{
			HttpRequestMessage request2 = ProcessRequest(request, cancellationToken);
			base.SendAsync(request2, cancellationToken).ContinueWith(delegate(Task<HttpResponseMessage> task, object state)
			{
				SendState sendState2 = (SendState)state;
				MessageProcessingHandler handler = sendState2._handler;
				CancellationToken token = sendState2._token;
				if (task.IsFaulted)
				{
					sendState2.TrySetException(task.Exception.GetBaseException());
				}
				else if (task.IsCanceled)
				{
					sendState2.TrySetCanceled(token);
				}
				else
				{
					if (task.Result != null)
					{
						try
						{
							HttpResponseMessage result = handler.ProcessResponse(task.Result, token);
							sendState2.TrySetResult(result);
							return;
						}
						catch (OperationCanceledException e2)
						{
							HandleCanceledOperations(token, sendState2, e2);
							return;
						}
						catch (Exception exception2)
						{
							sendState2.TrySetException(exception2);
							return;
						}
					}
					sendState2.TrySetException(ExceptionDispatchInfo.SetCurrentStackTrace(new InvalidOperationException(System.SR.net_http_handler_noresponse)));
				}
			}, sendState, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}
		catch (OperationCanceledException e)
		{
			HandleCanceledOperations(cancellationToken, sendState, e);
		}
		catch (Exception exception)
		{
			sendState.TrySetException(exception);
		}
		return sendState.Task;
	}

	private static void HandleCanceledOperations(CancellationToken cancellationToken, TaskCompletionSource<HttpResponseMessage> tcs, OperationCanceledException e)
	{
		if (cancellationToken.IsCancellationRequested && e.CancellationToken == cancellationToken)
		{
			tcs.TrySetCanceled(cancellationToken);
		}
		else
		{
			tcs.TrySetException(e);
		}
	}
}
