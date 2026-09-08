namespace Microsoft.XboxLive.Avatars.Internal;

public class LocalCacheSortRecord
{
	public string FileName { get; set; }

	public long FileSize { get; set; }

	public long LastAccessTime { get; set; }

	public LocalCacheSortRecord(string fileName, long fileSize, long lastAccessTime)
	{
		FileName = fileName;
		FileSize = fileSize;
		LastAccessTime = lastAccessTime;
	}

	public LocalCacheSortRecord(string fileName, LocalCacheRecord lcr)
	{
		FileName = fileName;
		FileSize = lcr.FileSize;
		LastAccessTime = lcr.LastAccessTime;
	}
}
