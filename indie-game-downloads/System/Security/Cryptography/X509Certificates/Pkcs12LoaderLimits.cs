using System.Runtime.CompilerServices;

namespace System.Security.Cryptography.X509Certificates;

public sealed class Pkcs12LoaderLimits
{
	private bool _isReadOnly;

	private int? _macIterationLimit = 300000;

	private int? _individualKdfIterationLimit = 300000;

	private int? _totalKdfIterationLimit = 1000000;

	private int? _maxKeys = 200;

	private int? _maxCertificates = 200;

	private bool _preserveStorageProvider;

	private bool _preserveKeyName;

	private bool _preserveCertificateAlias;

	private bool _preserveUnknownAttributes;

	private bool _ignorePrivateKeys;

	private bool _ignoreEncryptedAuthSafes;

	private bool _allowDuplicateAttributes;

	public static Pkcs12LoaderLimits Defaults { get; } = MakeReadOnly(new Pkcs12LoaderLimits());

	public static Pkcs12LoaderLimits DangerousNoLimits { get; } = MakeReadOnly(new Pkcs12LoaderLimits
	{
		MacIterationLimit = null,
		IndividualKdfIterationLimit = null,
		TotalKdfIterationLimit = null,
		MaxKeys = null,
		MaxCertificates = null,
		PreserveStorageProvider = true,
		PreserveKeyName = true,
		PreserveCertificateAlias = true,
		PreserveUnknownAttributes = true,
		AllowDuplicateAttributes = true
	});

	public bool IsReadOnly => _isReadOnly;

	public int? MacIterationLimit
	{
		get
		{
			return _macIterationLimit;
		}
		set
		{
			CheckNonNegative(value, "value");
			CheckReadOnly();
			_macIterationLimit = value;
		}
	}

	public int? IndividualKdfIterationLimit
	{
		get
		{
			return _individualKdfIterationLimit;
		}
		set
		{
			CheckNonNegative(value, "value");
			CheckReadOnly();
			_individualKdfIterationLimit = value;
		}
	}

	public int? TotalKdfIterationLimit
	{
		get
		{
			return _totalKdfIterationLimit;
		}
		set
		{
			CheckNonNegative(value, "value");
			CheckReadOnly();
			_totalKdfIterationLimit = value;
		}
	}

	public int? MaxKeys
	{
		get
		{
			return _maxKeys;
		}
		set
		{
			CheckNonNegative(value, "value");
			CheckReadOnly();
			_maxKeys = value;
		}
	}

	public int? MaxCertificates
	{
		get
		{
			return _maxCertificates;
		}
		set
		{
			CheckNonNegative(value, "value");
			CheckReadOnly();
			_maxCertificates = value;
		}
	}

	public bool PreserveStorageProvider
	{
		get
		{
			return _preserveStorageProvider;
		}
		set
		{
			CheckReadOnly();
			_preserveStorageProvider = value;
		}
	}

	public bool PreserveKeyName
	{
		get
		{
			return _preserveKeyName;
		}
		set
		{
			CheckReadOnly();
			_preserveKeyName = value;
		}
	}

	public bool PreserveCertificateAlias
	{
		get
		{
			return _preserveCertificateAlias;
		}
		set
		{
			CheckReadOnly();
			_preserveCertificateAlias = value;
		}
	}

	public bool PreserveUnknownAttributes
	{
		get
		{
			return _preserveUnknownAttributes;
		}
		set
		{
			CheckReadOnly();
			_preserveUnknownAttributes = value;
		}
	}

	public bool IgnorePrivateKeys
	{
		get
		{
			return _ignorePrivateKeys;
		}
		set
		{
			CheckReadOnly();
			_ignorePrivateKeys = value;
		}
	}

	public bool IgnoreEncryptedAuthSafes
	{
		get
		{
			return _ignoreEncryptedAuthSafes;
		}
		set
		{
			CheckReadOnly();
			_ignoreEncryptedAuthSafes = value;
		}
	}

	public bool AllowDuplicateAttributes
	{
		get
		{
			return _allowDuplicateAttributes;
		}
		set
		{
			CheckReadOnly();
			_allowDuplicateAttributes = value;
		}
	}

	public Pkcs12LoaderLimits()
	{
	}

	public Pkcs12LoaderLimits(Pkcs12LoaderLimits copyFrom)
	{
		ArgumentNullException.ThrowIfNull(copyFrom, "copyFrom");
		_macIterationLimit = copyFrom._macIterationLimit;
		_individualKdfIterationLimit = copyFrom._individualKdfIterationLimit;
		_totalKdfIterationLimit = copyFrom._totalKdfIterationLimit;
		_maxKeys = copyFrom._maxKeys;
		_maxCertificates = copyFrom._maxCertificates;
		_preserveStorageProvider = copyFrom._preserveStorageProvider;
		_preserveKeyName = copyFrom._preserveKeyName;
		_preserveCertificateAlias = copyFrom._preserveCertificateAlias;
		_preserveUnknownAttributes = copyFrom._preserveUnknownAttributes;
		_ignorePrivateKeys = copyFrom._ignorePrivateKeys;
		_ignoreEncryptedAuthSafes = copyFrom._ignoreEncryptedAuthSafes;
		_allowDuplicateAttributes = copyFrom._allowDuplicateAttributes;
	}

	public void MakeReadOnly()
	{
		_isReadOnly = true;
	}

	private static Pkcs12LoaderLimits MakeReadOnly(Pkcs12LoaderLimits limits)
	{
		limits.MakeReadOnly();
		return limits;
	}

	private void CheckReadOnly()
	{
		if (_isReadOnly)
		{
			throw new InvalidOperationException(System.SR.Cryptography_X509_PKCS12_LimitsReadOnly);
		}
	}

	private static void CheckNonNegative(int? value, [CallerArgumentExpression("value")] string paramName = null)
	{
		CheckNonNegative(value.GetValueOrDefault(), paramName);
	}

	private static void CheckNonNegative(int value, [CallerArgumentExpression("value")] string paramName = null)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
	}
}
