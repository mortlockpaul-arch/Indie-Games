using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates.Asn1;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates;

public class X509Certificate2Collection : X509CertificateCollection, IEnumerable<X509Certificate2>, IEnumerable
{
	public new X509Certificate2 this[int index]
	{
		get
		{
			return (X509Certificate2)base[index];
		}
		set
		{
			base[index] = value;
		}
	}

	public X509Certificate2Collection()
	{
	}

	public X509Certificate2Collection(X509Certificate2 certificate)
	{
		Add(certificate);
	}

	public X509Certificate2Collection(X509Certificate2[] certificates)
	{
		AddRange(certificates);
	}

	public X509Certificate2Collection(X509Certificate2Collection certificates)
	{
		AddRange(certificates);
	}

	public int Add(X509Certificate2 certificate)
	{
		ArgumentNullException.ThrowIfNull(certificate, "certificate");
		return Add((X509Certificate)certificate);
	}

	public void AddRange(X509Certificate2[] certificates)
	{
		ArgumentNullException.ThrowIfNull(certificates, "certificates");
		int i = 0;
		try
		{
			for (; i < certificates.Length; i++)
			{
				Add(certificates[i]);
			}
		}
		catch
		{
			for (int j = 0; j < i; j++)
			{
				Remove(certificates[j]);
			}
			throw;
		}
	}

	public void AddRange(X509Certificate2Collection certificates)
	{
		ArgumentNullException.ThrowIfNull(certificates, "certificates");
		int i = 0;
		try
		{
			for (; i < certificates.Count; i++)
			{
				Add(certificates[i]);
			}
		}
		catch
		{
			for (int j = 0; j < i; j++)
			{
				Remove(certificates[j]);
			}
			throw;
		}
	}

	public bool Contains(X509Certificate2 certificate)
	{
		return Contains((X509Certificate)certificate);
	}

	public byte[]? Export(X509ContentType contentType)
	{
		using IExportPal exportPal = StorePal.LinkFromCertificateCollection(this);
		return exportPal.Export(contentType, SafePasswordHandle.InvalidHandle);
	}

	public byte[] ExportPkcs12(Pkcs12ExportPbeParameters exportParameters, string? password)
	{
		Helpers.ThrowIfInvalidPkcs12ExportParameters(exportParameters);
		Helpers.ThrowIfPasswordContainsNullCharacter(password);
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		using IExportPal exportPal = StorePal.LinkFromCertificateCollection(this);
		return exportPal.ExportPkcs12(exportParameters, password2);
	}

	public byte[] ExportPkcs12(PbeParameters exportParameters, string? password)
	{
		ArgumentNullException.ThrowIfNull(exportParameters, "exportParameters");
		Helpers.ThrowIfInvalidPkcs12ExportParameters(exportParameters);
		Helpers.ThrowIfPasswordContainsNullCharacter(password);
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		using IExportPal exportPal = StorePal.LinkFromCertificateCollection(this);
		return exportPal.ExportPkcs12(exportParameters, password2);
	}

	public byte[]? Export(X509ContentType contentType, string? password)
	{
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		using IExportPal exportPal = StorePal.LinkFromCertificateCollection(this);
		return exportPal.Export(contentType, password2);
	}

	public X509Certificate2Collection Find(X509FindType findType, object findValue, bool validOnly)
	{
		ArgumentNullException.ThrowIfNull(findValue, "findValue");
		return FindPal.FindFromCollection(this, findType, findValue, validOnly);
	}

	public new X509Certificate2Enumerator GetEnumerator()
	{
		return new X509Certificate2Enumerator(this);
	}

