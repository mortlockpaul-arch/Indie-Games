using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Asn1.Pkcs12;
using System.Security.Cryptography.Asn1.Pkcs7;
using System.Security.Cryptography.Pkcs;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates;

[UnsupportedOSPlatform("browser")]
public static class X509CertificateLoader
{
	private delegate T LoadFromFileFunc<T>(ReadOnlyMemory<byte> data, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags, Pkcs12LoaderLimits loaderLimits);

	private readonly struct Pkcs12Return
	{
		private readonly ICertificatePal _pal;

		internal bool HasValue()
		{
			return _pal != null;
		}

		internal X509Certificate2 ToCertificate()
		{
			return new X509Certificate2(_pal);
		}

		internal Pkcs12Return(ICertificatePal pal)
		{
			_pal = pal;
		}

		internal ICertificatePal GetPal()
		{
			return _pal;
		}
	}

	private enum CngMachineKeyState
	{
		Unknown,
		Permitted,
		Denied
	}

	private struct BagState
	{
		private SafeBagAsn[] _certBags;

		private SafeBagAsn[] _keyBags;

		private byte[] _decryptBuffer;

		private byte[] _keyDecryptBuffer;

		private int _certCount;

		private int _keyCount;

		private int _decryptBufferOffset;

		private int _keyDecryptBufferOffset;

		private byte _passwordState;

		private X509KeyStorageFlags _storageFlags;

		internal readonly X509KeyStorageFlags StorageFlags => _storageFlags;

		public readonly int CertCount => _certCount;

		public readonly int KeyCount => _keyCount;

		internal bool LockedPassword => (_passwordState & 1) != 0;

		internal bool ConfirmedPassword => (_passwordState & 2) != 0;

		internal void SetStorageFlags(X509KeyStorageFlags storageFlags)
		{
			_storageFlags = storageFlags & (X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet);
		}

		internal void Init(Pkcs12LoaderLimits loaderLimits)
		{
			_certBags = ArrayPool<SafeBagAsn>.Shared.Rent(loaderLimits.MaxCertificates ?? 10);
			_keyBags = ArrayPool<SafeBagAsn>.Shared.Rent(loaderLimits.MaxKeys ?? 10);
			_certCount = 0;
			_keyCount = 0;
			_decryptBufferOffset = 0;
		}

		public void Dispose()
		{
			if (_certBags != null)
			{
				ArrayPool<SafeBagAsn>.Shared.Return(_certBags, clearArray: true);
			}
			if (_keyBags != null)
			{
				ArrayPool<SafeBagAsn>.Shared.Return(_keyBags, clearArray: true);
			}
			if (_decryptBuffer != null)
			{
				System.Security.Cryptography.CryptoPool.Return(_decryptBuffer, _decryptBufferOffset);
			}
			if (_keyDecryptBuffer != null)
			{
				System.Security.Cryptography.CryptoPool.Return(_keyDecryptBuffer, _keyDecryptBufferOffset);
			}
			this = default(BagState);
		}

		internal void LockPassword()
		{
			_passwordState |= 1;
		}

		internal void ConfirmPassword()
		{
			_passwordState |= 3;
		}

		internal void PrepareDecryptBuffer(int upperBound)
		{
			if (_decryptBuffer == null)
			{
				_decryptBuffer = System.Security.Cryptography.CryptoPool.Rent(upperBound);
			}
		}

		internal ReadOnlyMemory<byte> DecryptSafeContents(in AlgorithmIdentifierAsn algorithmIdentifier, ReadOnlySpan<char> passwordSpan, ReadOnlySpan<byte> encryptedContent)
		{
			int decryptBufferOffset = _decryptBufferOffset;
			_decryptBufferOffset = _decryptBuffer.Length;
			try
			{
				int num = PasswordBasedEncryption.Decrypt(in algorithmIdentifier, passwordSpan, default(ReadOnlySpan<byte>), encryptedContent, _decryptBuffer.AsSpan(decryptBufferOffset));
				_decryptBufferOffset = decryptBufferOffset + num;
				try
				{
					AsnValueReader asnValueReader = new AsnValueReader(_decryptBuffer.AsSpan(decryptBufferOffset, num), AsnEncodingRules.BER);
					asnValueReader.ReadSequence();
					asnValueReader.ThrowIfNotEmpty();
				}
				catch (AsnContentException)
				{
					ThrowWithHResult(System.SR.Cryptography_Der_Invalid_Encoding, -2146885630);
				}
				ConfirmPassword();
				return new ReadOnlyMemory<byte>(_decryptBuffer, decryptBufferOffset, num);
			}
			catch (PlatformNotSupportedException innerException)
			{
				ThrowWithHResult(System.SR.Cryptography_Pfx_BadPassword, -2147024810, innerException);
				throw;
			}
			catch (CryptographicException ex2)
			{
				CryptographicOperations.ZeroMemory(_decryptBuffer.AsSpan(decryptBufferOffset, _decryptBufferOffset - decryptBufferOffset));
				_decryptBufferOffset = decryptBufferOffset;
				if (ex2.HResult != -2146885630)
				{
					ex2.HResult = (ConfirmedPassword ? (-2146893792) : (-2147024810));
				}
				throw;
			}
		}

		internal void AddCert(SafeBagAsn bag)
		{
			GrowIfNeeded(ref _certBags, _certCount);
			_certBags[_certCount] = bag;
			_certCount++;
		}

		internal void AddKey(SafeBagAsn bag)
		{
			GrowIfNeeded(ref _keyBags, _keyCount);
			_keyBags[_keyCount] = bag;
			_keyCount++;
		}

		private static void GrowIfNeeded(ref SafeBagAsn[] array, int index)
		{
			if (array.Length <= index)
			{
				SafeBagAsn[] array2 = ArrayPool<SafeBagAsn>.Shared.Rent(checked(index + 1));
				array.AsSpan().CopyTo(array2);
				ArrayPool<SafeBagAsn>.Shared.Return(array, clearArray: true);
				array = array2;
			}
		}

