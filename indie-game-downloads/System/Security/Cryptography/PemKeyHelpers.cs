namespace System.Security.Cryptography;

internal static class PemKeyHelpers
{
	internal delegate TAlg ImportFactoryKeyAction<TAlg>(ReadOnlySpan<byte> source);

	internal delegate ImportFactoryKeyAction<TAlg> FindImportFactoryActionFunc<TAlg>(ReadOnlySpan<char> label);

	internal delegate TAlg ImportFactoryEncryptedKeyAction<TAlg, TPass>(ReadOnlySpan<TPass> password, ReadOnlySpan<byte> source);

	public delegate bool TryExportKeyAction<T>(T arg, Span<byte> destination, out int bytesWritten);

	public delegate bool TryExportEncryptedKeyAction<T, TPassword>(T arg, ReadOnlySpan<TPassword> password, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten);

	public delegate void ImportKeyAction(ReadOnlySpan<byte> source, out int bytesRead);

	public delegate ImportKeyAction FindImportActionFunc(ReadOnlySpan<char> label);

	public delegate void ImportEncryptedKeyAction<TPass>(ReadOnlySpan<TPass> password, ReadOnlySpan<byte> source, out int bytesRead);

	internal static TAlg ImportFactoryPem<TAlg>(ReadOnlySpan<char> source, FindImportFactoryActionFunc<TAlg> callback) where TAlg : class
	{
		ImportFactoryKeyAction<TAlg> importFactoryKeyAction = null;
		PemFields pemFields = default(PemFields);
		ReadOnlySpan<char> readOnlySpan = default(ReadOnlySpan<char>);
		bool flag = false;
		ReadOnlySpan<char> readOnlySpan2 = source;
		PemFields fields;
		Range label;
		while (PemEncoding.TryFind(readOnlySpan2, out fields))
		{
			label = fields.Label;
			ReadOnlySpan<char> readOnlySpan3 = readOnlySpan2[label.Start..label.End];
			ImportFactoryKeyAction<TAlg> importFactoryKeyAction2 = callback(readOnlySpan3);
			if (importFactoryKeyAction2 != null)
			{
				if ((importFactoryKeyAction != null) | flag)
				{
					throw new ArgumentException(System.SR.Argument_PemImport_AmbiguousPem, "source");
				}
				importFactoryKeyAction = importFactoryKeyAction2;
				pemFields = fields;
				readOnlySpan = readOnlySpan2;
			}
			else if (readOnlySpan3.SequenceEqual("ENCRYPTED PRIVATE KEY".AsSpan()))
			{
				if ((importFactoryKeyAction != null) | flag)
				{
					throw new ArgumentException(System.SR.Argument_PemImport_AmbiguousPem, "source");
				}
				flag = true;
			}
			Index end = fields.Location.End;
			Index index = end;
			int length = readOnlySpan2.Length;
			int offset = index.GetOffset(length);
			readOnlySpan2 = readOnlySpan2.Slice(offset, length - offset);
		}
		if (flag)
		{
			throw new ArgumentException(System.SR.Argument_PemImport_EncryptedPem, "source");
		}
		if (importFactoryKeyAction == null)
		{
			throw new ArgumentException(System.SR.Argument_PemImport_NoPemFound, "source");
		}
		label = pemFields.Base64Data;
		ReadOnlySpan<char> chars = readOnlySpan[label.Start..label.End];
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(pemFields.DecodedDataLength);
		int bytesWritten = 0;
		try
		{
			if (!Convert.TryFromBase64Chars(chars, array, out bytesWritten))
			{
				throw new ArgumentException();
			}
			Span<byte> span = array.AsSpan(0, bytesWritten);
			return importFactoryKeyAction(span);
		}
		finally
		{
			System.Security.Cryptography.CryptoPool.Return(array, bytesWritten);
		}
	}

