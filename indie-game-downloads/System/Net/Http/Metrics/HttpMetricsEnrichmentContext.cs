using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;

namespace System.Net.Http.Metrics;

public sealed class HttpMetricsEnrichmentContext
{
	private static readonly HttpRequestOptionsKey<List<Action<HttpMetricsEnrichmentContext>>> s_optionsKeyForCallbacks = new HttpRequestOptionsKey<List<Action<HttpMetricsEnrichmentContext>>>("HttpMetricsEnrichmentContext");

	private HttpRequestMessage _request;

	private HttpResponseMessage _response;

	private Exception _exception;

	private TagList _tags;

	public HttpRequestMessage Request => _request;

	public HttpResponseMessage? Response => _response;

	public Exception? Exception => _exception;

	private HttpMetricsEnrichmentContext()
	{
	}

	public void AddCustomTag(string name, object? value)
	{
		_tags.Add(name, value);
	}

	public static void AddCallback(HttpRequestMessage request, Action<HttpMetricsEnrichmentContext> callback)
	{
		ArgumentNullException.ThrowIfNull(request, "request");
		ArgumentNullException.ThrowIfNull(callback, "callback");
		HttpRequestOptions options = request.Options;
		if (options.TryGetValue(s_optionsKeyForCallbacks, out var value))
		{
			value.Add(callback);
			return;
		}
		HttpRequestOptionsKey<List<Action<HttpMetricsEnrichmentContext>>> key = s_optionsKeyForCallbacks;
		int num = 1;
		List<Action<HttpMetricsEnrichmentContext>> list = new List<Action<HttpMetricsEnrichmentContext>>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<Action<HttpMetricsEnrichmentContext>> span = CollectionsMarshal.AsSpan(list);
		int index = 0;
		span[index] = callback;
		options.Set(key, list);
	}

	internal static List<Action<HttpMetricsEnrichmentContext>> GetEnrichmentCallbacksForRequest(HttpRequestMessage request)
	{
		HttpRequestOptions options = request._options;
		if (options != null && options.Remove<string, object>(s_optionsKeyForCallbacks.Key, out var value))
		{
			return (List<Action<HttpMetricsEnrichmentContext>>)value;
		}
		return null;
	}

	internal static void RecordDurationWithEnrichment(List<Action<HttpMetricsEnrichmentContext>> callbacks, HttpRequestMessage request, HttpResponseMessage response, Exception exception, TimeSpan durationTime, in TagList commonTags, Histogram<double> requestDuration)
	{
		HttpMetricsEnrichmentContext httpMetricsEnrichmentContext = new HttpMetricsEnrichmentContext
		{
			_request = request,
			_response = response,
			_exception = exception,
			_tags = commonTags
		};
		foreach (Action<HttpMetricsEnrichmentContext> callback in callbacks)
		{
			callback(httpMetricsEnrichmentContext);
		}
		requestDuration.Record(durationTime.TotalSeconds, in httpMetricsEnrichmentContext._tags);
	}
}
