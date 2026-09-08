using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

internal sealed class DiagnosticsHandler : HttpMessageHandlerStage
{
	private sealed class ActivityStartData
	{
		public HttpRequestMessage Request { get; }

		[DynamicDependency("RequestUri", typeof(HttpRequestMessage))]
		[DynamicDependency("Method", typeof(HttpRequestMessage))]
		[DynamicDependency("Host", typeof(Uri))]
		[DynamicDependency("Port", typeof(Uri))]
		internal ActivityStartData(HttpRequestMessage request)
		{
			Request = request;
		}

		public override string ToString()
		{
			return $"{{ {"Request"} = {Request} }}";
		}
	}

	private sealed class ActivityStopData
	{
		public HttpResponseMessage Response { get; }

		public HttpRequestMessage Request { get; }

		public TaskStatus RequestTaskStatus { get; }

		internal ActivityStopData(HttpResponseMessage response, HttpRequestMessage request, TaskStatus requestTaskStatus)
		{
			Response = response;
			Request = request;
			RequestTaskStatus = requestTaskStatus;
		}

		public override string ToString()
		{
			return $"{{ {"Response"} = {Response}, {"Request"} = {Request}, {"RequestTaskStatus"} = {RequestTaskStatus} }}";
		}
	}

	private sealed class ExceptionData
	{
		public Exception Exception { get; }

		public HttpRequestMessage Request { get; }

		[DynamicDependency("RequestUri", typeof(HttpRequestMessage))]
		[DynamicDependency("Method", typeof(HttpRequestMessage))]
		[DynamicDependency("Host", typeof(Uri))]
		[DynamicDependency("Port", typeof(Uri))]
		[DynamicDependency("Message", typeof(Exception))]
		[DynamicDependency("StackTrace", typeof(Exception))]
		internal ExceptionData(Exception exception, HttpRequestMessage request)
		{
			Exception = exception;
			Request = request;
		}

		public override string ToString()
		{
			return $"{{ {"Exception"} = {Exception}, {"Request"} = {Request} }}";
		}
	}

	private sealed class RequestData
	{
		public HttpRequestMessage Request { get; }

		public Guid LoggingRequestId { get; }

		public long Timestamp { get; }

		[DynamicDependency("RequestUri", typeof(HttpRequestMessage))]
		[DynamicDependency("Method", typeof(HttpRequestMessage))]
		[DynamicDependency("Host", typeof(Uri))]
		[DynamicDependency("Port", typeof(Uri))]
		internal RequestData(HttpRequestMessage request, Guid loggingRequestId, long timestamp)
		{
			Request = request;
			LoggingRequestId = loggingRequestId;
			Timestamp = timestamp;
		}

		public override string ToString()
		{
			return $"{{ {"Request"} = {Request}, {"LoggingRequestId"} = {LoggingRequestId}, {"Timestamp"} = {Timestamp} }}";
		}
	}

	private sealed class ResponseData
	{
		public HttpResponseMessage Response { get; }

		public Guid LoggingRequestId { get; }

		public long Timestamp { get; }

		public TaskStatus RequestTaskStatus { get; }

		[DynamicDependency("StatusCode", typeof(HttpResponseMessage))]
		internal ResponseData(HttpResponseMessage response, Guid loggingRequestId, long timestamp, TaskStatus requestTaskStatus)
		{
			Response = response;
			LoggingRequestId = loggingRequestId;
			Timestamp = timestamp;
			RequestTaskStatus = requestTaskStatus;
		}

		public override string ToString()
		{
			return $"{{ {"Response"} = {Response}, {"LoggingRequestId"} = {LoggingRequestId}, {"Timestamp"} = {Timestamp}, {"RequestTaskStatus"} = {RequestTaskStatus} }}";
		}
	}

	private static readonly DiagnosticListener s_diagnosticListener = new DiagnosticListener("HttpHandlerDiagnosticListener");

	internal static readonly ActivitySource s_activitySource = new ActivitySource("System.Net.Http");

	private readonly HttpMessageHandler _innerHandler;

	private readonly DistributedContextPropagator _propagator;

	private readonly HeaderDescriptor[] _propagatorFields;

	private readonly IWebProxy _proxy;

	public DiagnosticsHandler(HttpMessageHandler innerHandler, DistributedContextPropagator propagator, IWebProxy proxy, bool autoRedirect = false)
	{
		_innerHandler = innerHandler;
		_propagator = propagator;
		_proxy = proxy;
		if (!autoRedirect)
		{
			return;
		}
		IReadOnlyCollection<string> fields = _propagator.Fields;
		if (fields == null || fields.Count <= 0)
		{
			return;
		}
		List<HeaderDescriptor> list = new List<HeaderDescriptor>(fields.Count);
		foreach (string item in fields)
		{
			if (item != null && HeaderDescriptor.TryGet(item, out var descriptor))
			{
				list.Add(descriptor);
			}
		}
		_propagatorFields = list.ToArray();
	}

	private static bool IsEnabled()
	{
		if (Activity.Current == null && !s_activitySource.HasListeners())
		{
			return s_diagnosticListener.IsEnabled();
		}
		return true;
	}

	private static Activity StartActivity(HttpRequestMessage request)
	{
		Activity activity = null;
		if (s_activitySource.HasListeners())
		{
			activity = s_activitySource.StartActivity("System.Net.Http.HttpRequestOut", ActivityKind.Client);
		}
		if (activity == null && (Activity.Current != null || s_diagnosticListener.IsEnabled("System.Net.Http.HttpRequestOut", request)))
		{
			activity = new Activity("System.Net.Http.HttpRequestOut").Start();
		}
		return activity;
	}

