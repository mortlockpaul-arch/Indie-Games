using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography;

internal static class Helpers
{
	public unsafe delegate TResult DecodedObjectReceiver<TResult>(void* pvDecodedObject, int cbDecodedObject);

	public unsafe delegate TResult DecodedObjectReceiver<TState, TResult>(void* pvDecodedObject, int cbDecodedObject, TState state);

	internal static readonly PbeParameters Windows3desPbe = new PbeParameters(PbeEncryptionAlgorithm.TripleDes3KeyPkcs12, HashAlgorithmName.SHA1, 2000);

	internal static readonly PbeParameters WindowsAesPbe = new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, 2000);

	[UnsupportedOSPlatformGuard("browser")]
	[UnsupportedOSPlatformGuard("wasi")]
	internal static bool HasSymmetricEncryption { get; } = !OperatingSystem.IsBrowser() && !OperatingSystem.IsWasi();

	[UnsupportedOSPlatformGuard("ios")]
	[UnsupportedOSPlatformGuard("tvos")]
	public static bool IsDSASupported
	{
		get
		{
			if (!OperatingSystem.IsIOS())
			{
				return !OperatingSystem.IsTvOS();
			}
			return false;
		}
	}

	[UnsupportedOSPlatformGuard("android")]
	[UnsupportedOSPlatformGuard("browser")]
	[UnsupportedOSPlatformGuard("wasi")]
	public static bool IsRC2Supported
	{
		get
		{
			if (!OperatingSystem.IsAndroid())
			{
				return !OperatingSystem.IsBrowser();
			}
			return false;
		}
	}

	[UnsupportedOSPlatformGuard("browser")]
	[UnsupportedOSPlatformGuard("wasi")]
	internal static bool HasMD5 { get; } = !OperatingSystem.IsBrowser() && !OperatingSystem.IsWasi();

	[SupportedOSPlatformGuard("windows")]
	internal static bool IsOSPlatformWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

	[return: NotNullIfNotNull("src")]
	public static byte[] CloneByteArray(this byte[] src)
	{
		if (src != null)
		{
			if (src.Length == 0)
			{
				return src;
			}
			return (byte[])src.Clone();
		}
		return null;
	}

	internal static bool ContainsNull<T>(this ReadOnlySpan<T> span)
	{
		return Unsafe.IsNullRef(in MemoryMarshal.GetReference(span));
	}

	internal static void RngFill(Span<byte> destination)
	{
		RandomNumberGenerator.Fill(destination);
	}

	internal static bool TryCopyToDestination(this ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
	{
		if (source.TryCopyTo(destination))
		{
			bytesWritten = source.Length;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	private static int? TryGetHashOidToByteLength(ReadOnlySpan<char> hashOid)
	{
		return hashOid switch
		{
			"1.2.840.113549.2.5" => 16, 
			"1.3.14.3.2.26" => 20, 
			"2.16.840.1.101.3.4.2.1" => 32, 
			"2.16.840.1.101.3.4.2.2" => 48, 
			"2.16.840.1.101.3.4.2.3" => 64, 
			"2.16.840.1.101.3.4.2.8" => 32, 
			"2.16.840.1.101.3.4.2.9" => 48, 
			"2.16.840.1.101.3.4.2.10" => 64, 
			"2.16.840.1.101.3.4.2.11" => 32, 
			"2.16.840.1.101.3.4.2.12" => 64, 
			_ => null, 
		};
	}

	internal static int HashOidToByteLength(string hashOid)
	{
		return TryGetHashOidToByteLength(hashOid.AsSpan()) ?? throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownHashAlgorithm, hashOid));
	}

	internal static void ValidateHashLength(ReadOnlySpan<byte> hash, ReadOnlySpan<char> hashAlgorithmOid)
	{
		int? num = TryGetHashOidToByteLength(hashAlgorithmOid);
		if (num.HasValue)
		{
			if (hash.Length != num)
			{
				throw new CryptographicException(System.SR.Cryptography_HashLengthMismatch);
			}
			return;
		}
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER, 16);
		try
		{
			asnWriter.WriteObjectIdentifier(hashAlgorithmOid);
		}
		catch (ArgumentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_HashLengthMismatch, inner);
		}
	}

	internal static bool HashAlgorithmRequired(string keyAlgorithm)
	{
		switch (keyAlgorithm)
		{
		case "1.2.840.113549.1.1.1":
		case "1.2.840.113549.1.1.10":
		case "1.2.840.10045.2.1":
		case "1.2.840.10040.4.1":
			return true;
		default:
			return false;
		}
	}

	internal static CryptographicException CreateAlgorithmUnknownException(AsnWriter encodedId)
	{
		return encodedId.Encode<CryptographicException>((Func<ReadOnlySpan<byte>, CryptographicException>)((ReadOnlySpan<byte> encoded) => CreateAlgorithmUnknownException(Convert.ToHexString(encoded))));
	}

	internal static CryptographicException CreateAlgorithmUnknownException(string algorithmId)
	{
		throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownAlgorithmIdentifier, algorithmId));
	}

	internal static string EncodeAsnWriterToPem(string label, AsnWriter writer, bool clear = true)
	{
		return writer.Encode<string, string>(label, (Func<string, ReadOnlySpan<byte>, string>)((string text, ReadOnlySpan<byte> span) => PemEncoding.WriteString(text.AsSpan(), span)));
	}

	internal static void ThrowIfAsnInvalidLength(ReadOnlySpan<byte> data)
	{
		int bytesConsumed;
		try
		{
			AsnDecoder.ReadEncodedValue(data, AsnEncodingRules.BER, out var _, out var _, out bytesConsumed);
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
		if (bytesConsumed != data.Length)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
	}

	internal static void ThrowIfDestinationWrongLength(Span<byte> destination, int expectedLength, [CallerArgumentExpression("destination")] string paramName = null)
	{
		if (destination.Length != expectedLength)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, expectedLength), paramName);
		}
	}

	internal static void AddRange<T>(this ICollection<T> coll, IEnumerable<T> newData)
	{
		foreach (T newDatum in newData)
		{
			coll.Add(newDatum);
		}
	}

	public static bool UsesIv(this CipherMode cipherMode)
	{
		return cipherMode != CipherMode.ECB;
	}

	public static byte[] GetCipherIv(this CipherMode cipherMode, byte[] iv)
	{
		if (cipherMode.UsesIv())
		{
			if (iv == null)
			{
				throw new CryptographicException(System.SR.Cryptography_MissingIV);
			}
			return iv;
		}
		return null;
	}

	public static byte[] FixupKeyParity(this byte[] key)
	{
		byte[] array = new byte[key.Length];
		for (int i = 0; i < key.Length; i++)
		{
			array[i] = (byte)(key[i] & 0xFE);
			byte b = (byte)((array[i] & 0xF) ^ (array[i] >> 4));
			byte b2 = (byte)((b & 3) ^ (b >> 2));
			if ((byte)((b2 & 1) ^ (b2 >> 1)) == 0)
			{
				array[i] |= 1;
			}
		}
		return array;
	}

	internal static char[] ToHexArrayUpper(this byte[] bytes)
	{
		char[] array = new char[bytes.Length * 2];
		System.HexConverter.EncodeToUtf16(bytes, array);
		return array;
	}

	internal static string ToHexStringUpper(this byte[] bytes)
	{
		return Convert.ToHexString(bytes);
	}

	internal static byte[] LaxDecodeHexString(this string hexString)
	{
		int num = 0;
		ReadOnlySpan<char> span = hexString.AsSpan();
		if (span.StartsWith('\u200e'))
		{
			span = span.Slice(1);
		}
		for (int i = 0; i < span.Length; i++)
		{
			if (char.IsWhiteSpace(span[i]))
			{
				num++;
			}
		}
		byte[] array = new byte[(uint)(span.Length - num) / 2u];
		byte b = 0;
		bool flag = false;
		int num2 = 0;
		for (int j = 0; j < span.Length; j++)
		{
			char c = span[j];
			if (!char.IsWhiteSpace(c))
			{
				b <<= 4;
				b |= (byte)System.HexConverter.FromChar(c);
				flag = !flag;
				if (!flag)
				{
					array[num2] = b;
					num2++;
				}
			}
		}
		return array;
	}

	internal static bool ContentsEqual(this byte[] a1, byte[] a2)
	{
		if (a1 == null)
		{
			return a2 == null;
		}
		if (a2 == null || a1.Length != a2.Length)
		{
			return false;
		}
		return ((ReadOnlySpan<byte>)a1.AsSpan()).SequenceEqual((ReadOnlySpan<byte>)a2);
	}

	internal static ReadOnlyMemory<byte> DecodeOctetStringAsMemory(ReadOnlyMemory<byte> encodedOctetString)
	{
		try
		{
			ReadOnlySpan<byte> span = encodedOctetString.Span;
			if (AsnDecoder.TryReadPrimitiveOctetString(span, AsnEncodingRules.BER, out var value, out var bytesConsumed))
			{
				if (bytesConsumed != span.Length)
				{
					throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
				}
				if (span.Overlaps(value, out var elementOffset))
				{
					return encodedOctetString.Slice(elementOffset, value.Length);
				}
			}
			byte[] array = AsnDecoder.ReadOctetString(span, AsnEncodingRules.BER, out bytesConsumed);
			if (bytesConsumed != span.Length)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			return array;
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}

	internal static bool AreSamePublicECParameters(ECParameters aParameters, ECParameters bParameters)
	{
		if (aParameters.Curve.CurveType != bParameters.Curve.CurveType)
		{
			return false;
		}
		if (!aParameters.Q.X.ContentsEqual(bParameters.Q.X) || !aParameters.Q.Y.ContentsEqual(bParameters.Q.Y))
		{
			return false;
		}
		ECCurve curve = aParameters.Curve;
		ECCurve curve2 = bParameters.Curve;
		if (curve.IsNamed)
		{
			if (curve.Oid.Value == curve2.Oid.Value)
			{
				return string.Equals(curve.Oid.FriendlyName, curve2.Oid.FriendlyName, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
		if (!curve.IsExplicit)
		{
			return false;
		}
		if (!curve.G.X.ContentsEqual(curve2.G.X) || !curve.G.Y.ContentsEqual(curve2.G.Y) || !curve.Order.ContentsEqual(curve2.Order) || !curve.A.ContentsEqual(curve2.A) || !curve.B.ContentsEqual(curve2.B))
		{
			return false;
		}
		if (curve.IsPrime)
		{
			return curve.Prime.ContentsEqual(curve2.Prime);
		}
		if (curve.IsCharacteristic2)
		{
			return curve.Polynomial.ContentsEqual(curve2.Polynomial);
		}
		return false;
	}

	internal static bool IsValidDay(this Calendar calendar, int year, int month, int day, int era)
	{
		if (calendar.IsValidMonth(year, month, era) && day >= 1)
		{
			return day <= calendar.GetDaysInMonth(year, month, era);
		}
		return false;
	}

	private static bool IsValidMonth(this Calendar calendar, int year, int month, int era)
	{
		if (calendar.IsValidYear(year) && month >= 1)
		{
			return month <= calendar.GetMonthsInYear(year, era);
		}
		return false;
	}

	private static bool IsValidYear(this Calendar calendar, int year)
	{
		if (year >= calendar.GetYear(calendar.MinSupportedDateTime))
		{
			return year <= calendar.GetYear(calendar.MaxSupportedDateTime);
		}
		return false;
	}

	internal static void ValidateDer(ReadOnlySpan<byte> encodedValue)
	{
		try
		{
			AsnValueReader asnValueReader = new AsnValueReader(encodedValue, AsnEncodingRules.DER);
			while (asnValueReader.HasData)
			{
				Asn1Tag asn1Tag = asnValueReader.PeekTag();
				if (asn1Tag.TagClass == TagClass.Universal)
				{
					switch ((UniversalTagNumber)asn1Tag.TagValue)
					{
					case UniversalTagNumber.External:
					case UniversalTagNumber.Embedded:
					case UniversalTagNumber.Sequence:
					case UniversalTagNumber.Set:
					case UniversalTagNumber.UnrestrictedCharacterString:
						if (!asn1Tag.IsConstructed)
						{
							throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
						}
						break;
					default:
						if (asn1Tag.IsConstructed)
						{
							throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
						}
						break;
					}
				}
				if (asn1Tag.IsConstructed)
				{
					ValidateDer(asnValueReader.PeekContentBytes());
				}
				asnValueReader.ReadEncodedValue();
			}
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}

	public static int GetPaddingSize(this SymmetricAlgorithm algorithm, CipherMode mode, int feedbackSizeInBits)
	{
		return ((mode == CipherMode.CFB) ? feedbackSizeInBits : algorithm.BlockSize) / 8;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static ref readonly byte GetNonNullPinnableReference(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length == 0)
		{
			return ref Unsafe.AsRef<byte>((void*)1);
		}
		return ref MemoryMarshal.GetReference(buffer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static ref byte GetNonNullPinnableReference(Span<byte> buffer)
	{
		if (buffer.Length == 0)
		{
			return ref Unsafe.AsRef<byte>((void*)1);
		}
		return ref MemoryMarshal.GetReference(buffer);
	}

	internal static ReadOnlySpan<byte> ArrayToSpanOrThrow(byte[] arg, [CallerArgumentExpression("arg")] string paramName = null)
	{
		ArgumentNullException.ThrowIfNull(arg, paramName);
		return arg;
	}

	internal static int HashLength(HashAlgorithmName hashAlgorithmName)
	{
		if (hashAlgorithmName == HashAlgorithmName.SHA1)
		{
			return 20;
		}
		if (hashAlgorithmName == HashAlgorithmName.SHA256)
		{
			return 32;
		}
		if (hashAlgorithmName == HashAlgorithmName.SHA384)
		{
			return 48;
		}
		if (hashAlgorithmName == HashAlgorithmName.SHA512)
		{
			return 64;
		}
		if (hashAlgorithmName == HashAlgorithmName.SHA3_256)
		{
			return 32;
		}
		if (hashAlgorithmName == HashAlgorithmName.SHA3_384)
		{
			return 48;
		}
		if (hashAlgorithmName == HashAlgorithmName.SHA3_512)
		{
			return 64;
		}
		if (hashAlgorithmName == HashAlgorithmName.MD5)
		{
			return 16;
		}
		throw new ArgumentOutOfRangeException("hashAlgorithmName");
	}

	internal static void ThrowIfInvalidPkcs12ExportParameters(Pkcs12ExportPbeParameters exportParameters)
	{
		if ((exportParameters < Pkcs12ExportPbeParameters.Default || exportParameters > Pkcs12ExportPbeParameters.Pbes2Aes256Sha256) ? true : false)
		{
			throw new ArgumentOutOfRangeException("exportParameters");
		}
	}

	internal static void ThrowIfInvalidPkcs12ExportParameters(PbeParameters exportParameters)
	{
		PbeEncryptionAlgorithm encryptionAlgorithm = exportParameters.EncryptionAlgorithm;
		if ((uint)(encryptionAlgorithm - 1) <= 2u)
		{
			switch (exportParameters.HashAlgorithm.Name)
			{
			case "SHA256":
				break;
			case "SHA384":
				break;
			case "SHA512":
				break;
			case null:
			case "":
				throw new CryptographicException(System.SR.Cryptography_HashAlgorithmNameNullOrEmpty);
			default:
				throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownAlgorithmIdentifier, exportParameters.HashAlgorithm.Name));
			}
			return;
		}
		if (exportParameters.EncryptionAlgorithm == PbeEncryptionAlgorithm.TripleDes3KeyPkcs12)
		{
			switch (exportParameters.HashAlgorithm.Name)
			{
			case null:
			case "":
				throw new CryptographicException(System.SR.Cryptography_HashAlgorithmNameNullOrEmpty);
			default:
				throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownAlgorithmIdentifier, exportParameters.HashAlgorithm.Name));
			}
		}
		throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownAlgorithmIdentifier, exportParameters.EncryptionAlgorithm));
	}

	internal static void ThrowIfPasswordContainsNullCharacter(string password)
	{
		if (password != null && password.Contains('\0'))
		{
			throw new ArgumentException(System.SR.Argument_PasswordNullChars, "password");
		}
	}

	internal static ReadOnlyMemory<byte>? ToNullableMemory(this byte[] array)
	{
		if (array == null)
		{
			return null;
		}
		return array;
	}

	internal static bool IsSlhDsaOid(string oid)
	{
		return (object)SlhDsaAlgorithm.GetAlgorithmFromOid(oid) != null;
	}

	public unsafe static SafeHandle ToLpstrArray(this OidCollection oids, out int numOids)
	{
		if (oids == null || oids.Count == 0)
		{
			numOids = 0;
			return SafeCrypt32Handle<SafeLocalAllocHandle>.InvalidHandle;
		}
		string[] array = new string[oids.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = oids[i].Value;
		}
		SafeLocalAllocHandle safeLocalAllocHandle;
		checked
		{
			int num = array.Length * sizeof(void*);
			string[] array2 = array;
			foreach (string text in array2)
			{
				num += text.Length + 1;
			}
			safeLocalAllocHandle = SafeLocalAllocHandle.Create(num);
		}
		byte** ptr = (byte**)safeLocalAllocHandle.DangerousGetHandle();
		byte* ptr2 = (byte*)ptr + (nint)array.Length * (nint)sizeof(byte*);
		for (int k = 0; k < array.Length; k++)
		{
			string text2 = array[k];
			ptr[k] = ptr2;
			Encoding.ASCII.GetBytes(text2.AsSpan(), new Span<byte>(ptr2, text2.Length));
			ptr2[text2.Length] = 0;
			ptr2 += text2.Length + 1;
		}
		numOids = array.Length;
		return safeLocalAllocHandle;
	}

	public unsafe static TResult DecodeObject<TResult>(this byte[] encoded, CryptDecodeObjectStructType lpszStructType, DecodedObjectReceiver<TResult> receiver)
	{
		int pcbStructInfo = 0;
		if (!global::Interop.crypt32.CryptDecodeObjectPointer(global::Interop.Crypt32.CertEncodingType.All, lpszStructType, encoded, encoded.Length, global::Interop.Crypt32.CryptDecodeObjectFlags.None, null, ref pcbStructInfo))
		{
			throw Marshal.GetLastPInvokeError().ToCryptographicException();
		}
		int num = 256;
		Span<byte> span = stackalloc byte[num];
		if ((uint)pcbStructInfo > num)
		{
			span = new byte[pcbStructInfo];
		}
		fixed (byte* ptr = span)
		{
			if (!global::Interop.crypt32.CryptDecodeObjectPointer(global::Interop.Crypt32.CertEncodingType.All, lpszStructType, encoded, encoded.Length, global::Interop.Crypt32.CryptDecodeObjectFlags.None, ptr, ref pcbStructInfo))
			{
				throw Marshal.GetLastPInvokeError().ToCryptographicException();
			}
			return receiver(ptr, pcbStructInfo);
		}
	}

	public unsafe static bool DecodeObjectNoThrow<TState, TResult>(this ReadOnlySpan<byte> encoded, CryptDecodeObjectStructType lpszStructType, TState state, DecodedObjectReceiver<TState, TResult> receiver, out TResult result)
	{
		int pcbStructInfo = 0;
		if (!global::Interop.crypt32.CryptDecodeObjectPointer(global::Interop.Crypt32.CertEncodingType.All, lpszStructType, encoded, global::Interop.Crypt32.CryptDecodeObjectFlags.None, null, ref pcbStructInfo))
		{
			result = default(TResult);
			return false;
		}
		Span<byte> span = stackalloc byte[256];
		if ((uint)pcbStructInfo > 256u)
		{
			span = new byte[pcbStructInfo];
		}
		fixed (byte* ptr = span)
		{
			if (!global::Interop.crypt32.CryptDecodeObjectPointer(global::Interop.Crypt32.CertEncodingType.All, lpszStructType, encoded, global::Interop.Crypt32.CryptDecodeObjectFlags.None, ptr, ref pcbStructInfo))
			{
				result = default(TResult);
				return false;
			}
			result = receiver(ptr, pcbStructInfo, state);
		}
		return true;
	}
}
