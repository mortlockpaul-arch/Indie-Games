namespace Microsoft.XboxLive.Avatars.Internal;

public class DataRequestString : DataRequest
{
	public string DataId { get; set; }

	public DataRequestString(string dataId, DataProvider dataSource, DownloadRequestEventHandler handler, object context)
		: base(dataSource, handler, context)
	{
		DataId = dataId;
	}

	public DataRequestString(string dataId, DataProvider dataSource, DownloadRequestEventHandler handler, object context, bool disableCacheWrite)
		: base(dataSource, handler, context, disableCacheWrite)
	{
		DataId = dataId;
	}

	public override void Clear()
	{
		base.Clear();
	}
}
