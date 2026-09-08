#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AssetDataManager : DataManagerBase, IDataManager
{
	public AssetStorageDataProvider assetStorageProvider;

	public Queue<IDataProvider> assetDataProviders = new Queue<IDataProvider>();

	public Queue<IDataProvider> manifestDataProviders = new Queue<IDataProvider>();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	public EventHandler<IncreaseQuotaEventArgs> IncreaseQuota;

	public bool IsCaching
	{
		get
		{
			return assetStorageProvider != null;
		}
		set
		{
			if (value)
			{
				if (assetStorageProvider == null)
				{
					assetStorageProvider = new AssetStorageDataProvider();
					AssetLocalCache localCache = assetStorageProvider.LocalCache;
					localCache.IncreaseQuota += localCache_IncreaseQuota;
				}
			}
			else
			{
				if (assetStorageProvider != null)
				{
					AssetLocalCache localCache2 = assetStorageProvider.LocalCache;
					localCache2.IncreaseQuota -= localCache_IncreaseQuota;
				}
				assetStorageProvider = null;
			}
		}
	}

	public event EventHandler<IncreaseQuotaEventArgs> IncreaseQuota
	{
		[CompilerGenerated]
		add
		{
			EventHandler<IncreaseQuotaEventArgs> eventHandler = this.IncreaseQuota;
			EventHandler<IncreaseQuotaEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<IncreaseQuotaEventArgs> value2 = (EventHandler<IncreaseQuotaEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.IncreaseQuota, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler<IncreaseQuotaEventArgs> eventHandler = this.IncreaseQuota;
			EventHandler<IncreaseQuotaEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<IncreaseQuotaEventArgs> value2 = (EventHandler<IncreaseQuotaEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.IncreaseQuota, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public AssetDataManager()
	{
		InitializeThread();
	}

	public void AddAssetProvider(IDataProvider dataProvider)
	{
		if (dataProvider == null)
		{
			throw new ArgumentNullException("dataProvider");
		}
		assetDataProviders.Enqueue(dataProvider);
	}

	public void AddManifestProvider(IDataProvider dataProvider)
	{
		if (dataProvider == null)
		{
			throw new ArgumentNullException("dataProvider");
		}
		manifestDataProviders.Enqueue(dataProvider);
	}

	public void ClearAssetProviders()
	{
		assetDataProviders.Clear();
	}

	public void ClearManifestProviders()
	{
		manifestDataProviders.Clear();
	}

	public void localCache_IncreaseQuota(object sender, IncreaseQuotaEventArgs e)
	{
		if (this.IncreaseQuota != null)
		{
			this.IncreaseQuota(this, e);
		}
	}

	public void DownloadOpenReadSyncCompleted(object sender, DataRequestCompletedEventArgs e)
	{
		SyncDownloadContext syncDownloadContext = e.UserState as SyncDownloadContext;
		if (e.Error != null)
		{
			syncDownloadContext.stream = null;
		}
		else if (e.Cancelled)
		{
			syncDownloadContext.stream = null;
		}
		else
		{
			syncDownloadContext.stream = e.Result;
		}
		syncDownloadContext.syncEvent.Set();
	}

	public static DataProvider GetDataProvider(Queue<IDataProvider> dataProviders)
	{
		return new DataProvider(dataProviders);
	}

	public virtual void GetAssetAsync(Guid avatarAsset, DownloadRequestEventHandler handler, object context)
	{
		DataProvider dataProvider = GetDataProvider(assetDataProviders);
		DataRequest request = new DataRequestGuid(avatarAsset, dataProvider, handler, context);
		EnqueueRequest(request, DownloadPriority.Normal);
	}

	public virtual void GetAssetAsync(string avatarAsset, DownloadRequestEventHandler handler, object context)
	{
		DataProvider dataProvider = GetDataProvider(assetDataProviders);
		DataRequest request = new DataRequestString(avatarAsset, dataProvider, handler, context);
		EnqueueRequest(request, DownloadPriority.Normal);
	}

	public Stream DownloadManifest(string gamerTag)
	{
		using SyncDownloadContext syncDownloadContext = new SyncDownloadContext();
		DataProvider dataProvider = GetDataProvider(manifestDataProviders);
		DataRequest request = new DataRequestString(gamerTag, dataProvider, DownloadOpenReadSyncCompleted, syncDownloadContext);
		EnqueueRequest(request, DownloadPriority.High);
		syncDownloadContext.syncEvent.WaitOne();
		return syncDownloadContext.DetachStream();
	}

	public void GetManifestAsync(string gamerTag, DownloadRequestEventHandler handler, object context)
	{
		DataProvider dataProvider = GetDataProvider(manifestDataProviders);
		DataRequest request = new DataRequestString(gamerTag, dataProvider, handler, context);
		EnqueueRequest(request, DownloadPriority.Normal);
	}

	public override void OnFinishRequest(DataRequestCompletedEventArgs eventArguments, DataRequest dataRequest)
	{
		if (eventArguments.Error == null && !eventArguments.Cancelled && IsCaching && !dataRequest.CacheWriteDisabled && dataRequest.DataSource.ProcessingProvider is NetDataProvider)
		{
			AssetLocalCache localCache = assetStorageProvider.LocalCache;
			NetDataProvider netDataProvider = (NetDataProvider)dataRequest.DataSource.ProcessingProvider;
			string assetId = null;
			if (dataRequest is DataRequestGuid)
			{
				assetId = ((DataRequestGuid)dataRequest).DataId.ToString();
			}
			else if (dataRequest is DataRequestString)
			{
				assetId = ((DataRequestString)dataRequest).DataId.ToString();
			}
			string addressFormat = netDataProvider.GetAddressFormat(dataRequest);
			string fullIdAddress = AssetStorageDataProvider.GetFullIdAddress(addressFormat, assetId);
			long ticks = DateTime.Now.Ticks;
			localCache.Store(fullIdAddress, eventArguments.Result);
			Debug.WriteLine("Writting to Cache : {0} in {1} ms", fullIdAddress, (float)(DateTime.Now.Ticks - ticks) / 10000f);
		}
	}

	public void CleanCache()
	{
		AssetStorageDataProvider.CleanCache();
	}

	public void IncreaseCacheQuotaTo(long newSize)
	{
		AssetStorageDataProvider.IncreaseCacheQuotaTo(newSize);
	}
}
