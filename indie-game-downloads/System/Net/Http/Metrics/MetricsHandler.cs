using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Metrics;

internal sealed class MetricsHandler : HttpMessageHandlerStage
{
	private sealed class SharedMeter : Meter
	{
		public static Meter Instance { get; } = new SharedMeter();

		private SharedMeter()
			: base("System.Net.Http")
		{
		}

		protected override void Dispose(bool disposing)
		{
		}
	}

	private readonly HttpMessageHandler _innerHandler;

	private readonly UpDownCounter<long> _activeRequests;

	private readonly Histogram<double> _requestsDuration;

	private readonly IWebProxy _proxy;

	public MetricsHandler(HttpMessageHandler innerHandler, IMeterFactory meterFactory, IWebProxy proxy, out Meter meter)
	{
		_innerHandler = innerHandler;
		_proxy = proxy;
		meter = meterFactory?.Create("System.Net.Http") ?? SharedMeter.Instance;
		_activeRequests = meter.CreateUpDownCounter<long>("http.client.active_requests", "{request}", "Number of outbound HTTP requests that are currently active on the client.");
		_requestsDuration = meter.CreateHistogram("http.client.request.duration", "s", "Duration of HTTP client requests.", null, DiagnosticsHelper.ShortHistogramAdvice);
	}

	internal override ValueTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, bool async, CancellationToken cancellationToken)
	{
		if (_activeRequests.Enabled || _requestsDuration.Enabled)
		{
			return SendAsyncWithMetrics(request, async, cancellationToken);
		}
		if (!async)
		{
			return new ValueTask<HttpResponseMessage>(_innerHandler.Send(request, cancellationToken));
		}
		return new ValueTask<HttpResponseMessage>(_innerHandler.SendAsync(request, cancellationToken));
	}

	private async ValueTask<HttpResponseMessage> SendAsyncWithMetrics(HttpRequestMessage request, bool async, CancellationToken cancellationToken)
	{
		(long, bool) tuple = RequestStart(request);
		long startTimestamp = tuple.Item1;
		bool recordCurrentRequests = tuple.Item2;
		HttpResponseMessage response = null;
		Exception exception = null;
		try
		{
			HttpResponseMessage httpResponseMessage = ((!async) ? _innerHandler.Send(request, cancellationToken) : (await _innerHandler.SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)));
			response = httpResponseMessage;
			return response;
		}
		catch (Exception ex)
		{
			exception = ex;
			throw;
		}
		finally
		{
			RequestStop(request, response, exception, startTimestamp, recordCurrentRequests);
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

	private (long StartTimestamp, bool RecordCurrentRequests) RequestStart(HttpRequestMessage request)
	{
		bool enabled = _activeRequests.Enabled;
		long timestamp = Stopwatch.GetTimestamp();
		if (enabled)
		{
			TagList tagList = InitializeCommonTags(request);
			_activeRequests.Add(1L, in tagList);
		}
		return (StartTimestamp: timestamp, RecordCurrentRequests: enabled);
	}

	private void RequestStop(HttpRequestMessage request, HttpResponseMessage response, Exception exception, long startTimestamp, bool recordCurrentRequests)
	{
		TagList tagList = InitializeCommonTags(request);
		if (recordCurrentRequests)
		{
			_activeRequests.Add(-1L, in tagList);
		}
		if (_requestsDuration.Enabled)
		{
			if (response != null)
			{
				tagList.Add("http.response.status_code", DiagnosticsHelper.GetBoxedInt32((int)response.StatusCode));
				tagList.Add("network.protocol.version", DiagnosticsHelper.GetProtocolVersionString(response.Version));
			}
			if (DiagnosticsHelper.TryGetErrorType(response, exception, out var errorType))
			{
				tagList.Add("error.type", errorType);
			}
			TimeSpan elapsedTime = Stopwatch.GetElapsedTime(startTimestamp, Stopwatch.GetTimestamp());
			List<Action<HttpMetricsEnrichmentContext>> enrichmentCallbacksForRequest = HttpMetricsEnrichmentContext.GetEnrichmentCallbacksForRequest(request);
			if (enrichmentCallbacksForRequest == null)
			{
				_requestsDuration.Record(elapsedTime.TotalSeconds, in tagList);
			}
			else
			{
				HttpMetricsEnrichmentContext.RecordDurationWithEnrichment(enrichmentCallbacksForRequest, request, response, exception, elapsedTime, in tagList, _requestsDuration);
			}
		}
	}

	private TagList InitializeCommonTags(HttpRequestMessage request)
	{
		TagList result = default(TagList);
		Uri requestUri = request.RequestUri;
		if ((object)requestUri != null && requestUri.IsAbsoluteUri)
		{
			result.Add("url.scheme", requestUri.Scheme);
			result.Add("server.address", DiagnosticsHelper.GetServerAddress(request, _proxy));
			result.Add("server.port", DiagnosticsHelper.GetBoxedInt32(requestUri.Port));
		}
		result.Add(DiagnosticsHelper.GetMethodTag(request.Method, out var _));
		return result;
	}
}
