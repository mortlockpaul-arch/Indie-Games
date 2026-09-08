using System.Threading;

namespace System.Net.Quic;

public abstract class QuicConnectionOptions
{
	internal QuicReceiveWindowSizes _initialReceiveWindowSizes;

	public int MaxInboundBidirectionalStreams { get; set; }

	public int MaxInboundUnidirectionalStreams { get; set; }

	public TimeSpan IdleTimeout { get; set; } = TimeSpan.Zero;

	public long DefaultStreamErrorCode { get; set; } = -1L;

	public long DefaultCloseErrorCode { get; set; } = -1L;

	public QuicReceiveWindowSizes InitialReceiveWindowSizes
	{
		get
		{
			return _initialReceiveWindowSizes ?? (_initialReceiveWindowSizes = new QuicReceiveWindowSizes());
		}
		set
		{
			_initialReceiveWindowSizes = value;
		}
	}

	public TimeSpan KeepAliveInterval { get; set; } = Timeout.InfiniteTimeSpan;

	public TimeSpan HandshakeTimeout { get; set; } = QuicDefaults.HandshakeTimeout;

	public Action<QuicConnection, QuicStreamCapacityChangedArgs>? StreamCapacityCallback { get; set; }

	internal QuicConnectionOptions()
	{
	}

	internal virtual void Validate(string argumentName)
	{
		ThrowHelper.ValidateInRange(argumentName, MaxInboundBidirectionalStreams, 65535L, "MaxInboundBidirectionalStreams");
		ThrowHelper.ValidateInRange(argumentName, MaxInboundUnidirectionalStreams, 65535L, "MaxInboundUnidirectionalStreams");
		ThrowHelper.ValidateTimeSpan(argumentName, IdleTimeout, "IdleTimeout");
		ThrowHelper.ValidateTimeSpan(argumentName, KeepAliveInterval, "KeepAliveInterval");
		ThrowHelper.ValidateErrorCode(argumentName, DefaultCloseErrorCode, "DefaultCloseErrorCode");
		ThrowHelper.ValidateErrorCode(argumentName, DefaultStreamErrorCode, "DefaultStreamErrorCode");
		ThrowHelper.ValidateTimeSpan(argumentName, HandshakeTimeout, "HandshakeTimeout");
		_initialReceiveWindowSizes?.Validate(argumentName);
	}
}
