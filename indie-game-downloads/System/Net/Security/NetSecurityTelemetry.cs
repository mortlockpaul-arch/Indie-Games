using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Security.Authentication;
using System.Threading;

namespace System.Net.Security;

[EventSource(Name = "System.Net.Security")]
internal sealed class NetSecurityTelemetry : EventSource
{
	private static readonly ActivitySource s_activitySource = new ActivitySource("Experimental.System.Net.Security");

	public static readonly NetSecurityTelemetry Log = new NetSecurityTelemetry();

	private IncrementingPollingCounter _tlsHandshakeRateCounter;

	private PollingCounter _totalTlsHandshakesCounter;

	private PollingCounter _currentTlsHandshakesCounter;

	private PollingCounter _failedTlsHandshakesCounter;

	private PollingCounter _sessionsOpenCounter;

	private PollingCounter _sessionsOpenTls10Counter;

	private PollingCounter _sessionsOpenTls11Counter;

	private PollingCounter _sessionsOpenTls12Counter;

	private PollingCounter _sessionsOpenTls13Counter;

	private EventCounter _handshakeDurationCounter;

	private EventCounter _handshakeDurationTls10Counter;

	private EventCounter _handshakeDurationTls11Counter;

	private EventCounter _handshakeDurationTls12Counter;

	private EventCounter _handshakeDurationTls13Counter;

	private long _finishedTlsHandshakes;

	private long _startedTlsHandshakes;

	private long _failedTlsHandshakes;

	private long _sessionsOpen;

	private long _sessionsOpenTls10;

	private long _sessionsOpenTls11;

	private long _sessionsOpenTls12;

	private long _sessionsOpenTls13;

	public static bool AnyTelemetryEnabled()
	{
		if (!Log.IsEnabled())
		{
			return s_activitySource.HasListeners();
		}
		return true;
	}

	protected override void OnEventCommand(EventCommandEventArgs command)
	{
		if (command.Command != EventCommand.Enable)
		{
			return;
		}
		if (_tlsHandshakeRateCounter == null)
		{
			_tlsHandshakeRateCounter = new IncrementingPollingCounter("tls-handshake-rate", this, () => Interlocked.Read(in _finishedTlsHandshakes))
			{
				DisplayName = "TLS handshakes completed",
				DisplayRateTimeScale = TimeSpan.FromSeconds(1L)
			};
		}
		if (_totalTlsHandshakesCounter == null)
		{
			_totalTlsHandshakesCounter = new PollingCounter("total-tls-handshakes", this, () => Interlocked.Read(in _finishedTlsHandshakes))
			{
				DisplayName = "Total TLS handshakes completed"
			};
		}
		if (_currentTlsHandshakesCounter == null)
		{
			_currentTlsHandshakesCounter = new PollingCounter("current-tls-handshakes", this, () => -Interlocked.Read(in _finishedTlsHandshakes) + Interlocked.Read(in _startedTlsHandshakes))
			{
				DisplayName = "Current TLS handshakes"
			};
		}
		if (_failedTlsHandshakesCounter == null)
		{
			_failedTlsHandshakesCounter = new PollingCounter("failed-tls-handshakes", this, () => Interlocked.Read(in _failedTlsHandshakes))
			{
				DisplayName = "Total TLS handshakes failed"
			};
		}
		if (_sessionsOpenCounter == null)
		{
			_sessionsOpenCounter = new PollingCounter("all-tls-sessions-open", this, () => Interlocked.Read(in _sessionsOpen))
			{
				DisplayName = "All TLS Sessions Active"
			};
		}
		if (_sessionsOpenTls10Counter == null)
		{
			_sessionsOpenTls10Counter = new PollingCounter("tls10-sessions-open", this, () => Interlocked.Read(in _sessionsOpenTls10))
			{
				DisplayName = "TLS 1.0 Sessions Active"
			};
		}
		if (_sessionsOpenTls11Counter == null)
		{
			_sessionsOpenTls11Counter = new PollingCounter("tls11-sessions-open", this, () => Interlocked.Read(in _sessionsOpenTls11))
			{
				DisplayName = "TLS 1.1 Sessions Active"
			};
		}
		if (_sessionsOpenTls12Counter == null)
		{
			_sessionsOpenTls12Counter = new PollingCounter("tls12-sessions-open", this, () => Interlocked.Read(in _sessionsOpenTls12))
			{
				DisplayName = "TLS 1.2 Sessions Active"
			};
		}
		if (_sessionsOpenTls13Counter == null)
		{
			_sessionsOpenTls13Counter = new PollingCounter("tls13-sessions-open", this, () => Interlocked.Read(in _sessionsOpenTls13))
			{
				DisplayName = "TLS 1.3 Sessions Active"
			};
		}
		if (_handshakeDurationCounter == null)
		{
			_handshakeDurationCounter = new EventCounter("all-tls-handshake-duration", this)
			{
				DisplayName = "TLS Handshake Duration",
				DisplayUnits = "ms"
			};
		}
		if (_handshakeDurationTls10Counter == null)
		{
			_handshakeDurationTls10Counter = new EventCounter("tls10-handshake-duration", this)
			{
				DisplayName = "TLS 1.0 Handshake Duration",
				DisplayUnits = "ms"
			};
		}
		if (_handshakeDurationTls11Counter == null)
		{
			_handshakeDurationTls11Counter = new EventCounter("tls11-handshake-duration", this)
			{
				DisplayName = "TLS 1.1 Handshake Duration",
				DisplayUnits = "ms"
			};
		}
		if (_handshakeDurationTls12Counter == null)
		{
			_handshakeDurationTls12Counter = new EventCounter("tls12-handshake-duration", this)
			{
				DisplayName = "TLS 1.2 Handshake Duration",
				DisplayUnits = "ms"
			};
		}
		if (_handshakeDurationTls13Counter == null)
		{
			_handshakeDurationTls13Counter = new EventCounter("tls13-handshake-duration", this)
			{
				DisplayName = "TLS 1.3 Handshake Duration",
				DisplayUnits = "ms"
			};
		}
	}

