using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public interface IDataManager
{
	void GetAssetAsync(Guid avatarAsset, DownloadRequestEventHandler handler, object context);

	void GetAssetAsync(string avatarAsset, DownloadRequestEventHandler handler, object context);
}
