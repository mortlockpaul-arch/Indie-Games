using System.Threading;

namespace System.Net.WebSockets;

public sealed class WebSocketCreationOptions
{
	private string _subProtocol;

	private TimeSpan _keepAliveInterval = WebSocketDefaults.DefaultKeepAliveInterval;

	private TimeSpan _keepAliveTimeout = WebSocketDefaults.DefaultKeepAliveTimeout;

	public bool IsServer { get; set; }

	public string? SubProtocol
	{
		get
		{
			return _subProtocol;
		}
		set
		{
			if (value != null)
			{
				WebSocketValidate.ValidateSubprotocol(value);
			}
			_subProtocol = value;
		}
	}

	public TimeSpan KeepAliveInterval
	{
		get
		{
			return _keepAliveInterval;
		}
		set
		{
			if (value != Timeout.InfiniteTimeSpan && value < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("KeepAliveInterval", value, System.SR.Format(System.SR.net_WebSockets_ArgumentOutOfRange_TooSmall, 0));
			}
			_keepAliveInterval = value;
		}
	}

	public TimeSpan KeepAliveTimeout
	{
		get
		{
			return _keepAliveTimeout;
		}
		set
		{
			if (value != Timeout.InfiniteTimeSpan && value < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("KeepAliveTimeout", value, System.SR.Format(System.SR.net_WebSockets_ArgumentOutOfRange_TooSmall, 0));
			}
			_keepAliveTimeout = value;
		}
	}

	public WebSocketDeflateOptions? DangerousDeflateOptions { get; set; }
}
