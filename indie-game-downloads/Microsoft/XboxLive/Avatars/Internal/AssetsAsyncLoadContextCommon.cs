using System.Threading;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AssetsAsyncLoadContextCommon
{
	public int numRequests;

	public bool failed;

	public AutoResetEvent syncEvent;

	public object syncRequestLock = new object();
}
