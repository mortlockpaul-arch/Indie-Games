using System.Diagnostics;

namespace System.Net.Http;

internal static class ConnectionSetupDistributedTracing
{
	private static readonly ActivitySource s_connectionsActivitySource = new ActivitySource("Experimental.System.Net.Http.Connections");

	public static Activity StartConnectionSetupActivity(bool isSecure, string serverAddress, int port)
	{
		Activity activity = null;
		if (s_connectionsActivitySource.HasListeners())
		{
			Activity.Current = null;
			activity = s_connectionsActivitySource.StartActivity("Experimental.System.Net.Http.Connections.ConnectionSetup");
		}
		if (activity != null)
		{
			activity.DisplayName = $"HTTP connection_setup {serverAddress}:{port}";
			if (activity.IsAllDataRequested)
			{
				activity.SetTag("server.address", serverAddress);
				activity.SetTag("server.port", port);
				activity.SetTag("url.scheme", isSecure ? "https" : "http");
			}
		}
		return activity;
	}

	public static void StopConnectionSetupActivity(Activity activity, Exception exception, IPEndPoint remoteEndPoint)
	{
		if (exception != null)
		{
			ReportError(activity, exception);
		}
		else if (activity.IsAllDataRequested && remoteEndPoint != null)
		{
			activity.SetTag("network.peer.address", remoteEndPoint.Address.ToString());
		}
		activity.Stop();
	}

	public static void ReportError(Activity activity, Exception exception)
	{
		if (activity != null)
		{
			activity.SetStatus(ActivityStatusCode.Error);
			if (activity.IsAllDataRequested)
			{
				DiagnosticsHelper.TryGetErrorType(null, exception, out var errorType);
				activity.SetTag("error.type", errorType);
			}
		}
	}

	public static Activity StartWaitForConnectionActivity(HttpAuthority authority)
	{
		Activity activity = s_connectionsActivitySource.StartActivity("Experimental.System.Net.Http.Connections.WaitForConnection");
		if (activity != null)
		{
			activity.DisplayName = $"HTTP wait_for_connection {authority.HostValue}:{authority.Port}";
		}
		return activity;
	}

	public static void AddConnectionLinkToRequestActivity(Activity connectionSetupActivity)
	{
		if (GlobalHttpSettings.DiagnosticsHandler.EnableActivityPropagation && DiagnosticsHandler.s_activitySource.HasListeners())
		{
			Activity current = Activity.Current;
			if (current?.Source == DiagnosticsHandler.s_activitySource)
			{
				current.AddLink(new ActivityLink(connectionSetupActivity.Context));
			}
		}
	}
}
