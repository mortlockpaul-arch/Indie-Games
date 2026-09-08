using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public class DataRequestGuid : DataRequest
{
	public Guid DataId { get; set; }

	public DataRequestGuid(Guid dataId, DataProvider dataSource, DownloadRequestEventHandler handler, object context)
		: base(dataSource, handler, context)
	{
		DataId = dataId;
	}

	public DataRequestGuid(Guid dataId, DataProvider dataSource, DownloadRequestEventHandler handler, object context, bool disableCacheWrite)
		: base(dataSource, handler, context, disableCacheWrite)
	{
		DataId = dataId;
	}

	public override void Clear()
	{
		base.Clear();
	}
}
