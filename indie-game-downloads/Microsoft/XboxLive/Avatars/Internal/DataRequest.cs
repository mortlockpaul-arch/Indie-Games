namespace Microsoft.XboxLive.Avatars.Internal;

public abstract class DataRequest
{
	public DataProvider DataSource { get; set; }

	public DownloadRequestEventHandler Handler { get; set; }

	public object Context { get; set; }

	public bool CacheWriteDisabled { get; set; }

	public DataRequest(DataProvider dataSource, DownloadRequestEventHandler handler, object context)
	{
		Handler = handler;
		Context = context;
		DataSource = dataSource;
		CacheWriteDisabled = false;
	}

	public DataRequest(DataProvider dataSource, DownloadRequestEventHandler handler, object context, bool disableCacheWrite)
	{
		Handler = handler;
		Context = context;
		DataSource = dataSource;
		CacheWriteDisabled = disableCacheWrite;
	}

	public virtual void Clear()
	{
		Handler = null;
		Context = null;
		DataSource = null;
		CacheWriteDisabled = false;
	}
}
