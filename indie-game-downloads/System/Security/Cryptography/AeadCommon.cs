using Internal.Cryptography;
using Internal.NativeCrypto;

namespace System.Security.Cryptography;

internal static class AeadCommon
{
	public unsafe static void Encrypt(SafeKeyHandle keyHandle, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> associatedData, ReadOnlySpan<byte> plaintext, Span<byte> ciphertext, Span<byte> tag)
	{
		fixed (byte* nonNullPinnableReference = &Helpers.GetNonNullPinnableReference(plaintext))
		{
			fixed (byte* nonNullPinnableReference2 = &Helpers.GetNonNullPinnableReference(nonce))
			{
				fixed (byte* nonNullPinnableReference3 = &Helpers.GetNonNullPinnableReference(ciphertext))
				{
					fixed (byte* nonNullPinnableReference4 = &Helpers.GetNonNullPinnableReference(tag))
					{
						fixed (byte* nonNullPinnableReference5 = &Helpers.GetNonNullPinnableReference(associatedData))
						{
							global::Interop.BCrypt.BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO = global::Interop.BCrypt.BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.Create();
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.pbNonce = nonNullPinnableReference2;
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.cbNonce = nonce.Length;
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.pbTag = nonNullPinnableReference4;
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.cbTag = tag.Length;
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.pbAuthData = nonNullPinnableReference5;
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.cbAuthData = associatedData.Length;
							global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.BCryptEncrypt(keyHandle, nonNullPinnableReference, plaintext.Length, new IntPtr(&bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO), null, 0, nonNullPinnableReference3, ciphertext.Length, out var _, 0);
							if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
							{
								throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS);
							}
						}
					}
				}
			}
		}
	}

	public unsafe static void Decrypt(SafeKeyHandle keyHandle, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> associatedData, ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> tag, Span<byte> plaintext, bool clearPlaintextOnFailure)
	{
		fixed (byte* nonNullPinnableReference = &Helpers.GetNonNullPinnableReference(plaintext))
		{
			fixed (byte* nonNullPinnableReference2 = &Helpers.GetNonNullPinnableReference(nonce))
			{
				fixed (byte* nonNullPinnableReference3 = &Helpers.GetNonNullPinnableReference(ciphertext))
				{
					fixed (byte* nonNullPinnableReference4 = &Helpers.GetNonNullPinnableReference(tag))
					{
						fixed (byte* nonNullPinnableReference5 = &Helpers.GetNonNullPinnableReference(associatedData))
						{
							global::Interop.BCrypt.BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO = global::Interop.BCrypt.BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.Create();
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.pbNonce = nonNullPinnableReference2;
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.cbNonce = nonce.Length;
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.pbTag = nonNullPinnableReference4;
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.cbTag = tag.Length;
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.pbAuthData = nonNullPinnableReference5;
							bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.cbAuthData = associatedData.Length;
							global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.BCryptDecrypt(keyHandle, nonNullPinnableReference3, ciphertext.Length, new IntPtr(&bCRYPT_AUTHENTICATED_CIPHER_MODE_INFO), null, 0, nonNullPinnableReference, plaintext.Length, out var _, 0);
							switch (nTSTATUS)
							{
							case global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS:
								break;
							case global::Interop.BCrypt.NTSTATUS.STATUS_AUTH_TAG_MISMATCH:
								if (clearPlaintextOnFailure)
								{
									CryptographicOperations.ZeroMemory(plaintext);
								}
								throw new AuthenticationTagMismatchException();
							default:
								throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS);
							}
						}
					}
				}
			}
		}
	}
}
