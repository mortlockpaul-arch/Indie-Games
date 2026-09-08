using System;

namespace Quasar.Global;

public class AssetChangedEventArgs : EventArgs
{
	public object NewReference;

	public AssetChangedEventArgs(object newRef)
	{
		NewReference = newRef;
	}
}
