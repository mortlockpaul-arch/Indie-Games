using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

internal sealed class HttpConnectionWaiter<T> : TaskCompletionSourceWithCancellation<T> where T : HttpConnectionBase
{
	public CancellationTokenSource ConnectionCancellationTokenSource;

	public bool CancelledByOriginatingRequestCompletion { get; set; }

	public ValueTask<T> WaitForConnectionAsync(HttpRequestMessage request, HttpConnectionPool pool, bool async, CancellationToken requestCancellationToken)
	{
		if (!HttpTelemetry.Log.IsEnabled() && (!GlobalHttpSettings.MetricsHandler.IsGloballyEnabled || !pool.Settings._metrics.RequestsQueueDuration.Enabled) && (!GlobalHttpSettings.DiagnosticsHandler.EnableActivityPropagation || Activity.Current?.Source != DiagnosticsHandler.s_activitySource))
		{
			return WaitWithCancellationAsync(async, requestCancellationToken);
		}
		return WaitForConnectionWithTelemetryAsync(request, pool, async, requestCancellationToken);
	}

	private async ValueTask<T> WaitForConnectionWithTelemetryAsync(HttpRequestMessage request, HttpConnectionPool pool, bool async, CancellationToken requestCancellationToken)
	{
		long startingTimestamp = Stopwatch.GetTimestamp();
		using Activity waitForConnectionActivity = ConnectionSetupDistributedTracing.StartWaitForConnectionActivity(pool.OriginAuthority);
		try
		{
			return await WaitWithCancellationAsync(async, requestCancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception exception) when (waitForConnectionActivity != null)
		{
			ConnectionSetupDistributedTracing.ReportError(waitForConnectionActivity, exception);
			throw;
		}
		finally
		{
			if (HttpTelemetry.Log.IsEnabled() || GlobalHttpSettings.MetricsHandler.IsGloballyEnabled)
			{
				TimeSpan elapsedTime = Stopwatch.GetElapsedTime(startingTimestamp);
				int versionMajor = ((typeof(T) == typeof(HttpConnection)) ? 1 : 2);
				if (GlobalHttpSettings.MetricsHandler.IsGloballyEnabled)
				{
					pool.Settings._metrics.RequestLeftQueue(request, pool, elapsedTime, versionMajor);
				}
				if (HttpTelemetry.Log.IsEnabled())
				{
					HttpTelemetry.Log.RequestLeftQueue(versionMajor, elapsedTime);
				}
			}
		}
	}

	public bool TrySignal(T connection)
	{
		if (TrySetResult(connection))
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				connection.Trace("Dequeued waiting request.", "TrySignal");
			}
			return true;
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			connection.Trace(base.Task.IsCanceled ? "Discarding canceled request from queue." : "Discarding signaled request waiter from queue.", "TrySignal");
		}
		return false;
	}

	public void SetTimeoutToPendingConnectionAttempt(HttpConnectionPool pool, bool requestCancelled)
	{
		int pendingConnectionTimeoutOnRequestCompletion = GlobalHttpSettings.SocketsHttpHandler.PendingConnectionTimeoutOnRequestCompletion;
		if (ConnectionCancellationTokenSource == null || pendingConnectionTimeoutOnRequestCompletion == -1 || (pool.Settings._connectTimeout != Timeout.InfiniteTimeSpan && pendingConnectionTimeoutOnRequestCompletion > (int)pool.Settings._connectTimeout.TotalMilliseconds))
		{
			return;
		}
		lock (this)
		{
			if (ConnectionCancellationTokenSource != null)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					pool.Trace($"Initiating cancellation of a pending connection attempt with delay of {pendingConnectionTimeoutOnRequestCompletion} ms, Reason: {(requestCancelled ? "Request cancelled" : "Request served by another connection")}.", "SetTimeoutToPendingConnectionAttempt");
				}
				CancelledByOriginatingRequestCompletion = true;
				if (pendingConnectionTimeoutOnRequestCompletion > 0)
				{
					ConnectionCancellationTokenSource.CancelAfter(pendingConnectionTimeoutOnRequestCompletion);
				}
				else
				{
					ConnectionCancellationTokenSource.Cancel();
				}
			}
		}
	}
}
