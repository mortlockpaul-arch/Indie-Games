using System;
using System.Threading;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarDescriptionAsyncResult : IAsyncResult
{
	private object syncObject;

	internal ManualResetEvent mre = new ManualResetEvent(initialState: true);

	public object AsyncState => syncObject;

	public WaitHandle AsyncWaitHandle => mre;

	public bool CompletedSynchronously => true;

	public bool IsCompleted => mre.WaitOne(0);

	public int PlayerIndex { get; set; }

	internal AvatarDescriptionAsyncResult(object stateObject, int playerIndex)
	{
		syncObject = stateObject;
		PlayerIndex = playerIndex;
	}
}
