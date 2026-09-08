using System.IO.Enumeration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

public static class ZipFile
{
	private enum CreateEntryType
	{
		File,
		Directory,
		Unsupported
	}

	public static ZipArchive OpenRead(string archiveFileName)
	{
		return Open(archiveFileName, ZipArchiveMode.Read);
	}

	public static ZipArchive Open(string archiveFileName, ZipArchiveMode mode)
	{
		return Open(archiveFileName, mode, null);
	}

	public static ZipArchive Open(string archiveFileName, ZipArchiveMode mode, Encoding? entryNameEncoding)
	{
		FileStream fileStreamForOpen = GetFileStreamForOpen(mode, archiveFileName, useAsync: false);
		try
		{
			return new ZipArchive(fileStreamForOpen, mode, leaveOpen: false, entryNameEncoding);
		}
		catch
		{
			fileStreamForOpen.Dispose();
			throw;
		}
	}

	public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName)
	{
		DoCreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, null, includeBaseDirectory: false, null);
	}

	public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory)
	{
		DoCreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, compressionLevel, includeBaseDirectory, null);
	}

	public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory, Encoding? entryNameEncoding)
	{
		DoCreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, compressionLevel, includeBaseDirectory, entryNameEncoding);
	}

	public static void CreateFromDirectory(string sourceDirectoryName, Stream destination)
	{
		DoCreateFromDirectory(sourceDirectoryName, destination, null, includeBaseDirectory: false, null);
	}

	public static void CreateFromDirectory(string sourceDirectoryName, Stream destination, CompressionLevel compressionLevel, bool includeBaseDirectory)
	{
		DoCreateFromDirectory(sourceDirectoryName, destination, compressionLevel, includeBaseDirectory, null);
	}

	public static void CreateFromDirectory(string sourceDirectoryName, Stream destination, CompressionLevel compressionLevel, bool includeBaseDirectory, Encoding? entryNameEncoding)
	{
		DoCreateFromDirectory(sourceDirectoryName, destination, compressionLevel, includeBaseDirectory, entryNameEncoding);
	}

	private static void DoCreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel? compressionLevel, bool includeBaseDirectory, Encoding entryNameEncoding)
	{
		(sourceDirectoryName, destinationArchiveFileName) = GetFullPathsForDoCreateFromDirectory(sourceDirectoryName, destinationArchiveFileName);
		using ZipArchive archive = Open(destinationArchiveFileName, ZipArchiveMode.Create, entryNameEncoding);
		CreateZipArchiveFromDirectory(sourceDirectoryName, archive, compressionLevel, includeBaseDirectory);
	}

	private static void DoCreateFromDirectory(string sourceDirectoryName, Stream destination, CompressionLevel? compressionLevel, bool includeBaseDirectory, Encoding entryNameEncoding)
	{
		sourceDirectoryName = ValidateAndGetFullPathForDoCreateFromDirectory(sourceDirectoryName, destination, compressionLevel);
		using ZipArchive archive = new ZipArchive(destination, ZipArchiveMode.Create, leaveOpen: true, entryNameEncoding);
		CreateZipArchiveFromDirectory(sourceDirectoryName, archive, compressionLevel, includeBaseDirectory);
	}

	private static void CreateZipArchiveFromDirectory(string sourceDirectoryName, ZipArchive archive, CompressionLevel? compressionLevel, bool includeBaseDirectory)
	{
		(bool, string, DirectoryInfo, FileSystemEnumerable<(string, CreateEntryType)>) tuple = InitializeCreateZipArchiveFromDirectory(sourceDirectoryName, includeBaseDirectory);
		var (directoryIsEmpty, text, di, _) = tuple;
		foreach (var item3 in tuple.Item4)
		{
			string item = item3.Item1;
			CreateEntryType item2 = item3.Item2;
			directoryIsEmpty = false;
			switch (item2)
			{
			case CreateEntryType.File:
			{
				string entryName2 = ArchivingUtils.EntryFromPath(item.AsSpan(text.Length));
				archive.DoCreateEntryFromFile(item, entryName2, compressionLevel);
				break;
			}
			case CreateEntryType.Directory:
				if (ArchivingUtils.IsDirEmpty(item))
				{
					string entryName = ArchivingUtils.EntryFromPath(item.AsSpan(text.Length), appendPathSeparator: true);
					archive.CreateEntry(entryName);
				}
				break;
			default:
				throw new IOException(System.SR.Format(System.SR.ZipUnsupportedFile, item));
			}
		}
		FinalizeCreateZipArchiveFromDirectory(archive, di, includeBaseDirectory, directoryIsEmpty);
	}

	private static FileStream GetFileStreamForOpen(ZipArchiveMode mode, string archiveFileName, bool useAsync)
	{
		var (mode2, access, share) = mode switch
		{
			ZipArchiveMode.Read => (FileMode.Open, FileAccess.Read, FileShare.Read), 
			ZipArchiveMode.Create => (FileMode.CreateNew, FileAccess.Write, FileShare.None), 
			ZipArchiveMode.Update => (FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None), 
			_ => throw new ArgumentOutOfRangeException("mode"), 
		};
		return new FileStream(archiveFileName, mode2, access, share, 16384, useAsync);
	}

	private static (string, string) GetFullPathsForDoCreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName)
	{
		sourceDirectoryName = Path.GetFullPath(sourceDirectoryName);
		destinationArchiveFileName = Path.GetFullPath(destinationArchiveFileName);
		return (sourceDirectoryName, destinationArchiveFileName);
	}

	private static string ValidateAndGetFullPathForDoCreateFromDirectory(string sourceDirectoryName, Stream destination, CompressionLevel? compressionLevel)
	{
		ArgumentNullException.ThrowIfNull(destination, "destination");
		if (!destination.CanWrite)
		{
			throw new ArgumentException(System.SR.UnwritableStream, "destination");
		}
		if (compressionLevel.HasValue && !Enum.IsDefined(compressionLevel.Value))
		{
			throw new ArgumentOutOfRangeException("compressionLevel");
		}
		return Path.GetFullPath(sourceDirectoryName);
	}

	private static (bool, string, DirectoryInfo, FileSystemEnumerable<(string, CreateEntryType)>) InitializeCreateZipArchiveFromDirectory(string sourceDirectoryName, bool includeBaseDirectory)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirectoryName);
		string fullName = directoryInfo.FullName;
		if (includeBaseDirectory && directoryInfo.Parent != null)
		{
			fullName = directoryInfo.Parent.FullName;
		}
		FileSystemEnumerable<(string, CreateEntryType)> item = CreateEnumerableForCreate(directoryInfo.FullName);
		return (true, fullName, directoryInfo, item);
	}

	private static void FinalizeCreateZipArchiveFromDirectory(ZipArchive archive, DirectoryInfo di, bool includeBaseDirectory, bool directoryIsEmpty)
	{
		if (includeBaseDirectory & directoryIsEmpty)
		{
			archive.CreateEntry(ArchivingUtils.EntryFromPath(di.Name.AsSpan(), appendPathSeparator: true));
		}
	}

	public static Task<ZipArchive> OpenReadAsync(string archiveFileName, CancellationToken cancellationToken = default(CancellationToken))
	{
		return OpenAsync(archiveFileName, ZipArchiveMode.Read, cancellationToken);
	}

	public static Task<ZipArchive> OpenAsync(string archiveFileName, ZipArchiveMode mode, CancellationToken cancellationToken = default(CancellationToken))
	{
		return OpenAsync(archiveFileName, mode, null, cancellationToken);
	}

	public static async Task<ZipArchive> OpenAsync(string archiveFileName, ZipArchiveMode mode, Encoding? entryNameEncoding, CancellationToken cancellationToken = default(CancellationToken))
	{
		cancellationToken.ThrowIfCancellationRequested();
		FileStream fs = GetFileStreamForOpen(mode, archiveFileName, useAsync: true);
		try
		{
			return await ZipArchive.CreateAsync(fs, mode, leaveOpen: false, entryNameEncoding, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch
		{
			await fs.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			throw;
		}
	}

	public static Task CreateFromDirectoryAsync(string sourceDirectoryName, string destinationArchiveFileName, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DoCreateFromDirectoryAsync(sourceDirectoryName, destinationArchiveFileName, null, includeBaseDirectory: false, null, cancellationToken);
	}

	public static Task CreateFromDirectoryAsync(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DoCreateFromDirectoryAsync(sourceDirectoryName, destinationArchiveFileName, compressionLevel, includeBaseDirectory, null, cancellationToken);
	}

	public static Task CreateFromDirectoryAsync(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory, Encoding? entryNameEncoding, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DoCreateFromDirectoryAsync(sourceDirectoryName, destinationArchiveFileName, compressionLevel, includeBaseDirectory, entryNameEncoding, cancellationToken);
	}

	public static Task CreateFromDirectoryAsync(string sourceDirectoryName, Stream destination, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DoCreateFromDirectoryAsync(sourceDirectoryName, destination, null, includeBaseDirectory: false, null, cancellationToken);
	}

	public static Task CreateFromDirectoryAsync(string sourceDirectoryName, Stream destination, CompressionLevel compressionLevel, bool includeBaseDirectory, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DoCreateFromDirectoryAsync(sourceDirectoryName, destination, compressionLevel, includeBaseDirectory, null, cancellationToken);
	}

	public static Task CreateFromDirectoryAsync(string sourceDirectoryName, Stream destination, CompressionLevel compressionLevel, bool includeBaseDirectory, Encoding? entryNameEncoding, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DoCreateFromDirectoryAsync(sourceDirectoryName, destination, compressionLevel, includeBaseDirectory, entryNameEncoding, cancellationToken);
	}

	private static async Task DoCreateFromDirectoryAsync(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel? compressionLevel, bool includeBaseDirectory, Encoding entryNameEncoding, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		(string, string) fullPathsForDoCreateFromDirectory = GetFullPathsForDoCreateFromDirectory(sourceDirectoryName, destinationArchiveFileName);
		sourceDirectoryName = fullPathsForDoCreateFromDirectory.Item1;
		destinationArchiveFileName = fullPathsForDoCreateFromDirectory.Item2;
		ZipArchive zipArchive = await OpenAsync(destinationArchiveFileName, ZipArchiveMode.Create, entryNameEncoding, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		await using (zipArchive)
		{
			await CreateZipArchiveFromDirectoryAsync(sourceDirectoryName, zipArchive, compressionLevel, includeBaseDirectory, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private static async Task DoCreateFromDirectoryAsync(string sourceDirectoryName, Stream destination, CompressionLevel? compressionLevel, bool includeBaseDirectory, Encoding entryNameEncoding, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		sourceDirectoryName = ValidateAndGetFullPathForDoCreateFromDirectory(sourceDirectoryName, destination, compressionLevel);
		ZipArchive zipArchive = await ZipArchive.CreateAsync(destination, ZipArchiveMode.Create, leaveOpen: true, entryNameEncoding, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		await using (zipArchive)
		{
			await CreateZipArchiveFromDirectoryAsync(sourceDirectoryName, zipArchive, compressionLevel, includeBaseDirectory, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private static async Task CreateZipArchiveFromDirectoryAsync(string sourceDirectoryName, ZipArchive archive, CompressionLevel? compressionLevel, bool includeBaseDirectory, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var (directoryIsEmpty, basePath, di, fileSystemEnumerable) = InitializeCreateZipArchiveFromDirectory(sourceDirectoryName, includeBaseDirectory);
		foreach (var item3 in fileSystemEnumerable)
		{
			string item = item3.Item1;
			CreateEntryType item2 = item3.Item2;
			directoryIsEmpty = false;
			switch (item2)
			{
			case CreateEntryType.File:
			{
				string entryName2 = ArchivingUtils.EntryFromPath(item.AsSpan(basePath.Length));
				await archive.DoCreateEntryFromFileAsync(item, entryName2, compressionLevel, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				break;
			}
			case CreateEntryType.Directory:
				if (ArchivingUtils.IsDirEmpty(item))
				{
					string entryName = ArchivingUtils.EntryFromPath(item.AsSpan(basePath.Length), appendPathSeparator: true);
					archive.CreateEntry(entryName);
				}
				break;
			default:
				throw new IOException(System.SR.Format(System.SR.ZipUnsupportedFile, item));
			}
		}
		FinalizeCreateZipArchiveFromDirectory(archive, di, includeBaseDirectory, directoryIsEmpty);
	}

	public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName)
	{
		ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName, null, overwriteFiles: false);
	}

	public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, bool overwriteFiles)
	{
		ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName, null, overwriteFiles);
	}

	public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, Encoding? entryNameEncoding)
	{
		ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName, entryNameEncoding, overwriteFiles: false);
	}

	public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, Encoding? entryNameEncoding, bool overwriteFiles)
	{
		ArgumentNullException.ThrowIfNull(sourceArchiveFileName, "sourceArchiveFileName");
		using ZipArchive source = Open(sourceArchiveFileName, ZipArchiveMode.Read, entryNameEncoding);
		source.ExtractToDirectory(destinationDirectoryName, overwriteFiles);
	}

	public static void ExtractToDirectory(Stream source, string destinationDirectoryName)
	{
		ExtractToDirectory(source, destinationDirectoryName, null, overwriteFiles: false);
	}

	public static void ExtractToDirectory(Stream source, string destinationDirectoryName, bool overwriteFiles)
	{
		ExtractToDirectory(source, destinationDirectoryName, null, overwriteFiles);
	}

	public static void ExtractToDirectory(Stream source, string destinationDirectoryName, Encoding? entryNameEncoding)
	{
		ExtractToDirectory(source, destinationDirectoryName, entryNameEncoding, overwriteFiles: false);
	}

	public static void ExtractToDirectory(Stream source, string destinationDirectoryName, Encoding? entryNameEncoding, bool overwriteFiles)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.CanRead)
		{
			throw new ArgumentException(System.SR.UnreadableStream, "source");
		}
		using ZipArchive source2 = new ZipArchive(source, ZipArchiveMode.Read, leaveOpen: true, entryNameEncoding);
		source2.ExtractToDirectory(destinationDirectoryName, overwriteFiles);
	}

	public static Task ExtractToDirectoryAsync(string sourceArchiveFileName, string destinationDirectoryName, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ExtractToDirectoryAsync(sourceArchiveFileName, destinationDirectoryName, null, overwriteFiles: false, cancellationToken);
	}

	public static Task ExtractToDirectoryAsync(string sourceArchiveFileName, string destinationDirectoryName, bool overwriteFiles, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ExtractToDirectoryAsync(sourceArchiveFileName, destinationDirectoryName, null, overwriteFiles, cancellationToken);
	}

	public static Task ExtractToDirectoryAsync(string sourceArchiveFileName, string destinationDirectoryName, Encoding? entryNameEncoding, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ExtractToDirectoryAsync(sourceArchiveFileName, destinationDirectoryName, entryNameEncoding, overwriteFiles: false, cancellationToken);
	}

	public static async Task ExtractToDirectoryAsync(string sourceArchiveFileName, string destinationDirectoryName, Encoding? entryNameEncoding, bool overwriteFiles, CancellationToken cancellationToken = default(CancellationToken))
	{
		cancellationToken.ThrowIfCancellationRequested();
		ArgumentNullException.ThrowIfNull(sourceArchiveFileName, "sourceArchiveFileName");
		ZipArchive zipArchive = await OpenAsync(sourceArchiveFileName, ZipArchiveMode.Read, entryNameEncoding, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		await using (zipArchive)
		{
			await zipArchive.ExtractToDirectoryAsync(destinationDirectoryName, overwriteFiles, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	public static Task ExtractToDirectoryAsync(Stream source, string destinationDirectoryName, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ExtractToDirectoryAsync(source, destinationDirectoryName, null, overwriteFiles: false, cancellationToken);
	}

	public static Task ExtractToDirectoryAsync(Stream source, string destinationDirectoryName, bool overwriteFiles, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ExtractToDirectoryAsync(source, destinationDirectoryName, null, overwriteFiles, cancellationToken);
	}

	public static Task ExtractToDirectoryAsync(Stream source, string destinationDirectoryName, Encoding? entryNameEncoding, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ExtractToDirectoryAsync(source, destinationDirectoryName, entryNameEncoding, overwriteFiles: false, cancellationToken);
	}

	public static async Task ExtractToDirectoryAsync(Stream source, string destinationDirectoryName, Encoding? entryNameEncoding, bool overwriteFiles, CancellationToken cancellationToken = default(CancellationToken))
	{
		cancellationToken.ThrowIfCancellationRequested();
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.CanRead)
		{
			throw new ArgumentException(System.SR.UnreadableStream, "source");
		}
		ZipArchive zipArchive = await ZipArchive.CreateAsync(source, ZipArchiveMode.Read, leaveOpen: true, entryNameEncoding, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		await using (zipArchive)
		{
			await zipArchive.ExtractToDirectoryAsync(destinationDirectoryName, overwriteFiles, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private static FileSystemEnumerable<(string, CreateEntryType)> CreateEnumerableForCreate(string directoryFullPath)
	{
		return new FileSystemEnumerable<(string, CreateEntryType)>(directoryFullPath, delegate(ref FileSystemEntry entry)
		{
			return (entry.ToFullPath(), entry.IsDirectory ? CreateEntryType.Directory : CreateEntryType.File);
		}, new EnumerationOptions
		{
			RecurseSubdirectories = true,
			AttributesToSkip = FileAttributes.None,
			IgnoreInaccessible = false
		});
	}
}
