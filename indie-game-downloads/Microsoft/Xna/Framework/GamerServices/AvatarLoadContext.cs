using System;
using System.IO;
using System.Threading;
using Microsoft.XboxLive.Avatars.Internal;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarLoadContext : IAsyncResult
{
	public string Gamertag { get; private set; }

	public AsyncCallback OriginalCallback { get; private set; }

	public Stream Stream { get; private set; }

	public object AsyncState { get; private set; }

	public WaitHandle AsyncWaitHandle
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool CompletedSynchronously { get; private set; }

	public bool IsCompleted { get; private set; }

	public Exception Exception { get; private set; }

	public string SourceAddress { get; private set; }

	public AvatarLoadContext(AsyncCallback originalCallback, string gamertag, object state)
	{
		Gamertag = gamertag;
		OriginalCallback = originalCallback;
		IsCompleted = false;
		CompletedSynchronously = false;
		AsyncState = state;
	}

	internal void OnLoadComplete(DataRequestCompletedEventArgs e)
	{
		AvatarLoadContext ar = (AvatarLoadContext)e.UserState;
		if (e.Error != null)
		{
			Exception = e.Error;
		}
		else if (e.Cancelled)
		{
			Exception = new Exception("OperationCanceled");
		}
		else
		{
			Stream = e.Result;
			SourceAddress = e.SourceAddress;
		}
		IsCompleted = true;
		if (OriginalCallback != null)
		{
			OriginalCallback(ar);
		}
	}
}
