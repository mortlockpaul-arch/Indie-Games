namespace System.Net.Quic;

public readonly struct QuicStreamCapacityChangedArgs
{
	public int BidirectionalIncrement { get; init; }

	public int UnidirectionalIncrement { get; init; }
}
