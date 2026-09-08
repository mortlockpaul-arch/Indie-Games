using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

/// <summary>Represents a compressed file within a zip archive.</summary>
public class ZipArchiveEntry
{
	private sealed class DirectToArchiveWriterStream : Stream
	{
		private long _position;

		private readonly CheckSumAndSizeWriteStream _crcSizeStream;

		private bool _everWritten;

		private bool _isDisposed;

		private readonly ZipArchiveEntry _entry;

		private bool _usedZip64inLH;

		private bool _canWrite;

		public override long Length
		{
			get
			{
				ThrowIfDisposed();
				throw new NotSupportedException(System.SR.SeekingNotSupported);
			}
		}

		public override long Position
		{
			get
			{
				ThrowIfDisposed();
				return _position;
			}
			set
			{
				ThrowIfDisposed();
				throw new NotSupportedException(System.SR.SeekingNotSupported);
			}
		}

		public override bool CanRead => false;

		public override bool CanSeek => false;

		public override bool CanWrite => _canWrite;

		public DirectToArchiveWriterStream(CheckSumAndSizeWriteStream crcSizeStream, ZipArchiveEntry entry)
		{
			_position = 0L;
			_crcSizeStream = crcSizeStream;
			_everWritten = false;
			_isDisposed = false;
			_entry = entry;
			_usedZip64inLH = false;
			_canWrite = true;
		}

		private void ThrowIfDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(GetType().ToString(), System.SR.HiddenStreamName);
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			ThrowIfDisposed();
			throw new NotSupportedException(System.SR.ReadingNotSupported);
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			ThrowIfDisposed();
			throw new NotSupportedException(System.SR.ReadingNotSupported);
		}

