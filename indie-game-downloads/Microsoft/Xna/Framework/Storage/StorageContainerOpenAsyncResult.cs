using System;
using System.Threading;

namespace Microsoft.Xna.Framework.Storage;

internal class StorageContainerOpenAsyncResult : IAsyncResult
{
	internal ManualResetEvent mre = new ManualResetEvent(initialState: true);

	internal StorageDevice storageDevice;

	internal int playerIndex;

	internal string displayName;

	private object stateObject;

	internal bool endHasBeenCalled;

	public object AsyncState => stateObject;

	public WaitHandle AsyncWaitHandle => mre;

	public bool CompletedSynchronously => true;

	public bool IsCompleted => mre.WaitOne(0);

	internal StorageContainerOpenAsyncResult(StorageDevice storageDevice, int playerIndex, string displayName, object stateObject)
	{
		this.storageDevice = storageDevice;
		this.playerIndex = playerIndex;
		this.displayName = displayName;
		this.stateObject = stateObject;
	}
}
