using System;
using System.Runtime.InteropServices;

namespace Microsoft.Quic;

internal struct QUIC_SETTINGS : IEquatable<QUIC_SETTINGS>
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct _Anonymous1_e__Union
	{
		internal struct _IsSet_e__Struct
		{
			internal ulong _bitfield;

			internal ulong HandshakeIdleTimeoutMs
			{
				set
				{
					_bitfield = (_bitfield & 0xFFFFFFFFFFFFFFFDuL) | ((value & 1) << 1);
				}
			}

			internal ulong IdleTimeoutMs
			{
				set
				{
					_bitfield = (_bitfield & 0xFFFFFFFFFFFFFFFBuL) | ((value & 1) << 2);
				}
			}

			internal ulong ConnFlowControlWindow
			{
				set
				{
					_bitfield = (_bitfield & 0xFFFFFFFFFFFFFEFFuL) | ((value & 1) << 8);
				}
			}

			internal ulong KeepAliveIntervalMs
			{
				set
				{
					_bitfield = (_bitfield & 0xFFFFFFFFFFFEFFFFuL) | ((value & 1) << 16);
				}
			}

			internal ulong PeerBidiStreamCount
			{
				set
				{
					_bitfield = (_bitfield & 0xFFFFFFFFFFFBFFFFuL) | ((value & 1) << 18);
				}
			}

			internal ulong PeerUnidiStreamCount
			{
				set
				{
					_bitfield = (_bitfield & 0xFFFFFFFFFFF7FFFFuL) | ((value & 1) << 19);
				}
			}

			internal ulong StreamRecvWindowBidiLocalDefault
			{
				set
				{
					_bitfield = (_bitfield & 0xFFFFFFF7FFFFFFFFuL) | ((value & 1) << 35);
				}
			}

			internal ulong StreamRecvWindowBidiRemoteDefault
			{
				set
				{
					_bitfield = (_bitfield & 0xFFFFFFEFFFFFFFFFuL) | ((value & 1) << 36);
				}
			}

			internal ulong StreamRecvWindowUnidiDefault
			{
				set
				{
					_bitfield = (_bitfield & 0xFFFFFFDFFFFFFFFFuL) | ((value & 1) << 37);
				}
			}
		}

		[FieldOffset(0)]
		internal ulong IsSetFlags;

		[FieldOffset(0)]
		internal _IsSet_e__Struct IsSet;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct _Anonymous2_e__Union
	{
		internal struct _Anonymous_e__Struct
		{
			internal ulong _bitfield;
		}

		[FieldOffset(0)]
		internal ulong Flags;

		[FieldOffset(0)]
		internal _Anonymous_e__Struct Anonymous;
	}

	internal _Anonymous1_e__Union Anonymous1;

	internal ulong MaxBytesPerKey;

	internal ulong HandshakeIdleTimeoutMs;

	internal ulong IdleTimeoutMs;

	internal ulong MtuDiscoverySearchCompleteTimeoutUs;

	internal uint TlsClientMaxSendBuffer;

	internal uint TlsServerMaxSendBuffer;

	internal uint StreamRecvWindowDefault;

	internal uint StreamRecvBufferDefault;

	internal uint ConnFlowControlWindow;

	internal uint MaxWorkerQueueDelayUs;

	internal uint MaxStatelessOperations;

	internal uint InitialWindowPackets;

	internal uint SendIdleTimeoutMs;

	internal uint InitialRttMs;

	internal uint MaxAckDelayMs;

	internal uint DisconnectTimeoutMs;

	internal uint KeepAliveIntervalMs;

	internal ushort CongestionControlAlgorithm;

	internal ushort PeerBidiStreamCount;

	internal ushort PeerUnidiStreamCount;

	internal ushort MaxBindingStatelessOperations;

	internal ushort StatelessOperationExpirationMs;

	internal ushort MinimumMtu;

	internal ushort MaximumMtu;

	internal byte _bitfield;

	internal byte MaxOperationsPerDrain;

	internal byte MtuDiscoveryMissingProbeCount;

	internal uint DestCidUpdateIdleTimeoutMs;

	internal _Anonymous2_e__Union Anonymous2;

	internal uint StreamRecvWindowBidiLocalDefault;

	internal uint StreamRecvWindowBidiRemoteDefault;

	internal uint StreamRecvWindowUnidiDefault;

	internal ref _Anonymous1_e__Union._IsSet_e__Struct IsSet => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous1.IsSet, 1));

	public readonly bool Equals(QUIC_SETTINGS other)
	{
		if (Anonymous1.IsSetFlags == other.Anonymous1.IsSetFlags && MaxBytesPerKey == other.MaxBytesPerKey && HandshakeIdleTimeoutMs == other.HandshakeIdleTimeoutMs && IdleTimeoutMs == other.IdleTimeoutMs && MtuDiscoverySearchCompleteTimeoutUs == other.MtuDiscoverySearchCompleteTimeoutUs && TlsClientMaxSendBuffer == other.TlsClientMaxSendBuffer && TlsServerMaxSendBuffer == other.TlsServerMaxSendBuffer && StreamRecvWindowDefault == other.StreamRecvWindowDefault && StreamRecvBufferDefault == other.StreamRecvBufferDefault && ConnFlowControlWindow == other.ConnFlowControlWindow && MaxWorkerQueueDelayUs == other.MaxWorkerQueueDelayUs && MaxStatelessOperations == other.MaxStatelessOperations && InitialWindowPackets == other.InitialWindowPackets && SendIdleTimeoutMs == other.SendIdleTimeoutMs && InitialRttMs == other.InitialRttMs && MaxAckDelayMs == other.MaxAckDelayMs && DisconnectTimeoutMs == other.DisconnectTimeoutMs && KeepAliveIntervalMs == other.KeepAliveIntervalMs && CongestionControlAlgorithm == other.CongestionControlAlgorithm && PeerBidiStreamCount == other.PeerBidiStreamCount && PeerUnidiStreamCount == other.PeerUnidiStreamCount && MaxBindingStatelessOperations == other.MaxBindingStatelessOperations && StatelessOperationExpirationMs == other.StatelessOperationExpirationMs && MinimumMtu == other.MinimumMtu && MaximumMtu == other.MaximumMtu && _bitfield == other._bitfield && MaxOperationsPerDrain == other.MaxOperationsPerDrain && MtuDiscoveryMissingProbeCount == other.MtuDiscoveryMissingProbeCount && DestCidUpdateIdleTimeoutMs == other.DestCidUpdateIdleTimeoutMs && Anonymous2.Flags == other.Anonymous2.Flags && StreamRecvWindowBidiLocalDefault == other.StreamRecvWindowBidiLocalDefault && StreamRecvWindowBidiRemoteDefault == other.StreamRecvWindowBidiRemoteDefault)
		{
			return StreamRecvWindowUnidiDefault == other.StreamRecvWindowUnidiDefault;
		}
		return false;
	}

	public override readonly int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		hashCode.Add(Anonymous1.IsSetFlags);
		hashCode.Add(MaxBytesPerKey);
		hashCode.Add(HandshakeIdleTimeoutMs);
		hashCode.Add(IdleTimeoutMs);
		hashCode.Add(MtuDiscoverySearchCompleteTimeoutUs);
		hashCode.Add(TlsClientMaxSendBuffer);
		hashCode.Add(TlsServerMaxSendBuffer);
		hashCode.Add(StreamRecvWindowDefault);
		hashCode.Add(StreamRecvBufferDefault);
		hashCode.Add(ConnFlowControlWindow);
		hashCode.Add(MaxWorkerQueueDelayUs);
		hashCode.Add(MaxStatelessOperations);
		hashCode.Add(InitialWindowPackets);
		hashCode.Add(SendIdleTimeoutMs);
		hashCode.Add(InitialRttMs);
		hashCode.Add(MaxAckDelayMs);
		hashCode.Add(DisconnectTimeoutMs);
		hashCode.Add(KeepAliveIntervalMs);
		hashCode.Add(CongestionControlAlgorithm);
		hashCode.Add(PeerBidiStreamCount);
		hashCode.Add(PeerUnidiStreamCount);
		hashCode.Add(MaxBindingStatelessOperations);
		hashCode.Add(StatelessOperationExpirationMs);
		hashCode.Add(MinimumMtu);
		hashCode.Add(MaximumMtu);
		hashCode.Add(_bitfield);
		hashCode.Add(MaxOperationsPerDrain);
		hashCode.Add(MtuDiscoveryMissingProbeCount);
		hashCode.Add(DestCidUpdateIdleTimeoutMs);
		hashCode.Add(Anonymous2.Flags);
		hashCode.Add(StreamRecvWindowBidiLocalDefault);
		hashCode.Add(StreamRecvWindowBidiRemoteDefault);
		hashCode.Add(StreamRecvWindowUnidiDefault);
		return hashCode.ToHashCode();
	}

	public override readonly bool Equals(object obj)
	{
		if (obj is QUIC_SETTINGS other)
		{
			return Equals(other);
		}
		return false;
	}
}
