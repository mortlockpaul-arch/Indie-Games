using System.Diagnostics;

namespace System.Net;

internal readonly struct NameResolutionActivity
{
	private static readonly ActivitySource s_activitySource = new ActivitySource("Experimental.System.Net.NameResolution");

	private readonly long _startingTimestamp;

	private readonly Activity _activity;

	public NameResolutionActivity(object hostNameOrAddress, long startingTimestamp)
	{
		_startingTimestamp = startingTimestamp;
		_activity = s_activitySource.StartActivity("Experimental.System.Net.NameResolution.DnsLookup");
		if (_activity != null)
		{
			string hostnameFromStateObject = NameResolutionTelemetry.GetHostnameFromStateObject(hostNameOrAddress);
			_activity.DisplayName = ((hostNameOrAddress is IPAddress) ? ("DNS reverse lookup " + hostnameFromStateObject) : ("DNS lookup " + hostnameFromStateObject));
			if (_activity.IsAllDataRequested)
			{
				_activity.SetTag("dns.question.name", hostnameFromStateObject);
			}
		}
	}

	public static bool IsTracingEnabled()
	{
		return s_activitySource.HasListeners();
	}

	public bool Stop(object answer, Exception exception, out TimeSpan duration)
	{
		if (_activity != null)
		{
			if (_activity.IsAllDataRequested)
			{
				if (answer != null)
				{
					string[] array = ((answer is string text) ? new string[1] { text } : ((answer is IPAddress[] addresses) ? GetStringValues(addresses) : ((!(answer is IPHostEntry iPHostEntry)) ? null : GetStringValues(iPHostEntry.AddressList))));
					string[] value = array;
					_activity.SetTag("dns.answers", value);
				}
				else
				{
					string errorType = NameResolutionTelemetry.GetErrorType(exception);
					_activity.SetTag("error.type", errorType);
				}
			}
			if (exception != null)
			{
				_activity.SetStatus(ActivityStatusCode.Error);
			}
			_activity.Stop();
		}
		if (_startingTimestamp == 0L)
		{
			duration = default(TimeSpan);
			return false;
		}
		duration = Stopwatch.GetElapsedTime(_startingTimestamp);
		return true;
		static string[] GetStringValues(IPAddress[] array3)
		{
			string[] array2 = new string[array3.Length];
			for (int i = 0; i < array3.Length; i++)
			{
				array2[i] = array3[i].ToString();
			}
			return array2;
		}
	}
}
