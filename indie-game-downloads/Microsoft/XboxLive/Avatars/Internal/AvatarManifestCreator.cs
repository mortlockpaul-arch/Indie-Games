using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.XboxLive.Avatars.Internal.Version1;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AvatarManifestCreator
{
	public class AvatarManifestCreatorState
	{
		public AsyncOperation m_asyncOp;

		public int AvatarsCount { get; set; }

		public AvatarGender BodyMask { get; set; }

		public AsyncOperation AsyncOp => m_asyncOp;

		public AvatarManifestCreatorState(AsyncOperation asyncOp)
		{
			m_asyncOp = asyncOp;
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	public CreateRandomManifestCompletedEventHandler CreateRandomCompleted;

	public event CreateRandomManifestCompletedEventHandler CreateRandomCompleted
	{
		[CompilerGenerated]
		add
		{
			CreateRandomManifestCompletedEventHandler createRandomManifestCompletedEventHandler = this.CreateRandomCompleted;
			CreateRandomManifestCompletedEventHandler createRandomManifestCompletedEventHandler2;
			do
			{
				createRandomManifestCompletedEventHandler2 = createRandomManifestCompletedEventHandler;
				CreateRandomManifestCompletedEventHandler value2 = (CreateRandomManifestCompletedEventHandler)Delegate.Combine(createRandomManifestCompletedEventHandler2, value);
				createRandomManifestCompletedEventHandler = Interlocked.CompareExchange(ref this.CreateRandomCompleted, value2, createRandomManifestCompletedEventHandler2);
			}
			while ((object)createRandomManifestCompletedEventHandler != createRandomManifestCompletedEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			CreateRandomManifestCompletedEventHandler createRandomManifestCompletedEventHandler = this.CreateRandomCompleted;
			CreateRandomManifestCompletedEventHandler createRandomManifestCompletedEventHandler2;
			do
			{
				createRandomManifestCompletedEventHandler2 = createRandomManifestCompletedEventHandler;
				CreateRandomManifestCompletedEventHandler value2 = (CreateRandomManifestCompletedEventHandler)Delegate.Remove(createRandomManifestCompletedEventHandler2, value);
				createRandomManifestCompletedEventHandler = Interlocked.CompareExchange(ref this.CreateRandomCompleted, value2, createRandomManifestCompletedEventHandler2);
			}
			while ((object)createRandomManifestCompletedEventHandler != createRandomManifestCompletedEventHandler2);
		}
	}

	public void CreateRandomMnifestWorker(object state)
	{
		AvatarManifestCreatorState avatarManifestCreatorState = state as AvatarManifestCreatorState;
		Exception e = null;
		AvatarManifest[] randomManifests = null;
		try
		{
			RandomAvatar randomAvatar = new RandomAvatar();
			randomManifests = randomAvatar.CreateAvatars(avatarManifestCreatorState.BodyMask, avatarManifestCreatorState.AvatarsCount);
		}
		catch (Exception ex)
		{
			e = ex;
		}
		CreateRandomManifestCompletedEventArgs arg = new CreateRandomManifestCompletedEventArgs(e, randomManifests);
		avatarManifestCreatorState.AsyncOp.PostOperationCompleted(OnCreateRandomCompleted, arg);
	}

	public void OnCreateRandomCompleted(object operationState)
	{
		CreateRandomManifestCompletedEventArgs e = operationState as CreateRandomManifestCompletedEventArgs;
		if (this.CreateRandomCompleted != null)
		{
			this.CreateRandomCompleted(this, e);
		}
	}

	public void CreateRandomAsync(AvatarGender bodyMask, int avatarsCount)
	{
		AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(null);
		AvatarManifestCreatorState avatarManifestCreatorState = new AvatarManifestCreatorState(asyncOp);
		avatarManifestCreatorState.AvatarsCount = avatarsCount;
		avatarManifestCreatorState.BodyMask = bodyMask;
		ThreadPool.QueueUserWorkItem(CreateRandomMnifestWorker, avatarManifestCreatorState);
	}
}
