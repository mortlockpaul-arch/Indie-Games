using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading;

namespace System.Net.Sockets;

[EventSource(Name = "System.Net.Sockets")]
internal sealed class SocketsTelemetry : EventSource
{
	private static readonly ActivitySource s_connectActivitySource = new ActivitySource("Experimental.System.Net.Sockets");

	public static readonly SocketsTelemetry Log = new SocketsTelemetry();

	private PollingCounter _currentOutgoingConnectAttemptsCounter;

	private PollingCounter _outgoingConnectionsEstablishedCounter;

	private PollingCounter _incomingConnectionsEstablishedCounter;

	private PollingCounter _bytesReceivedCounter;

	private PollingCounter _bytesSentCounter;

	private PollingCounter _datagramsReceivedCounter;

	private PollingCounter _datagramsSentCounter;

	private long _currentOutgoingConnectAttempts;

	private long _outgoingConnectionsEstablished;

	private long _incomingConnectionsEstablished;

	private long _bytesReceived;

	private long _bytesSent;

	private long _datagramsReceived;

	private long _datagramsSent;

	[Event(1, Level = EventLevel.Informational)]
	private void ConnectStart(string address)
	{
		WriteEvent(1, address);
	}

	[Event(2, Level = EventLevel.Informational)]
	private void ConnectStop()
	{
		if (IsEnabled(EventLevel.Informational, EventKeywords.All))
		{
			WriteEvent(2);
		}
	}

	[Event(3, Level = EventLevel.Error)]
	private void ConnectFailed(SocketError error, string exceptionMessage)
	{
		if (IsEnabled(EventLevel.Error, EventKeywords.All))
		{
			WriteEvent(3, (int)error, exceptionMessage);
		}
	}

	[Event(4, Level = EventLevel.Informational)]
	private void AcceptStart(string address)
	{
		WriteEvent(4, address);
	}

	[Event(5, Level = EventLevel.Informational)]
	private void AcceptStop()
	{
		if (IsEnabled(EventLevel.Informational, EventKeywords.All))
		{
			WriteEvent(5);
		}
	}

	[Event(6, Level = EventLevel.Error)]
	private void AcceptFailed(SocketError error, string exceptionMessage)
	{
		if (IsEnabled(EventLevel.Error, EventKeywords.All))
		{
			WriteEvent(6, (int)error, exceptionMessage);
		}
	}

	[NonEvent]
	public Activity ConnectStart(SocketAddress address, ProtocolType protocolType, EndPoint endPoint, bool keepActivityCurrent)
	{
		Interlocked.Increment(ref _currentOutgoingConnectAttempts);
		if (IsEnabled(EventLevel.Informational, EventKeywords.All))
		{
			ConnectStart(address.ToString());
		}
		Activity activity = null;
		if (s_connectActivitySource.HasListeners())
		{
			Activity current = (keepActivityCurrent ? Activity.Current : null);
			activity = s_connectActivitySource.StartActivity("Experimental.System.Net.Sockets.Connect");
			if (keepActivityCurrent)
			{
				Activity.Current = current;
			}
		}
		if (activity != null)
		{
			if (endPoint is IPEndPoint { Port: var port } iPEndPoint)
			{
				activity.DisplayName = $"socket connect {iPEndPoint.Address}:{port}";
				if (activity.IsAllDataRequested)
				{
					activity.SetTag("network.peer.address", iPEndPoint.Address.ToString());
					activity.SetTag("network.peer.port", port);
					activity.SetTag("network.type", (iPEndPoint.AddressFamily == AddressFamily.InterNetwork) ? "ipv4" : "ipv6");
					switch (protocolType)
					{
					case ProtocolType.Tcp:
						SetNetworkTransport(activity, "tcp");
						break;
					case ProtocolType.Udp:
						SetNetworkTransport(activity, "udp");
						break;
					}
				}
			}
			else if (endPoint is UnixDomainSocketEndPoint unixDomainSocketEndPoint)
			{
				string text = unixDomainSocketEndPoint.ToString();
				activity.DisplayName = "socket connect " + text;
				if (activity.IsAllDataRequested)
				{
					activity.SetTag("network.peer.address", text);
					SetNetworkTransport(activity, "unix");
				}
			}
		}
		return activity;
		static void SetNetworkTransport(Activity activity2, string transportType)
		{
			activity2.SetTag("network.transport", transportType);
		}
	}

	[NonEvent]
	public void AfterConnect(SocketError error, Activity activity, string exceptionMessage = null)
	{
		Interlocked.Decrement(ref _currentOutgoingConnectAttempts);
		if (activity != null)
		{
			if (error != SocketError.Success)
			{
				activity.SetStatus(ActivityStatusCode.Error);
				activity.SetTag("error.type", GetErrorType(error));
			}
			activity.Stop();
		}
		if (error == SocketError.Success)
		{
			Interlocked.Increment(ref _outgoingConnectionsEstablished);
		}
		else
		{
			ConnectFailed(error, exceptionMessage);
		}
		ConnectStop();
	}