	[Event(1, Level = EventLevel.Informational)]
	public void HandshakeStart(bool isServer, string targetHost)
	{
		Interlocked.Increment(ref _startedTlsHandshakes);
		if (IsEnabled(EventLevel.Informational, EventKeywords.None))
		{
			WriteEvent(1, isServer, targetHost);
		}
	}

	[Event(2, Level = EventLevel.Informational)]
	private void HandshakeStop(SslProtocols protocol)
	{
		if (IsEnabled(EventLevel.Informational, EventKeywords.None))
		{
			WriteEvent(2, (int)protocol);
		}
	}

	[Event(3, Level = EventLevel.Error)]
	private void HandshakeFailed(bool isServer, double elapsedMilliseconds, string exceptionMessage)
	{
		WriteEvent(3, isServer, elapsedMilliseconds, exceptionMessage);
	}

	[NonEvent]
	public void HandshakeFailed(bool isServer, long startingTimestamp, string exceptionMessage)
	{
		Interlocked.Increment(ref _finishedTlsHandshakes);
		Interlocked.Increment(ref _failedTlsHandshakes);
		if (IsEnabled(EventLevel.Error, EventKeywords.None))
		{
			HandshakeFailed(isServer, Stopwatch.GetElapsedTime(startingTimestamp).TotalMilliseconds, exceptionMessage);
		}
		HandshakeStop(SslProtocols.None);
	}

	[NonEvent]
	public void HandshakeCompleted(SslProtocols protocol, long startingTimestamp, bool connectionOpen)
	{
		Interlocked.Increment(ref _finishedTlsHandshakes);
		long num = 0L;
		ref long location = ref num;
		EventCounter eventCounter = null;
		switch (protocol)
		{
		case SslProtocols.Tls:
			location = ref _sessionsOpenTls10;
			eventCounter = _handshakeDurationTls10Counter;
			break;
		case SslProtocols.Tls11:
			location = ref _sessionsOpenTls11;
			eventCounter = _handshakeDurationTls11Counter;
			break;
		case SslProtocols.Tls12:
			location = ref _sessionsOpenTls12;
			eventCounter = _handshakeDurationTls12Counter;
			break;
		case SslProtocols.Tls13:
			location = ref _sessionsOpenTls13;
			eventCounter = _handshakeDurationTls13Counter;
			break;
		}
		if (connectionOpen)
		{
			Interlocked.Increment(ref location);
			Interlocked.Increment(ref _sessionsOpen);
		}
		double totalMilliseconds = Stopwatch.GetElapsedTime(startingTimestamp).TotalMilliseconds;
		eventCounter?.WriteMetric(totalMilliseconds);
		_handshakeDurationCounter?.WriteMetric(totalMilliseconds);
		HandshakeStop(protocol);
	}

