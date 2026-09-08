using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace System.Net.Http.Metrics;

internal sealed class SocketsHttpHandlerMetrics(Meter meter)
{
	public readonly UpDownCounter<long> OpenConnections = meter.CreateUpDownCounter<long>("http.client.open_connections", "{connection}", "Number of outbound HTTP connections that are currently active or idle on the client.");

	public readonly Histogram<double> ConnectionDuration = meter.CreateHistogram("http.client.connection.duration", "s", "The duration of successfully established outbound HTTP connections.", null, new InstrumentAdvice<double>
	{
		HistogramBucketBoundaries = new global::_003C_003Ez__ReadOnlyArray<double>(new double[14]
		{
			0.01, 0.02, 0.05, 0.1, 0.2, 0.5, 1.0, 2.0, 5.0, 10.0,
			30.0, 60.0, 120.0, 300.0
		})
	});

	public readonly Histogram<double> RequestsQueueDuration = meter.CreateHistogram("http.client.request.time_in_queue", "s", "The amount of time requests spent on a queue waiting for an available connection.", null, DiagnosticsHelper.ShortHistogramAdvice);

	public void RequestLeftQueue(HttpRequestMessage request, HttpConnectionPool pool, TimeSpan duration, int versionMajor)
	{
		if (RequestsQueueDuration.Enabled)
		{
			TagList tagList = new TagList
			{
				{
					"network.protocol.version",
					versionMajor switch
					{
						1 => "1.1", 
						2 => "2", 
						_ => "3", 
					}
				},
				{
					"url.scheme",
					pool.IsSecure ? "https" : "http"
				},
				{ "server.address", pool.TelemetryServerAddress }
			};
			if (!pool.IsDefaultPort)
			{
				tagList.Add("server.port", pool.OriginAuthority.Port);
			}
			tagList.Add(DiagnosticsHelper.GetMethodTag(request.Method, out var _));
			RequestsQueueDuration.Record(duration.TotalSeconds, in tagList);
		}
	}
}