	[NonEvent]
	public void AcceptStart(SocketAddress address)
	{
		if (IsEnabled(EventLevel.Informational, EventKeywords.All))
		{
			AcceptStart(address.ToString());
		}
	}

	[NonEvent]
	public void AcceptStart(EndPoint address)
	{
		if (IsEnabled(EventLevel.Informational, EventKeywords.All))
		{
			AcceptStart(address.Serialize().ToString());
		}
	}

	[NonEvent]
	public void AfterAccept(SocketError error, string exceptionMessage = null)
	{
		if (error == SocketError.Success)
		{
			Interlocked.Increment(ref _incomingConnectionsEstablished);
		}
		else
		{
			AcceptFailed(error, exceptionMessage);
		}
		AcceptStop();
	}

	[NonEvent]
	public void BytesReceived(int count)
	{
		Interlocked.Add(ref _bytesReceived, count);
	}

	[NonEvent]
	public void BytesSent(int count)
	{
		Interlocked.Add(ref _bytesSent, count);
	}

	[NonEvent]
	public void DatagramReceived()
	{
		Interlocked.Increment(ref _datagramsReceived);
	}

	[NonEvent]
	public void DatagramSent()
	{
		Interlocked.Increment(ref _datagramsSent);
	}

	private static string GetErrorType(SocketError socketError)
	{
		return socketError switch
		{
			SocketError.NetworkDown => "network_down", 
			SocketError.AddressAlreadyInUse => "address_already_in_use", 
			SocketError.Interrupted => "interrupted", 
			SocketError.InProgress => "in_progress", 
			SocketError.AlreadyInProgress => "already_in_progress", 
			SocketError.AddressNotAvailable => "address_not_available", 
			SocketError.AddressFamilyNotSupported => "address_family_not_supported", 
			SocketError.ConnectionRefused => "connection_refused", 
			SocketError.Fault => "fault", 
			SocketError.InvalidArgument => "invalid_argument", 
			SocketError.IsConnected => "is_connected", 
			SocketError.NetworkUnreachable => "network_unreachable", 
			SocketError.HostUnreachable => "host_unreachable", 
			SocketError.NoBufferSpaceAvailable => "no_buffer_space_available", 
			SocketError.TimedOut => "timed_out", 
			SocketError.AccessDenied => "access_denied", 
			SocketError.ProtocolType => "protocol_type", 
			_ => "_OTHER", 
		};
	}

	protected override void OnEventCommand(EventCommandEventArgs command)
	{
		if (command.Command != EventCommand.Enable)
		{
			return;
		}
		if (_currentOutgoingConnectAttemptsCounter == null)
		{
			_currentOutgoingConnectAttemptsCounter = new PollingCounter("current-outgoing-connect-attempts", this, () => Interlocked.Read(in _currentOutgoingConnectAttempts))
			{
				DisplayName = "Current Outgoing Connect Attempts"
			};
		}
		if (_outgoingConnectionsEstablishedCounter == null)
		{
			_outgoingConnectionsEstablishedCounter = new PollingCounter("outgoing-connections-established", this, () => Interlocked.Read(in _outgoingConnectionsEstablished))
			{
				DisplayName = "Outgoing Connections Established"
			};
		}
		if (_incomingConnectionsEstablishedCounter == null)
		{
			_incomingConnectionsEstablishedCounter = new PollingCounter("incoming-connections-established", this, () => Interlocked.Read(in _incomingConnectionsEstablished))
			{
				DisplayName = "Incoming Connections Established"
			};
		}
		if (_bytesReceivedCounter == null)
		{
			_bytesReceivedCounter = new PollingCounter("bytes-received", this, () => Interlocked.Read(in _bytesReceived))
			{
				DisplayName = "Bytes Received"
			};
		}
		if (_bytesSentCounter == null)
		{
			_bytesSentCounter = new PollingCounter("bytes-sent", this, () => Interlocked.Read(in _bytesSent))
			{
				DisplayName = "Bytes Sent"
			};
		}
		if (_datagramsReceivedCounter == null)
		{
			_datagramsReceivedCounter = new PollingCounter("datagrams-received", this, () => Interlocked.Read(in _datagramsReceived))
			{
				DisplayName = "Datagrams Received"
			};
		}
		if (_datagramsSentCounter == null)
		{
			_datagramsSentCounter = new PollingCounter("datagrams-sent", this, () => Interlocked.Read(in _datagramsSent))
			{
				DisplayName = "Datagrams Sent"
			};
		}
	}
}