		public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			ThrowIfDisposed();
			throw new NotSupportedException(System.SR.ReadingNotSupported);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			ThrowIfDisposed();
			throw new NotSupportedException(System.SR.SeekingNotSupported);
		}

		public override void SetLength(long value)
		{
			ThrowIfDisposed();
			throw new NotSupportedException(System.SR.SetLengthRequiresSeekingAndWriting);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			Stream.ValidateBufferArguments(buffer, offset, count);
			ThrowIfDisposed();
			if (count != 0)
			{
				if (!_everWritten)
				{
					_everWritten = true;
					_usedZip64inLH = _entry.WriteLocalFileHeader(isEmptyFile: false, forceWrite: true);
				}
				_crcSizeStream.Write(buffer, offset, count);
				_position += count;
			}
		}

		public override void Write(ReadOnlySpan<byte> source)
		{
			ThrowIfDisposed();
			if (source.Length != 0)
			{
				if (!_everWritten)
				{
					_everWritten = true;
					_usedZip64inLH = _entry.WriteLocalFileHeader(isEmptyFile: false, forceWrite: true);
				}
				_crcSizeStream.Write(source);
				_position += source.Length;
			}
		}

		public override void WriteByte(byte value)
		{
			Write(new ReadOnlySpan<byte>(in value));
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			Stream.ValidateBufferArguments(buffer, offset, count);
			return WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();
		}

		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			ThrowIfDisposed();
			if (buffer.IsEmpty)
			{
				return default(ValueTask);
			}
			return Core(buffer, cancellationToken);
			async ValueTask Core(ReadOnlyMemory<byte> buffer2, CancellationToken cancellationToken2)
			{
				if (!_everWritten)
				{
					_everWritten = true;
					_usedZip64inLH = await _entry.WriteLocalFileHeaderAsync(isEmptyFile: false, forceWrite: true, cancellationToken2).ConfigureAwait(continueOnCapturedContext: false);
				}
				await _crcSizeStream.WriteAsync(buffer2, cancellationToken2).ConfigureAwait(continueOnCapturedContext: false);
				_position += buffer2.Length;
			}
		}

		public override void Flush()
		{
			ThrowIfDisposed();
			_crcSizeStream.Flush();
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			ThrowIfDisposed();
			return _crcSizeStream.FlushAsync(cancellationToken);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_isDisposed)
			{
				_crcSizeStream.Dispose();
				if (!_everWritten)
				{
					_entry.WriteLocalFileHeader(isEmptyFile: true, forceWrite: true);
				}
				else if (_entry._archive.ArchiveStream.CanSeek)
				{
					_entry.WriteCrcAndSizesInLocalHeader(_usedZip64inLH);
				}
				else
				{
					_entry.WriteDataDescriptor();
				}
				_canWrite = false;
				_isDisposed = true;
			}
			base.Dispose(disposing);
		}

		public override async ValueTask DisposeAsync()
		{
			if (!_isDisposed)
			{
				await _crcSizeStream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				if (!_everWritten)
				{
					await _entry.WriteLocalFileHeaderAsync(isEmptyFile: true, forceWrite: true, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
				else if (!_entry._archive.ArchiveStream.CanSeek)
				{
					await _entry.WriteDataDescriptorAsync(default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
				else
				{
					await _entry.WriteCrcAndSizesInLocalHeaderAsync(_usedZip64inLH, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
				}
				_canWrite = false;
				_isDisposed = true;
			}
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	[Flags]
	internal enum BitFlagValues : ushort
	{
		IsEncrypted = 1,
		DataDescriptor = 8,
		UnicodeFileNameAndComment = 0x800
	}

	internal enum CompressionMethodValues : ushort
	{
		Stored = 0,
		Deflate = 8,
		Deflate64 = 9,
		BZip2 = 12,
		LZMA = 14
	}

	internal sealed class LocalHeaderOffsetComparer : Comparer<ZipArchiveEntry>
	{
		private static readonly LocalHeaderOffsetComparer s_instance = new LocalHeaderOffsetComparer();

		public static LocalHeaderOffsetComparer Instance => s_instance;

		public override int Compare(ZipArchiveEntry x, ZipArchiveEntry y)
		{
			long num = ((x != null && !x.OriginallyInArchive) ? long.MaxValue : (x?.OffsetOfLocalHeader ?? long.MinValue));
			long value = ((y != null && !y.OriginallyInArchive) ? long.MaxValue : (y?.OffsetOfLocalHeader ?? long.MinValue));
			return num.CompareTo(value);
		}
	}

	private ZipArchive _archive;

	private readonly bool _originallyInArchive;

	private readonly uint _diskNumberStart;

	private readonly ZipVersionMadeByPlatform _versionMadeByPlatform;

	private ZipVersionNeededValues _versionMadeBySpecification;

	private ZipVersionNeededValues _versionToExtract;

	private BitFlagValues _generalPurposeBitFlag;

	private readonly bool _isEncrypted;

	private CompressionMethodValues _storedCompressionMethod;

	private DateTimeOffset _lastModified;

	private long _compressedSize;

	private long _uncompressedSize;

	private long _offsetOfLocalHeader;

	private long? _storedOffsetOfCompressedData;

	private uint _crc32;

	private byte[][] _compressedBytes;

	private MemoryStream _storedUncompressedData;

	private bool _currentlyOpenForWrite;

	private bool _everOpenedForWrite;

	private Stream _outstandingWriteStream;

	private uint _externalFileAttr;

	private string _storedEntryName;

	private byte[] _storedEntryNameBytes;

	private List<ZipGenericExtraField> _cdUnknownExtraFields;

	private byte[] _cdTrailingExtraFieldData;

	private List<ZipGenericExtraField> _lhUnknownExtraFields;

	private byte[] _lhTrailingExtraFieldData;

	private byte[] _fileComment;

	private readonly CompressionLevel _compressionLevel;

	private static readonly bool s_allowLargeZipArchiveEntriesInUpdateMode = IntPtr.Size > 4;

	/// <summary>Gets the zip archive that the entry belongs to.</summary>
	/// <returns>The zip archive that the entry belongs to, or <see langword="null" /> if the entry has been deleted.</returns>
	public ZipArchive Archive => _archive;

	[CLSCompliant(false)]
	public uint Crc32 => _crc32;

	public bool IsEncrypted => _isEncrypted;

	/// <summary>Gets the compressed size of the entry in the zip archive.</summary>
	/// <returns>The compressed size of the entry in the zip archive.</returns>
	/// <exception cref="T:System.InvalidOperationException">The value of the property is not available because the entry has been modified.</exception>
	public long CompressedLength
	{
		get
		{
			if (_everOpenedForWrite)
			{
				throw new InvalidOperationException(System.SR.LengthAfterWrite);
			}
			return _compressedSize;
		}
	}

	/// <summary>
	/// 		  OS and Application specific file attributes.
	/// </summary>
	/// <returns>The external attributes written by the application when this entry was written. It is both host OS and application dependent.</returns>
	public int ExternalAttributes
	{
		get
		{
			return (int)_externalFileAttr;
		}
		set
		{
			ThrowIfInvalidArchive();
			_externalFileAttr = (uint)value;
			Changes |= ZipArchive.ChangeState.FixedLengthMetadata;
		}
	}

	public string Comment
	{
		get
		{
			return DecodeEntryString(_fileComment);
		}
		[param: AllowNull]
		set
		{
			_fileComment = ZipHelper.GetEncodedTruncatedBytesFromString(value, _archive.EntryNameAndCommentEncoding, 65535, out var isUTF);
			if (isUTF)
			{
				_generalPurposeBitFlag |= BitFlagValues.UnicodeFileNameAndComment;
			}
			Changes |= ZipArchive.ChangeState.DynamicLengthMetadata;
		}
	}

	/// <summary>Gets the relative path of the entry in the zip archive.</summary>
	/// <returns>The relative path of the entry in the zip archive.</returns>
	public string FullName
	{
		get
		{
			return _storedEntryName;
		}
		[MemberNotNull("_storedEntryNameBytes")]
		[MemberNotNull("_storedEntryName")]
		private set
		{
			ArgumentNullException.ThrowIfNull(value, "FullName");
			_storedEntryNameBytes = ZipHelper.GetEncodedTruncatedBytesFromString(value, _archive.EntryNameAndCommentEncoding, 0, out var isUTF);
			_storedEntryName = value;
			if (isUTF)
			{
				_generalPurposeBitFlag |= BitFlagValues.UnicodeFileNameAndComment;
			}
			else
			{
				_generalPurposeBitFlag &= ~BitFlagValues.UnicodeFileNameAndComment;
			}
			DetectEntryNameVersion();
		}
	}

	/// <summary>Gets or sets the last time the entry in the zip archive was changed.</summary>
	/// <returns>The last time the entry in the zip archive was changed.</returns>
	/// <exception cref="T:System.NotSupportedException">The attempt to set this property failed, because the zip archive for the entry is in <see cref="F:System.IO.Compression.ZipArchiveMode.Read" /> mode.</exception>
	/// <exception cref="T:System.IO.IOException">The archive mode is set to <see cref="F:System.IO.Compression.ZipArchiveMode.Create" />.- or -The archive mode is set to <see cref="F:System.IO.Compression.ZipArchiveMode.Update" /> and the entry has been opened.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">An attempt was made to set this property to a value that is either earlier than 1980 January 1 0:00:00 (midnight) or later than 2107 December 31 23:59:58 (one second before midnight).</exception>
	public DateTimeOffset LastWriteTime
	{
		get
		{
			return _lastModified;
		}
		set
		{
			ThrowIfInvalidArchive();
			if (_archive.Mode == ZipArchiveMode.Read)
			{
				throw new NotSupportedException(System.SR.ReadOnlyArchive);
			}
			if (_archive.Mode == ZipArchiveMode.Create && _everOpenedForWrite)
			{
				throw new IOException(System.SR.FrozenAfterWrite);
			}
			if (value.DateTime.Year < 1980 || value.DateTime.Year > 2107)
			{
				throw new ArgumentOutOfRangeException("value", System.SR.DateTimeOutOfRange);
			}
			_lastModified = value;
			Changes |= ZipArchive.ChangeState.FixedLengthMetadata;
		}
	}

	/// <summary>Gets the uncompressed size of the entry in the zip archive.</summary>
	/// <returns>The uncompressed size of the entry in the zip archive.</returns>
	/// <exception cref="T:System.InvalidOperationException">The value of the property is not available because the entry has been modified.</exception>
	public long Length
	{
		get
		{
			if (_everOpenedForWrite)
			{
				throw new InvalidOperationException(System.SR.LengthAfterWrite);
			}
			return _uncompressedSize;
		}
	}

	/// <summary>Gets the file name of the entry in the zip archive.</summary>
	/// <returns>The file name of the entry in the zip archive.</returns>
	public string Name => ParseFileName(FullName, _versionMadeByPlatform);

	internal ZipArchive.ChangeState Changes { get; private set; }

	internal bool OriginallyInArchive => _originallyInArchive;

	internal long OffsetOfLocalHeader => _offsetOfLocalHeader;

	internal bool EverOpenedForWrite => _everOpenedForWrite;

	private CompressionMethodValues CompressionMethod
	{
		get
		{
			return _storedCompressionMethod;
		}
		set
		{
			switch (value)
			{
			case CompressionMethodValues.Deflate:
				VersionToExtractAtLeast(ZipVersionNeededValues.ExplicitDirectory);
				break;
			case CompressionMethodValues.Deflate64:
				VersionToExtractAtLeast(ZipVersionNeededValues.Deflate64);
				break;
			}
			_storedCompressionMethod = value;
		}
	}

	private bool AreSizesTooLarge
	{
		get
		{
			if (_compressedSize <= uint.MaxValue)
			{
				return _uncompressedSize > uint.MaxValue;
			}
			return true;
		}
	}

	private bool IsOffsetTooLarge => _offsetOfLocalHeader > uint.MaxValue;

	private bool ShouldUseZIP64
	{
		get
		{
			if (!AreSizesTooLarge)
			{
				return IsOffsetTooLarge;
			}
			return true;
		}
	}

	internal ZipArchiveEntry(ZipArchive archive, ZipCentralDirectoryFileHeader cd)
	{
		_archive = archive;
		_originallyInArchive = true;
		Changes = ZipArchive.ChangeState.Unchanged;
		_diskNumberStart = cd.DiskNumberStart;
		_versionMadeByPlatform = (ZipVersionMadeByPlatform)cd.VersionMadeByCompatibility;
		_versionMadeBySpecification = (ZipVersionNeededValues)cd.VersionMadeBySpecification;
		_versionToExtract = (ZipVersionNeededValues)cd.VersionNeededToExtract;
		_generalPurposeBitFlag = (BitFlagValues)cd.GeneralPurposeBitFlag;
		_isEncrypted = (_generalPurposeBitFlag & BitFlagValues.IsEncrypted) != 0;
		CompressionMethod = (CompressionMethodValues)cd.CompressionMethod;
		_lastModified = new DateTimeOffset(ZipHelper.DosTimeToDateTime(cd.LastModified));
		_compressedSize = cd.CompressedSize;
		_uncompressedSize = cd.UncompressedSize;
		_externalFileAttr = cd.ExternalFileAttributes;
		_offsetOfLocalHeader = cd.RelativeOffsetOfLocalHeader;
		_storedOffsetOfCompressedData = null;
		_crc32 = cd.Crc32;
		_compressedBytes = null;
		_storedUncompressedData = null;
		_currentlyOpenForWrite = false;
		_everOpenedForWrite = false;
		_outstandingWriteStream = null;
		_storedEntryNameBytes = cd.Filename;
		_storedEntryName = DecodeEntryString(_storedEntryNameBytes);
		DetectEntryNameVersion();
		_lhUnknownExtraFields = null;
		_cdUnknownExtraFields = cd.ExtraFields;
		_cdTrailingExtraFieldData = cd.TrailingExtraFieldData;
		_fileComment = cd.FileComment;
		_compressionLevel = MapCompressionLevel(_generalPurposeBitFlag, CompressionMethod);
	}

	internal ZipArchiveEntry(ZipArchive archive, string entryName, CompressionLevel compressionLevel)
		: this(archive, entryName)
	{
		_compressionLevel = compressionLevel;
		if (_compressionLevel == CompressionLevel.NoCompression)
		{
			CompressionMethod = CompressionMethodValues.Stored;
		}
		_generalPurposeBitFlag = MapDeflateCompressionOption(_generalPurposeBitFlag, _compressionLevel, CompressionMethod);
	}

	internal ZipArchiveEntry(ZipArchive archive, string entryName)
	{
		_archive = archive;
		_originallyInArchive = false;
		_diskNumberStart = 0u;
		_versionMadeByPlatform = ZipVersionMadeByPlatform.Windows;
		_versionMadeBySpecification = ZipVersionNeededValues.Default;
		_versionToExtract = ZipVersionNeededValues.Default;
		_compressionLevel = CompressionLevel.Optimal;
		CompressionMethod = CompressionMethodValues.Deflate;
		_generalPurposeBitFlag = MapDeflateCompressionOption((BitFlagValues)0, _compressionLevel, CompressionMethod);
		_lastModified = DateTimeOffset.Now;
		_compressedSize = 0L;
		_uncompressedSize = 0L;
		_externalFileAttr = ((entryName.EndsWith(Path.DirectorySeparatorChar) || entryName.EndsWith(Path.AltDirectorySeparatorChar)) ? 0u : 0u);
		_offsetOfLocalHeader = 0L;
		_storedOffsetOfCompressedData = null;
		_crc32 = 0u;
		_compressedBytes = null;
		_storedUncompressedData = null;
		_currentlyOpenForWrite = false;
		_everOpenedForWrite = false;
		_outstandingWriteStream = null;
		FullName = entryName;
		_cdUnknownExtraFields = null;
		_lhUnknownExtraFields = null;
		_fileComment = Array.Empty<byte>();
		if (_storedEntryNameBytes.Length > 65535)
		{
			throw new ArgumentException(System.SR.EntryNamesTooLong);
		}
		if (_archive.Mode == ZipArchiveMode.Create)
		{
			_archive.AcquireArchiveStream(this);
		}
		Changes = ZipArchive.ChangeState.Unchanged;
	}

	/// <summary>Deletes the entry from the zip archive.</summary>
	/// <exception cref="T:System.IO.IOException">The entry is already open for reading or writing.</exception>
	/// <exception cref="T:System.NotSupportedException">The zip archive for this entry was opened in a mode other than <see cref="F:System.IO.Compression.ZipArchiveMode.Update" />. </exception>
	/// <exception cref="T:System.ObjectDisposedException">The zip archive for this entry has been disposed.</exception>
	public void Delete()
	{
		if (_archive != null)
		{
			if (_currentlyOpenForWrite)
			{
				throw new IOException(System.SR.DeleteOpenEntry);
			}
			if (_archive.Mode != ZipArchiveMode.Update)
			{
				throw new NotSupportedException(System.SR.DeleteOnlyInUpdate);
			}
			_archive.ThrowIfDisposed();
			_archive.RemoveEntry(this);
			_archive = null;
			UnloadStreams();
		}
	}

	/// <summary>Opens the entry from the zip archive.</summary>
	/// <returns>The stream that represents the contents of the entry.</returns>
	/// <exception cref="T:System.IO.IOException">The entry is already currently open for writing.-or-The entry has been deleted from the archive.-or-The archive for this entry was opened with the <see cref="F:System.IO.Compression.ZipArchiveMode.Create" /> mode, and this entry has already been written to. </exception>
	/// <exception cref="T:System.IO.InvalidDataException">The entry is either missing from the archive or is corrupt and cannot be read. -or-The entry has been compressed by using a compression method that is not supported.</exception>
	/// <exception cref="T:System.ObjectDisposedException">The zip archive for this entry has been disposed.</exception>
	public Stream Open()
	{
		ThrowIfInvalidArchive();
		return _archive.Mode switch
		{
			ZipArchiveMode.Read => OpenInReadMode(checkOpenable: true), 
			ZipArchiveMode.Create => OpenInWriteMode(), 
			_ => OpenInUpdateMode(), 
		};
	}

	/// <summary>Retrieves the relative path of the entry in the zip archive.</summary>
	/// <returns>The relative path of the entry, which is the value stored in the <see cref="P:System.IO.Compression.ZipArchiveEntry.FullName" /> property.</returns>
	public override string ToString()
	{
		return FullName;
	}

	private string DecodeEntryString(byte[] entryStringBytes)
	{
		return (((_generalPurposeBitFlag & BitFlagValues.UnicodeFileNameAndComment) == BitFlagValues.UnicodeFileNameAndComment) ? Encoding.UTF8 : (_archive?.EntryNameAndCommentEncoding ?? Encoding.UTF8)).GetString(entryStringBytes);
	}

	internal long GetOffsetOfCompressedData()
	{
		if (!_storedOffsetOfCompressedData.HasValue)
		{
			_archive.ArchiveStream.Seek(_offsetOfLocalHeader, SeekOrigin.Begin);
			if (!ZipLocalFileHeader.TrySkipBlock(_archive.ArchiveStream))
			{
				throw new InvalidDataException(System.SR.LocalFileHeaderCorrupt);
			}
			_storedOffsetOfCompressedData = _archive.ArchiveStream.Position;
		}
		return _storedOffsetOfCompressedData.Value;
	}

	private MemoryStream GetUncompressedData()
	{
		if (_storedUncompressedData == null)
		{
			_storedUncompressedData = new MemoryStream((int)_uncompressedSize);
			if (_originallyInArchive)
			{
				using Stream stream = OpenInReadMode(checkOpenable: false);
				try
				{
					stream.CopyTo(_storedUncompressedData);
				}
				catch (InvalidDataException)
				{
					_storedUncompressedData.Dispose();
					_storedUncompressedData = null;
					_currentlyOpenForWrite = false;
					_everOpenedForWrite = false;
					throw;
				}
			}
			if (CompressionMethod != CompressionMethodValues.Stored)
			{
				CompressionMethod = CompressionMethodValues.Deflate;
			}
		}
		return _storedUncompressedData;
	}

	internal void WriteAndFinishLocalEntry(bool forceWrite)
	{
		CloseStreams();
		WriteLocalFileHeaderAndDataIfNeeded(forceWrite);
		UnloadStreams();
	}

	private bool WriteCentralDirectoryFileHeaderInitialize(bool forceWrite, out Zip64ExtraField zip64ExtraField, out uint compressedSizeTruncated, out uint uncompressedSizeTruncated, out ushort extraFieldLength, out uint offsetOfLocalHeaderTruncated)
	{
		zip64ExtraField = null;
		if (AreSizesTooLarge)
		{
			compressedSizeTruncated = uint.MaxValue;
			uncompressedSizeTruncated = uint.MaxValue;
			zip64ExtraField = new Zip64ExtraField
			{
				CompressedSize = _compressedSize,
				UncompressedSize = _uncompressedSize
			};
		}
		else
		{
			compressedSizeTruncated = (uint)_compressedSize;
			uncompressedSizeTruncated = (uint)_uncompressedSize;
		}
		if (IsOffsetTooLarge)
		{
			offsetOfLocalHeaderTruncated = uint.MaxValue;
			if (zip64ExtraField == null)
			{
				zip64ExtraField = new Zip64ExtraField();
			}
			zip64ExtraField.LocalHeaderOffset = _offsetOfLocalHeader;
		}
		else
		{
			offsetOfLocalHeaderTruncated = (uint)_offsetOfLocalHeader;
		}
		if (zip64ExtraField != null)
		{
			VersionToExtractAtLeast(ZipVersionNeededValues.Zip64);
		}
		List<ZipGenericExtraField> cdUnknownExtraFields = _cdUnknownExtraFields;
		byte[] cdTrailingExtraFieldData = _cdTrailingExtraFieldData;
		int num = ZipGenericExtraField.TotalSize(cdUnknownExtraFields, (cdTrailingExtraFieldData != null) ? cdTrailingExtraFieldData.Length : 0);
		int num2 = ((zip64ExtraField != null) ? zip64ExtraField.TotalSize : 0) + num;
		if (num2 > 65535)
		{
			extraFieldLength = (ushort)((zip64ExtraField != null) ? zip64ExtraField.TotalSize : 0);
			_cdUnknownExtraFields = null;
		}
		else
		{
			extraFieldLength = (ushort)num2;
		}
		if (_originallyInArchive && Changes == ZipArchive.ChangeState.Unchanged && !forceWrite)
		{
			long offset = 46 + _storedEntryNameBytes.Length + ((zip64ExtraField != null) ? zip64ExtraField.TotalSize : 0) + num + _fileComment.Length;
			_archive.ArchiveStream.Seek(offset, SeekOrigin.Current);
			return false;
		}
		return true;
	}

	private void WriteCentralDirectoryFileHeaderPrepare(Span<byte> cdStaticHeader, uint compressedSizeTruncated, uint uncompressedSizeTruncated, ushort extraFieldLength, uint offsetOfLocalHeaderTruncated)
	{
		ReadOnlySpan<byte> signatureConstantBytes = ZipCentralDirectoryFileHeader.SignatureConstantBytes;
		signatureConstantBytes.CopyTo(cdStaticHeader.Slice(0, cdStaticHeader.Length));
		cdStaticHeader[4] = (byte)_versionMadeBySpecification;
		cdStaticHeader[5] = 0;
		BinaryPrimitives.WriteUInt16LittleEndian(cdStaticHeader.Slice(6, cdStaticHeader.Length - 6), (ushort)_versionToExtract);
		BinaryPrimitives.WriteUInt16LittleEndian(cdStaticHeader.Slice(8, cdStaticHeader.Length - 8), (ushort)_generalPurposeBitFlag);
		BinaryPrimitives.WriteUInt16LittleEndian(cdStaticHeader.Slice(10, cdStaticHeader.Length - 10), (ushort)CompressionMethod);
		BinaryPrimitives.WriteUInt32LittleEndian(cdStaticHeader.Slice(12, cdStaticHeader.Length - 12), ZipHelper.DateTimeToDosTime(_lastModified.DateTime));
		BinaryPrimitives.WriteUInt32LittleEndian(cdStaticHeader.Slice(16, cdStaticHeader.Length - 16), _crc32);
		BinaryPrimitives.WriteUInt32LittleEndian(cdStaticHeader.Slice(20, cdStaticHeader.Length - 20), compressedSizeTruncated);
		BinaryPrimitives.WriteUInt32LittleEndian(cdStaticHeader.Slice(24, cdStaticHeader.Length - 24), uncompressedSizeTruncated);
		BinaryPrimitives.WriteUInt16LittleEndian(cdStaticHeader.Slice(28, cdStaticHeader.Length - 28), (ushort)_storedEntryNameBytes.Length);
		BinaryPrimitives.WriteUInt16LittleEndian(cdStaticHeader.Slice(30, cdStaticHeader.Length - 30), extraFieldLength);
		BinaryPrimitives.WriteUInt16LittleEndian(cdStaticHeader.Slice(32, cdStaticHeader.Length - 32), (ushort)_fileComment.Length);
		BinaryPrimitives.WriteUInt16LittleEndian(cdStaticHeader.Slice(34, cdStaticHeader.Length - 34), 0);
		BinaryPrimitives.WriteUInt16LittleEndian(cdStaticHeader.Slice(36, cdStaticHeader.Length - 36), 0);
		BinaryPrimitives.WriteUInt32LittleEndian(cdStaticHeader.Slice(38, cdStaticHeader.Length - 38), _externalFileAttr);
		BinaryPrimitives.WriteUInt32LittleEndian(cdStaticHeader.Slice(42, cdStaticHeader.Length - 42), offsetOfLocalHeaderTruncated);
	}

	internal void WriteCentralDirectoryFileHeader(bool forceWrite)
	{
		if (WriteCentralDirectoryFileHeaderInitialize(forceWrite, out var zip64ExtraField, out var compressedSizeTruncated, out var uncompressedSizeTruncated, out var extraFieldLength, out var offsetOfLocalHeaderTruncated))
		{
			Span<byte> span = stackalloc byte[46];
			WriteCentralDirectoryFileHeaderPrepare(span, compressedSizeTruncated, uncompressedSizeTruncated, extraFieldLength, offsetOfLocalHeaderTruncated);
			_archive.ArchiveStream.Write(span);
			_archive.ArchiveStream.Write(_storedEntryNameBytes);
			zip64ExtraField?.WriteBlock(_archive.ArchiveStream);
			ZipGenericExtraField.WriteAllBlocks(_cdUnknownExtraFields, _cdTrailingExtraFieldData ?? Array.Empty<byte>(), _archive.ArchiveStream);
			if (_fileComment.Length != 0)
			{
				_archive.ArchiveStream.Write(_fileComment);
			}
		}
	}

	internal void LoadLocalHeaderExtraFieldIfNeeded()
	{
		if (_originallyInArchive)
		{
			_archive.ArchiveStream.Seek(_offsetOfLocalHeader, SeekOrigin.Begin);
			_lhUnknownExtraFields = ZipLocalFileHeader.GetExtraFields(_archive.ArchiveStream, out _lhTrailingExtraFieldData);
		}
	}

	private byte[][] LoadCompressedBytesIfNeededInitialize(out int maxSingleBufferSize)
	{
		maxSingleBufferSize = Array.MaxLength;
		byte[][] array = new byte[_compressedSize / maxSingleBufferSize + 1][];
		for (int i = 0; i < array.Length - 1; i++)
		{
			array[i] = new byte[maxSingleBufferSize];
		}
		array[^1] = new byte[_compressedSize % maxSingleBufferSize];
		return array;
	}

	internal void LoadCompressedBytesIfNeeded()
	{
		if (!_everOpenedForWrite && _originallyInArchive)
		{
			_compressedBytes = LoadCompressedBytesIfNeededInitialize(out var maxSingleBufferSize);
			_archive.ArchiveStream.Seek(GetOffsetOfCompressedData(), SeekOrigin.Begin);
			for (int i = 0; i < _compressedBytes.Length - 1; i++)
			{
				_archive.ArchiveStream.ReadAtLeast(_compressedBytes[i], maxSingleBufferSize);
			}
			_archive.ArchiveStream.ReadAtLeast(_compressedBytes[_compressedBytes.Length - 1], (int)(_compressedSize % maxSingleBufferSize));
		}
	}

	internal void ThrowIfNotOpenable(bool needToUncompress, bool needToLoadIntoMemory)
	{
		if (!IsOpenable(needToUncompress, needToLoadIntoMemory, out var message))
		{
			throw new InvalidDataException(message);
		}
	}

	private void DetectEntryNameVersion()
	{
		if (ParseFileName(_storedEntryName, _versionMadeByPlatform) == "")
		{
			VersionToExtractAtLeast(ZipVersionNeededValues.ExplicitDirectory);
		}
	}

	private CheckSumAndSizeWriteStream GetDataCompressor(Stream backingStream, bool leaveBackingStreamOpen, EventHandler onClose)
	{
		bool flag = true;
		Stream baseStream;
		switch (CompressionMethod)
		{
		case CompressionMethodValues.Stored:
			baseStream = backingStream;
			flag = false;
			break;
		default:
			baseStream = new DeflateStream(backingStream, _compressionLevel, leaveBackingStreamOpen);
			break;
		}
		bool leaveOpenOnClose = leaveBackingStreamOpen && !flag;
		return new CheckSumAndSizeWriteStream(baseStream, backingStream, leaveOpenOnClose, this, onClose, delegate(long initialPosition, long currentPosition, uint checkSum, Stream backing, ZipArchiveEntry thisRef, EventHandler closeHandler)
		{
			thisRef._crc32 = checkSum;
			thisRef._uncompressedSize = currentPosition;
			thisRef._compressedSize = backing.Position - initialPosition;
			closeHandler?.Invoke(thisRef, EventArgs.Empty);
		});
	}

	private Stream GetDataDecompressor(Stream compressedStreamToRead)
	{
		return CompressionMethod switch
		{
			CompressionMethodValues.Deflate => new DeflateStream(compressedStreamToRead, CompressionMode.Decompress, _uncompressedSize), 
			CompressionMethodValues.Deflate64 => new DeflateManagedStream(compressedStreamToRead, CompressionMethodValues.Deflate64, _uncompressedSize), 
			_ => compressedStreamToRead, 
		};
	}

	private Stream OpenInReadMode(bool checkOpenable)
	{
		if (checkOpenable)
		{
			ThrowIfNotOpenable(needToUncompress: true, needToLoadIntoMemory: false);
		}
		return OpenInReadModeGetDataCompressor(GetOffsetOfCompressedData());
	}

	private Stream OpenInReadModeGetDataCompressor(long offsetOfCompressedData)
	{
		Stream compressedStreamToRead = new SubReadStream(_archive.ArchiveStream, offsetOfCompressedData, _compressedSize);
		return GetDataDecompressor(compressedStreamToRead);
	}

	private WrappedStream OpenInWriteMode()
	{
		if (_everOpenedForWrite)
		{
			throw new IOException(System.SR.CreateModeWriteOnceAndOneEntryAtATime);
		}
		_everOpenedForWrite = true;
		Changes |= ZipArchive.ChangeState.StoredData;
		CheckSumAndSizeWriteStream dataCompressor = GetDataCompressor(_archive.ArchiveStream, leaveBackingStreamOpen: true, delegate(object o, EventArgs e)
		{
			ZipArchiveEntry zipArchiveEntry = (ZipArchiveEntry)o;
			zipArchiveEntry._archive.ReleaseArchiveStream(zipArchiveEntry);
			zipArchiveEntry._outstandingWriteStream = null;
		});
		_outstandingWriteStream = new DirectToArchiveWriterStream(dataCompressor, this);
		return new WrappedStream(_outstandingWriteStream, closeBaseStream: true);
	}

	private WrappedStream OpenInUpdateMode()
	{
		if (_currentlyOpenForWrite)
		{
			throw new IOException(System.SR.UpdateModeOneStream);
		}
		ThrowIfNotOpenable(needToUncompress: true, needToLoadIntoMemory: true);
		_everOpenedForWrite = true;
		Changes |= ZipArchive.ChangeState.StoredData;
		_currentlyOpenForWrite = true;
		MemoryStream uncompressedData = GetUncompressedData();
		uncompressedData.Seek(0L, SeekOrigin.Begin);
		return new WrappedStream(uncompressedData, this, delegate(ZipArchiveEntry thisRef)
		{
			thisRef._currentlyOpenForWrite = false;
		});
	}

	private bool IsOpenable(bool needToUncompress, bool needToLoadIntoMemory, out string message)
	{
		message = null;
		if (_originallyInArchive)
		{
			if (!IsOpenableInitialVerifications(needToUncompress, out message))
			{
				return false;
			}
			if (!ZipLocalFileHeader.TrySkipBlock(_archive.ArchiveStream))
			{
				message = System.SR.LocalFileHeaderCorrupt;
				return false;
			}
			long offsetOfCompressedData = GetOffsetOfCompressedData();
			if (!IsOpenableFinalVerifications(needToLoadIntoMemory, offsetOfCompressedData, out message))
			{
				return false;
			}
			return true;
		}
		return true;
	}

	private bool IsOpenableInitialVerifications(bool needToUncompress, out string message)
	{
		message = null;
		if (needToUncompress && CompressionMethod != CompressionMethodValues.Stored && CompressionMethod != CompressionMethodValues.Deflate && CompressionMethod != CompressionMethodValues.Deflate64)
		{
			CompressionMethodValues compressionMethod = CompressionMethod;
			string text = ((compressionMethod != CompressionMethodValues.BZip2 && compressionMethod != CompressionMethodValues.LZMA) ? System.SR.UnsupportedCompression : System.SR.Format(System.SR.UnsupportedCompressionMethod, CompressionMethod.ToString()));
			message = text;
			return false;
		}
		if (_diskNumberStart != _archive.NumberOfThisDisk)
		{
			message = System.SR.SplitSpanned;
			return false;
		}
		if (_offsetOfLocalHeader > _archive.ArchiveStream.Length)
		{
			message = System.SR.LocalFileHeaderCorrupt;
			return false;
		}
		_archive.ArchiveStream.Seek(_offsetOfLocalHeader, SeekOrigin.Begin);
		return true;
	}

	private bool IsOpenableFinalVerifications(bool needToLoadIntoMemory, long offsetOfCompressedData, out string message)
	{
		message = null;
		if (offsetOfCompressedData + _compressedSize > _archive.ArchiveStream.Length)
		{
			message = System.SR.LocalFileHeaderCorrupt;
			return false;
		}
		if (needToLoadIntoMemory && _compressedSize > int.MaxValue && !s_allowLargeZipArchiveEntriesInUpdateMode)
		{
			message = System.SR.EntryTooLarge;
			return false;
		}
		return true;
	}

	private static CompressionLevel MapCompressionLevel(BitFlagValues generalPurposeBitFlag, CompressionMethodValues compressionMethod)
	{
		if (compressionMethod == CompressionMethodValues.Deflate || compressionMethod == CompressionMethodValues.Deflate64)
		{
			return (int)(generalPurposeBitFlag & (BitFlagValues)6) switch
			{
				0 => CompressionLevel.Optimal, 
				2 => CompressionLevel.SmallestSize, 
				4 => CompressionLevel.Fastest, 
				6 => CompressionLevel.Fastest, 
				_ => CompressionLevel.Optimal, 
			};
		}
		return CompressionLevel.NoCompression;
	}

	private static BitFlagValues MapDeflateCompressionOption(BitFlagValues generalPurposeBitFlag, CompressionLevel compressionLevel, CompressionMethodValues compressionMethod)
	{
		int num = ((compressionMethod == CompressionMethodValues.Deflate || compressionMethod == CompressionMethodValues.Deflate64) ? (compressionLevel switch
		{
			CompressionLevel.Optimal => 0, 
			CompressionLevel.SmallestSize => 2, 
			CompressionLevel.Fastest => 6, 
			CompressionLevel.NoCompression => 6, 
			_ => 0, 
		}) : 0);
		ushort num2 = (ushort)num;
		return (BitFlagValues)(((uint)generalPurposeBitFlag & 0xFFFFFFF9u) | num2);
	}

	private bool WriteLocalFileHeaderInitialize(bool isEmptyFile, bool forceWrite, out Zip64ExtraField zip64ExtraField, out uint compressedSizeTruncated, out uint uncompressedSizeTruncated, out ushort extraFieldLength)
	{
		zip64ExtraField = null;
		_offsetOfLocalHeader = _archive.ArchiveStream.Position;
		if (isEmptyFile)
		{
			CompressionMethod = CompressionMethodValues.Stored;
			compressedSizeTruncated = 0u;
			uncompressedSizeTruncated = 0u;
		}
		else if (_archive.Mode == ZipArchiveMode.Create && !_archive.ArchiveStream.CanSeek)
		{
			_generalPurposeBitFlag |= BitFlagValues.DataDescriptor;
			compressedSizeTruncated = 0u;
			uncompressedSizeTruncated = 0u;
		}
		else
		{
			_generalPurposeBitFlag &= ~BitFlagValues.DataDescriptor;
			if (ShouldUseZIP64)
			{
				compressedSizeTruncated = uint.MaxValue;
				uncompressedSizeTruncated = uint.MaxValue;
				zip64ExtraField = new Zip64ExtraField
				{
					CompressedSize = _compressedSize,
					UncompressedSize = _uncompressedSize
				};
				VersionToExtractAtLeast(ZipVersionNeededValues.Zip64);
			}
			else
			{
				compressedSizeTruncated = (uint)_compressedSize;
				uncompressedSizeTruncated = (uint)_uncompressedSize;
			}
		}
		_offsetOfLocalHeader = _archive.ArchiveStream.Position;
		List<ZipGenericExtraField> lhUnknownExtraFields = _lhUnknownExtraFields;
		byte[] lhTrailingExtraFieldData = _lhTrailingExtraFieldData;
		int num = ZipGenericExtraField.TotalSize(lhUnknownExtraFields, (lhTrailingExtraFieldData != null) ? lhTrailingExtraFieldData.Length : 0);
		int num2 = ((zip64ExtraField != null) ? zip64ExtraField.TotalSize : 0) + num;
		if (num2 > 65535)
		{
			extraFieldLength = (ushort)((zip64ExtraField != null) ? zip64ExtraField.TotalSize : 0);
			_lhUnknownExtraFields = null;
		}
		else
		{
			extraFieldLength = (ushort)num2;
		}
		if (_originallyInArchive && Changes == ZipArchive.ChangeState.Unchanged && !forceWrite)
		{
			_archive.ArchiveStream.Seek(30 + _storedEntryNameBytes.Length, SeekOrigin.Current);
			if (zip64ExtraField != null)
			{
				_archive.ArchiveStream.Seek(zip64ExtraField.TotalSize, SeekOrigin.Current);
			}
			_archive.ArchiveStream.Seek(num, SeekOrigin.Current);
			return false;
		}
		return true;
	}

	private void WriteLocalFileHeaderPrepare(Span<byte> lfStaticHeader, uint compressedSizeTruncated, uint uncompressedSizeTruncated, ushort extraFieldLength)
	{
		ReadOnlySpan<byte> signatureConstantBytes = ZipLocalFileHeader.SignatureConstantBytes;
		signatureConstantBytes.CopyTo(lfStaticHeader.Slice(0, lfStaticHeader.Length));
		BinaryPrimitives.WriteUInt16LittleEndian(lfStaticHeader.Slice(4, lfStaticHeader.Length - 4), (ushort)_versionToExtract);
		BinaryPrimitives.WriteUInt16LittleEndian(lfStaticHeader.Slice(6, lfStaticHeader.Length - 6), (ushort)_generalPurposeBitFlag);
		BinaryPrimitives.WriteUInt16LittleEndian(lfStaticHeader.Slice(8, lfStaticHeader.Length - 8), (ushort)CompressionMethod);
		BinaryPrimitives.WriteUInt32LittleEndian(lfStaticHeader.Slice(10, lfStaticHeader.Length - 10), ZipHelper.DateTimeToDosTime(_lastModified.DateTime));
		BinaryPrimitives.WriteUInt32LittleEndian(lfStaticHeader.Slice(14, lfStaticHeader.Length - 14), _crc32);
		BinaryPrimitives.WriteUInt32LittleEndian(lfStaticHeader.Slice(18, lfStaticHeader.Length - 18), compressedSizeTruncated);
		BinaryPrimitives.WriteUInt32LittleEndian(lfStaticHeader.Slice(22, lfStaticHeader.Length - 22), uncompressedSizeTruncated);
		BinaryPrimitives.WriteUInt16LittleEndian(lfStaticHeader.Slice(26, lfStaticHeader.Length - 26), (ushort)_storedEntryNameBytes.Length);
		BinaryPrimitives.WriteUInt16LittleEndian(lfStaticHeader.Slice(28, lfStaticHeader.Length - 28), extraFieldLength);
	}

	private bool WriteLocalFileHeader(bool isEmptyFile, bool forceWrite)
	{
		if (WriteLocalFileHeaderInitialize(isEmptyFile, forceWrite, out var zip64ExtraField, out var compressedSizeTruncated, out var uncompressedSizeTruncated, out var extraFieldLength))
		{
			Span<byte> span = stackalloc byte[30];
			WriteLocalFileHeaderPrepare(span, compressedSizeTruncated, uncompressedSizeTruncated, extraFieldLength);
			_archive.ArchiveStream.Write(span);
			_archive.ArchiveStream.Write(_storedEntryNameBytes);
			zip64ExtraField?.WriteBlock(_archive.ArchiveStream);
			ZipGenericExtraField.WriteAllBlocks(_lhUnknownExtraFields, _lhTrailingExtraFieldData ?? Array.Empty<byte>(), _archive.ArchiveStream);
		}
		return zip64ExtraField != null;
	}

	private void WriteLocalFileHeaderAndDataIfNeeded(bool forceWrite)
	{
		if (_storedUncompressedData != null || _compressedBytes != null)
		{
			if (_storedUncompressedData != null)
			{
				_uncompressedSize = _storedUncompressedData.Length;
				using DirectToArchiveWriterStream destination = new DirectToArchiveWriterStream(GetDataCompressor(_archive.ArchiveStream, leaveBackingStreamOpen: true, null), this);
				_storedUncompressedData.Seek(0L, SeekOrigin.Begin);
				_storedUncompressedData.CopyTo(destination);
				_storedUncompressedData.Dispose();
				_storedUncompressedData = null;
				return;
			}
			if (_uncompressedSize == 0L)
			{
				_compressedSize = 0L;
			}
			WriteLocalFileHeader(_uncompressedSize == 0, forceWrite: true);
			if (_uncompressedSize != 0L)
			{
				byte[][] compressedBytes = _compressedBytes;
				foreach (byte[] array in compressedBytes)
				{
					_archive.ArchiveStream.Write(array, 0, array.Length);
				}
			}
		}
		else if (_archive.Mode == ZipArchiveMode.Update || !_everOpenedForWrite)
		{
			_everOpenedForWrite = true;
			WriteLocalFileHeader(_uncompressedSize == 0, forceWrite);
			if (_compressedSize != 0L)
			{
				_archive.ArchiveStream.Seek(_compressedSize, SeekOrigin.Current);
			}
		}
	}

	private void WriteCrcAndSizesInLocalHeader(bool zip64HeaderUsed)
	{
		Span<byte> writeBuffer = stackalloc byte[20];
		WriteCrcAndSizesInLocalHeaderInitialize(zip64HeaderUsed, out var finalPosition, out var pretendStreaming, out var compressedSizeTruncated, out var uncompressedSizeTruncated);
		if (pretendStreaming)
		{
			WriteCrcAndSizesInLocalHeaderPrepareForZip64PretendStreaming(writeBuffer);
			_archive.ArchiveStream.Write(writeBuffer.Slice(0, 4));
		}
		WriteCrcAndSizesInLocalHeaderPrepareFor32bitValuesWriting(pretendStreaming, writeBuffer, compressedSizeTruncated, uncompressedSizeTruncated);
		_archive.ArchiveStream.Write(writeBuffer.Slice(0, 12));
		if (zip64HeaderUsed)
		{
			WriteCrcAndSizesInLocalHeaderPrepareForWritingWhenZip64HeaderUsed(writeBuffer);
			_archive.ArchiveStream.Write(writeBuffer.Slice(0, 16));
		}
		_archive.ArchiveStream.Seek(finalPosition, SeekOrigin.Begin);
		if (pretendStreaming)
		{
			WriteCrcAndSizesInLocalHeaderPrepareForWritingDataDescriptor(writeBuffer);
			_archive.ArchiveStream.Write(writeBuffer.Slice(0, 20));
		}
	}

	private void WriteCrcAndSizesInLocalHeaderInitialize(bool zip64HeaderUsed, out long finalPosition, out bool pretendStreaming, out uint compressedSizeTruncated, out uint uncompressedSizeTruncated)
	{
		finalPosition = _archive.ArchiveStream.Position;
		bool shouldUseZIP = ShouldUseZIP64;
		pretendStreaming = shouldUseZIP && !zip64HeaderUsed;
		compressedSizeTruncated = (uint)(shouldUseZIP ? uint.MaxValue : _compressedSize);
		uncompressedSizeTruncated = (uint)(shouldUseZIP ? uint.MaxValue : _uncompressedSize);
	}

	private void WriteCrcAndSizesInLocalHeaderPrepareForZip64PretendStreaming(Span<byte> writeBuffer)
	{
		int num = 2;
		VersionToExtractAtLeast(ZipVersionNeededValues.Zip64);
		_generalPurposeBitFlag |= BitFlagValues.DataDescriptor;
		_archive.ArchiveStream.Seek(_offsetOfLocalHeader + 4, SeekOrigin.Begin);
		int num2 = 0;
		BinaryPrimitives.WriteUInt16LittleEndian(writeBuffer.Slice(num2, writeBuffer.Length - num2), (ushort)_versionToExtract);
		num2 = num;
		BinaryPrimitives.WriteUInt16LittleEndian(writeBuffer.Slice(num2, writeBuffer.Length - num2), (ushort)_generalPurposeBitFlag);
	}

	private void WriteCrcAndSizesInLocalHeaderPrepareFor32bitValuesWriting(bool pretendStreaming, Span<byte> writeBuffer, uint compressedSizeTruncated, uint uncompressedSizeTruncated)
	{
		_archive.ArchiveStream.Seek(_offsetOfLocalHeader + 14, SeekOrigin.Begin);
		if (!pretendStreaming)
		{
			int num = 4;
			int num2 = 8;
			int num3 = 0;
			BinaryPrimitives.WriteUInt32LittleEndian(writeBuffer.Slice(num3, writeBuffer.Length - num3), _crc32);
			num3 = num;
			BinaryPrimitives.WriteUInt32LittleEndian(writeBuffer.Slice(num3, writeBuffer.Length - num3), compressedSizeTruncated);
			num3 = num2;
			BinaryPrimitives.WriteUInt32LittleEndian(writeBuffer.Slice(num3, writeBuffer.Length - num3), uncompressedSizeTruncated);
		}
		else
		{
			writeBuffer.Slice(0, 12).Clear();
		}
	}

	private void WriteCrcAndSizesInLocalHeaderPrepareForWritingWhenZip64HeaderUsed(Span<byte> writeBuffer)
	{
		int num = 8;
		_archive.ArchiveStream.Seek(_offsetOfLocalHeader + 30 + _storedEntryNameBytes.Length + 4, SeekOrigin.Begin);
		int num2 = 0;
		BinaryPrimitives.WriteInt64LittleEndian(writeBuffer.Slice(num2, writeBuffer.Length - num2), _uncompressedSize);
		num2 = num;
		BinaryPrimitives.WriteInt64LittleEndian(writeBuffer.Slice(num2, writeBuffer.Length - num2), _compressedSize);
	}

	private void WriteCrcAndSizesInLocalHeaderPrepareForWritingDataDescriptor(Span<byte> writeBuffer)
	{
		int start = 0;
		int start2 = 4;
		int start3 = 12;
		BinaryPrimitives.WriteUInt32LittleEndian(writeBuffer.Slice(start), _crc32);
		BinaryPrimitives.WriteInt64LittleEndian(writeBuffer.Slice(start2), _compressedSize);
		BinaryPrimitives.WriteInt64LittleEndian(writeBuffer.Slice(start3), _uncompressedSize);
	}

	private void WriteDataDescriptor()
	{
		Span<byte> dataDescriptor = stackalloc byte[24];
		int length = PrepareToWriteDataDescriptor(dataDescriptor);
		_archive.ArchiveStream.Write(dataDescriptor.Slice(0, length));
	}

	private int PrepareToWriteDataDescriptor(Span<byte> dataDescriptor)
	{
		ReadOnlySpan<byte> dataDescriptorSignatureConstantBytes = ZipLocalFileHeader.DataDescriptorSignatureConstantBytes;
		dataDescriptorSignatureConstantBytes.CopyTo(dataDescriptor.Slice(0, dataDescriptor.Length));
		BinaryPrimitives.WriteUInt32LittleEndian(dataDescriptor.Slice(4, dataDescriptor.Length - 4), _crc32);
		if (AreSizesTooLarge)
		{
			BinaryPrimitives.WriteInt64LittleEndian(dataDescriptor.Slice(8, dataDescriptor.Length - 8), _compressedSize);
			BinaryPrimitives.WriteInt64LittleEndian(dataDescriptor.Slice(16, dataDescriptor.Length - 16), _uncompressedSize);
			return 24;
		}
		BinaryPrimitives.WriteUInt32LittleEndian(dataDescriptor.Slice(8, dataDescriptor.Length - 8), (uint)_compressedSize);
		BinaryPrimitives.WriteUInt32LittleEndian(dataDescriptor.Slice(12, dataDescriptor.Length - 12), (uint)_uncompressedSize);
		return 16;
	}

	private void UnloadStreams()
	{
		_storedUncompressedData?.Dispose();
		_compressedBytes = null;
		_outstandingWriteStream = null;
	}

	private void CloseStreams()
	{
		_outstandingWriteStream?.Dispose();
	}

	private void VersionToExtractAtLeast(ZipVersionNeededValues value)
	{
		if ((int)_versionToExtract < (int)value)
		{
			_versionToExtract = value;
			Changes |= ZipArchive.ChangeState.FixedLengthMetadata;
		}
		if ((int)_versionMadeBySpecification < (int)value)
		{
			_versionMadeBySpecification = value;
			Changes |= ZipArchive.ChangeState.FixedLengthMetadata;
		}
	}

	private void ThrowIfInvalidArchive()
	{
		if (_archive == null)
		{
			throw new InvalidOperationException(System.SR.DeletedEntry);
		}
		_archive.ThrowIfDisposed();
	}

	private static string GetFileName_Windows(string path)
	{
		int num = path.AsSpan().LastIndexOfAny('\\', '/', ':');
		if (num < 0)
		{
			return path;
		}
		return path.Substring(num + 1);
	}

	private static string GetFileName_Unix(string path)
	{
		int num = path.LastIndexOf('/');
		if (num < 0)
		{
			return path;
		}
		return path.Substring(num + 1);
	}

	public async Task<Stream> OpenAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfInvalidArchive();
		return _archive.Mode switch
		{
			ZipArchiveMode.Read => await OpenInReadModeAsync(checkOpenable: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), 
			ZipArchiveMode.Create => OpenInWriteMode(), 
			_ => await OpenInUpdateModeAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false), 
		};
	}

	internal async Task<long> GetOffsetOfCompressedDataAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (!_storedOffsetOfCompressedData.HasValue)
		{
			_archive.ArchiveStream.Seek(_offsetOfLocalHeader, SeekOrigin.Begin);
			if (!(await ZipLocalFileHeader.TrySkipBlockAsync(_archive.ArchiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)))
			{
				throw new InvalidDataException(System.SR.LocalFileHeaderCorrupt);
			}
			_storedOffsetOfCompressedData = _archive.ArchiveStream.Position;
		}
		return _storedOffsetOfCompressedData.Value;
	}

	private async Task<MemoryStream> GetUncompressedDataAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (_storedUncompressedData == null)
		{
			_storedUncompressedData = new MemoryStream((int)_uncompressedSize);
			if (_originallyInArchive)
			{
				Stream stream = await OpenInReadModeAsync(checkOpenable: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				await using (stream)
				{
					try
					{
						await stream.CopyToAsync(_storedUncompressedData, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (InvalidDataException)
					{
						await _storedUncompressedData.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
						_storedUncompressedData = null;
						_currentlyOpenForWrite = false;
						_everOpenedForWrite = false;
						throw;
					}
				}
			}
			if (CompressionMethod != CompressionMethodValues.Stored)
			{
				CompressionMethod = CompressionMethodValues.Deflate;
			}
		}
		return _storedUncompressedData;
	}

	internal async Task WriteAndFinishLocalEntryAsync(bool forceWrite, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		await CloseStreamsAsync().ConfigureAwait(continueOnCapturedContext: false);
		await WriteLocalFileHeaderAndDataIfNeededAsync(forceWrite, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		await UnloadStreamsAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	internal async Task WriteCentralDirectoryFileHeaderAsync(bool forceWrite, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (WriteCentralDirectoryFileHeaderInitialize(forceWrite, out var zip64ExtraField, out var compressedSizeTruncated, out var uncompressedSizeTruncated, out var extraFieldLength, out var offsetOfLocalHeaderTruncated))
		{
			byte[] array = new byte[46];
			WriteCentralDirectoryFileHeaderPrepare(array, compressedSizeTruncated, uncompressedSizeTruncated, extraFieldLength, offsetOfLocalHeaderTruncated);
			await _archive.ArchiveStream.WriteAsync(array, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			await _archive.ArchiveStream.WriteAsync(_storedEntryNameBytes, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (zip64ExtraField != null)
			{
				await zip64ExtraField.WriteBlockAsync(_archive.ArchiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			await ZipGenericExtraField.WriteAllBlocksAsync(_cdUnknownExtraFields, _cdTrailingExtraFieldData ?? Array.Empty<byte>(), _archive.ArchiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (_fileComment.Length != 0)
			{
				await _archive.ArchiveStream.WriteAsync(_fileComment, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
	}

	internal async Task LoadLocalHeaderExtraFieldIfNeededAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (_originallyInArchive)
		{
			_archive.ArchiveStream.Seek(_offsetOfLocalHeader, SeekOrigin.Begin);
			(_lhUnknownExtraFields, _lhTrailingExtraFieldData) = await ZipLocalFileHeader.GetExtraFieldsAsync(_archive.ArchiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	internal async Task LoadCompressedBytesIfNeededAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (!_everOpenedForWrite && _originallyInArchive)
		{
			_compressedBytes = LoadCompressedBytesIfNeededInitialize(out var maxSingleBufferSize);
			Stream archiveStream = _archive.ArchiveStream;
			archiveStream.Seek(await GetOffsetOfCompressedDataAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false), SeekOrigin.Begin);
			for (int i = 0; i < _compressedBytes.Length - 1; i++)
			{
				await _archive.ArchiveStream.ReadAtLeastAsync(_compressedBytes[i], maxSingleBufferSize, throwOnEndOfStream: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			await _archive.ArchiveStream.ReadAtLeastAsync(_compressedBytes[_compressedBytes.Length - 1], (int)(_compressedSize % maxSingleBufferSize), throwOnEndOfStream: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	internal async Task ThrowIfNotOpenableAsync(bool needToUncompress, bool needToLoadIntoMemory, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var (flag, message) = await IsOpenableAsync(needToUncompress, needToLoadIntoMemory, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (!flag)
		{
			throw new InvalidDataException(message);
		}
	}

	private async Task<Stream> OpenInReadModeAsync(bool checkOpenable, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (checkOpenable)
		{
			await ThrowIfNotOpenableAsync(needToUncompress: true, needToLoadIntoMemory: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		return OpenInReadModeGetDataCompressor(await GetOffsetOfCompressedDataAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
	}

	private async Task<WrappedStream> OpenInUpdateModeAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (_currentlyOpenForWrite)
		{
			throw new IOException(System.SR.UpdateModeOneStream);
		}
		await ThrowIfNotOpenableAsync(needToUncompress: true, needToLoadIntoMemory: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		_everOpenedForWrite = true;
		Changes |= ZipArchive.ChangeState.StoredData;
		_currentlyOpenForWrite = true;
		MemoryStream obj = await GetUncompressedDataAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		obj.Seek(0L, SeekOrigin.Begin);
		return new WrappedStream(obj, this, delegate(ZipArchiveEntry thisRef)
		{
			thisRef._currentlyOpenForWrite = false;
		});
	}

	private async Task<(bool, string)> IsOpenableAsync(bool needToUncompress, bool needToLoadIntoMemory, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		string message = null;
		if (!_originallyInArchive)
		{
			return (true, message);
		}
		if (!IsOpenableInitialVerifications(needToUncompress, out message))
		{
			return (false, message);
		}
		if (!(await ZipLocalFileHeader.TrySkipBlockAsync(_archive.ArchiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)))
		{
			message = System.SR.LocalFileHeaderCorrupt;
			return (false, message);
		}
		if (!IsOpenableFinalVerifications(needToLoadIntoMemory, await GetOffsetOfCompressedDataAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false), out message))
		{
			return (false, message);
		}
		return (true, message);
	}

	private async Task<bool> WriteLocalFileHeaderAsync(bool isEmptyFile, bool forceWrite, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (WriteLocalFileHeaderInitialize(isEmptyFile, forceWrite, out var zip64ExtraField, out var compressedSizeTruncated, out var uncompressedSizeTruncated, out var extraFieldLength))
		{
			byte[] array = new byte[30];
			WriteLocalFileHeaderPrepare(array, compressedSizeTruncated, uncompressedSizeTruncated, extraFieldLength);
			await _archive.ArchiveStream.WriteAsync(array, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			await _archive.ArchiveStream.WriteAsync(_storedEntryNameBytes, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (zip64ExtraField != null)
			{
				await zip64ExtraField.WriteBlockAsync(_archive.ArchiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			await ZipGenericExtraField.WriteAllBlocksAsync(_lhUnknownExtraFields, _lhTrailingExtraFieldData ?? Array.Empty<byte>(), _archive.ArchiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		return zip64ExtraField != null;
	}

	private async Task WriteLocalFileHeaderAndDataIfNeededAsync(bool forceWrite, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (_storedUncompressedData != null || _compressedBytes != null)
		{
			if (_storedUncompressedData != null)
			{
				_uncompressedSize = _storedUncompressedData.Length;
				DirectToArchiveWriterStream directToArchiveWriterStream = new DirectToArchiveWriterStream(GetDataCompressor(_archive.ArchiveStream, leaveBackingStreamOpen: true, null), this);
				await using (directToArchiveWriterStream)
				{
					_storedUncompressedData.Seek(0L, SeekOrigin.Begin);
					await _storedUncompressedData.CopyToAsync(directToArchiveWriterStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					await _storedUncompressedData.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
					_storedUncompressedData = null;
				}
				return;
			}
			if (_uncompressedSize == 0L)
			{
				_compressedSize = 0L;
			}
			await WriteLocalFileHeaderAsync(_uncompressedSize == 0, forceWrite: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (_uncompressedSize != 0L)
			{
				byte[][] compressedBytes = _compressedBytes;
				foreach (byte[] array in compressedBytes)
				{
					await _archive.ArchiveStream.WriteAsync(array, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}
		else if (_archive.Mode == ZipArchiveMode.Update || !_everOpenedForWrite)
		{
			_everOpenedForWrite = true;
			await WriteLocalFileHeaderAsync(_uncompressedSize == 0, forceWrite, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (_compressedSize != 0L)
			{
				_archive.ArchiveStream.Seek(_compressedSize, SeekOrigin.Current);
			}
		}
	}

	private async Task WriteCrcAndSizesInLocalHeaderAsync(bool zip64HeaderUsed, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] writeBuffer = new byte[20];
		WriteCrcAndSizesInLocalHeaderInitialize(zip64HeaderUsed, out var finalPosition, out var pretendStreaming, out var compressedSizeTruncated, out var uncompressedSizeTruncated);
		if (pretendStreaming)
		{
			WriteCrcAndSizesInLocalHeaderPrepareForZip64PretendStreaming(writeBuffer);
			await _archive.ArchiveStream.WriteAsync(writeBuffer.AsMemory(0, 4), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		WriteCrcAndSizesInLocalHeaderPrepareFor32bitValuesWriting(pretendStreaming, writeBuffer, compressedSizeTruncated, uncompressedSizeTruncated);
		await _archive.ArchiveStream.WriteAsync(writeBuffer.AsMemory(0, 12), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (zip64HeaderUsed)
		{
			WriteCrcAndSizesInLocalHeaderPrepareForWritingWhenZip64HeaderUsed(writeBuffer);
			await _archive.ArchiveStream.WriteAsync(writeBuffer.AsMemory(0, 16), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		_archive.ArchiveStream.Seek(finalPosition, SeekOrigin.Begin);
		if (pretendStreaming)
		{
			WriteCrcAndSizesInLocalHeaderPrepareForWritingDataDescriptor(writeBuffer);
			await _archive.ArchiveStream.WriteAsync(writeBuffer.AsMemory(0, 20), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private ValueTask WriteDataDescriptorAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] array = new byte[24];
		int length = PrepareToWriteDataDescriptor(array);
		return _archive.ArchiveStream.WriteAsync(array.AsMemory(0, length), cancellationToken);
	}

	private async Task UnloadStreamsAsync()
	{
		if (_storedUncompressedData != null)
		{
			await _storedUncompressedData.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
		_compressedBytes = null;
		_outstandingWriteStream = null;
	}

	private async Task CloseStreamsAsync()
	{
		if (_outstandingWriteStream != null)
		{
			await _outstandingWriteStream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	internal static string ParseFileName(string path, ZipVersionMadeByPlatform madeByPlatform)
	{
		if (madeByPlatform != ZipVersionMadeByPlatform.Unix)
		{
			return GetFileName_Windows(path);
		}
		return GetFileName_Unix(path);
	}
}
