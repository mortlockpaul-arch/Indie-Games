using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Quasar.ContentPipeline;

namespace Quasar.Global;

public class ContentTracker : ContentManager
{
	private Dictionary<string, AssetTracker> loadedAssets = new Dictionary<string, AssetTracker>();

	private List<string> loadedAssetNames = new List<string>();

	private Stack<AssetTracker> loadingAssetsStack = new Stack<AssetTracker>();

	private object loadSyncRoot = new object();

	public bool IsLoading => loadingAssetsStack.Count > 0;

	public object LoadSyncRoot => loadSyncRoot;

	public ContentTracker(IServiceProvider serviceProvider)
		: base(serviceProvider)
	{
	}

	public ContentTracker(IServiceProvider serviceProvider, string rootDirectory)
		: base(serviceProvider, rootDirectory)
	{
	}

	public override T Load<T>(string assetName)
	{
		return Load<T>(assetName, null);
	}

	private bool CheckAsset<T>(string assetName, AssetTracker tracker, out T result)
	{
		if (loadedAssets.ContainsKey(assetName))
		{
			AssetTracker assetTracker = loadedAssets[assetName];
			T val = (T)assetTracker.Asset;
			assetTracker.RefCount++;
			if (Monitor.TryEnter(((ICollection)loadingAssetsStack).SyncRoot))
			{
				try
				{
					if (loadingAssetsStack.Count > 0)
					{
						loadingAssetsStack.Peek().RefersTo.Add(assetName);
						assetTracker.ReferredToBy.Add(loadingAssetsStack.Peek().AssetName);
					}
				}
				finally
				{
					Monitor.Exit(((ICollection)loadingAssetsStack).SyncRoot);
				}
			}
			result = val;
			return true;
		}
		result = default(T);
		return false;
	}

	private T Load<T>(string assetName, AssetTracker tracker)
	{
		if (assetName.Contains("\\"))
		{
			assetName = assetName.Replace("\\", "/");
		}
		lock (((ICollection)loadedAssets).SyncRoot)
		{
			if (CheckAsset<T>(assetName, tracker, out var result))
			{
				return result;
			}
		}
		lock (((ICollection)loadingAssetsStack).SyncRoot)
		{
			if (CheckAsset<T>(assetName, tracker, out var result2))
			{
				return result2;
			}
			if (tracker == null)
			{
				tracker = new AssetTracker();
				tracker.RefCount = 1;
				tracker.AssetName = assetName;
			}
			if (loadingAssetsStack.Count > 0)
			{
				loadingAssetsStack.Peek().RefersTo.Add(assetName);
				tracker.ReferredToBy.Add(loadingAssetsStack.Peek().AssetName);
			}
			loadingAssetsStack.Push(tracker);
			try
			{
				bool flag = true;
				if ((object)typeof(T) == typeof(XmlSource))
				{
					flag = false;
				}
				if ((object)typeof(T) == typeof(SoundEffect))
				{
					flag = false;
				}
				try
				{
					if (flag)
					{
						Monitor.Enter(loadSyncRoot);
					}
					tracker.Asset = ReadAsset<T>(assetName, tracker.TrackDisposableAsset);
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(loadSyncRoot);
					}
				}
				string text = "";
				for (int num = tracker.Disposables.Count - 1; num >= 0; num--)
				{
					text = "";
					IDisposable disposable = tracker.Disposables[num];
					if (tracker.Asset == disposable || SearchForAsset(disposable, out text))
					{
						tracker.Disposables.RemoveAt(num);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				loadingAssetsStack.Pop();
			}
			lock (((ICollection)loadedAssets).SyncRoot)
			{
				loadedAssets.Add(assetName, tracker);
				loadedAssetNames.Add(assetName);
				tracker.Status = AssetStatus.Active;
			}
		}
		return (T)tracker.Asset;
	}

	public override void Unload()
	{
		lock (((ICollection)loadingAssetsStack).SyncRoot)
		{
			lock (((ICollection)loadedAssets).SyncRoot)
			{
				Dictionary<string, AssetTracker>.Enumerator enumerator = loadedAssets.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Value.OnAssetChanged(new AssetChangedEventArgs(null));
					DisposeAssetTracker(enumerator.Current.Value, releaseChildren: false);
				}
				loadedAssets.Clear();
			}
		}
	}

	public void Release(string assetName)
	{
		lock (((ICollection)loadedAssets).SyncRoot)
		{
			if (loadedAssets.ContainsKey(assetName))
			{
				AssetTracker assetTracker = loadedAssets[assetName];
				assetTracker.RefCount--;
				if (assetTracker.RefCount == 0)
				{
					assetTracker.OnAssetChanged(new AssetChangedEventArgs(null));
					DisposeAssetTracker(assetTracker, releaseChildren: true);
					loadedAssets.Remove(assetName);
					loadedAssetNames.Remove(assetName);
				}
			}
		}
	}

	public void Unload(string assetName, bool releaseChildren)
	{
		if (loadedAssets.ContainsKey(assetName))
		{
			AssetTracker assetTracker = loadedAssets[assetName];
			assetTracker.OnAssetChanged(new AssetChangedEventArgs(null));
			DisposeAssetTracker(assetTracker, releaseChildren);
			loadedAssets.Remove(assetName);
			loadedAssetNames.Remove(assetName);
		}
	}

	private void DisposeAssetTracker(AssetTracker tracker, bool releaseChildren)
	{
		tracker.OnAssetChanged(new AssetChangedEventArgs(null));
		tracker.Status = AssetStatus.Disposed;
		foreach (IDisposable disposable in tracker.Disposables)
		{
			disposable.Dispose();
		}
		if (tracker.Asset is IDisposable)
		{
			((IDisposable)tracker.Asset).Dispose();
		}
		foreach (string item in tracker.RefersTo)
		{
			if (loadedAssets.ContainsKey(item))
			{
				loadedAssets[item].ReferredToBy.Remove(tracker.AssetName);
				if (releaseChildren)
				{
					Release(item);
				}
			}
		}
	}

	public T Reload<T>(string assetName)
	{
		if (loadedAssets.ContainsKey(assetName))
		{
			AssetTracker assetTracker = loadedAssets[assetName];
			loadedAssets.Remove(assetName);
			loadedAssetNames.Remove(assetName);
			T val = Load<T>(assetName);
			assetTracker.OnAssetChanged(new AssetChangedEventArgs(val));
			DisposeAssetTracker(assetTracker, releaseChildren: true);
			return val;
		}
		return Load<T>(assetName);
	}

	public bool IsLoaded(string assetName)
	{
		if (loadedAssets.ContainsKey(assetName))
		{
			return true;
		}
		return false;
	}

	public AssetTracker GetTracker(string assetName)
	{
		if (!loadedAssets.ContainsKey(assetName))
		{
			return null;
		}
		return loadedAssets[assetName];
	}

	public int GetReferenceCount(string assetName)
	{
		if (!loadedAssets.ContainsKey(assetName))
		{
			return 0;
		}
		return loadedAssets[assetName].RefCount;
	}

	public List<string> GetLoadedAssetNames()
	{
		return loadedAssetNames;
	}

	public bool SearchForAsset(object asset, out string assetName)
	{
		Dictionary<string, AssetTracker>.Enumerator enumerator = loadedAssets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (asset == enumerator.Current.Value.Asset)
			{
				assetName = enumerator.Current.Key;
				return true;
			}
		}
		assetName = "";
		return false;
	}
}
