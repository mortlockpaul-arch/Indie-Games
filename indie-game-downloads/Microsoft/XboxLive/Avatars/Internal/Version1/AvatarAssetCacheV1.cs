using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class AvatarAssetCacheV1 : AvatarAssetCache
{
	public class CachedAssetsAsyncLoadContextCommon
	{
		public int numRequests;

		public bool failed;

		public AutoResetEvent syncEvent;

		public object syncRequestLock;

		public List<BinaryAsset> requestedAssets;
	}

	public class CachedAssetsAsyncLoadContext
	{
		public int index;

		public CachedAssetsAsyncLoadContextCommon common = new CachedAssetsAsyncLoadContextCommon();
	}

	public class PendingAsset : IDisposable
	{
		public int m_References;

		public CachedBinaryAsset m_CacheItem;

		public int m_OriginalIndex;

		public BinaryAsset m_BinaryAsset;

		public ManualResetEvent m_WaitEvent = new ManualResetEvent(initialState: false);

		public PendingAsset(BinaryAsset asset, int originalIndex)
		{
			m_References = 1;
			m_BinaryAsset = asset;
			m_OriginalIndex = originalIndex;
		}

		public void DownloadCompleted(CachedBinaryAsset parsedAsset)
		{
			m_CacheItem = parsedAsset;
			m_WaitEvent.Set();
			ReleaseReference();
		}

		public void ReleaseReference()
		{
			if (Interlocked.Decrement(ref m_References) == 0)
			{
				m_WaitEvent.Close();
				m_WaitEvent = null;
				m_CacheItem = null;
			}
		}

		public void AddReference()
		{
			Interlocked.Increment(ref m_References);
		}

		public virtual void Dispose(bool disposing)
		{
			if (disposing && m_WaitEvent != null)
			{
				m_WaitEvent.Close();
			}
			m_WaitEvent = null;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}

	public class PendingAssetPair
	{
		public PendingAsset pendingAsset;

		public BinaryAsset binaryAsset;

		public PendingAssetPair(PendingAsset pendingAsset, BinaryAsset binaryAsset)
		{
			this.pendingAsset = pendingAsset;
			this.binaryAsset = binaryAsset;
		}
	}

	public Dictionary<AssetCacheKey, PendingAsset> pendingAssets = new Dictionary<AssetCacheKey, PendingAsset>();

	public override int GetVersion()
	{
		return 1;
	}

	public void DownloadAssetCompleted(object sender, DataRequestCompletedEventArgs e)
	{
		CachedAssetsAsyncLoadContext cachedAssetsAsyncLoadContext = e.UserState as CachedAssetsAsyncLoadContext;
		lock (cachedAssetsAsyncLoadContext.common.syncRequestLock)
		{
			cachedAssetsAsyncLoadContext.common.numRequests--;
			if (e.Error != null)
			{
				cachedAssetsAsyncLoadContext.common.failed = true;
			}
			else if (e.Cancelled)
			{
				cachedAssetsAsyncLoadContext.common.failed = true;
			}
			else
			{
				cachedAssetsAsyncLoadContext.common.requestedAssets[cachedAssetsAsyncLoadContext.index].Stream = e.Result;
			}
			if (cachedAssetsAsyncLoadContext.common.numRequests == 0)
			{
				cachedAssetsAsyncLoadContext.common.syncEvent.Set();
			}
		}
	}

	public bool LoadAssets(List<BinaryAsset> assets, BinaryAssetParseContext parseContext, IDataManager dataManager)
	{
		int count = assets.Count;
		List<PendingAsset> list = new List<PendingAsset>();
		List<PendingAssetPair> list2 = new List<PendingAssetPair>();
		bool flag = false;
		AvatarAssetCacheManager assetCache = parseContext.m_AssetCache;
		bool lockTaken = false;
		Dictionary<AssetCacheKey, PendingAsset> obj = null;
		try
		{
			Monitor.Enter(obj = pendingAssets, ref lockTaken);
			for (int i = 0; i < count; i++)
			{
				AssetCacheKey assetKey = assets[i].GetAssetKey(assets[i].AssetId, parseContext.m_CoordinateSystem, assets[i].m_SkeletonVersion);
				LinkedListNode<CachedAsset> cachedAsset = assetCache.GetCachedAsset(assetKey);
				if (cachedAsset != null)
				{
					assets[i].m_Cache = cachedAsset.Value as CachedBinaryAsset;
					assetCache.Renew(cachedAsset.Value);
					continue;
				}
				pendingAssets.TryGetValue(assetKey, out var value);
				if (value != null)
				{
					value.AddReference();
					list2.Add(new PendingAssetPair(value, assets[i]));
				}
				else
				{
					value = new PendingAsset(assets[i], i);
					pendingAssets.Add(assetKey, value);
					list.Add(value);
				}
			}
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(obj);
			}
		}
		int count2 = list.Count;
		if (count2 > 0)
		{
			CachedAssetsAsyncLoadContextCommon cachedAssetsAsyncLoadContextCommon = new CachedAssetsAsyncLoadContextCommon();
			cachedAssetsAsyncLoadContextCommon.syncRequestLock = new object();
			using (cachedAssetsAsyncLoadContextCommon.syncEvent = new AutoResetEvent(initialState: false))
			{
				cachedAssetsAsyncLoadContextCommon.numRequests = count2;
				cachedAssetsAsyncLoadContextCommon.requestedAssets = assets;
				for (int j = 0; j < count2; j++)
				{
					BinaryAsset binaryAsset = list[j].m_BinaryAsset;
					CachedBinaryAsset cacheItem = (binaryAsset.m_Cache = binaryAsset.CreateCacheItem());
					list[j].m_CacheItem = cacheItem;
					CachedAssetsAsyncLoadContext cachedAssetsAsyncLoadContext = new CachedAssetsAsyncLoadContext();
					cachedAssetsAsyncLoadContext.index = list[j].m_OriginalIndex;
					cachedAssetsAsyncLoadContext.common = cachedAssetsAsyncLoadContextCommon;
					dataManager.GetAssetAsync(binaryAsset.AssetId, DownloadAssetCompleted, cachedAssetsAsyncLoadContext);
				}
				cachedAssetsAsyncLoadContextCommon.syncEvent.WaitOne();
			}
			flag = cachedAssetsAsyncLoadContextCommon.failed;
			for (int k = 0; k < count2; k++)
			{
				BinaryAsset binaryAsset2 = list[k].m_BinaryAsset;
				AssetCacheKey assetKey2 = binaryAsset2.GetAssetKey(binaryAsset2.AssetId, parseContext.m_CoordinateSystem, binaryAsset2.m_SkeletonVersion);
				CachedBinaryAsset cache = binaryAsset2.m_Cache;
				if (binaryAsset2.Stream == null)
				{
					Logger.Log(new DebugLog(this, $"Asset \"{binaryAsset2.AssetId}\" could not be downloaded"));
					cache.m_AssetState = CachedBinaryAsset.AssetState.Invalid;
					bool lockTaken2 = false;
					try
					{
						Monitor.Enter(obj = pendingAssets, ref lockTaken2);
						list[k].DownloadCompleted(cache);
						pendingAssets.Remove(assetKey2);
					}
					finally
					{
						if (lockTaken2)
						{
							Monitor.Exit(obj);
						}
					}
					continue;
				}
				bool flag2;
				try
				{
					flag2 = cache.Parse(binaryAsset2.Stream, parseContext);
				}
				catch (Exception ex)
				{
					Logger.Log(new DebugLog(this, $"Error \"{ex.Message}\" occured during parsing of asset {binaryAsset2.AssetId}"));
					flag2 = false;
				}
				if (flag2)
				{
					cache.m_AssetState = CachedBinaryAsset.AssetState.Parsed;
					bool lockTaken3 = false;
					try
					{
						Monitor.Enter(obj = pendingAssets, ref lockTaken3);
						assetCache.AddToCache(cache);
						list[k].DownloadCompleted(cache);
						pendingAssets.Remove(assetKey2);
					}
					finally
					{
						if (lockTaken3)
						{
							Monitor.Exit(obj);
						}
					}
				}
				else
				{
					cache.m_AssetState = CachedBinaryAsset.AssetState.Downloaded;
					bool lockTaken4 = false;
					try
					{
						Monitor.Enter(obj = pendingAssets, ref lockTaken4);
						list[k].DownloadCompleted(cache);
						pendingAssets.Remove(assetKey2);
					}
					finally
					{
						if (lockTaken4)
						{
							Monitor.Exit(obj);
						}
					}
					flag = true;
				}
				binaryAsset2.Stream.Close();
				binaryAsset2.Stream = null;
			}
		}
		int count3 = list2.Count;
		if (count3 > 0)
		{
			for (int l = 0; l < count3; l++)
			{
				PendingAsset pendingAsset = list2[l].pendingAsset;
				pendingAsset.m_WaitEvent.WaitOne();
				if (pendingAsset.m_CacheItem.m_AssetState != CachedBinaryAsset.AssetState.Parsed)
				{
					flag = true;
				}
				list2[l].binaryAsset.m_Cache = pendingAsset.m_CacheItem;
				pendingAsset.ReleaseReference();
			}
		}
		return !flag;
	}
}
