using System;
using System.Collections.Generic;

namespace Quasar.Global;

public class AssetTracker
{
	public string AssetName;

	public object Asset;

	public List<IDisposable> Disposables = new List<IDisposable>();

	public int RefCount;

	public AssetStatus Status = AssetStatus.Disposed;

	public List<string> RefersTo = new List<string>();

	public List<string> ReferredToBy = new List<string>();

	public event AssetChangedEvent AssetChanged;

	public void TrackDisposableAsset(IDisposable disposable)
	{
		Disposables.Add(disposable);
	}

	protected internal void OnAssetChanged(AssetChangedEventArgs args)
	{
		if (AssetChanged != null)
		{
			AssetChanged(this, args);
		}
	}
}