	[NonEvent]
	public void ConnectionClosed(SslProtocols protocol)
	{
		switch (protocol)
		{
		case SslProtocols.Tls:
			Interlocked.Decrement(ref _sessionsOpenTls10);
			break;
		case SslProtocols.Tls11:
			Interlocked.Decrement(ref _sessionsOpenTls11);
			break;
		case SslProtocols.Tls12:
			Interlocked.Decrement(ref _sessionsOpenTls12);
			break;
		case SslProtocols.Tls13:
			Interlocked.Decrement(ref _sessionsOpenTls13);
			break;
		}
		Interlocked.Decrement(ref _sessionsOpen);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern", Justification = "Parameters to this method are primitive and are trimmer safe")]
	[NonEvent]
	private unsafe void WriteEvent(int eventId, bool arg1, string arg2)
	{
		if (arg2 == null)
		{
			arg2 = string.Empty;
		}
		fixed (char* dataPointer = arg2)
		{
			EventData* ptr = stackalloc EventData[2];
			*ptr = new EventData
			{
				DataPointer = (nint)(&arg1),
				Size = 4
			};
			ptr[1] = new EventData
			{
				DataPointer = (nint)dataPointer,
				Size = (arg2.Length + 1) * 2
			};
			WriteEventCore(eventId, 2, ptr);
		}
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern", Justification = "Parameters to this method are primitive and are trimmer safe")]
	[NonEvent]
	private unsafe void WriteEvent(int eventId, bool arg1, double arg2, string arg3)
	{
		if (arg3 == null)
		{
			arg3 = string.Empty;
		}
		fixed (char* dataPointer = arg3)
		{
			EventData* ptr = stackalloc EventData[3];
			*ptr = new EventData
			{
				DataPointer = (nint)(&arg1),
				Size = 4
			};
			ptr[1] = new EventData
			{
				DataPointer = (nint)(&arg2),
				Size = 8
			};
			ptr[2] = new EventData
			{
				DataPointer = (nint)dataPointer,
				Size = (arg3.Length + 1) * 2
			};
			WriteEventCore(eventId, 3, ptr);
		}
	}

	[NonEvent]
	public static Activity StartActivity(SslStream stream)
	{
		using Activity activity = s_activitySource.StartActivity("Experimental.System.Net.Security.TlsHandshake");
		if (activity != null)
		{
			activity.DisplayName = (stream.IsServer ? "TLS server handshake" : ("TLS client handshake " + stream.TargetHostName));
			if (activity.IsAllDataRequested && !stream.IsServer)
			{
				activity.SetTag("server.address", stream.TargetHostName);
			}
		}
		return activity;
	}

	[NonEvent]
	public static void StopActivity(Activity activity, Exception exception, SslStream stream)
	{
		if (activity != null && activity.IsAllDataRequested)
		{
			var (text, value) = GetNameAndVersionString(stream.GetSslProtocolInternal());
			if (text != null)
			{
				activity.SetTag("tls.protocol.name", text);
				activity.SetTag("tls.protocol.version", value);
			}
			if (exception != null)
			{
				activity.SetStatus(ActivityStatusCode.Error);
				activity.SetTag("error.type", exception.GetType().FullName);
			}
		}
		static (string, string) GetNameAndVersionString(SslProtocols protocol)
		{
			return protocol switch
			{
				SslProtocols.Ssl2 => ("ssl", "2"), 
				SslProtocols.Ssl3 => ("ssl", "3"), 
				SslProtocols.Tls => ("tls", "1"), 
				SslProtocols.Tls12 => ("tls", "1.2"), 
				SslProtocols.Tls13 => ("tls", "1.3"), 
				_ => (null, null), 
			};
		}
	}
}