	IEnumerator<X509Certificate2> IEnumerable<X509Certificate2>.GetEnumerator()
	{
		return GetEnumerator();
	}

	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void Import(byte[] rawData)
	{
		ArgumentNullException.ThrowIfNull(rawData, "rawData");
		Import(rawData.AsSpan());
	}

	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void Import(ReadOnlySpan<byte> rawData)
	{
		using ILoaderPal loaderPal = StorePal.FromBlob(rawData, SafePasswordHandle.InvalidHandle, X509KeyStorageFlags.DefaultKeySet);
		loaderPal.MoveTo(this);
	}

	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void Import(byte[] rawData, string? password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
	{
		ArgumentNullException.ThrowIfNull(rawData, "rawData");
		Import(rawData.AsSpan(), password.AsSpan(), keyStorageFlags);
	}

	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void Import(ReadOnlySpan<byte> rawData, string? password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
	{
		Import(rawData, password.AsSpan(), keyStorageFlags);
	}

	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void Import(ReadOnlySpan<byte> rawData, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
	{
		X509Certificate.ValidateKeyStorageFlags(keyStorageFlags);
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		using ILoaderPal loaderPal = StorePal.FromBlob(rawData, password2, keyStorageFlags);
		loaderPal.MoveTo(this);
	}

	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void Import(string fileName)
	{
		ArgumentNullException.ThrowIfNull(fileName, "fileName");
		using ILoaderPal loaderPal = StorePal.FromFile(fileName, SafePasswordHandle.InvalidHandle, X509KeyStorageFlags.DefaultKeySet);
		loaderPal.MoveTo(this);
	}

	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void Import(string fileName, string? password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
	{
		ArgumentNullException.ThrowIfNull(fileName, "fileName");
		X509Certificate.ValidateKeyStorageFlags(keyStorageFlags);
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		using ILoaderPal loaderPal = StorePal.FromFile(fileName, password2, keyStorageFlags);
		loaderPal.MoveTo(this);
	}

	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void Import(string fileName, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
	{
		ArgumentNullException.ThrowIfNull(fileName, "fileName");
		X509Certificate.ValidateKeyStorageFlags(keyStorageFlags);
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		using ILoaderPal loaderPal = StorePal.FromFile(fileName, password2, keyStorageFlags);
		loaderPal.MoveTo(this);
	}

	public void Insert(int index, X509Certificate2 certificate)
	{
		ArgumentNullException.ThrowIfNull(certificate, "certificate");
		Insert(index, (X509Certificate)certificate);
	}

	public void Remove(X509Certificate2 certificate)
	{
		ArgumentNullException.ThrowIfNull(certificate, "certificate");
		Remove((X509Certificate)certificate);
	}

	public void RemoveRange(X509Certificate2[] certificates)
	{
		ArgumentNullException.ThrowIfNull(certificates, "certificates");
		int i = 0;
		try
		{
			for (; i < certificates.Length; i++)
			{
				Remove(certificates[i]);
			}
		}
		catch
		{
			for (int j = 0; j < i; j++)
			{
				Add(certificates[j]);
			}
			throw;
		}
	}

	public void RemoveRange(X509Certificate2Collection certificates)
	{
		ArgumentNullException.ThrowIfNull(certificates, "certificates");
		int i = 0;
		try
		{
			for (; i < certificates.Count; i++)
			{
				Remove(certificates[i]);
			}
		}
		catch
		{
			for (int j = 0; j < i; j++)
			{
				Add(certificates[j]);
			}
			throw;
		}
	}

	public void ImportFromPemFile(string certPemFilePath)
	{
		ArgumentNullException.ThrowIfNull(certPemFilePath, "certPemFilePath");
		ReadOnlySpan<char> certPem = File.ReadAllText(certPemFilePath).AsSpan();
		ImportFromPem(certPem);
	}

	public void ImportFromPem(ReadOnlySpan<char> certPem)
	{
		int num = 0;
		try
		{
			PemEnumerator<char>.Enumerator enumerator = PemEnumerator.Utf16(certPem).GetEnumerator();
			while (enumerator.MoveNext())
			{
				var (readOnlySpan2, pemFields2) = enumerator.Current;
				Range label = pemFields2.Label;
				if (readOnlySpan2[label.Start..label.End].SequenceEqual("CERTIFICATE".AsSpan()))
				{
					byte[] array = GC.AllocateUninitializedArray<byte>(pemFields2.DecodedDataLength);
					label = pemFields2.Base64Data;
					if (!Convert.TryFromBase64Chars(readOnlySpan2[label.Start..label.End], array, out var bytesWritten) || bytesWritten != pemFields2.DecodedDataLength)
					{
						throw new CryptographicException(System.SR.Cryptography_X509_NoPemCertificate);
					}
					try
					{
						CertificateAsn.Decode(array, AsnEncodingRules.DER);
					}
					catch (CryptographicException)
					{
						throw new CryptographicException(System.SR.Cryptography_X509_NoPemCertificate);
					}
					Add(X509CertificateLoader.LoadCertificate(array));
					num++;
				}
			}
		}
		catch
		{
			for (int i = 0; i < num; i++)
			{
				RemoveAt(base.Count - 1);
			}
			throw;
		}
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("tvos")]
	public string ExportPkcs7Pem()
	{
		byte[] array = Export(X509ContentType.Pkcs7);
		if (array == null)
		{
			throw new CryptographicException(System.SR.Cryptography_X509_ExportFailed);
		}
		return PemEncoding.WriteString("PKCS7".AsSpan(), array);
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("tvos")]
	public bool TryExportPkcs7Pem(Span<char> destination, out int charsWritten)
	{
		byte[] array = Export(X509ContentType.Pkcs7);
		if (array == null)
		{
			throw new CryptographicException(System.SR.Cryptography_X509_ExportFailed);
		}
		return PemEncoding.TryWrite("PKCS7".AsSpan(), array, destination, out charsWritten);
	}

	public string ExportCertificatePems()
	{
		return string.Create(GetCertificatePemsSize(), this, delegate(Span<char> destination, X509Certificate2Collection col)
		{
			if (!col.TryExportCertificatePems(destination, out var charsWritten) || charsWritten != destination.Length)
			{
				throw new CryptographicException();
			}
		});
	}

	public bool TryExportCertificatePems(Span<char> destination, out int charsWritten)
	{
		Span<char> destination2 = destination;
		int num = 0;
		for (int i = 0; i < base.Count; i++)
		{
			ReadOnlyMemory<byte> rawDataMemory = this[i].RawDataMemory;
			int encodedSize = PemEncoding.GetEncodedSize("CERTIFICATE".Length, rawDataMemory.Length);
			if (destination2.Length < encodedSize)
			{
				charsWritten = 0;
				return false;
			}
			if (!PemEncoding.TryWrite("CERTIFICATE".AsSpan(), rawDataMemory.Span, destination2, out var charsWritten2) || charsWritten2 != encodedSize)
			{
				throw new CryptographicException();
			}
			destination2 = destination2.Slice(charsWritten2);
			num += charsWritten2;
			if (i < base.Count - 1)
			{
				if (destination2.IsEmpty)
				{
					charsWritten = 0;
					return false;
				}
				destination2[0] = '\n';
				destination2 = destination2.Slice(1);
				num++;
			}
		}
		charsWritten = num;
		return true;
	}

	public X509Certificate2Collection FindByThumbprint(HashAlgorithmName hashAlgorithm, string thumbprintHex)
	{
		ArgumentNullException.ThrowIfNull(thumbprintHex, "thumbprintHex");
		return FindByThumbprint(hashAlgorithm, thumbprintHex.AsSpan());
	}

	public X509Certificate2Collection FindByThumbprint(HashAlgorithmName hashAlgorithm, ReadOnlySpan<char> thumbprintHex)
	{
		ArgumentException.ThrowIfNullOrEmpty(hashAlgorithm.Name, "hashAlgorithm");
		int num = checked(thumbprintHex.Length + 1) / 2;
		Span<byte> span = ((num <= 64) ? stackalloc byte[64] : ((Span<byte>)new byte[num]));
		Span<byte> destination = span;
		int charsConsumed;
		int bytesWritten;
		switch (Convert.FromHexString(thumbprintHex, destination, out charsConsumed, out bytesWritten))
		{
		case OperationStatus.NeedMoreData:
		case OperationStatus.InvalidData:
			throw new ArgumentException(System.SR.Argument_Thumbprint_Invalid, "thumbprintHex");
		case OperationStatus.DestinationTooSmall:
			throw new CryptographicException();
		default:
			return FindByThumbprintCore(hashAlgorithm, destination.Slice(0, bytesWritten));
		}
	}

	public X509Certificate2Collection FindByThumbprint(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> thumbprintBytes)
	{
		ArgumentException.ThrowIfNullOrEmpty(hashAlgorithm.Name, "hashAlgorithm");
		return FindByThumbprintCore(hashAlgorithm, thumbprintBytes);
	}

	private X509Certificate2Collection FindByThumbprintCore(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> thumbprintBytes)
	{
		Span<byte> destination = stackalloc byte[64];
		X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
		using X509Certificate2Enumerator x509Certificate2Enumerator = GetEnumerator();
		while (x509Certificate2Enumerator.MoveNext())
		{
			X509Certificate2 current = x509Certificate2Enumerator.Current;
			if (((ReadOnlySpan<byte>)destination[..CryptographicOperations.HashData(hashAlgorithm, current.RawDataMemory.Span, destination)]).SequenceEqual(thumbprintBytes))
			{
				x509Certificate2Collection.Add(current);
			}
		}
		return x509Certificate2Collection;
	}

	private int GetCertificatePemsSize()
	{
		int num = 0;
		checked
		{
			for (int i = 0; i < base.Count; i++)
			{
				num += PemEncoding.GetEncodedSize("CERTIFICATE".Length, this[i].RawDataMemory.Length);
				if (i < base.Count - 1)
				{
					num++;
				}
			}
			return num;
		}
	}
}
