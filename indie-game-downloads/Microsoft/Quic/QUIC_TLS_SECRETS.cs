namespace Microsoft.Quic;

internal struct QUIC_TLS_SECRETS
{
	internal struct _IsSet_e__Struct
	{
		internal byte _bitfield;

		internal byte ClientRandom => (byte)(_bitfield & 1);

		internal byte ClientEarlyTrafficSecret
		{
			get
			{
				return (byte)((ulong)(_bitfield >> 1) & 1uL);
			}
			set
			{
				_bitfield = (byte)((_bitfield & -3) | ((value & 1) << 1));
			}
		}

		internal byte ClientHandshakeTrafficSecret
		{
			get
			{
				return (byte)((ulong)(_bitfield >> 2) & 1uL);
			}
			set
			{
				_bitfield = (byte)((_bitfield & -5) | ((value & 1) << 2));
			}
		}

		internal byte ServerHandshakeTrafficSecret
		{
			get
			{
				return (byte)((ulong)(_bitfield >> 3) & 1uL);
			}
			set
			{
				_bitfield = (byte)((_bitfield & -9) | ((value & 1) << 3));
			}
		}

		internal byte ClientTrafficSecret0
		{
			get
			{
				return (byte)((ulong)(_bitfield >> 4) & 1uL);
			}
			set
			{
				_bitfield = (byte)((_bitfield & -17) | ((value & 1) << 4));
			}
		}

		internal byte ServerTrafficSecret0
		{
			get
			{
				return (byte)((ulong)(_bitfield >> 5) & 1uL);
			}
			set
			{
				_bitfield = (byte)((_bitfield & -33) | ((value & 1) << 5));
			}
		}
	}

	internal byte SecretLength;

	internal _IsSet_e__Struct IsSet;

	internal unsafe fixed byte ClientRandom[32];

	internal unsafe fixed byte ClientEarlyTrafficSecret[64];

	internal unsafe fixed byte ClientHandshakeTrafficSecret[64];

	internal unsafe fixed byte ServerHandshakeTrafficSecret[64];

	internal unsafe fixed byte ClientTrafficSecret0[64];

	internal unsafe fixed byte ServerTrafficSecret0[64];
}
