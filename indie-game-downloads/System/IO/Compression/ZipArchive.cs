using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

/// <summary>Represents a package of compressed files in the zip archive format.</summary>
public class ZipArchive : IDisposable, IAsyncDisposable
{
	[Flags]
	internal enum ChangeState
	{
		Unchanged = 0,
		FixedLengthMetadata = 1,
		DynamicLengthMetadata = 2,
		StoredData = 4
	}

	private readonly Stream _archiveStream;

	private ZipArchiveEntry _archiveStreamOwner;

	private readonly ZipArchiveMode _mode;

	private readonly List<ZipArchiveEntry> _entries;

	private readonly ReadOnlyCollection<ZipArchiveEntry> _entriesCollection;

	private readonly Dictionary<string, ZipArchiveEntry> _entriesDictionary;

	private bool _readEntries;

	private readonly bool _leaveOpen;

	private long _centralDirectoryStart;

	private bool _isDisposed;

	private uint _numberOfThisDisk;

	private long _expectedNumberOfEntries;

	private readonly Stream _backingStream;

	private byte[] _archiveComment;

	private Encoding _entryNameAndCommentEncoding;

	private long _firstDeletedEntryOffset;

	public string Comment
	{
		get
		{
			return (EntryNameAndCommentEncoding ?? Encoding.UTF8).GetString(_archiveComment);
		}
		[param: AllowNull]
		set
		{
			_archiveComment = ZipHelper.GetEncodedTruncatedBytesFromString(value, EntryNameAndCommentEncoding, 65535, out var _);
			Changed |= ChangeState.DynamicLengthMetadata;
		}
	}

	/// <summary>Gets the collection of entries that are currently in the zip archive.</summary>
	/// <returns>The collection of entries that are currently in the zip archive.</returns>
	/// <exception cref="T:System.NotSupportedException">The zip archive does not support reading.</exception>
	/// <exception cref="T:System.ObjectDisposedException">The zip archive has been disposed.</exception>
	/// <exception cref="T:System.IO.InvalidDataException">The zip archive is corrupt, and its entries cannot be retrieved.</exception>
	public ReadOnlyCollection<ZipArchiveEntry> Entries
	{
		get
		{
			if (_mode == ZipArchiveMode.Create)
			{
				throw new NotSupportedException(System.SR.EntriesInCreateMode);
			}
			ThrowIfDisposed();
			EnsureCentralDirectoryRead();
			return _entriesCollection;
		}
	}

	/// <summary>Gets a value that describes the type of action the zip archive can perform on entries.</summary>
	/// <returns>One of the enumeration values that describes the type of action (read, create, or update) the zip archive can perform on entries.</returns>
	public ZipArchiveMode Mode => _mode;

	internal Stream ArchiveStream => _archiveStream;

	internal uint NumberOfThisDisk => _numberOfThisDisk;

	internal Encoding? EntryNameAndCommentEncoding
	{
		get
		{
			return _entryNameAndCommentEncoding;
		}
		private set
		{
			if (value != null && (value.Equals(Encoding.BigEndianUnicode) || value.Equals(Encoding.Unicode)))
			{
				throw new ArgumentException(System.SR.EntryNameAndCommentEncodingNotSupported, "EntryNameAndCommentEncoding");
			}
			_entryNameAndCommentEncoding = value;
		}
	}

	internal ChangeState Changed { get; private set; }

