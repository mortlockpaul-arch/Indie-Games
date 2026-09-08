using System.Collections.Generic;
using Microsoft.XboxLive.Avatars.Internal.Version1;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AvatarAssetCacheManager
{
	public Dictionary<int, AvatarAssetCache> m_AvatarAssetCache = new Dictionary<int, AvatarAssetCache>();

	public LinkedList<CachedAsset> assetCache = new LinkedList<CachedAsset>();

	public Dictionary<AssetCacheKey, LinkedListNode<CachedAsset>> assetCacheDistionary = new Dictionary<AssetCacheKey, LinkedListNode<CachedAsset>>();

	public int estimatedMemoryUsage;

	public int minCachedAssets = 2;

	public int MaximalMemoryUsage { get; set; }

	public AvatarAssetCacheManager(int cacheSize)
	{
		MaximalMemoryUsage = cacheSize;
	}

	public void SetCacheSize(int cacheSize)
	{
		lock (m_AvatarAssetCache)
		{
			if (MaximalMemoryUsage > cacheSize)
			{
				MaximalMemoryUsage = cacheSize;
				CleanupCache();
			}
			else
			{
				MaximalMemoryUsage = cacheSize;
			}
		}
	}

	public AvatarAssetCache GetAssetCache(int version)
	{
		AvatarAssetCache value;
		lock (m_AvatarAssetCache)
		{
			m_AvatarAssetCache.TryGetValue(version, out value);
			if (value == null)
			{
				if (version != 1)
				{
					return null;
				}
				value = new AvatarAssetCacheV1();
				m_AvatarAssetCache.Add(version, value);
			}
		}
		return value;
	}

	public void CleanupCache()
	{
		int maximalMemoryUsage = MaximalMemoryUsage;
		while (assetCacheDistionary.Count > minCachedAssets && estimatedMemoryUsage > maximalMemoryUsage)
		{
			LinkedListNode<CachedAsset> last = assetCache.Last;
			CachedAsset value = last.Value;
			assetCacheDistionary.Remove(value.AssetKey);
			estimatedMemoryUsage -= value.MemoryUsage;
			assetCache.RemoveLast();
		}
	}

	public void AddToCache(CachedAsset asset)
	{
		lock (assetCache)
		{
			estimatedMemoryUsage += asset.MemoryUsage;
			CleanupCache();
			LinkedListNode<CachedAsset> linkedListNode = new LinkedListNode<CachedAsset>(asset);
			assetCache.AddFirst(linkedListNode);
			if (!assetCacheDistionary.ContainsKey(asset.AssetKey))
			{
				assetCacheDistionary.Add(asset.AssetKey, linkedListNode);
			}
		}
	}

	public LinkedListNode<CachedAsset> GetCachedAsset(AssetCacheKey key)
	{
		LinkedListNode<CachedAsset> value;
		lock (assetCache)
		{
			assetCacheDistionary.TryGetValue(key, out value);
		}
		return value;
	}

	public void Renew(CachedAsset asset)
	{
		lock (assetCache)
		{
			if (assetCache.Count > 1)
			{
				LinkedListNode<CachedAsset> node = assetCacheDistionary[asset.AssetKey];
				assetCache.Remove(node);
				assetCache.AddFirst(node);
			}
		}
	}

	public void ReleaseCache()
	{
		lock (assetCache)
		{
			assetCacheDistionary.Clear();
			assetCache.Clear();
			estimatedMemoryUsage = 0;
		}
	}

	public int GetEstimatedMemoryUsage()
	{
		return estimatedMemoryUsage;
	}

	public void AddAssetCache(AvatarAssetCache cache)
	{
		lock (m_AvatarAssetCache)
		{
			m_AvatarAssetCache.Add(cache.GetVersion(), cache);
		}
	}
}
