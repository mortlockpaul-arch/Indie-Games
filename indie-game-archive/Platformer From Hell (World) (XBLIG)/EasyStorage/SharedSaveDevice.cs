using System;
using Microsoft.Xna.Framework.Storage;

namespace EasyStorage;

public sealed class SharedSaveDevice : SaveDevice
{
	protected override void GetStorageDevice(AsyncCallback callback)
	{
		StorageDevice.BeginShowSelector(callback, null);
	}
}