	internal static TAlg ImportEncryptedFactoryPem<TAlg, TPass>(ReadOnlySpan<char> source, ReadOnlySpan<TPass> password, ImportFactoryEncryptedKeyAction<TAlg, TPass> importAction) where TAlg : class
	{
		bool flag = false;
		PemFields pemFields = default(PemFields);
		ReadOnlySpan<char> readOnlySpan = default(ReadOnlySpan<char>);
		ReadOnlySpan<char> readOnlySpan2 = source;
		PemFields fields;
		Range label;
		while (PemEncoding.TryFind(readOnlySpan2, out fields))
		{
			label = fields.Label;
			if (readOnlySpan2[label.Start..label.End].SequenceEqual("ENCRYPTED PRIVATE KEY".AsSpan()))
			{
				if (flag)
				{
					throw new ArgumentException(System.SR.Argument_PemImport_AmbiguousPem, "source");
				}
				flag = true;
				pemFields = fields;
				readOnlySpan = readOnlySpan2;
			}
			Index end = fields.Location.End;
			Index index = end;
			int length = readOnlySpan2.Length;
			int offset = index.GetOffset(length);
			readOnlySpan2 = readOnlySpan2.Slice(offset, length - offset);
		}
		if (!flag)
		{
			throw new ArgumentException(System.SR.Argument_PemImport_NoPemFound, "source");
		}
		label = pemFields.Base64Data;
		ReadOnlySpan<char> chars = readOnlySpan[label.Start..label.End];
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(pemFields.DecodedDataLength);
		int bytesWritten = 0;
		try
		{
			if (!Convert.TryFromBase64Chars(chars, array, out bytesWritten))
			{
				throw new ArgumentException();
			}
			Span<byte> span = array.AsSpan(0, bytesWritten);
			return importAction(password, span);
		}
		finally
		{
			System.Security.Cryptography.CryptoPool.Return(array, bytesWritten);
		}
	}

	public unsafe static bool TryExportToEncryptedPem<T, TPassword>(T arg, ReadOnlySpan<TPassword> password, PbeParameters pbeParameters, TryExportEncryptedKeyAction<T, TPassword> exporter, Span<char> destination, out int charsWritten)
	{
		int minimumLength = 4096;
		while (true)
		{
			byte[] array = System.Security.Cryptography.CryptoPool.Rent(minimumLength);
			int bytesWritten = 0;
			minimumLength = array.Length;
			fixed (byte* ptr = array)
			{
				try
				{
					if (exporter(arg, password, pbeParameters, array, out bytesWritten))
					{
						return PemEncoding.TryWrite(data: new Span<byte>(array, 0, bytesWritten), label: "ENCRYPTED PRIVATE KEY".AsSpan(), destination: destination, charsWritten: out charsWritten);
					}
				}
				finally
				{
					System.Security.Cryptography.CryptoPool.Return(array, bytesWritten);
				}
				minimumLength = checked(minimumLength * 2);
			}
		}
	}

	public unsafe static bool TryExportToPem<T>(T arg, string label, TryExportKeyAction<T> exporter, Span<char> destination, out int charsWritten)
	{
		int minimumLength = 4096;
		while (true)
		{
			byte[] array = System.Security.Cryptography.CryptoPool.Rent(minimumLength);
			int bytesWritten = 0;
			minimumLength = array.Length;
			fixed (byte* ptr = array)
			{
				try
				{
					if (exporter(arg, array, out bytesWritten))
					{
						return PemEncoding.TryWrite(data: new Span<byte>(array, 0, bytesWritten), label: label.AsSpan(), destination: destination, charsWritten: out charsWritten);
					}
				}
				finally
				{
					System.Security.Cryptography.CryptoPool.Return(array, bytesWritten);
				}
				minimumLength = checked(minimumLength * 2);
			}
		}
	}

