using System;
using System.Threading;
using Microsoft.XboxLive.Avatars.Internal.Animations;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarPropLoadContext : IAsyncResult, IDisposable
{
	private AvatarDescription avatarDescription;

	private AsyncCallback callback;

	private ManualResetEvent waitHandle = new ManualResetEvent(initialState: false);

	internal Microsoft.XboxLive.Avatars.Internal.Animations.AvatarAnimation Animation { get; private set; }

	public object AsyncState { get; private set; }

	public bool CompletedSynchronously { get; private set; }

	public bool IsCompleted { get; private set; }

	internal bool ResultUsed { get; set; }

	public WaitHandle AsyncWaitHandle => waitHandle;

	internal AvatarPropLoadContext(AvatarDescription avatarDescription, AsyncCallback callback, object asyncState)
	{
		this.avatarDescription = avatarDescription;
		this.callback = callback;
		IsCompleted = false;
		CompletedSynchronously = false;
		AsyncState = asyncState;
		ResultUsed = false;
	}

	internal void Load()
	{
		if (avatarDescription.HasProp)
		{
			avatarDescription.LoadAvatar(OnAvatarLoaded);
			return;
		}
		CompletedSynchronously = true;
		IsCompleted = true;
		waitHandle.Set();
		if (callback != null)
		{
			callback(this);
		}
	}

	private void OnAvatarLoaded(object sender, AvatarLoadedEventArgs eventArgs)
	{
		if (eventArgs.Result == AvatarAssetLoadResult.Loaded && eventArgs.Avatar.Carryable != null)
		{
			Animation = eventArgs.Avatar.Carryable.Animation;
		}
		IsCompleted = true;
		waitHandle.Set();
		if (callback != null)
		{
			callback(this);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing && waitHandle != null)
		{
			waitHandle.Dispose();
		}
		waitHandle = null;
		avatarDescription = null;
		callback = null;
		Animation = null;
		AsyncState = null;
	}

	~AvatarPropLoadContext()
	{
		Dispose(disposing: false);
	}
}
