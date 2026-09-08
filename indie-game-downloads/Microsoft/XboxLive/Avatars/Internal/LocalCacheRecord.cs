namespace Microsoft.XboxLive.Avatars.Internal;

public class LocalCacheRecord
{
	public long FileSize { get; set; }

	public long LastAccessTime { get; set; }

	public LocalCacheRecord(long fileSize, long lastAccessTime)
	{
		FileSize = fileSize;
		LastAccessTime = lastAccessTime;
	}
}