		internal void UnshroudKeys(ref ReadOnlySpan<char> password)
		{
			int num = 0;
			for (int i = 0; i < _keyCount; i++)
			{
				SafeBagAsn safeBagAsn = _keyBags[i];
				if (safeBagAsn.BagId == "1.2.840.113549.1.12.10.1.2")
				{
					num += safeBagAsn.BagValue.Length;
				}
			}
			_keyDecryptBuffer = System.Security.Cryptography.CryptoPool.Rent(num);
			for (int j = 0; j < _keyCount; j++)
			{
				ref SafeBagAsn reference = ref _keyBags[j];
				if (!(reference.BagId == "1.2.840.113549.1.12.10.1.2"))
				{
					continue;
				}
				ArraySegment<byte> arraySegment = default(ArraySegment<byte>);
				int bytesRead = 0;
				if (!LockedPassword)
				{
					try
					{
						arraySegment = KeyFormatHelper.DecryptPkcs8(password, reference.BagValue, out bytesRead);
						try
						{
							AsnValueReader asnValueReader = new AsnValueReader(arraySegment, AsnEncodingRules.BER);
							asnValueReader.ReadSequence();
							asnValueReader.ThrowIfNotEmpty();
						}
						catch (AsnContentException)
						{
							System.Security.Cryptography.CryptoPool.Return(arraySegment);
							arraySegment = default(ArraySegment<byte>);
							throw new CryptographicException();
						}
					}
					catch (CryptographicException)
					{
						password = (password.ContainsNull() ? "".AsSpan() : default(ReadOnlySpan<char>));
					}
				}
				if (arraySegment.Array == null)
				{
					try
					{
						arraySegment = KeyFormatHelper.DecryptPkcs8(password, reference.BagValue, out bytesRead);
						try
						{
							AsnValueReader asnValueReader2 = new AsnValueReader(arraySegment, AsnEncodingRules.BER);
							asnValueReader2.ReadSequence();
							asnValueReader2.ThrowIfNotEmpty();
						}
						catch (AsnContentException)
						{
							System.Security.Cryptography.CryptoPool.Return(arraySegment);
							arraySegment = default(ArraySegment<byte>);
							throw new CryptographicException();
						}
					}
					catch (CryptographicException)
					{
						continue;
					}
				}
				ConfirmPassword();
				arraySegment.AsSpan().CopyTo(_keyDecryptBuffer.AsSpan(_keyDecryptBufferOffset));
				ReadOnlyMemory<byte> bagValue = new ReadOnlyMemory<byte>(_keyDecryptBuffer, _keyDecryptBufferOffset, arraySegment.Count);
				System.Security.Cryptography.CryptoPool.Return(arraySegment);
				_keyDecryptBufferOffset += bagValue.Length;
				if (bytesRead != reference.BagValue.Length)
				{
					throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
				}
				reference.BagValue = bagValue;
				reference.BagId = "1.2.840.113549.1.12.10.1.1";
			}
		}

