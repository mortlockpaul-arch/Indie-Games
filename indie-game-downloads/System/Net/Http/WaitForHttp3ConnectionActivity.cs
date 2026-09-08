using System.Diagnostics;

namespace System.Net.Http;

internal struct WaitForHttp3ConnectionActivity(HttpConnectionSettings settings, HttpAuthority authority)
{
	private HttpConnectionSettings _settings = settings;

	private readonly HttpAuthority _authority = authority;

	private Activity _activity = null;

	private long _startTimestamp = 0L;

	public bool Started { get; private set; } = false;

	public void Start()
	{
		_startTimestamp = ((HttpTelemetry.Log.IsEnabled() || (GlobalHttpSettings.MetricsHandler.IsGloballyEnabled && _settings._metrics.RequestsQueueDuration.Enabled)) ? Stopwatch.GetTimestamp() : 0);
		_activity = ConnectionSetupDistributedTracing.StartWaitForConnectionActivity(_authority);
		Started = true;
	}

	public void Stop(HttpRequestMessage request, HttpConnectionPool pool, Exception exception)
	{
		if (exception != null)
		{
			ConnectionSetupDistributedTracing.ReportError(_activity, exception);
		}
		_activity?.Stop();
		if (_startTimestamp != 0L)
		{
			TimeSpan elapsedTime = Stopwatch.GetElapsedTime(_startTimestamp);
			if (GlobalHttpSettings.MetricsHandler.IsGloballyEnabled)
			{
				_settings._metrics.RequestLeftQueue(request, pool, elapsedTime, 3);
			}
			if (HttpTelemetry.Log.IsEnabled())
			{
				HttpTelemetry.Log.RequestLeftQueue(3, elapsedTime);
			}
		}
	}
}
