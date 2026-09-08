using System;
using System.Threading;

namespace Microsoft.Xna.Framework.Storage;

internal class StorageDeviceAsyncResult : IAsyncResult
{
	private object syncObject;

	internal ManualResetEvent mre = new ManualResetEvent(initialState: true);

	internal int playerIndex;

	internal bool endHasBeenCalled;

	public object AsyncState => syncObject;

	public WaitHandle AsyncWaitHandle => mre;

	public bool CompletedSynchronously => true;

	public bool IsCompleted => mre.WaitOne(0, exitContext: false);

	internal StorageDeviceAsyncResult(object stateObject, int player)
	{
		syncObject = stateObject;
		playerIndex = player;
	}
}
