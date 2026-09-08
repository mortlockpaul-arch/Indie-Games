using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ZipFileExtensions
{
	public static ZipArchiveEntry CreateEntryFromFile(this ZipArchive destination, string sourceFileName, string entryName)
	{
		return destination.DoCreateEntryFromFile(sourceFileName, entryName, null);
	}

	public static ZipArchiveEntry CreateEntryFromFile(this ZipArchive destination, string sourceFileName, string entryName, CompressionLevel compressionLevel)
	{
		return destination.DoCreateEntryFromFile(sourceFileName, entryName, compressionLevel);
	}

	internal static ZipArchiveEntry DoCreateEntryFromFile(this ZipArchive destination, string sourceFileName, string entryName, CompressionLevel? compressionLevel)
	{
		var (fileStream, zipArchiveEntry) = InitializeDoCreateEntryFromFile(destination, sourceFileName, entryName, compressionLevel, useAsync: true);
		using (fileStream)
		{
			using Stream destination2 = zipArchiveEntry.Open();
			fileStream.CopyTo(destination2);
			return zipArchiveEntry;
		}
	}

	private static (FileStream, ZipArchiveEntry) InitializeDoCreateEntryFromFile(ZipArchive destination, string sourceFileName, string entryName, CompressionLevel? compressionLevel, bool useAsync)
	{
		ArgumentNullException.ThrowIfNull(destination, "destination");
		ArgumentNullException.ThrowIfNull(sourceFileName, "sourceFileName");
		ArgumentNullException.ThrowIfNull(entryName, "entryName");
		FileStream item = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 16384, useAsync);
		ZipArchiveEntry zipArchiveEntry = (compressionLevel.HasValue ? destination.CreateEntry(entryName, compressionLevel.Value) : destination.CreateEntry(entryName));
		DateTime dateTime = File.GetLastWriteTime(sourceFileName);
		int year = dateTime.Year;
		if ((year < 1980 || year > 2107) ? true : false)
		{
			dateTime = new DateTime(1980, 1, 1, 0, 0, 0);
		}
		zipArchiveEntry.LastWriteTime = dateTime;
		return (item, zipArchiveEntry);
	}

	public static Task<ZipArchiveEntry> CreateEntryFromFileAsync(this ZipArchive destination, string sourceFileName, string entryName, CancellationToken cancellationToken = default(CancellationToken))
	{
		return destination.DoCreateEntryFromFileAsync(sourceFileName, entryName, null, cancellationToken);
	}

	public static Task<ZipArchiveEntry> CreateEntryFromFileAsync(this ZipArchive destination, string sourceFileName, string entryName, CompressionLevel compressionLevel, CancellationToken cancellationToken = default(CancellationToken))
	{
		return destination.DoCreateEntryFromFileAsync(sourceFileName, entryName, compressionLevel, cancellationToken);
	}

	internal static async Task<ZipArchiveEntry> DoCreateEntryFromFileAsync(this ZipArchive destination, string sourceFileName, string entryName, CompressionLevel? compressionLevel, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var (fs, entry) = InitializeDoCreateEntryFromFile(destination, sourceFileName, entryName, compressionLevel, useAsync: true);
		await using (fs)
		{
			Stream stream = await entry.OpenAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			await using (stream)
			{
				await fs.CopyToAsync(stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		return entry;
	}

	public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName)
	{
		source.ExtractToDirectory(destinationDirectoryName, overwriteFiles: false);
	}

	public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName, bool overwriteFiles)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(destinationDirectoryName, "destinationDirectoryName");
		foreach (ZipArchiveEntry entry in source.Entries)
		{
			entry.ExtractRelativeToDirectory(destinationDirectoryName, overwriteFiles);
		}
	}

	public static Task ExtractToDirectoryAsync(this ZipArchive source, string destinationDirectoryName, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.ExtractToDirectoryAsync(destinationDirectoryName, overwriteFiles: false, cancellationToken);
	}

	public static async Task ExtractToDirectoryAsync(this ZipArchive source, string destinationDirectoryName, bool overwriteFiles, CancellationToken cancellationToken = default(CancellationToken))
	{
		cancellationToken.ThrowIfCancellationRequested();
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(destinationDirectoryName, "destinationDirectoryName");
		foreach (ZipArchiveEntry entry in source.Entries)
		{
			await entry.ExtractRelativeToDirectoryAsync(destinationDirectoryName, overwriteFiles, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	public static void ExtractToFile(this ZipArchiveEntry source, string destinationFileName)
	{
		source.ExtractToFile(destinationFileName, overwrite: false);
	}

	public static void ExtractToFile(this ZipArchiveEntry source, string destinationFileName, bool overwrite)
	{
		ExtractToFileInitialize(source, destinationFileName, overwrite, out var fileStreamOptions);
		using (FileStream destination = new FileStream(destinationFileName, fileStreamOptions))
		{
			using Stream stream = source.Open();
			stream.CopyTo(destination);
		}
		ExtractToFileFinalize(source, destinationFileName);
	}

	private static void ExtractToFileInitialize(ZipArchiveEntry source, string destinationFileName, bool overwrite, out FileStreamOptions fileStreamOptions)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(destinationFileName, "destinationFileName");
		fileStreamOptions = new FileStreamOptions
		{
			Access = FileAccess.Write,
			Mode = ((!overwrite) ? FileMode.CreateNew : FileMode.Create),
			Share = FileShare.None,
			BufferSize = 16384
		};
		UnixFileMode unixFileMode = (UnixFileMode)((source.ExternalAttributes >> 16) & 0x1FF);
		if (unixFileMode != UnixFileMode.None && !OperatingSystem.IsWindows())
		{
			fileStreamOptions.UnixCreateMode = unixFileMode;
		}
	}

	private static void ExtractToFileFinalize(ZipArchiveEntry source, string destinationFileName)
	{
		ArchivingUtils.AttemptSetLastWriteTime(destinationFileName, source.LastWriteTime);
	}

	private static bool ExtractRelativeToDirectoryCheckIfFile(ZipArchiveEntry source, string destinationDirectoryName, out string fileDestinationPath)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(destinationDirectoryName, "destinationDirectoryName");
		string text = Directory.CreateDirectory(destinationDirectoryName).FullName;
		if (!text.EndsWith(Path.DirectorySeparatorChar))
		{
			char reference = Path.DirectorySeparatorChar;
			text = string.Concat(text.AsSpan(), new ReadOnlySpan<char>(in reference));
		}
		fileDestinationPath = Path.GetFullPath(Path.Combine(text, ArchivingUtils.SanitizeEntryFilePath(source.FullName)));
		if (!fileDestinationPath.StartsWith(text, System.IO.PathInternal.StringComparison))
		{
			throw new IOException(System.SR.IO_ExtractingResultsInOutside);
		}
		if (Path.GetFileName(fileDestinationPath).Length == 0)
		{
			if (source.Length != 0L)
			{
				throw new IOException(System.SR.IO_DirectoryNameWithData);
			}
			Directory.CreateDirectory(fileDestinationPath);
			return false;
		}
		return true;
	}

	internal static void ExtractRelativeToDirectory(this ZipArchiveEntry source, string destinationDirectoryName, bool overwrite)
	{
		if (ExtractRelativeToDirectoryCheckIfFile(source, destinationDirectoryName, out var fileDestinationPath))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(fileDestinationPath));
			source.ExtractToFile(fileDestinationPath, overwrite);
		}
	}

	public static Task ExtractToFileAsync(this ZipArchiveEntry source, string destinationFileName, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.ExtractToFileAsync(destinationFileName, overwrite: false, cancellationToken);
	}

	public static async Task ExtractToFileAsync(this ZipArchiveEntry source, string destinationFileName, bool overwrite, CancellationToken cancellationToken = default(CancellationToken))
	{
		cancellationToken.ThrowIfCancellationRequested();
		ExtractToFileInitialize(source, destinationFileName, overwrite, out var fileStreamOptions);
		FileStream fs = new FileStream(destinationFileName, fileStreamOptions);
		await using (fs)
		{
			Stream stream = await source.OpenAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			await using (stream)
			{
				await stream.CopyToAsync(fs, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		ExtractToFileFinalize(source, destinationFileName);
	}

	internal static async Task ExtractRelativeToDirectoryAsync(this ZipArchiveEntry source, string destinationDirectoryName, bool overwrite, CancellationToken cancellationToken = default(CancellationToken))
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (ExtractRelativeToDirectoryCheckIfFile(source, destinationDirectoryName, out var fileDestinationPath))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(fileDestinationPath));
			await source.ExtractToFileAsync(fileDestinationPath, overwrite, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
