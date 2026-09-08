using System;

namespace Microsoft.Quic;

internal static class MsQuic
{
	public static int QUIC_STATUS_SUCCESS
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 0;
				}
				return 0;
			}
			return 0;
		}
	}

	public static int QUIC_STATUS_PENDING
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return -2;
				}
				return -2;
			}
			return 459749;
		}
	}

	public static int QUIC_STATUS_CONTINUE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return -1;
				}
				return -1;
			}
			return 459998;
		}
	}

	public static int QUIC_STATUS_OUT_OF_MEMORY
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 12;
				}
				return 12;
			}
			return -2147024882;
		}
	}

	public static int QUIC_STATUS_INVALID_PARAMETER
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 22;
				}
				return 22;
			}
			return -2147024809;
		}
	}

	public static int QUIC_STATUS_INVALID_STATE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 1;
				}
				return 1;
			}
			return -2147019873;
		}
	}

	public static int QUIC_STATUS_NOT_SUPPORTED
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 102;
				}
				return 95;
			}
			return -2147467262;
		}
	}

	public static int QUIC_STATUS_NOT_FOUND
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 2;
				}
				return 2;
			}
			return -2147023728;
		}
	}

	public static int QUIC_STATUS_BUFFER_TOO_SMALL
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 84;
				}
				return 75;
			}
			return -2147024774;
		}
	}

	public static int QUIC_STATUS_HANDSHAKE_FAILURE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 53;
				}
				return 103;
			}
			return -2143223808;
		}
	}

	public static int QUIC_STATUS_ABORTED
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 89;
				}
				return 125;
			}
			return -2147467260;
		}
	}

	public static int QUIC_STATUS_ADDRESS_IN_USE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 48;
				}
				return 98;
			}
			return -2147014848;
		}
	}

	public static int QUIC_STATUS_INVALID_ADDRESS
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 47;
				}
				return 97;
			}
			return -2147014847;
		}
	}

	public static int QUIC_STATUS_CONNECTION_TIMEOUT
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 60;
				}
				return 110;
			}
			return -2143223802;
		}
	}

	public static int QUIC_STATUS_CONNECTION_IDLE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 101;
				}
				return 62;
			}
			return -2143223803;
		}
	}

	public static int QUIC_STATUS_UNREACHABLE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 65;
				}
				return 113;
			}
			return -2147023664;
		}
	}

	public static int QUIC_STATUS_INTERNAL_ERROR
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 5;
				}
				return 5;
			}
			return -2143223805;
		}
	}

	public static int QUIC_STATUS_CONNECTION_REFUSED
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 61;
				}
				return 111;
			}
			return -2147023671;
		}
	}

	public static int QUIC_STATUS_PROTOCOL_ERROR
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 100;
				}
				return 71;
			}
			return -2143223804;
		}
	}

	public static int QUIC_STATUS_VER_NEG_ERROR
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 43;
				}
				return 93;
			}
			return -2143223807;
		}
	}

	public static int QUIC_STATUS_TLS_ERROR
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 126;
				}
				return 126;
			}
			return -2147013864;
		}
	}

	public static int QUIC_STATUS_USER_CANCELED
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 105;
				}
				return 130;
			}
			return -2143223806;
		}
	}

	public static int QUIC_STATUS_ALPN_NEG_FAILURE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 42;
				}
				return 92;
			}
			return -2143223801;
		}
	}

	public static int QUIC_STATUS_STREAM_LIMIT_REACHED
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 86;
				}
				return 86;
			}
			return -2143223800;
		}
	}

	public static int QUIC_STATUS_ALPN_IN_USE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 41;
				}
				return 91;
			}
			return -2143223799;
		}
	}

	public static int QUIC_STATUS_CLOSE_NOTIFY
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 200000256;
				}
				return 200000256;
			}
			return -2143223552;
		}
	}

	public static int QUIC_STATUS_BAD_CERTIFICATE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 200000298;
				}
				return 200000298;
			}
			return -2143223510;
		}
	}

	public static int QUIC_STATUS_UNSUPPORTED_CERTIFICATE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 200000299;
				}
				return 200000299;
			}
			return -2143223509;
		}
	}

	public static int QUIC_STATUS_REVOKED_CERTIFICATE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 200000300;
				}
				return 200000300;
			}
			return -2143223508;
		}
	}

	public static int QUIC_STATUS_EXPIRED_CERTIFICATE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 200000301;
				}
				return 200000301;
			}
			return -2143223507;
		}
	}

	public static int QUIC_STATUS_UNKNOWN_CERTIFICATE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 200000302;
				}
				return 200000302;
			}
			return -2143223506;
		}
	}

	public static int QUIC_STATUS_REQUIRED_CERTIFICATE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 200000372;
				}
				return 200000372;
			}
			return -2143223436;
		}
	}

	public static int QUIC_STATUS_CERT_EXPIRED
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 200000513;
				}
				return 200000513;
			}
			return -2146762495;
		}
	}

	public static int QUIC_STATUS_CERT_UNTRUSTED_ROOT
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 200000514;
				}
				return 200000514;
			}
			return -2146762487;
		}
	}

	public static int QUIC_STATUS_CERT_NO_CERT
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 200000515;
				}
				return 200000515;
			}
			return -2146893042;
		}
	}

	public static int QUIC_STATUS_ADDRESS_NOT_AVAILABLE
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 49;
				}
				return 99;
			}
			return -2147014849;
		}
	}

	public static int QUIC_ADDRESS_FAMILY_UNSPEC
	{
		get
		{
			if (!OperatingSystem.IsWindows())
			{
				if (!OperatingSystem.IsLinux() && !OperatingSystem.IsAndroid())
				{
					return 0;
				}
				return 0;
			}
			return 0;
		}
	}

	public static bool StatusSucceeded(int status)
	{
		if (OperatingSystem.IsWindows())
		{
			return status >= 0;
		}
		return status <= 0;
	}

	public static bool StatusFailed(int status)
	{
		if (OperatingSystem.IsWindows())
		{
			return status < 0;
		}
		return status > 0;
	}
}
