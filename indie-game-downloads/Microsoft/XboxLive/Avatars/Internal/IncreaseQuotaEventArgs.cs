using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public class IncreaseQuotaEventArgs : EventArgs
{
	public long AvailableFreeSpace { get; set; }

	public long Quota { get; set; }

	public long RecommendedQuota { get; set; }

	public IncreaseQuotaEventArgs(long availableFreeSpace, long quota, long recommendedQuota)
	{
		AvailableFreeSpace = availableFreeSpace;
		Quota = quota;
		RecommendedQuota = recommendedQuota;
	}
}
