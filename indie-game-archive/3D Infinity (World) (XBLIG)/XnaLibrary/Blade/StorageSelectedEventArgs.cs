using System;
using Microsoft.Xna.Framework.Storage;

namespace XnaLibrary.Blade;

public class StorageSelectedEventArgs : EventArgs
{
	public bool IsCancel { get; set; }

	public StorageContainer Container { get; set; }
}