		internal ArraySegment<byte> ToPfx(ReadOnlySpan<char> password)
		{
			ContentInfoAsn contentInfoAsn = new ContentInfoAsn
			{
				ContentType = "1.2.840.113549.1.7.1"
			};
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.BER);
			using (asnWriter.PushOctetString())
			{
				using (asnWriter.PushSequence())
				{
					for (int i = 0; i < _certCount; i++)
					{
						SafeBagAsn safeBagAsn = _certBags[i];
						safeBagAsn.Encode(asnWriter);
					}
					for (int j = 0; j < _keyCount; j++)
					{
						SafeBagAsn safeBagAsn2 = _keyBags[j];
						safeBagAsn2.Encode(asnWriter);
					}
				}
			}
			contentInfoAsn.Content = asnWriter.Encode();
			asnWriter.Reset();
			using (asnWriter.PushSequence())
			{
				contentInfoAsn.Encode(asnWriter);
			}
			byte[] array = asnWriter.Encode();
			asnWriter.Reset();
			HashAlgorithmName sHA = HashAlgorithmName.SHA1;
			Span<byte> span = stackalloc byte[20];
			Helpers.RngFill(span);
			Span<byte> span2 = stackalloc byte[20];
			Pkcs12Kdf.DeriveMacKey(password, sHA, 1, span, span2);
			using (IncrementalHash incrementalHash = IncrementalHash.CreateHMAC(sHA, span2))
			{
				incrementalHash.AppendData(array);
				if (!incrementalHash.TryGetHashAndReset(span2, out var bytesWritten) || bytesWritten != span2.Length)
				{
					throw new CryptographicException();
				}
			}
			using (asnWriter.PushSequence())
			{
				asnWriter.WriteInteger(3L);
				using (asnWriter.PushSequence())
				{
					asnWriter.WriteObjectIdentifierForCrypto("1.2.840.113549.1.7.1");
					Asn1Tag value = new Asn1Tag(TagClass.ContextSpecific, 0);
					using (asnWriter.PushSequence(value))
					{
						asnWriter.WriteOctetString(array);
					}
				}
				using (asnWriter.PushSequence())
				{
					using (asnWriter.PushSequence())
					{
						using (asnWriter.PushSequence())
						{
							asnWriter.WriteObjectIdentifierForCrypto("1.3.14.3.2.26");
						}
						asnWriter.WriteOctetString(span2);
					}
					asnWriter.WriteOctetString(span);
				}
			}
			byte[] array2 = System.Security.Cryptography.CryptoPool.Rent(asnWriter.GetEncodedLength());
			int count = asnWriter.Encode(array2);
			return new ArraySegment<byte>(array2, 0, count);
		}
	}

	private static readonly AttributeAsn s_syntheticKspAttribute = BuildSyntheticKspAttribute();

	private static readonly AttributeAsn s_syntheticCapiCspAttribute = BuildSyntheticCapiAttribute();

	private static bool ProviderNameIsRelevant => OperatingSystem.IsWindows();

	public static X509Certificate2 LoadCertificate(ReadOnlySpan<byte> data)
	{
		if (data.IsEmpty)
		{
			ThrowWithHResult(System.SR.Cryptography_Der_Invalid_Encoding, -2146885630);
		}
		return new X509Certificate2(LoadCertificatePal(data));
	}

	public static X509Certificate2 LoadCertificate(byte[] data)
	{
		ArgumentNullException.ThrowIfNull(data, "data");
		return LoadCertificate(new ReadOnlySpan<byte>(data));
	}

	public static X509Certificate2 LoadCertificateFromFile(string path)
	{
		ArgumentException.ThrowIfNullOrEmpty(path, "path");
		return new X509Certificate2(LoadCertificatePalFromFile(path));
	}

	public static X509Certificate2 LoadPkcs12(byte[] data, string? password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet, Pkcs12LoaderLimits? loaderLimits = null)
	{
		ArgumentNullException.ThrowIfNull(data, "data");
		ValidateKeyStorageFlagsCore(keyStorageFlags);
		return LoadPkcs12(new ReadOnlyMemory<byte>(data), password.AsSpan(), keyStorageFlags, loaderLimits ?? Pkcs12LoaderLimits.Defaults).ToCertificate();
	}

	public unsafe static X509Certificate2 LoadPkcs12(ReadOnlySpan<byte> data, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet, Pkcs12LoaderLimits? loaderLimits = null)
	{
		fixed (byte* pointer = data)
		{
			using PointerMemoryManager<byte> pointerMemoryManager = new PointerMemoryManager<byte>(pointer, data.Length);
			return LoadPkcs12(pointerMemoryManager.Memory, password, keyStorageFlags, loaderLimits ?? Pkcs12LoaderLimits.Defaults).ToCertificate();
		}
	}

	public static X509Certificate2 LoadPkcs12FromFile(string path, string? password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet, Pkcs12LoaderLimits? loaderLimits = null)
	{
		return LoadPkcs12FromFile(path, password.AsSpan(), keyStorageFlags, loaderLimits);
	}

	public static X509Certificate2 LoadPkcs12FromFile(string path, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet, Pkcs12LoaderLimits? loaderLimits = null)
	{
		ThrowIfNullOrEmpty(path, "path");
		ValidateKeyStorageFlagsCore(keyStorageFlags);
		return LoadFromFile(path, password, keyStorageFlags, loaderLimits ?? Pkcs12LoaderLimits.Defaults, LoadPkcs12).ToCertificate();
	}

	public static X509Certificate2Collection LoadPkcs12Collection(byte[] data, string? password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet, Pkcs12LoaderLimits? loaderLimits = null)
	{
		ArgumentNullException.ThrowIfNull(data, "data");
		ValidateKeyStorageFlagsCore(keyStorageFlags);
		return LoadPkcs12Collection(new ReadOnlyMemory<byte>(data), password.AsSpan(), keyStorageFlags, loaderLimits ?? Pkcs12LoaderLimits.Defaults);
	}

	public unsafe static X509Certificate2Collection LoadPkcs12Collection(ReadOnlySpan<byte> data, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet, Pkcs12LoaderLimits? loaderLimits = null)
	{
		ValidateKeyStorageFlagsCore(keyStorageFlags);
		fixed (byte* pointer = data)
		{
			using PointerMemoryManager<byte> pointerMemoryManager = new PointerMemoryManager<byte>(pointer, data.Length);
			return LoadPkcs12Collection(pointerMemoryManager.Memory, password, keyStorageFlags, loaderLimits ?? Pkcs12LoaderLimits.Defaults);
		}
	}

	public static X509Certificate2Collection LoadPkcs12CollectionFromFile(string path, string? password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet, Pkcs12LoaderLimits? loaderLimits = null)
	{
		return LoadPkcs12CollectionFromFile(path, password.AsSpan(), keyStorageFlags, loaderLimits);
	}

	public static X509Certificate2Collection LoadPkcs12CollectionFromFile(string path, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet, Pkcs12LoaderLimits? loaderLimits = null)
	{
		ThrowIfNullOrEmpty(path, "path");
		ValidateKeyStorageFlagsCore(keyStorageFlags);
		return LoadFromFile(path, password, keyStorageFlags, loaderLimits ?? Pkcs12LoaderLimits.Defaults, LoadPkcs12Collection);
	}

	private static T LoadFromFile<T>(string path, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags, Pkcs12LoaderLimits loaderLimits, LoadFromFileFunc<T> loader)
	{
		var (array, num, memoryManager) = ReadAllBytesIfBerSequence(path);
		try
		{
			ReadOnlyMemory<byte> data = ((ReadOnlyMemory<byte>?)memoryManager?.Memory) ?? new ReadOnlyMemory<byte>(array, 0, num);
			return loader(data, password, keyStorageFlags, loaderLimits);
		}
		finally
		{
			((IDisposable)memoryManager)?.Dispose();
			if (array != null)
			{
				System.Security.Cryptography.CryptoPool.Return(array, num);
			}
		}
	}

	private static (byte[], int, MemoryManager<byte>) ReadAllBytesIfBerSequence(string path)
	{
		Span<byte> buffer = stackalloc byte[129];
		try
		{
			using FileStream fileStream = File.OpenRead(path);
			int num = fileStream.ReadAtLeast(buffer, 2);
			bool flag = buffer[0] != 48;
			if (!flag)
			{
				byte b = buffer[1];
				bool flag2 = (uint)b <= 1u;
				flag = flag2;
			}
			if (flag)
			{
				ThrowWithHResult(System.SR.Cryptography_Der_Invalid_Encoding, -2146885630);
			}
			int num2;
			if (buffer[1] < 128)
			{
				num2 = buffer[1] + 2;
			}
			else if (buffer[1] == 128)
			{
				long length = fileStream.Length;
				num2 = (int)((length >= 1048576) ? (-1) : length);
			}
			else
			{
				int num3 = buffer[1] - 128 - num;
				if (num3 > 0)
				{
					int num4 = fileStream.ReadAtLeast(buffer.Slice(num), num3);
					num += num4;
				}
				if (!AsnDecoder.TryDecodeLength(buffer.Slice(1, num - 1), AsnEncodingRules.BER, out var decodedLength, out var bytesConsumed))
				{
					ThrowWithHResult(System.SR.Cryptography_Der_Invalid_Encoding, -2146885630);
				}
				num2 = decodedLength.GetValueOrDefault() + bytesConsumed + 1;
			}
			if (num2 >= 0)
			{
				byte[] array = System.Security.Cryptography.CryptoPool.Rent(num2);
				buffer.Slice(0, num).CopyTo(array);
				fileStream.ReadExactly(array.AsSpan(num, num2 - num));
				return (array, num2, null);
			}
			return (null, 0, MemoryMappedFileMemoryManager.CreateFromFileClamped(fileStream));
		}
		catch (IOException inner)
		{
			throw new CryptographicException(System.SR.Arg_CryptographyException, inner);
		}
		catch (UnauthorizedAccessException inner2)
		{
			throw new CryptographicException(System.SR.Arg_CryptographyException, inner2);
		}
	}

	[DoesNotReturn]
	private static void ThrowWithHResult(string message, int hResult)
	{
		throw new CryptographicException(message)
		{
			HResult = hResult
		};
	}

	private static void ValidateKeyStorageFlagsCore(X509KeyStorageFlags keyStorageFlags)
	{
		if ((keyStorageFlags & ~(X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserProtected | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.EphemeralKeySet)) != X509KeyStorageFlags.DefaultKeySet)
		{
			throw new ArgumentException(System.SR.Argument_InvalidFlag, "keyStorageFlags");
		}
		X509KeyStorageFlags x509KeyStorageFlags = keyStorageFlags & (X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.EphemeralKeySet);
		if (x509KeyStorageFlags == (X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.EphemeralKeySet))
		{
			throw new ArgumentException(System.SR.Format(System.SR.Cryptography_X509_InvalidFlagCombination, x509KeyStorageFlags), "keyStorageFlags");
		}
	}

	[DoesNotReturn]
	private static void ThrowWithHResult(string message, int hResult, Exception innerException)
	{
		throw new CryptographicException(message, innerException)
		{
			HResult = hResult
		};
	}

	private static void ThrowIfNullOrEmpty([NotNull] string argument, [CallerArgumentExpression("argument")] string paramName = null)
	{
		if (string.IsNullOrEmpty(argument))
		{
			ThrowNullOrEmpty(argument, paramName);
		}
	}

	[DoesNotReturn]
	private static void ThrowNullOrEmpty(string argument, string paramName)
	{
		ArgumentNullException.ThrowIfNull(argument, paramName);
		throw new ArgumentException(System.SR.Argument_EmptyString, paramName);
	}

	private static void LoadPkcs12NoLimits(ReadOnlyMemory<byte> data, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags, ref Pkcs12Return earlyReturn)
	{
		bool deleteKeyContainer = ShouldDeleteKeyContainer(keyStorageFlags);
		using SafeCertStoreHandle storeHandle = ImportPfx(data.Span, password, keyStorageFlags);
		CertificatePal pal = LoadPkcs12(storeHandle, deleteKeyContainer);
		earlyReturn = new Pkcs12Return(pal);
	}

	private static void LoadPkcs12NoLimits(ReadOnlyMemory<byte> data, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags, ref X509Certificate2Collection earlyReturn)
	{
		bool deleteKeyContainers = ShouldDeleteKeyContainer(keyStorageFlags);
		using SafeCertStoreHandle storeHandle = ImportPfx(data.Span, password, keyStorageFlags);
		earlyReturn = LoadPkcs12Collection(storeHandle, deleteKeyContainers);
	}

	private static Pkcs12Return LoadPkcs12(ref BagState bagState, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags)
	{
		bool deleteKeyContainer = ShouldDeleteKeyContainer(keyStorageFlags);
		using SafeCertStoreHandle storeHandle = ImportPfx(ref bagState, password, keyStorageFlags);
		return new Pkcs12Return(LoadPkcs12(storeHandle, deleteKeyContainer));
	}

	private static X509Certificate2Collection LoadPkcs12Collection(ref BagState bagState, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags)
	{
		bool deleteKeyContainers = ShouldDeleteKeyContainer(keyStorageFlags);
		using SafeCertStoreHandle storeHandle = ImportPfx(ref bagState, password, keyStorageFlags);
		return LoadPkcs12Collection(storeHandle, deleteKeyContainers);
	}

	private static Pkcs12Return LoadPkcs12(ReadOnlyMemory<byte> data, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags, Pkcs12LoaderLimits loaderLimits)
	{
		if (loaderLimits == Pkcs12LoaderLimits.DangerousNoLimits)
		{
			Pkcs12Return earlyReturn = default(Pkcs12Return);
			LoadPkcs12NoLimits(data, password, keyStorageFlags, ref earlyReturn);
			if (earlyReturn.HasValue())
			{
				return earlyReturn;
			}
		}
		BagState bagState = default(BagState);
		bagState.SetStorageFlags(keyStorageFlags);
		try
		{
			ReadCertsAndKeys(ref bagState, data, ref password, loaderLimits);
			if (bagState.CertCount == 0)
			{
				throw new CryptographicException(System.SR.Cryptography_Pfx_NoCertificates);
			}
			bagState.UnshroudKeys(ref password);
			return LoadPkcs12(ref bagState, password, keyStorageFlags);
		}
		finally
		{
			bagState.Dispose();
		}
	}

	private static X509Certificate2Collection LoadPkcs12Collection(ReadOnlyMemory<byte> data, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags, Pkcs12LoaderLimits loaderLimits)
	{
		if (loaderLimits == Pkcs12LoaderLimits.DangerousNoLimits)
		{
			X509Certificate2Collection earlyReturn = null;
			LoadPkcs12NoLimits(data, password, keyStorageFlags, ref earlyReturn);
			if (earlyReturn != null)
			{
				return earlyReturn;
			}
		}
		BagState bagState = default(BagState);
		try
		{
			ReadCertsAndKeys(ref bagState, data, ref password, loaderLimits);
			if (bagState.CertCount == 0)
			{
				return new X509Certificate2Collection();
			}
			bagState.UnshroudKeys(ref password);
			return LoadPkcs12Collection(ref bagState, password, keyStorageFlags);
		}
		finally
		{
			bagState.Dispose();
		}
	}

	private static void ReadCertsAndKeys(ref BagState bagState, ReadOnlyMemory<byte> data, ref ReadOnlySpan<char> password, Pkcs12LoaderLimits loaderLimits)
	{
		try
		{
			AsnDecoder.ReadSequence(data.Span, AsnEncodingRules.BER, out var _, out var _, out var bytesConsumed);
			data = data.Slice(0, bytesConsumed);
			PfxAsn pfxAsn = PfxAsn.Decode(data, AsnEncodingRules.BER);
			if (pfxAsn.AuthSafe.ContentType != "1.2.840.113549.1.7.1")
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			ReadOnlySpan<byte> span = Helpers.DecodeOctetStringAsMemory(pfxAsn.AuthSafe.Content).Span;
			if (!password.IsEmpty)
			{
				bagState.LockPassword();
			}
			if (pfxAsn.MacData.HasValue)
			{
				if (pfxAsn.MacData.Value.IterationCount > loaderLimits.MacIterationLimit)
				{
					throw new Pkcs12LoadLimitExceededException("MacIterationLimit");
				}
				bool flag = false;
				if (!bagState.LockedPassword)
				{
					if (!pfxAsn.VerifyMac(password, span))
					{
						password = (password.ContainsNull() ? "".AsSpan() : default(ReadOnlySpan<char>));
					}
					else
					{
						flag = true;
					}
				}
				if (!flag && !pfxAsn.VerifyMac(password, span))
				{
					ThrowWithHResult(System.SR.Cryptography_Pfx_BadPassword, -2147024810);
				}
				bagState.ConfirmPassword();
			}
			AsnValueReader asnValueReader = new AsnValueReader(span, AsnEncodingRules.BER);
			AsnValueReader reader = asnValueReader.ReadSequence();
			asnValueReader.ThrowIfNotEmpty();
			ReadOnlyMemory<byte> content = pfxAsn.AuthSafe.Content;
			bagState.Init(loaderLimits);
			int? workRemaining = loaderLimits.TotalKdfIterationLimit;
			while (reader.HasData)
			{
				ContentInfoAsn.Decode(ref reader, content, out var decoded);
				ReadOnlyMemory<byte> contentData;
				if (decoded.ContentType == "1.2.840.113549.1.7.1")
				{
					contentData = Helpers.DecodeOctetStringAsMemory(decoded.Content);
				}
				else
				{
					if (!(decoded.ContentType == "1.2.840.113549.1.7.6"))
					{
						throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
					}
					if (loaderLimits.IgnoreEncryptedAuthSafes)
					{
						continue;
					}
					bagState.PrepareDecryptBuffer(span.Length);
					if (!bagState.LockedPassword)
					{
						bagState.LockPassword();
						int? num = workRemaining;
						try
						{
							contentData = DecryptSafeContents(decoded, loaderLimits, password, ref bagState, ref workRemaining);
						}
						catch (CryptographicException)
						{
							password = (password.ContainsNull() ? "".AsSpan() : default(ReadOnlySpan<char>));
							workRemaining = num;
							contentData = DecryptSafeContents(decoded, loaderLimits, password, ref bagState, ref workRemaining);
						}
					}
					else
					{
						contentData = DecryptSafeContents(decoded, loaderLimits, password, ref bagState, ref workRemaining);
					}
				}
				ProcessSafeContents(contentData, loaderLimits, ref workRemaining, ref bagState);
			}
		}
		catch (AsnContentException innerException)
		{
			ThrowWithHResult(System.SR.Cryptography_Der_Invalid_Encoding, -2146885630, innerException);
		}
	}

	private static void ProcessSafeContents(ReadOnlyMemory<byte> contentData, Pkcs12LoaderLimits loaderLimits, ref int? workRemaining, ref BagState bagState)
	{
		AsnValueReader asnValueReader = new AsnValueReader(contentData.Span, AsnEncodingRules.BER);
		AsnValueReader reader = asnValueReader.ReadSequence();
		asnValueReader.ThrowIfNotEmpty();
		HashSet<string> duplicateAttributeCheck = new HashSet<string>();
		CngMachineKeyState machineKeyState = CngMachineKeyState.Unknown;
		while (reader.HasData)
		{
			SafeBagAsn.Decode(ref reader, contentData, out var decoded);
			if (decoded.BagId == "1.2.840.113549.1.12.10.1.3")
			{
				if (decoded.BagAttributes != null && !loaderLimits.AllowDuplicateAttributes)
				{
					RejectDuplicateAttributes(decoded.BagAttributes, duplicateAttributeCheck);
				}
				if (!(CertBagAsn.Decode(decoded.BagValue, AsnEncodingRules.BER).CertId == "1.2.840.113549.1.9.22.1"))
				{
					continue;
				}
				if (bagState.CertCount >= loaderLimits.MaxCertificates)
				{
					throw new Pkcs12LoadLimitExceededException("MaxCertificates");
				}
				if (decoded.BagAttributes != null)
				{
					FilterAttributes(loaderLimits, ref decoded, delegate(Pkcs12LoaderLimits limits, string oid)
					{
						if (oid == "1.2.840.113549.1.9.21")
						{
							return true;
						}
						return (oid == "1.2.840.113549.1.9.20") ? limits.PreserveCertificateAlias : limits.PreserveUnknownAttributes;
					});
				}
				bagState.AddCert(decoded);
				continue;
			}
			string bagId = decoded.BagId;
			if ((!(bagId == "1.2.840.113549.1.12.10.1.1") && !(bagId == "1.2.840.113549.1.12.10.1.2")) || 1 == 0)
			{
				continue;
			}
			if (decoded.BagAttributes != null && !loaderLimits.AllowDuplicateAttributes)
			{
				RejectDuplicateAttributes(decoded.BagAttributes, duplicateAttributeCheck);
			}
			if (loaderLimits.IgnorePrivateKeys)
			{
				continue;
			}
			if (bagState.KeyCount >= loaderLimits.MaxKeys)
			{
				throw new Pkcs12LoadLimitExceededException("MaxKeys");
			}
			AttributeAsn? attributeAsn;
			checked
			{
				if (decoded.BagId == "1.2.840.113549.1.12.10.1.2")
				{
					EncryptedPrivateKeyInfoAsn encryptedPrivateKeyInfoAsn = EncryptedPrivateKeyInfoAsn.Decode(decoded.BagValue, AsnEncodingRules.BER);
					int kdfCount = GetKdfCount(in encryptedPrivateKeyInfoAsn.EncryptionAlgorithm);
					if (kdfCount > loaderLimits.IndividualKdfIterationLimit || kdfCount > workRemaining)
					{
						throw new Pkcs12LoadLimitExceededException((kdfCount > loaderLimits.IndividualKdfIterationLimit) ? "IndividualKdfIterationLimit" : "TotalKdfIterationLimit");
					}
					if (workRemaining.HasValue)
					{
						workRemaining -= kdfCount;
					}
				}
				if (decoded.BagAttributes != null)
				{
					FilterAttributes(loaderLimits, ref decoded, (Pkcs12LoaderLimits limits, string attrType) => attrType switch
					{
						"1.2.840.113549.1.9.21" => true, 
						"1.3.6.1.4.1.311.17.2" => true, 
						"1.2.840.113549.1.9.20" => limits.PreserveKeyName, 
						"1.3.6.1.4.1.311.17.1" => ProviderNameIsRelevant, 
						_ => limits.PreserveUnknownAttributes, 
					});
				}
				attributeAsn = null;
			}
			if (ProviderNameIsRelevant)
			{
				if ((bagState.StorageFlags & X509KeyStorageFlags.EphemeralKeySet) == 0 && !loaderLimits.PreserveStorageProvider)
				{
					attributeAsn = DetermineStorageProvider(decoded.BagAttributes, bagState.StorageFlags, ref machineKeyState);
				}
				bool flag = false;
				AttributeAsn[] array = decoded.BagAttributes ?? Array.Empty<AttributeAsn>();
				for (int num = 0; num < array.Length; num++)
				{
					AttributeAsn attributeAsn2 = array[num];
					if (!(attributeAsn2.AttrType == "1.3.6.1.4.1.311.17.1"))
					{
						continue;
					}
					flag = true;
					if (!attributeAsn.HasValue)
					{
						continue;
					}
					ReadOnlyMemory<byte>[] attrValues = attributeAsn2.AttrValues;
					if (attrValues != null && attrValues.Length != 0)
					{
						for (int num2 = 0; num2 < attributeAsn2.AttrValues.Length; num2++)
						{
							attributeAsn2.AttrValues[num2] = attributeAsn.GetValueOrDefault().AttrValues[0];
						}
					}
				}
				if (!flag)
				{
					int num3 = decoded.BagAttributes?.Length ?? 0;
					Array.Resize(ref decoded.BagAttributes, num3 + 1);
					decoded.BagAttributes[num3] = s_syntheticKspAttribute;
				}
			}
			bagState.AddKey(decoded);
		}
	}

	private static void RejectDuplicateAttributes(AttributeAsn[] bagAttributes, HashSet<string> duplicateAttributeCheck)
	{
		duplicateAttributeCheck.Clear();
		for (int i = 0; i < bagAttributes.Length; i++)
		{
			AttributeAsn attributeAsn = bagAttributes[i];
			if (!duplicateAttributeCheck.Add(attributeAsn.AttrType) || attributeAsn.AttrValues.Length > 1)
			{
				throw new Pkcs12LoadLimitExceededException("AllowDuplicateAttributes");
			}
		}
	}

	private static void FilterAttributes(Pkcs12LoaderLimits loaderLimits, ref SafeBagAsn bag, Func<Pkcs12LoaderLimits, string, bool> filter)
	{
		if (bag.BagAttributes == null)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < bag.BagAttributes.Length; i++)
		{
			string attrType = bag.BagAttributes[i].AttrType;
			if (filter(loaderLimits, attrType))
			{
				num++;
				if (num != i)
				{
					AttributeAsn attributeAsn = bag.BagAttributes[i];
					bag.BagAttributes[num] = attributeAsn;
				}
			}
		}
		int num2 = num + 1;
		if (num2 < bag.BagAttributes.Length)
		{
			if (num2 == 0)
			{
				bag.BagAttributes = null;
			}
			else
			{
				Array.Resize(ref bag.BagAttributes, num2);
			}
		}
	}

	private static ReadOnlyMemory<byte> DecryptSafeContents(ContentInfoAsn safeContentsAsn, Pkcs12LoaderLimits loaderLimits, ReadOnlySpan<char> passwordSpan, ref BagState bagState, ref int? workRemaining)
	{
		EncryptedDataAsn encryptedDataAsn = EncryptedDataAsn.Decode(safeContentsAsn.Content, AsnEncodingRules.BER);
		if (encryptedDataAsn.Version != 0 && encryptedDataAsn.Version != 2)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		if (encryptedDataAsn.EncryptedContentInfo.ContentType != "1.2.840.113549.1.7.1")
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		if (!encryptedDataAsn.EncryptedContentInfo.EncryptedContent.HasValue)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		ReadOnlyMemory<byte> value = encryptedDataAsn.EncryptedContentInfo.EncryptedContent.Value;
		int kdfCount = GetKdfCount(in encryptedDataAsn.EncryptedContentInfo.ContentEncryptionAlgorithm);
		if (kdfCount > loaderLimits.IndividualKdfIterationLimit || kdfCount > workRemaining)
		{
			throw new Pkcs12LoadLimitExceededException((kdfCount > loaderLimits.IndividualKdfIterationLimit) ? "IndividualKdfIterationLimit" : "TotalKdfIterationLimit");
		}
		checked
		{
			if (workRemaining.HasValue)
			{
				workRemaining -= kdfCount;
			}
			return bagState.DecryptSafeContents(in encryptedDataAsn.EncryptedContentInfo.ContentEncryptionAlgorithm, passwordSpan, value.Span);
		}
	}

	private static int GetKdfCount(in AlgorithmIdentifierAsn algorithmIdentifier)
	{
		int num = GetRawKdfCount(in algorithmIdentifier);
		if (num < 0)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		return num;
		static int GetRawKdfCount(in AlgorithmIdentifierAsn reference)
		{
			if (!reference.Parameters.HasValue)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			switch (reference.Algorithm)
			{
			case "1.2.840.113549.1.5.3":
			case "1.2.840.113549.1.5.6":
			case "1.2.840.113549.1.5.10":
			case "1.2.840.113549.1.5.11":
			case "1.2.840.113549.1.12.1.3":
			case "1.2.840.113549.1.12.1.4":
			case "1.2.840.113549.1.12.1.5":
			case "1.2.840.113549.1.12.1.6":
				return PBEParameter.Decode(reference.Parameters.Value, AsnEncodingRules.BER).IterationCount;
			case "1.2.840.113549.1.5.13":
			{
				PBES2Params pBES2Params = PBES2Params.Decode(reference.Parameters.Value, AsnEncodingRules.BER);
				if (pBES2Params.KeyDerivationFunc.Algorithm != "1.2.840.113549.1.5.12")
				{
					throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownAlgorithmIdentifier, pBES2Params.EncryptionScheme.Algorithm));
				}
				if (!pBES2Params.KeyDerivationFunc.Parameters.HasValue)
				{
					throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
				}
				return Pbkdf2Params.Decode(pBES2Params.KeyDerivationFunc.Parameters.Value, AsnEncodingRules.BER).IterationCount;
			}
			default:
				throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownAlgorithmIdentifier, reference.Algorithm));
			}
		}
	}

	private static AttributeAsn BuildSyntheticKspAttribute()
	{
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		asnWriter.WriteCharacterString(UniversalTagNumber.BMPString, "Microsoft Software Key Storage Provider");
		return new AttributeAsn
		{
			AttrType = "1.3.6.1.4.1.311.17.1",
			AttrValues = new ReadOnlyMemory<byte>[1]
			{
				new ReadOnlyMemory<byte>(asnWriter.Encode())
			}
		};
	}

	private static AttributeAsn BuildSyntheticCapiAttribute()
	{
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		asnWriter.WriteCharacterString(UniversalTagNumber.BMPString, "Microsoft Enhanced RSA and AES Cryptographic Provider");
		return new AttributeAsn
		{
			AttrType = "1.3.6.1.4.1.311.17.1",
			AttrValues = new ReadOnlyMemory<byte>[1]
			{
				new ReadOnlyMemory<byte>(asnWriter.Encode())
			}
		};
	}

	private static AttributeAsn? DetermineStorageProvider(AttributeAsn[] bagAttributes, X509KeyStorageFlags storageFlags, ref CngMachineKeyState machineKeyState)
	{
		if (HasMachineKey(bagAttributes, storageFlags) && HasCapiCsp(bagAttributes, out var isRsa))
		{
			if (machineKeyState == CngMachineKeyState.Unknown)
			{
				machineKeyState = CheckMachineKeyPermissions();
			}
			if (machineKeyState == CngMachineKeyState.Denied)
			{
				if (isRsa)
				{
					return s_syntheticCapiCspAttribute;
				}
				return null;
			}
		}
		return s_syntheticKspAttribute;
		static CngMachineKeyState CheckMachineKeyPermissions()
		{
			CngKey cngKey = null;
			try
			{
				cngKey = CngKey.Create(CngAlgorithm.Rsa, "netperm-" + Guid.NewGuid().ToString("B"), new CngKeyCreationParameters
				{
					Provider = CngProvider.MicrosoftSoftwareKeyStorageProvider,
					KeyCreationOptions = CngKeyCreationOptions.MachineKey
				});
				return CngMachineKeyState.Permitted;
			}
			catch (CryptographicException)
			{
				return CngMachineKeyState.Denied;
			}
			finally
			{
				cngKey?.Delete();
			}
		}
		static bool HasCapiCsp(AttributeAsn[] array, out bool reference)
		{
			reference = false;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					AttributeAsn attributeAsn = array[i];
					if (attributeAsn.AttrType == "1.3.6.1.4.1.311.17.1")
					{
						ReadOnlyMemory<byte>[] attrValues = attributeAsn.AttrValues;
						if (attrValues != null && attrValues.Length != 0)
						{
							return HasCapiValue(attributeAsn.AttrValues[0].Span, out reference);
						}
					}
				}
			}
			return true;
		}
		static bool HasCapiValue(ReadOnlySpan<byte> encodedAttribute, out bool reference)
		{
			reference = false;
			if (!Asn1Tag.TryDecode(encodedAttribute, out var tag, out var _))
			{
				return false;
			}
			if (tag.TagClass != TagClass.Universal)
			{
				return false;
			}
			string text2;
			try
			{
				UniversalTagNumber tagValue = (UniversalTagNumber)tag.TagValue;
				int bytesConsumed2 = 0;
				string text;
				switch (tagValue)
				{
				case UniversalTagNumber.UTF8String:
				case UniversalTagNumber.PrintableString:
				case UniversalTagNumber.TeletexString:
				case UniversalTagNumber.IA5String:
				case UniversalTagNumber.VisibleString:
				case UniversalTagNumber.BMPString:
					text = AsnDecoder.ReadCharacterString(encodedAttribute, AsnEncodingRules.BER, tagValue, out bytesConsumed2);
					break;
				default:
					text = null;
					break;
				}
				text2 = text;
				if (bytesConsumed2 != encodedAttribute.Length)
				{
					return false;
				}
			}
			catch (AsnContentException)
			{
				return false;
			}
			bool flag;
			switch (text2)
			{
			case "Microsoft Enhanced Cryptographic Provider v1.0":
			case "Microsoft RSA Signature Cryptographic Provider":
			case "Microsoft Base Cryptographic Provider v1.0":
			case "Microsoft Strong Cryptographic Provider":
			case "Microsoft RSA SChannel Cryptographic Provider":
			case "Microsoft Enhanced RSA and AES Cryptographic Provider":
			case "Microsoft Enhanced RSA and AES Cryptographic Provider (Prototype)":
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
			reference = flag;
			flag = reference;
			if (!flag)
			{
				bool flag2;
				switch (text2)
				{
				case "Microsoft Base DSS Cryptographic Provider":
				case "Microsoft Base DSS and Diffie-Hellman Cryptographic Provider":
				case "Microsoft Enhanced DSS and Diffie-Hellman Cryptographic Provider":
				case "Microsoft DH SChannel Cryptographic Provider":
					flag2 = true;
					break;
				default:
					flag2 = false;
					break;
				}
				flag = flag2;
			}
			return flag;
		}
		static bool HasMachineKey(AttributeAsn[] array, X509KeyStorageFlags storageKind)
		{
			if ((storageKind & X509KeyStorageFlags.UserKeySet) != X509KeyStorageFlags.DefaultKeySet)
			{
				return false;
			}
			if ((storageKind & X509KeyStorageFlags.MachineKeySet) != X509KeyStorageFlags.DefaultKeySet)
			{
				return true;
			}
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].AttrType == "1.3.6.1.4.1.311.17.2")
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	private unsafe static ICertificatePal LoadCertificatePal(ReadOnlySpan<byte> data)
	{
		fixed (byte* handle = data)
		{
			global::Interop.Crypt32.DATA_BLOB dATA_BLOB = new global::Interop.Crypt32.DATA_BLOB((nint)handle, (uint)data.Length);
			return LoadCertificate(global::Interop.Crypt32.CertQueryObjectType.CERT_QUERY_OBJECT_BLOB, &dATA_BLOB);
		}
	}

	private unsafe static ICertificatePal LoadCertificatePalFromFile(string path)
	{
		fixed (char* pvObject = path)
		{
			return LoadCertificate(global::Interop.Crypt32.CertQueryObjectType.CERT_QUERY_OBJECT_FILE, pvObject);
		}
	}

	internal unsafe static ICertificatePal LoadPkcs12Pal(ReadOnlySpan<byte> data, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags, Pkcs12LoaderLimits loaderLimits)
	{
		fixed (byte* pointer = data)
		{
			using PointerMemoryManager<byte> pointerMemoryManager = new PointerMemoryManager<byte>(pointer, data.Length);
			return LoadPkcs12(pointerMemoryManager.Memory, password, keyStorageFlags, loaderLimits).GetPal();
		}
	}

	internal static ICertificatePal LoadPkcs12PalFromFile(string path, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags, Pkcs12LoaderLimits loaderLimits)
	{
		ThrowIfNullOrEmpty(path, "path");
		return LoadFromFile(path, password, keyStorageFlags, loaderLimits, LoadPkcs12).GetPal();
	}

	internal static void ValidateKeyStorageFlags(X509KeyStorageFlags keyStorageFlags)
	{
		ValidateKeyStorageFlagsCore(keyStorageFlags);
	}

	private static CertificatePal LoadPkcs12(SafeCertStoreHandle storeHandle, bool deleteKeyContainer)
	{
		SafeCertContextHandle safeCertContextHandle = null;
		SafeCertContextHandle pCertContext = null;
		bool flag = false;
		while (global::Interop.crypt32.CertEnumCertificatesInStore(storeHandle, ref pCertContext))
		{
			if (pCertContext.ContainsPrivateKey)
			{
				if (safeCertContextHandle != null && safeCertContextHandle.ContainsPrivateKey)
				{
					if (pCertContext.HasPersistedPrivateKey)
					{
						SafeCertContextHandleWithKeyContainerDeletion.DeleteKeyContainer(pCertContext);
					}
				}
				else
				{
					safeCertContextHandle?.Dispose();
					safeCertContextHandle = pCertContext.Duplicate();
					flag = true;
				}
			}
			else if (safeCertContextHandle == null)
			{
				safeCertContextHandle = pCertContext.Duplicate();
			}
		}
		if (safeCertContextHandle == null)
		{
			throw new CryptographicException(System.SR.Cryptography_Pfx_NoCertificates);
		}
		bool deleteKeyContainer2 = flag & deleteKeyContainer;
		return new CertificatePal(safeCertContextHandle, deleteKeyContainer2);
	}

	private static X509Certificate2Collection LoadPkcs12Collection(SafeCertStoreHandle storeHandle, bool deleteKeyContainers)
	{
		X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
		SafeCertContextHandle pCertContext = null;
		while (global::Interop.crypt32.CertEnumCertificatesInStore(storeHandle, ref pCertContext))
		{
			bool deleteKeyContainer = deleteKeyContainers && pCertContext.HasPersistedPrivateKey;
			CertificatePal pal = new CertificatePal(pCertContext.Duplicate(), deleteKeyContainer);
			x509Certificate2Collection.Add(new X509Certificate2(pal));
		}
		return x509Certificate2Collection;
	}

	private unsafe static CertificatePal LoadCertificate(global::Interop.Crypt32.CertQueryObjectType objectType, void* pvObject)
	{
		if (!global::Interop.Crypt32.CryptQueryObject(objectType, pvObject, global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_CERT, global::Interop.Crypt32.ExpectedFormatTypeFlags.CERT_QUERY_FORMAT_FLAG_ALL, 0, IntPtr.Zero, out var pdwContentType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, out var ppvContext))
		{
			ppvContext.Dispose();
			throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
		}
		if (pdwContentType != global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_CERT || ppvContext.IsInvalid)
		{
			ppvContext.Dispose();
			throw new CryptographicException();
		}
		return new CertificatePal(ppvContext, deleteKeyContainer: false);
	}

	private static SafeCertStoreHandle ImportPfx(ref BagState bagState, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags)
	{
		ArraySegment<byte> arraySegment = bagState.ToPfx(password);
		SafeCertStoreHandle result = ImportPfx(arraySegment, password, keyStorageFlags);
		System.Security.Cryptography.CryptoPool.Return(arraySegment);
		return result;
	}

	private unsafe static SafeCertStoreHandle ImportPfx(ReadOnlySpan<byte> data, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags)
	{
		Span<char> span = stackalloc char[65];
		global::Interop.Crypt32.PfxCertStoreFlags dwFlags = MapKeyStorageFlags(keyStorageFlags);
		if (password.Length >= 64)
		{
			span = new char[password.Length + 1];
		}
		SafeCertStoreHandle safeCertStoreHandle;
		fixed (byte* handle = data)
		{
			fixed (char* password2 = span)
			{
				try
				{
					password.CopyTo(span);
					span[password.Length] = '\0';
					global::Interop.Crypt32.DATA_BLOB pPFX = new global::Interop.Crypt32.DATA_BLOB((nint)handle, (uint)data.Length);
					safeCertStoreHandle = global::Interop.Crypt32.PFXImportCertStore(ref pPFX, password2, dwFlags);
				}
				finally
				{
					CryptographicOperations.ZeroMemory(MemoryMarshal.AsBytes(span));
				}
			}
		}
		if (safeCertStoreHandle.IsInvalid)
		{
			CryptographicException ex = Marshal.GetHRForLastWin32Error().ToCryptographicException();
			safeCertStoreHandle.Dispose();
			throw ex;
		}
		return safeCertStoreHandle;
	}

	private static global::Interop.Crypt32.PfxCertStoreFlags MapKeyStorageFlags(X509KeyStorageFlags keyStorageFlags)
	{
		global::Interop.Crypt32.PfxCertStoreFlags pfxCertStoreFlags = global::Interop.Crypt32.PfxCertStoreFlags.None;
		if ((keyStorageFlags & X509KeyStorageFlags.UserKeySet) == X509KeyStorageFlags.UserKeySet)
		{
			pfxCertStoreFlags |= global::Interop.Crypt32.PfxCertStoreFlags.CRYPT_USER_KEYSET;
		}
		else if ((keyStorageFlags & X509KeyStorageFlags.MachineKeySet) == X509KeyStorageFlags.MachineKeySet)
		{
			pfxCertStoreFlags |= global::Interop.Crypt32.PfxCertStoreFlags.CRYPT_MACHINE_KEYSET;
		}
		if ((keyStorageFlags & X509KeyStorageFlags.Exportable) == X509KeyStorageFlags.Exportable)
		{
			pfxCertStoreFlags |= global::Interop.Crypt32.PfxCertStoreFlags.CRYPT_EXPORTABLE;
		}
		if ((keyStorageFlags & X509KeyStorageFlags.UserProtected) == X509KeyStorageFlags.UserProtected)
		{
			pfxCertStoreFlags |= global::Interop.Crypt32.PfxCertStoreFlags.CRYPT_USER_PROTECTED;
		}
		if ((keyStorageFlags & X509KeyStorageFlags.EphemeralKeySet) == X509KeyStorageFlags.EphemeralKeySet)
		{
			pfxCertStoreFlags |= global::Interop.Crypt32.PfxCertStoreFlags.PKCS12_ALWAYS_CNG_KSP | global::Interop.Crypt32.PfxCertStoreFlags.PKCS12_NO_PERSIST_KEY;
		}
		return pfxCertStoreFlags;
	}

	private static bool ShouldDeleteKeyContainer(X509KeyStorageFlags keyStorageFlags)
	{
		return (keyStorageFlags & (X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.EphemeralKeySet)) == 0;
	}
}
