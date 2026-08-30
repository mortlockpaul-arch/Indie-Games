using System;
using Microsoft.Xna.Framework.Storage;

namespace Kobingo.Xna.Library.Game;

public class LoadingEventArgs : EventArgs
{
	public StorageContainer Container { get; set; }
}