	public static void ImportEncryptedPem<TPass>(ReadOnlySpan<char> input, ReadOnlySpan<TPass> password, ImportEncryptedKeyAction<TPass> importAction)
	{
		bool flag = false;
		PemFields pemFields = default(PemFields);
		ReadOnlySpan<char> readOnlySpan = default(ReadOnlySpan<char>);
		ReadOnlySpan<char> readOnlySpan2 = input;
		PemFields fields;
		Range label;
		int offset;
		int length;
		while (PemEncoding.TryFind(readOnlySpan2, out fields))
		{
			label = fields.Label;
			if (readOnlySpan2[label.Start..label.End].SequenceEqual("ENCRYPTED PRIVATE KEY".AsSpan()))
			{
				if (flag)
				{
					throw new ArgumentException(System.SR.Argument_PemImport_AmbiguousPem, "input");
				}
				flag = true;
				pemFields = fields;
				readOnlySpan = readOnlySpan2;
			}
			Index end = fields.Location.End;
			Index index = end;
			length = readOnlySpan2.Length;
			offset = index.GetOffset(length);
			readOnlySpan2 = readOnlySpan2.Slice(offset, length - offset);
		}
		if (!flag)
		{
			throw new ArgumentException(System.SR.Argument_PemImport_NoPemFound, "input");
		}
		label = pemFields.Base64Data;
		offset = readOnlySpan.Length;
		length = label.Start.GetOffset(offset);
		int bytesRead = label.End.GetOffset(offset) - length;
		ReadOnlySpan<char> chars = readOnlySpan.Slice(length, bytesRead);
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(pemFields.DecodedDataLength);
		int bytesWritten = 0;
		try
		{
			if (!Convert.TryFromBase64Chars(chars, array, out bytesWritten))
			{
				throw new ArgumentException();
			}
			Span<byte> span = array.AsSpan(0, bytesWritten);
			importAction(password, span, out bytesRead);
		}
		finally
		{
			System.Security.Cryptography.CryptoPool.Return(array, bytesWritten);
		}
	}

	public static void ImportPem(ReadOnlySpan<char> input, FindImportActionFunc callback)
	{
		ImportKeyAction importKeyAction = null;
		PemFields pemFields = default(PemFields);
		ReadOnlySpan<char> readOnlySpan = default(ReadOnlySpan<char>);
		bool flag = false;
		ReadOnlySpan<char> readOnlySpan2 = input;
		PemFields fields;
		Range label;
		int offset;
		int length;
		while (PemEncoding.TryFind(readOnlySpan2, out fields))
		{
			label = fields.Label;
			ReadOnlySpan<char> readOnlySpan3 = readOnlySpan2[label.Start..label.End];
			ImportKeyAction importKeyAction2 = callback(readOnlySpan3);
			if (importKeyAction2 != null)
			{
				if ((importKeyAction != null) | flag)
				{
					throw new ArgumentException(System.SR.Argument_PemImport_AmbiguousPem, "input");
				}
				importKeyAction = importKeyAction2;
				pemFields = fields;
				readOnlySpan = readOnlySpan2;
			}
			else if (readOnlySpan3.SequenceEqual("ENCRYPTED PRIVATE KEY".AsSpan()))
			{
				if ((importKeyAction != null) | flag)
				{
					throw new ArgumentException(System.SR.Argument_PemImport_AmbiguousPem, "input");
				}
				flag = true;
			}
			Index end = fields.Location.End;
			Index index = end;
			length = readOnlySpan2.Length;
			offset = index.GetOffset(length);
			readOnlySpan2 = readOnlySpan2.Slice(offset, length - offset);
		}
		if (flag)
		{
			throw new ArgumentException(System.SR.Argument_PemImport_EncryptedPem, "input");
		}
		if (importKeyAction == null)
		{
			throw new ArgumentException(System.SR.Argument_PemImport_NoPemFound, "input");
		}
		label = pemFields.Base64Data;
		offset = readOnlySpan.Length;
		length = label.Start.GetOffset(offset);
		int bytesRead = label.End.GetOffset(offset) - length;
		ReadOnlySpan<char> chars = readOnlySpan.Slice(length, bytesRead);
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(pemFields.DecodedDataLength);
		int bytesWritten = 0;
		try
		{
			if (!Convert.TryFromBase64Chars(chars, array, out bytesWritten))
			{
				throw new ArgumentException();
			}
			Span<byte> span = array.AsSpan(0, bytesWritten);
			importKeyAction(span, out bytesRead);
		}
		finally
		{
			System.Security.Cryptography.CryptoPool.Return(array, bytesWritten);
		}
	}
}
