using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace System.Net;

internal static class NameResolutionMetrics
{
	private static readonly Meter s_meter = new Meter("System.Net.NameResolution");

	private static readonly Histogram<double> s_lookupDuration = s_meter.CreateHistogram("dns.lookup.duration", "s", "Measures the time taken to perform a DNS lookup.", null, new InstrumentAdvice<double>
	{
		HistogramBucketBoundaries = new _003C_003Ez__ReadOnlyArray<double>(new double[14]
		{
			0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1.0,
			2.5, 5.0, 7.5, 10.0
		})
	});

	public static bool IsEnabled()
	{
		return s_lookupDuration.Enabled;
	}

	public static void AfterResolution(TimeSpan duration, string hostName, Exception exception)
	{
		KeyValuePair<string, object> keyValuePair = KeyValuePair.Create("dns.question.name", (object)hostName);
		if (exception == null)
		{
			s_lookupDuration.Record(duration.TotalSeconds, keyValuePair);
			return;
		}
		string errorType = NameResolutionTelemetry.GetErrorType(exception);
		KeyValuePair<string, object> tag = KeyValuePair.Create("error.type", (object)errorType);
		s_lookupDuration.Record(duration.TotalSeconds, keyValuePair, tag);
	}
}