	internal override ValueTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, bool async, CancellationToken cancellationToken)
	{
		if (IsEnabled())
		{
			return SendAsyncCore(request, async, cancellationToken);
		}
		if (!async)
		{
			return new ValueTask<HttpResponseMessage>(_innerHandler.Send(request, cancellationToken));
		}
		return new ValueTask<HttpResponseMessage>(_innerHandler.SendAsync(request, cancellationToken));
	}

	private async ValueTask<HttpResponseMessage> SendAsyncCore(HttpRequestMessage request, bool async, CancellationToken cancellationToken)
	{
		if (request.WasPropagatorStateInjectedByDiagnosticsHandler())
		{
			HeaderDescriptor[] propagatorFields = _propagatorFields;
			if (propagatorFields != null)
			{
				HeaderDescriptor[] array = propagatorFields;
				foreach (HeaderDescriptor key in array)
				{
					request.Headers.Remove(key);
				}
			}
		}
		DiagnosticListener diagnosticListener = s_diagnosticListener;
		Guid loggingRequestId = Guid.Empty;
		Activity activity = StartActivity(request);
		if (activity != null)
		{
			activity.DisplayName = HttpMethod.GetKnownMethod(request.Method.Method.AsSpan())?.Method ?? "HTTP";
			if (activity.IsAllDataRequested)
			{
				KeyValuePair<string, object> methodTag = DiagnosticsHelper.GetMethodTag(request.Method, out var isUnknownMethod);
				activity.SetTag(methodTag.Key, methodTag.Value);
				if (isUnknownMethod)
				{
					activity.SetTag("http.request.method_original", request.Method.Method);
				}
				Uri requestUri = request.RequestUri;
				if ((object)requestUri != null && requestUri.IsAbsoluteUri)
				{
					activity.SetTag("server.address", DiagnosticsHelper.GetServerAddress(request, _proxy));
					activity.SetTag("server.port", requestUri.Port);
					activity.SetTag("url.full", UriRedactionHelper.GetRedactedUriString(requestUri));
				}
			}
			if (diagnosticListener.IsEnabled("System.Net.Http.HttpRequestOut.Start"))
			{
				Write(diagnosticListener, "System.Net.Http.HttpRequestOut.Start", new ActivityStartData(request));
			}
		}
		if (diagnosticListener.IsEnabled("System.Net.Http.Request"))
		{
			long timestamp = Stopwatch.GetTimestamp();
			loggingRequestId = Guid.NewGuid();
			Write(diagnosticListener, "System.Net.Http.Request", new RequestData(request, loggingRequestId, timestamp));
		}
		if (activity != null)
		{
			InjectHeaders(activity, request);
		}
		HttpResponseMessage response = null;
		Exception exception = null;
		TaskStatus taskStatus = TaskStatus.RanToCompletion;
		try
		{
			HttpResponseMessage httpResponseMessage = ((!async) ? _innerHandler.Send(request, cancellationToken) : (await _innerHandler.SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)));
			response = httpResponseMessage;
			return response;
		}
		catch (OperationCanceledException ex)
		{
			taskStatus = TaskStatus.Canceled;
			exception = ex;
			throw;
		}
		catch (Exception ex2)
		{
			taskStatus = TaskStatus.Faulted;
			exception = ex2;
			if (diagnosticListener.IsEnabled("System.Net.Http.Exception"))
			{
				Write(diagnosticListener, "System.Net.Http.Exception", new ExceptionData(ex2, request));
			}
			throw;
		}
		finally
		{
			if (activity != null)
			{
				activity.SetEndTime(DateTime.UtcNow);
				if (activity.IsAllDataRequested)
				{
					if (response != null)
					{
						activity.SetTag("http.response.status_code", DiagnosticsHelper.GetBoxedInt32((int)response.StatusCode));
						activity.SetTag("network.protocol.version", DiagnosticsHelper.GetProtocolVersionString(response.Version));
					}
					if (DiagnosticsHelper.TryGetErrorType(response, exception, out var errorType))
					{
						activity.SetTag("error.type", errorType);
						activity.SetStatus(ActivityStatusCode.Error);
						if (exception != null)
						{
							Exception exception2 = exception;
							DateTimeOffset timestamp2 = activity.StartTimeUtc + activity.Duration;
							activity.AddException(exception2, default(TagList), timestamp2);
						}
					}
				}
				if (diagnosticListener.IsEnabled("System.Net.Http.HttpRequestOut.Stop"))
				{
					Write(diagnosticListener, "System.Net.Http.HttpRequestOut.Stop", new ActivityStopData(response, request, taskStatus));
				}
				activity.Stop();
			}
			if (diagnosticListener.IsEnabled("System.Net.Http.Response"))
			{
				long timestamp3 = Stopwatch.GetTimestamp();
				Write(diagnosticListener, "System.Net.Http.Response", new ResponseData(response, loggingRequestId, timestamp3, taskStatus));
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_innerHandler.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InjectHeaders(Activity currentActivity, HttpRequestMessage request)
	{
		_propagator.Inject(currentActivity, request, delegate(object carrier, string key, string value)
		{
			if (carrier is HttpRequestMessage httpRequestMessage && key != null)
			{
				HeaderDescriptor headerDescriptor = httpRequestMessage.Headers.GetHeaderDescriptor(key);
				if (!httpRequestMessage.Headers.Contains(headerDescriptor))
				{
					httpRequestMessage.Headers.Add(headerDescriptor, value);
				}
			}
		});
		request.MarkPropagatorStateInjectedByDiagnosticsHandler();
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern", Justification = "The values being passed into Write have the commonly used properties being preserved with DynamicDependency.")]
	private static void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(DiagnosticSource diagnosticSource, string name, T value)
	{
		diagnosticSource.Write(name, value);
	}
}