	/// <summary>Initializes a new instance of the <see cref="T:System.IO.Compression.ZipArchive" /> class from the specified stream.</summary>
	/// <param name="stream">The stream that contains the archive to be read.</param>
	/// <exception cref="T:System.ArgumentException">The stream is already closed or does not support reading.</exception>
	/// <exception cref="T:System.ArgumentNullException">
	///         <paramref name="stream" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.IO.InvalidDataException">The contents of the stream are not in the zip archive format.</exception>
	public ZipArchive(Stream stream)
		: this(stream, ZipArchiveMode.Read, leaveOpen: false, null)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.IO.Compression.ZipArchive" /> class from the specified stream and with the specified mode.</summary>
	/// <param name="stream">The input or output stream.</param>
	/// <param name="mode">One of the enumeration values that indicates whether the zip archive is used to read, create, or update entries.</param>
	/// <exception cref="T:System.ArgumentException">The stream is already closed, or the capabilities of the stream do not match the mode.</exception>
	/// <exception cref="T:System.ArgumentNullException">
	///         <paramref name="stream" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///         <paramref name="mode" /> is an invalid value.</exception>
	/// <exception cref="T:System.IO.InvalidDataException">The contents of the stream could not be interpreted as a zip archive.-or-
	///         <paramref name="mode" /> is <see cref="F:System.IO.Compression.ZipArchiveMode.Update" /> and an entry is missing from the archive or is corrupt and cannot be read.-or-
	///         <paramref name="mode" /> is <see cref="F:System.IO.Compression.ZipArchiveMode.Update" /> and an entry is too large to fit into memory.</exception>
	public ZipArchive(Stream stream, ZipArchiveMode mode)
		: this(stream, mode, leaveOpen: false, null)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.IO.Compression.ZipArchive" /> class on the specified stream for the specified mode, and optionally leaves the stream open.</summary>
	/// <param name="stream">The input or output stream.</param>
	/// <param name="mode">One of the enumeration values that indicates whether the zip archive is used to read, create, or update entries.</param>
	/// <param name="leaveOpen">
	///       <see langword="true" /> to leave the stream open after the <see cref="T:System.IO.Compression.ZipArchive" /> object is disposed; otherwise, <see langword="false" />.</param>
	/// <exception cref="T:System.ArgumentException">The stream is already closed, or the capabilities of the stream do not match the mode.</exception>
	/// <exception cref="T:System.ArgumentNullException">
	///         <paramref name="stream" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///         <paramref name="mode" /> is an invalid value.</exception>
	/// <exception cref="T:System.IO.InvalidDataException">The contents of the stream could not be interpreted as a zip archive.-or-
	///         <paramref name="mode" /> is <see cref="F:System.IO.Compression.ZipArchiveMode.Update" /> and an entry is missing from the archive or is corrupt and cannot be read.-or-
	///         <paramref name="mode" /> is <see cref="F:System.IO.Compression.ZipArchiveMode.Update" /> and an entry is too large to fit into memory.</exception>
	public ZipArchive(Stream stream, ZipArchiveMode mode, bool leaveOpen)
		: this(stream, mode, leaveOpen, null)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.IO.Compression.ZipArchive" /> class on the specified stream for the specified mode, uses the specified encoding for entry names, and optionally leaves the stream open.</summary>
	/// <param name="stream">The input or output stream.</param>
	/// <param name="mode">One of the enumeration values that indicates whether the zip archive is used to read, create, or update entries.</param>
	/// <param name="leaveOpen">
	///       <see langword="true" /> to leave the stream open after the <see cref="T:System.IO.Compression.ZipArchive" /> object is disposed; otherwise, <see langword="false" />.</param>
	/// <param name="entryNameEncoding">The encoding to use when reading or writing entry names in this archive. Specify a value for this parameter only when an encoding is required for interoperability with zip archive tools and libraries that do not support UTF-8 encoding for entry names.</param>
	/// <exception cref="T:System.ArgumentException">The stream is already closed, or the capabilities of the stream do not match the mode.</exception>
	/// <exception cref="T:System.ArgumentNullException">
	///         <paramref name="stream" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///         <paramref name="mode" /> is an invalid value.</exception>
	/// <exception cref="T:System.IO.InvalidDataException">The contents of the stream could not be interpreted as a zip archive.-or-
	///         <paramref name="mode" /> is <see cref="F:System.IO.Compression.ZipArchiveMode.Update" /> and an entry is missing from the archive or is corrupt and cannot be read.-or-
	///         <paramref name="mode" /> is <see cref="F:System.IO.Compression.ZipArchiveMode.Update" /> and an entry is too large to fit into memory.</exception>
	public ZipArchive(Stream stream, ZipArchiveMode mode, bool leaveOpen, Encoding? entryNameEncoding)
		: this(mode, leaveOpen, entryNameEncoding, null, DecideArchiveStream(mode, stream))
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		Stream stream2 = null;
		try
		{
			_backingStream = null;
			if (ValidateMode(mode, stream))
			{
				_backingStream = stream;
				stream2 = (stream = new MemoryStream());
				_backingStream.CopyTo(stream);
				stream.Seek(0L, SeekOrigin.Begin);
			}
			_archiveStream = DecideArchiveStream(mode, stream);
			switch (mode)
			{
			case ZipArchiveMode.Create:
				_readEntries = true;
				return;
			case ZipArchiveMode.Read:
				ReadEndOfCentralDirectory();
				return;
			}
			if (_archiveStream.Length == 0L)
			{
				_readEntries = true;
				return;
			}
			ReadEndOfCentralDirectory();
			EnsureCentralDirectoryRead();
			foreach (ZipArchiveEntry entry in _entries)
			{
				entry.ThrowIfNotOpenable(needToUncompress: false, needToLoadIntoMemory: true);
			}
		}
		catch (Exception)
		{
			stream2?.Dispose();
			throw;
		}
	}

	private ZipArchive(ZipArchiveMode mode, bool leaveOpen, Encoding entryNameEncoding, Stream backingStream, Stream archiveStream)
	{
		_backingStream = backingStream;
		_archiveStream = archiveStream;
		_mode = mode;
		EntryNameAndCommentEncoding = entryNameEncoding;
		_archiveStreamOwner = null;
		_entries = new List<ZipArchiveEntry>();
		_entriesCollection = new ReadOnlyCollection<ZipArchiveEntry>(_entries);
		_entriesDictionary = new Dictionary<string, ZipArchiveEntry>();
		Changed = ChangeState.Unchanged;
		_readEntries = false;
		_leaveOpen = leaveOpen;
		_centralDirectoryStart = 0L;
		_isDisposed = false;
		_numberOfThisDisk = 0u;
		_archiveComment = Array.Empty<byte>();
		_firstDeletedEntryOffset = long.MaxValue;
	}

	/// <summary>Creates an empty entry that has the specified path and entry name in the zip archive.</summary>
	/// <param name="entryName">A path, relative to the root of the archive, that specifies the name of the entry to be created.</param>
	/// <returns>An empty entry in the zip archive.</returns>
	/// <exception cref="T:System.ArgumentException">
	///         <paramref name="entryName" /> is <see cref="F:System.String.Empty" />.</exception>
	/// <exception cref="T:System.ArgumentNullException">
	///         <paramref name="entryName" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.NotSupportedException">The zip archive does not support writing.</exception>
	/// <exception cref="T:System.ObjectDisposedException">The zip archive has been disposed.</exception>
	public ZipArchiveEntry CreateEntry(string entryName)
	{
		return DoCreateEntry(entryName, null);
	}

	/// <summary>Creates an empty entry that has the specified entry name and compression level in the zip archive.</summary>
	/// <param name="entryName">A path, relative to the root of the archive, that specifies the name of the entry to be created.</param>
	/// <param name="compressionLevel">One of the enumeration values that indicates whether to emphasize speed or compression effectiveness when creating the entry.</param>
	/// <returns>An empty entry in the zip archive.</returns>
	/// <exception cref="T:System.ArgumentException">
	///         <paramref name="entryName" /> is <see cref="F:System.String.Empty" />.</exception>
	/// <exception cref="T:System.ArgumentNullException">
	///         <paramref name="entryName" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.NotSupportedException">The zip archive does not support writing.</exception>
	/// <exception cref="T:System.ObjectDisposedException">The zip archive has been disposed.</exception>
	public ZipArchiveEntry CreateEntry(string entryName, CompressionLevel compressionLevel)
	{
		return DoCreateEntry(entryName, compressionLevel);
	}

	/// <summary>Called by the <see cref="M:System.IO.Compression.ZipArchive.Dispose" /> and <see cref="M:System.Object.Finalize" /> methods to release the unmanaged resources used by the current instance of the <see cref="T:System.IO.Compression.ZipArchive" /> class, and optionally finishes writing the archive and releases the managed resources.</summary>
	/// <param name="disposing">
	///       <see langword="true" /> to finish writing the archive and release unmanaged and managed resources; <see langword="false" /> to release only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (!disposing || _isDisposed)
		{
			return;
		}
		try
		{
			ZipArchiveMode mode = _mode;
			if (mode != ZipArchiveMode.Read)
			{
				_ = mode - 1;
				_ = 1;
				WriteFile();
			}
		}
		finally
		{
			CloseStreams();
			_isDisposed = true;
		}
	}

	/// <summary>Releases the resources used by the current instance of the <see cref="T:System.IO.Compression.ZipArchive" /> class.</summary>
	public void Dispose()
	{
		Dispose(disposing: true);
	}

	/// <summary>Retrieves a wrapper for the specified entry in the zip archive.</summary>
	/// <param name="entryName">A path, relative to the root of the archive, that identifies the entry to retrieve.</param>
	/// <returns>A wrapper for the specified entry in the archive; <see langword="null" /> if the entry does not exist in the archive.</returns>
	/// <exception cref="T:System.ArgumentException">
	///         <paramref name="entryName" /> is <see cref="F:System.String.Empty" />.</exception>
	/// <exception cref="T:System.ArgumentNullException">
	///         <paramref name="entryName" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.NotSupportedException">The zip archive does not support reading.</exception>
	/// <exception cref="T:System.ObjectDisposedException">The zip archive has been disposed.</exception>
	/// <exception cref="T:System.IO.InvalidDataException">The zip archive is corrupt, and its entries cannot be retrieved.</exception>
	public ZipArchiveEntry? GetEntry(string entryName)
	{
		ArgumentNullException.ThrowIfNull(entryName, "entryName");
		if (_mode == ZipArchiveMode.Create)
		{
			throw new NotSupportedException(System.SR.EntriesInCreateMode);
		}
		EnsureCentralDirectoryRead();
		_entriesDictionary.TryGetValue(entryName, out var value);
		return value;
	}

	private ZipArchiveEntry DoCreateEntry(string entryName, CompressionLevel? compressionLevel)
	{
		ArgumentException.ThrowIfNullOrEmpty(entryName, "entryName");
		if (_mode == ZipArchiveMode.Read)
		{
			throw new NotSupportedException(System.SR.CreateInReadMode);
		}
		ThrowIfDisposed();
		ZipArchiveEntry zipArchiveEntry = (compressionLevel.HasValue ? new ZipArchiveEntry(this, entryName, compressionLevel.Value) : new ZipArchiveEntry(this, entryName));
		AddEntry(zipArchiveEntry);
		return zipArchiveEntry;
	}

	internal void AcquireArchiveStream(ZipArchiveEntry entry)
	{
		if (_archiveStreamOwner != null)
		{
			if (_archiveStreamOwner.EverOpenedForWrite)
			{
				throw new IOException(System.SR.CreateModeCreateEntryWhileOpen);
			}
			_archiveStreamOwner.WriteAndFinishLocalEntry(forceWrite: true);
		}
		_archiveStreamOwner = entry;
	}

	private void AddEntry(ZipArchiveEntry entry)
	{
		_entries.Add(entry);
		_entriesDictionary.TryAdd(entry.FullName, entry);
	}

	internal void ReleaseArchiveStream(ZipArchiveEntry entry)
	{
		_archiveStreamOwner = null;
	}

	internal void RemoveEntry(ZipArchiveEntry entry)
	{
		_entries.Remove(entry);
		_entriesDictionary.Remove(entry.FullName);
		if (entry.OriginallyInArchive && entry.OffsetOfLocalHeader < _firstDeletedEntryOffset)
		{
			_firstDeletedEntryOffset = entry.OffsetOfLocalHeader;
		}
	}

	internal void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_isDisposed, this);
	}

	private void CloseStreams()
	{
		if (!_leaveOpen)
		{
			_archiveStream.Dispose();
			_backingStream?.Dispose();
		}
		else if (_backingStream != null)
		{
			_archiveStream.Dispose();
		}
	}

	private void EnsureCentralDirectoryRead()
	{
		if (!_readEntries)
		{
			ReadCentralDirectory();
			_readEntries = true;
		}
	}

	private void ReadCentralDirectoryInitialize(out byte[] fileBuffer, out long numberOfEntries, out bool saveExtraFieldsAndComments, out bool continueReadingCentralDirectory, out int bytesRead, out int currPosition, out int bytesConsumed)
	{
		fileBuffer = new byte[4096];
		_archiveStream.Seek(_centralDirectoryStart, SeekOrigin.Begin);
		numberOfEntries = 0L;
		saveExtraFieldsAndComments = Mode == ZipArchiveMode.Update;
		continueReadingCentralDirectory = true;
		bytesRead = 0;
		currPosition = 0;
		bytesConsumed = 0;
		_entries.Clear();
		_entriesDictionary.Clear();
	}

	private bool ReadCentralDirectoryEndOfInnerLoopWork(bool result, ZipCentralDirectoryFileHeader currentHeader, int bytesConsumed, ref bool continueReadingCentralDirectory, ref long numberOfEntries, ref int currPosition, ref int bytesRead)
	{
		if (!result)
		{
			continueReadingCentralDirectory = false;
			return false;
		}
		AddEntry(new ZipArchiveEntry(this, currentHeader));
		numberOfEntries++;
		if (numberOfEntries > _expectedNumberOfEntries)
		{
			throw new InvalidDataException(System.SR.NumEntriesWrong);
		}
		currPosition += bytesConsumed;
		bytesRead += bytesConsumed;
		return true;
	}

	private void ReadCentralDirectoryEndOfOuterLoopWork(ref int currPosition, ReadOnlySpan<byte> sizedFileBuffer)
	{
		if (currPosition < sizedFileBuffer.Length)
		{
			_archiveStream.Seek(-(sizedFileBuffer.Length - currPosition), SeekOrigin.Current);
		}
		currPosition = 0;
	}

	private void ReadCentralDirectoryPostOuterLoopWork(long numberOfEntries)
	{
		if (numberOfEntries != _expectedNumberOfEntries)
		{
			throw new InvalidDataException(System.SR.NumEntriesWrong);
		}
		if (Mode == ZipArchiveMode.Update)
		{
			_entries.Sort(ZipArchiveEntry.LocalHeaderOffsetComparer.Instance);
		}
	}

	private void ReadCentralDirectory()
	{
		try
		{
			ReadCentralDirectoryInitialize(out var fileBuffer, out var numberOfEntries, out var saveExtraFieldsAndComments, out var continueReadingCentralDirectory, out var bytesRead, out var currPosition, out var bytesConsumed);
			Span<byte> buffer = fileBuffer.AsSpan();
			while (continueReadingCentralDirectory)
			{
				int num = _archiveStream.ReadAtLeast(buffer, 46, throwOnEndOfStream: false);
				ReadOnlySpan<byte> sizedFileBuffer = buffer.Slice(0, num);
				continueReadingCentralDirectory = num >= 46;
				while (currPosition + 46 <= num)
				{
					bool result = ZipCentralDirectoryFileHeader.TryReadBlock(sizedFileBuffer.Slice(currPosition), _archiveStream, saveExtraFieldsAndComments, out bytesConsumed, out var header);
					if (!ReadCentralDirectoryEndOfInnerLoopWork(result, header, bytesConsumed, ref continueReadingCentralDirectory, ref numberOfEntries, ref currPosition, ref bytesRead))
					{
						break;
					}
				}
				ReadCentralDirectoryEndOfOuterLoopWork(ref currPosition, sizedFileBuffer);
			}
			ReadCentralDirectoryPostOuterLoopWork(numberOfEntries);
		}
		catch (EndOfStreamException p)
		{
			throw new InvalidDataException(System.SR.Format(System.SR.CentralDirectoryInvalid, p));
		}
	}

	private void ReadEndOfCentralDirectoryInnerWork(ZipEndOfCentralDirectoryBlock eocd)
	{
		if (eocd.NumberOfThisDisk != eocd.NumberOfTheDiskWithTheStartOfTheCentralDirectory)
		{
			throw new InvalidDataException(System.SR.SplitSpanned);
		}
		_numberOfThisDisk = eocd.NumberOfThisDisk;
		_centralDirectoryStart = eocd.OffsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber;
		if (eocd.NumberOfEntriesInTheCentralDirectory != eocd.NumberOfEntriesInTheCentralDirectoryOnThisDisk)
		{
			throw new InvalidDataException(System.SR.SplitSpanned);
		}
		_expectedNumberOfEntries = eocd.NumberOfEntriesInTheCentralDirectory;
		_archiveComment = eocd.ArchiveComment;
	}

	private void ReadEndOfCentralDirectory()
	{
		try
		{
			_archiveStream.Seek(-18L, SeekOrigin.End);
			if (!ZipHelper.SeekBackwardsToSignature(_archiveStream, ZipEndOfCentralDirectoryBlock.SignatureConstantBytes, 65539))
			{
				throw new InvalidDataException(System.SR.EOCDNotFound);
			}
			long position = _archiveStream.Position;
			ZipEndOfCentralDirectoryBlock eocd = ZipEndOfCentralDirectoryBlock.ReadBlock(_archiveStream);
			ReadEndOfCentralDirectoryInnerWork(eocd);
			TryReadZip64EndOfCentralDirectory(eocd, position);
			if (_centralDirectoryStart > _archiveStream.Length)
			{
				throw new InvalidDataException(System.SR.FieldTooBigOffsetToCD);
			}
		}
		catch (EndOfStreamException innerException)
		{
			throw new InvalidDataException(System.SR.CDCorrupt, innerException);
		}
		catch (IOException innerException2)
		{
			throw new InvalidDataException(System.SR.CDCorrupt, innerException2);
		}
	}

	private void TryReadZip64EndOfCentralDirectoryInnerInitialWork(Zip64EndOfCentralDirectoryLocator locator)
	{
		if (locator == null || locator.OffsetOfZip64EOCD > long.MaxValue)
		{
			throw new InvalidDataException(System.SR.FieldTooBigOffsetToZip64EOCD);
		}
		long offsetOfZip64EOCD = (long)locator.OffsetOfZip64EOCD;
		if (offsetOfZip64EOCD < 0 || offsetOfZip64EOCD > _archiveStream.Length)
		{
			throw new InvalidDataException(System.SR.InvalidOffsetToZip64EOCD);
		}
		_archiveStream.Seek(offsetOfZip64EOCD, SeekOrigin.Begin);
	}

	private void TryReadZip64EndOfCentralDirectoryInnerFinalWork(Zip64EndOfCentralDirectoryRecord record)
	{
		_numberOfThisDisk = record.NumberOfThisDisk;
		if (record.NumberOfEntriesTotal > long.MaxValue)
		{
			throw new InvalidDataException(System.SR.FieldTooBigNumEntries);
		}
		if (record.OffsetOfCentralDirectory > long.MaxValue)
		{
			throw new InvalidDataException(System.SR.FieldTooBigOffsetToCD);
		}
		if (record.NumberOfEntriesTotal != record.NumberOfEntriesOnThisDisk)
		{
			throw new InvalidDataException(System.SR.SplitSpanned);
		}
		_expectedNumberOfEntries = (long)record.NumberOfEntriesTotal;
		_centralDirectoryStart = (long)record.OffsetOfCentralDirectory;
	}

	private void TryReadZip64EndOfCentralDirectory(ZipEndOfCentralDirectoryBlock eocd, long eocdStart)
	{
		if (eocd.NumberOfThisDisk == ushort.MaxValue || eocd.OffsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber == uint.MaxValue || eocd.NumberOfEntriesInTheCentralDirectory == ushort.MaxValue)
		{
			if (eocdStart < 20)
			{
				throw new InvalidDataException(System.SR.Zip64EOCDNotWhereExpected);
			}
			_archiveStream.Seek(eocdStart - 16, SeekOrigin.Begin);
			if (ZipHelper.SeekBackwardsToSignature(_archiveStream, Zip64EndOfCentralDirectoryLocator.SignatureConstantBytes, 4))
			{
				Zip64EndOfCentralDirectoryLocator locator = Zip64EndOfCentralDirectoryLocator.TryReadBlock(_archiveStream);
				TryReadZip64EndOfCentralDirectoryInnerInitialWork(locator);
				Zip64EndOfCentralDirectoryRecord record = Zip64EndOfCentralDirectoryRecord.TryReadBlock(_archiveStream);
				TryReadZip64EndOfCentralDirectoryInnerFinalWork(record);
			}
		}
	}

	private static void WriteFileCalculateOffsets(ZipArchiveEntry entry, ref long startingOffset, ref long nextFileOffset)
	{
		if (entry.Changes == ChangeState.Unchanged)
		{
			nextFileOffset = Math.Max(nextFileOffset, entry.GetOffsetOfCompressedData() + entry.CompressedLength);
		}
		else
		{
			startingOffset = Math.Min(startingOffset, entry.OffsetOfLocalHeader);
		}
	}

	private static void WriteFileCheckStartingOffset(ZipArchiveEntry entry, ref long completeRewriteStartingOffset)
	{
		if ((entry.Changes & (ChangeState.DynamicLengthMetadata | ChangeState.StoredData)) != ChangeState.Unchanged)
		{
			completeRewriteStartingOffset = Math.Min(completeRewriteStartingOffset, entry.OffsetOfLocalHeader);
		}
	}

	private void WriteFileUpdateModeFinalWork(long startingOffset, long nextFileOffset)
	{
		if (startingOffset == long.MaxValue)
		{
			startingOffset = nextFileOffset;
		}
		_archiveStream.Seek(startingOffset, SeekOrigin.Begin);
	}

	private void WriteFileFinalWork()
	{
		if (_mode == ZipArchiveMode.Update && _archiveStream.Position != _archiveStream.Length)
		{
			_archiveStream.SetLength(_archiveStream.Position);
		}
	}

	private void WriteFile()
	{
		long completeRewriteStartingOffset = 0L;
		List<ZipArchiveEntry> list = _entries;
		if (_mode == ZipArchiveMode.Update)
		{
			long startingOffset = _firstDeletedEntryOffset;
			long nextFileOffset = 0L;
			completeRewriteStartingOffset = startingOffset;
			list = new List<ZipArchiveEntry>(_entries.Count);
			foreach (ZipArchiveEntry entry in _entries)
			{
				if (!entry.OriginallyInArchive)
				{
					list.Add(entry);
					continue;
				}
				WriteFileCalculateOffsets(entry, ref startingOffset, ref nextFileOffset);
				if (entry.OffsetOfLocalHeader >= startingOffset)
				{
					WriteFileCheckStartingOffset(entry, ref completeRewriteStartingOffset);
					entry.LoadLocalHeaderExtraFieldIfNeeded();
					if (entry.OffsetOfLocalHeader >= completeRewriteStartingOffset)
					{
						entry.LoadCompressedBytesIfNeeded();
					}
					list.Add(entry);
				}
			}
			WriteFileUpdateModeFinalWork(startingOffset, nextFileOffset);
		}
		foreach (ZipArchiveEntry item in list)
		{
			bool forceWrite = !item.OriginallyInArchive || (item.OriginallyInArchive && item.OffsetOfLocalHeader >= completeRewriteStartingOffset);
			item.WriteAndFinishLocalEntry(forceWrite);
		}
		long position = _archiveStream.Position;
		bool flag = _entries.Count == 0;
		foreach (ZipArchiveEntry entry2 in _entries)
		{
			bool flag2 = position != _centralDirectoryStart || !entry2.OriginallyInArchive || entry2.OffsetOfLocalHeader >= completeRewriteStartingOffset;
			entry2.WriteCentralDirectoryFileHeader(flag2);
			flag |= flag2;
		}
		long sizeOfCentralDirectory = _archiveStream.Position - position;
		WriteArchiveEpilogue(position, sizeOfCentralDirectory, flag);
		WriteFileFinalWork();
	}

	private void WriteArchiveEpilogueNoCDChangesWork()
	{
		_archiveStream.Seek(56L, SeekOrigin.Current);
		_archiveStream.Seek(20L, SeekOrigin.Current);
	}

	private void WriteArchiveEpilogue(long startOfCentralDirectory, long sizeOfCentralDirectory, bool centralDirectoryChanged)
	{
		if (startOfCentralDirectory >= uint.MaxValue || sizeOfCentralDirectory >= uint.MaxValue || _entries.Count >= 65535)
		{
			long position = _archiveStream.Position;
			if (centralDirectoryChanged)
			{
				Zip64EndOfCentralDirectoryRecord.WriteBlock(_archiveStream, _entries.Count, startOfCentralDirectory, sizeOfCentralDirectory);
				Zip64EndOfCentralDirectoryLocator.WriteBlock(_archiveStream, position);
			}
			else
			{
				WriteArchiveEpilogueNoCDChangesWork();
			}
		}
		if (centralDirectoryChanged || Changed != ChangeState.Unchanged)
		{
			ZipEndOfCentralDirectoryBlock.WriteBlock(_archiveStream, _entries.Count, startOfCentralDirectory, sizeOfCentralDirectory, _archiveComment);
		}
		else
		{
			_archiveStream.Seek(22 + _archiveComment.Length, SeekOrigin.Current);
		}
	}

	private static bool ValidateMode(ZipArchiveMode mode, Stream stream)
	{
		bool result = false;
		switch (mode)
		{
		case ZipArchiveMode.Create:
			if (!stream.CanWrite)
			{
				throw new ArgumentException(System.SR.CreateModeCapabilities);
			}
			break;
		case ZipArchiveMode.Read:
			if (!stream.CanRead)
			{
				throw new ArgumentException(System.SR.ReadModeCapabilities);
			}
			if (!stream.CanSeek)
			{
				result = true;
			}
			break;
		case ZipArchiveMode.Update:
			if (!stream.CanRead || !stream.CanWrite || !stream.CanSeek)
			{
				throw new ArgumentException(System.SR.UpdateModeCapabilities);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("mode");
		}
		return result;
	}

	private static Stream DecideArchiveStream(ZipArchiveMode mode, Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		if (mode != ZipArchiveMode.Create || stream.CanSeek)
		{
			return stream;
		}
		return new PositionPreservingWriteOnlyStreamWrapper(stream);
	}

	public static async Task<ZipArchive> CreateAsync(Stream stream, ZipArchiveMode mode, bool leaveOpen, Encoding? entryNameEncoding, CancellationToken cancellationToken = default(CancellationToken))
	{
		cancellationToken.ThrowIfCancellationRequested();
		ArgumentNullException.ThrowIfNull(stream, "stream");
		Stream extraTempStream = null;
		try
		{
			Stream backingStream = null;
			if (ValidateMode(mode, stream))
			{
				backingStream = stream;
				Stream stream2;
				stream = (stream2 = new MemoryStream());
				extraTempStream = stream2;
				await backingStream.CopyToAsync(stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				stream.Seek(0L, SeekOrigin.Begin);
			}
			ZipArchive zipArchive = new ZipArchive(mode, leaveOpen, entryNameEncoding, backingStream, DecideArchiveStream(mode, stream));
			switch (mode)
			{
			case ZipArchiveMode.Create:
				zipArchive._readEntries = true;
				break;
			case ZipArchiveMode.Read:
				await zipArchive.ReadEndOfCentralDirectoryAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				break;
			default:
				if (zipArchive._archiveStream.Length == 0L)
				{
					zipArchive._readEntries = true;
					break;
				}
				await zipArchive.ReadEndOfCentralDirectoryAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				await zipArchive.EnsureCentralDirectoryReadAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				foreach (ZipArchiveEntry entry in zipArchive._entries)
				{
					await entry.ThrowIfNotOpenableAsync(needToUncompress: false, needToLoadIntoMemory: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				break;
			}
			return zipArchive;
		}
		catch (Exception)
		{
			if (extraTempStream != null)
			{
				await extraTempStream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			throw;
		}
	}

	public async ValueTask DisposeAsync()
	{
		await DisposeAsyncCore().ConfigureAwait(continueOnCapturedContext: false);
	}

	protected virtual async ValueTask DisposeAsyncCore()
	{
		if (_isDisposed)
		{
			return;
		}
		try
		{
			ZipArchiveMode mode = _mode;
			if (mode != ZipArchiveMode.Read)
			{
				_ = mode - 1;
				_ = 1;
				await WriteFileAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		finally
		{
			await CloseStreamsAsync().ConfigureAwait(continueOnCapturedContext: false);
			_isDisposed = true;
		}
	}

	private async Task CloseStreamsAsync()
	{
		if (!_leaveOpen)
		{
			await _archiveStream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			if (_backingStream != null)
			{
				await _backingStream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		else if (_backingStream != null)
		{
			await _archiveStream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private async Task EnsureCentralDirectoryReadAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (!_readEntries)
		{
			await ReadCentralDirectoryAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			_readEntries = true;
		}
	}

	private async Task ReadCentralDirectoryAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		try
		{
			ReadCentralDirectoryInitialize(out var fileBuffer, out var numberOfEntries, out var saveExtraFieldsAndComments, out var continueReadingCentralDirectory, out var bytesRead, out var currPosition, out var bytesConsumed);
			while (continueReadingCentralDirectory)
			{
				int currBytesRead = await _archiveStream.ReadAtLeastAsync(fileBuffer, 46, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				byte[] sizedFileBuffer = fileBuffer[0..currBytesRead];
				continueReadingCentralDirectory = currBytesRead >= 46;
				while (currPosition + 46 <= currBytesRead)
				{
					bool result;
					ZipCentralDirectoryFileHeader currentHeader;
					(result, bytesConsumed, currentHeader) = await ZipCentralDirectoryFileHeader.TryReadBlockAsync(sizedFileBuffer.AsMemory(currPosition), _archiveStream, saveExtraFieldsAndComments, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (!ReadCentralDirectoryEndOfInnerLoopWork(result, currentHeader, bytesConsumed, ref continueReadingCentralDirectory, ref numberOfEntries, ref currPosition, ref bytesRead))
					{
						break;
					}
				}
				ReadCentralDirectoryEndOfOuterLoopWork(ref currPosition, sizedFileBuffer);
			}
			ReadCentralDirectoryPostOuterLoopWork(numberOfEntries);
		}
		catch (EndOfStreamException p)
		{
			throw new InvalidDataException(System.SR.Format(System.SR.CentralDirectoryInvalid, p));
		}
	}

	private async Task ReadEndOfCentralDirectoryAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		try
		{
			_archiveStream.Seek(-18L, SeekOrigin.End);
			if (!(await ZipHelper.SeekBackwardsToSignatureAsync(_archiveStream, ZipEndOfCentralDirectoryBlock.SignatureConstantBytes, 65539, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)))
			{
				throw new InvalidDataException(System.SR.EOCDNotFound);
			}
			long eocdStart = _archiveStream.Position;
			ZipEndOfCentralDirectoryBlock eocd = await ZipEndOfCentralDirectoryBlock.ReadBlockAsync(_archiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			ReadEndOfCentralDirectoryInnerWork(eocd);
			await TryReadZip64EndOfCentralDirectoryAsync(eocd, eocdStart, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (_centralDirectoryStart > _archiveStream.Length)
			{
				throw new InvalidDataException(System.SR.FieldTooBigOffsetToCD);
			}
		}
		catch (EndOfStreamException innerException)
		{
			throw new InvalidDataException(System.SR.CDCorrupt, innerException);
		}
		catch (IOException innerException2)
		{
			throw new InvalidDataException(System.SR.CDCorrupt, innerException2);
		}
	}

	private async ValueTask TryReadZip64EndOfCentralDirectoryAsync(ZipEndOfCentralDirectoryBlock eocd, long eocdStart, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (eocd.NumberOfThisDisk == ushort.MaxValue || eocd.OffsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber == uint.MaxValue || eocd.NumberOfEntriesInTheCentralDirectory == ushort.MaxValue)
		{
			if (eocdStart < 20)
			{
				throw new InvalidDataException(System.SR.Zip64EOCDNotWhereExpected);
			}
			_archiveStream.Seek(eocdStart - 16, SeekOrigin.Begin);
			if (await ZipHelper.SeekBackwardsToSignatureAsync(_archiveStream, Zip64EndOfCentralDirectoryLocator.SignatureConstantBytes, 4, cancellationToken).ConfigureAwait(continueOnCapturedContext: false))
			{
				TryReadZip64EndOfCentralDirectoryInnerInitialWork(await Zip64EndOfCentralDirectoryLocator.TryReadBlockAsync(_archiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
				TryReadZip64EndOfCentralDirectoryInnerFinalWork(await Zip64EndOfCentralDirectoryRecord.TryReadBlockAsync(_archiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
			}
		}
	}

	private async ValueTask WriteFileAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		cancellationToken.ThrowIfCancellationRequested();
		long completeRewriteStartingOffset = 0L;
		List<ZipArchiveEntry> entriesToWrite = _entries;
		if (_mode == ZipArchiveMode.Update)
		{
			long startingOffset = _firstDeletedEntryOffset;
			long nextFileOffset = 0L;
			completeRewriteStartingOffset = startingOffset;
			entriesToWrite = new List<ZipArchiveEntry>(_entries.Count);
			foreach (ZipArchiveEntry entry in _entries)
			{
				if (!entry.OriginallyInArchive)
				{
					entriesToWrite.Add(entry);
					continue;
				}
				WriteFileCalculateOffsets(entry, ref startingOffset, ref nextFileOffset);
				if (entry.OffsetOfLocalHeader >= startingOffset)
				{
					WriteFileCheckStartingOffset(entry, ref completeRewriteStartingOffset);
					await entry.LoadLocalHeaderExtraFieldIfNeededAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (entry.OffsetOfLocalHeader >= completeRewriteStartingOffset)
					{
						await entry.LoadCompressedBytesIfNeededAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
					entriesToWrite.Add(entry);
				}
			}
			WriteFileUpdateModeFinalWork(startingOffset, nextFileOffset);
		}
		foreach (ZipArchiveEntry item in entriesToWrite)
		{
			bool forceWrite = !item.OriginallyInArchive || (item.OriginallyInArchive && item.OffsetOfLocalHeader >= completeRewriteStartingOffset);
			await item.WriteAndFinishLocalEntryAsync(forceWrite, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		long plannedCentralDirectoryPosition = _archiveStream.Position;
		bool archiveEpilogueRequiresUpdate = _entries.Count == 0;
		foreach (ZipArchiveEntry entry2 in _entries)
		{
			bool centralDirectoryEntryRequiresUpdate = plannedCentralDirectoryPosition != _centralDirectoryStart || !entry2.OriginallyInArchive || entry2.OffsetOfLocalHeader >= completeRewriteStartingOffset;
			await entry2.WriteCentralDirectoryFileHeaderAsync(centralDirectoryEntryRequiresUpdate, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			archiveEpilogueRequiresUpdate |= centralDirectoryEntryRequiresUpdate;
		}
		long sizeOfCentralDirectory = _archiveStream.Position - plannedCentralDirectoryPosition;
		await WriteArchiveEpilogueAsync(plannedCentralDirectoryPosition, sizeOfCentralDirectory, archiveEpilogueRequiresUpdate, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		WriteFileFinalWork();
	}

	private async ValueTask WriteArchiveEpilogueAsync(long startOfCentralDirectory, long sizeOfCentralDirectory, bool centralDirectoryChanged, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (startOfCentralDirectory >= uint.MaxValue || sizeOfCentralDirectory >= uint.MaxValue || _entries.Count >= 65535)
		{
			long zip64EOCDRecordStart = _archiveStream.Position;
			if (centralDirectoryChanged)
			{
				await Zip64EndOfCentralDirectoryRecord.WriteBlockAsync(_archiveStream, _entries.Count, startOfCentralDirectory, sizeOfCentralDirectory, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				await Zip64EndOfCentralDirectoryLocator.WriteBlockAsync(_archiveStream, zip64EOCDRecordStart, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				WriteArchiveEpilogueNoCDChangesWork();
			}
		}
		if (centralDirectoryChanged || Changed != ChangeState.Unchanged)
		{
			await ZipEndOfCentralDirectoryBlock.WriteBlockAsync(_archiveStream, _entries.Count, startOfCentralDirectory, sizeOfCentralDirectory, _archiveComment, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		else
		{
			_archiveStream.Seek(22 + _archiveComment.Length, SeekOrigin.Current);
		}
	}
}
