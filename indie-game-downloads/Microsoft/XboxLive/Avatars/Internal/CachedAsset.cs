using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public abstract class CachedAsset
{
	public CoordinateSystem m_CoordinateSystem;

	public Guid AssetId;

	public int m_MemoryUsage;

	public int MemoryUsage => m_MemoryUsage;

	public abstract AssetCacheKey AssetKey { get; }

	public virtual int GetMemoryUsageInternal()
	{
		return 64;
	}

	public CachedAsset()
	{
	}
}
