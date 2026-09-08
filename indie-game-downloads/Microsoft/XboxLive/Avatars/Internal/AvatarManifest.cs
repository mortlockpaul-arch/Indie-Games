using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;
using Microsoft.XboxLive.Avatars.Internal.Version1;

namespace Microsoft.XboxLive.Avatars.Internal;

public abstract class AvatarManifest
{
	public class AvatarManifestObjectState
	{
		public AsyncOperation m_asyncOp;

		public IDataManager DataManager { get; set; }

		public Guid AssetId { get; set; }

		public AsyncOperation AsyncOp => m_asyncOp;

		public AvatarManifestObjectState(AsyncOperation asyncOp)
		{
			m_asyncOp = asyncOp;
		}
	}

	public const int MANIFEST_LEN_IN_BYTES = 1000;

	public int m_VersionNumber;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	public ManifestUpdateCompletedEventHandler UpdateCompleted;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	public ManifestUpdateDependenciesCompletedEventHandler UpdateDependenciesCompleted;

	public int Version => m_VersionNumber;

	public abstract AvatarGender BodyType { get; }

	public event ManifestUpdateCompletedEventHandler UpdateCompleted
	{
		[CompilerGenerated]
		add
		{
			ManifestUpdateCompletedEventHandler manifestUpdateCompletedEventHandler = this.UpdateCompleted;
			ManifestUpdateCompletedEventHandler manifestUpdateCompletedEventHandler2;
			do
			{
				manifestUpdateCompletedEventHandler2 = manifestUpdateCompletedEventHandler;
				ManifestUpdateCompletedEventHandler value2 = (ManifestUpdateCompletedEventHandler)Delegate.Combine(manifestUpdateCompletedEventHandler2, value);
				manifestUpdateCompletedEventHandler = Interlocked.CompareExchange(ref this.UpdateCompleted, value2, manifestUpdateCompletedEventHandler2);
			}
			while ((object)manifestUpdateCompletedEventHandler != manifestUpdateCompletedEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			ManifestUpdateCompletedEventHandler manifestUpdateCompletedEventHandler = this.UpdateCompleted;
			ManifestUpdateCompletedEventHandler manifestUpdateCompletedEventHandler2;
			do
			{
				manifestUpdateCompletedEventHandler2 = manifestUpdateCompletedEventHandler;
				ManifestUpdateCompletedEventHandler value2 = (ManifestUpdateCompletedEventHandler)Delegate.Remove(manifestUpdateCompletedEventHandler2, value);
				manifestUpdateCompletedEventHandler = Interlocked.CompareExchange(ref this.UpdateCompleted, value2, manifestUpdateCompletedEventHandler2);
			}
			while ((object)manifestUpdateCompletedEventHandler != manifestUpdateCompletedEventHandler2);
		}
	}

	public event ManifestUpdateDependenciesCompletedEventHandler UpdateDependenciesCompleted
	{
		[CompilerGenerated]
		add
		{
			ManifestUpdateDependenciesCompletedEventHandler manifestUpdateDependenciesCompletedEventHandler = this.UpdateDependenciesCompleted;
			ManifestUpdateDependenciesCompletedEventHandler manifestUpdateDependenciesCompletedEventHandler2;
			do
			{
				manifestUpdateDependenciesCompletedEventHandler2 = manifestUpdateDependenciesCompletedEventHandler;
				ManifestUpdateDependenciesCompletedEventHandler value2 = (ManifestUpdateDependenciesCompletedEventHandler)Delegate.Combine(manifestUpdateDependenciesCompletedEventHandler2, value);
				manifestUpdateDependenciesCompletedEventHandler = Interlocked.CompareExchange(ref this.UpdateDependenciesCompleted, value2, manifestUpdateDependenciesCompletedEventHandler2);
			}
			while ((object)manifestUpdateDependenciesCompletedEventHandler != manifestUpdateDependenciesCompletedEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			ManifestUpdateDependenciesCompletedEventHandler manifestUpdateDependenciesCompletedEventHandler = this.UpdateDependenciesCompleted;
			ManifestUpdateDependenciesCompletedEventHandler manifestUpdateDependenciesCompletedEventHandler2;
			do
			{
				manifestUpdateDependenciesCompletedEventHandler2 = manifestUpdateDependenciesCompletedEventHandler;
				ManifestUpdateDependenciesCompletedEventHandler value2 = (ManifestUpdateDependenciesCompletedEventHandler)Delegate.Remove(manifestUpdateDependenciesCompletedEventHandler2, value);
				manifestUpdateDependenciesCompletedEventHandler = Interlocked.CompareExchange(ref this.UpdateDependenciesCompleted, value2, manifestUpdateDependenciesCompletedEventHandler2);
			}
			while ((object)manifestUpdateDependenciesCompletedEventHandler != manifestUpdateDependenciesCompletedEventHandler2);
		}
	}

	public void ManifestUpdateWorker(object state)
	{
		AvatarManifestObjectState avatarManifestObjectState = state as AvatarManifestObjectState;
		Exception e = null;
		bool updateStatus = false;
		try
		{
			updateStatus = Update(avatarManifestObjectState.DataManager, avatarManifestObjectState.AssetId);
		}
		catch (Exception ex)
		{
			e = ex;
		}
		ManifestUpdateCompletedEventArgs arg = new ManifestUpdateCompletedEventArgs(e, updateStatus);
		avatarManifestObjectState.AsyncOp.PostOperationCompleted(OnUpdateCompleted, arg);
	}

	public void OnUpdateCompleted(object operationState)
	{
		ManifestUpdateCompletedEventArgs e = operationState as ManifestUpdateCompletedEventArgs;
		if (this.UpdateCompleted != null)
		{
			this.UpdateCompleted(this, e);
		}
	}

	public void UpdateAsync(IDataManager dataManager, Guid newAssetId)
	{
		AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(null);
		AvatarManifestObjectState avatarManifestObjectState = new AvatarManifestObjectState(asyncOp);
		avatarManifestObjectState.AssetId = newAssetId;
		avatarManifestObjectState.DataManager = dataManager;
		ThreadPool.QueueUserWorkItem(ManifestUpdateWorker, avatarManifestObjectState);
	}

	public static AvatarManifest Create(byte[] description)
	{
		AvatarManifestV1 avatarManifestV = new AvatarManifestV1();
		MemoryStream inputStream = new MemoryStream(description);
		EndianStream endianStream = new EndianStream(inputStream);
		bool flag = false;
		if (endianStream.ReadUInt() == 0)
		{
			flag = avatarManifestV.InitFromBinary(endianStream);
		}
		if (!flag)
		{
			return null;
		}
		return avatarManifestV;
	}

	public static Dictionary<string, AvatarManifest> Create(XmlReader description)
	{
		Dictionary<string, AvatarManifest> dictionary = new Dictionary<string, AvatarManifest>();
		try
		{
			if (!description.ReadToFollowing("AvatarManifests"))
			{
				return null;
			}
			if (description.ReadToDescendant("Manifests") && description.ReadToDescendant("AvatarManifest"))
			{
				while (description.ReadToDescendant("Gamertag"))
				{
					try
					{
						string text = description.ReadElementContentAsString();
						byte[] array = new byte[1000];
						description.ReadElementContentAsBinHex(array, 0, 1000);
						AvatarManifest value = Create(array);
						dictionary.Add(text.ToLower(), value);
					}
					catch (Exception ex)
					{
						string message = ex.Message;
						Logger.Log(new DebugLog(new AvatarManifestV1(), ex.Message));
					}
					finally
					{
						description.ReadToFollowing("AvatarManifest");
					}
				}
			}
		}
		catch (XmlException ex2)
		{
			string message2 = ex2.Message;
			return null;
		}
		return dictionary;
	}

	public static AvatarManifest Create(XmlReader description, string gamerTag)
	{
		try
		{
			if (!description.ReadToFollowing("AvatarManifests"))
			{
				return null;
			}
			if (description.ReadToDescendant("Manifests") && description.ReadToDescendant("AvatarManifest"))
			{
				while (description.ReadToDescendant("Gamertag"))
				{
					string text = description.ReadElementContentAsString();
					if (text.ToLower() == gamerTag.ToLower())
					{
						byte[] array = new byte[1000];
						description.ReadElementContentAsBinHex(array, 0, 1000);
						return Create(array);
					}
					description.ReadToFollowing("AvatarManifest");
				}
			}
			throw new NoAvatarManifestException("Manifest doesn't exists. Either Gamertag is invalid or there is no avatar associted with current gamertag.");
		}
		catch (XmlException)
		{
			return null;
		}
	}

	public abstract AvatarManifest Clone();

	public static bool operator ==(AvatarManifest a, AvatarManifest b)
	{
		if ((object)a == b)
		{
			return true;
		}
		if (!(a is AvatarManifestV1) || !(b is AvatarManifestV1))
		{
			return false;
		}
		AvatarManifestV1 avatarManifestV = a as AvatarManifestV1;
		AvatarManifestV1 avatarManifestV2 = b as AvatarManifestV1;
		return avatarManifestV == avatarManifestV2;
	}

	public static bool operator !=(AvatarManifest a, AvatarManifest b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is AvatarManifest))
		{
			return false;
		}
		return this == (AvatarManifest)obj;
	}

	public override int GetHashCode()
	{
		return 0;
	}

	public abstract List<ComponentInfo> GetComponents(AvatarComponentMasks mask);

	public abstract bool RemoveComponents(AvatarComponentMasks mask);

	public abstract void ReplaceComponent(ComponentInfo newComponent);

	public abstract bool IsComponentPresent(AvatarComponentType componentId);

	public abstract void UpdateDependencies(IDataManager dataManager);

	public abstract byte[] SaveToBinary();

	public static void DownloadAssetCompleted(object sender, DataRequestCompletedEventArgs e)
	{
		SyncDownloadContext syncDownloadContext = (SyncDownloadContext)e.UserState;
		if (e.Error != null)
		{
			syncDownloadContext.error = e.Error;
			syncDownloadContext.stream = null;
		}
		else if (e.Cancelled)
		{
			syncDownloadContext.stream = null;
		}
		else
		{
			syncDownloadContext.stream = e.Result;
		}
		syncDownloadContext.syncEvent.Set();
	}

	public bool Update(IDataManager dataManager, Guid newAssetId)
	{
		if (dataManager == null)
		{
			throw new ArgumentNullException("dataManager");
		}
		using (SyncDownloadContext syncDownloadContext = new SyncDownloadContext())
		{
			dataManager.GetAssetAsync(newAssetId, DownloadAssetCompleted, syncDownloadContext);
			syncDownloadContext.syncEvent.WaitOne();
			Stream stream = syncDownloadContext.stream;
			if (stream == null)
			{
				return false;
			}
			StructuredBinary structuredBinary = new StructuredBinary();
			if (!structuredBinary.Open(stream))
			{
				return false;
			}
			BlockIterator iterator = structuredBinary.Iterator;
			AssetMetadataParser assetMetadataParser = new AssetMetadataParser();
			if (!assetMetadataParser.LoadFromStrb(iterator))
			{
				throw new AvatarException(Resources.InvalidAnimationAssetFileText);
			}
			if ((assetMetadataParser.BodyTypeMask & BodyType) == 0)
			{
				return false;
			}
			if (assetMetadataParser.AssetType != BinaryAssetType.Component)
			{
				return false;
			}
			ReplaceComponent(new ComponentInfo
			{
				m_AssetId = newAssetId,
				m_ComponentMask = (AvatarComponentMasks)assetMetadataParser.AssetTypeDetails
			});
		}
		return true;
	}

	public static AvatarManifest[] CreateRandom(AvatarGender bodyMask, int avatarsCount)
	{
		if (bodyMask != AvatarGender.Male && bodyMask != AvatarGender.Female && bodyMask != AvatarGender.Both)
		{
			throw new ArgumentOutOfRangeException("bodyMask");
		}
		RandomAvatar randomAvatar = new RandomAvatar();
		return randomAvatar.CreateAvatars(bodyMask, avatarsCount);
	}

	public static AvatarManifest[] CreateRandom(IDataManager dataManager, AvatarGender bodyMask, int avatarsCount)
	{
		if (bodyMask != AvatarGender.Male && bodyMask != AvatarGender.Female && bodyMask != AvatarGender.Both)
		{
			throw new ArgumentOutOfRangeException("bodyMask");
		}
		Logger.Log(new DebugLog(new object(), "deprecated funtion call, use AvatarManifest.CreateRandom(bodymask, avatarscount) instead."));
		RandomAvatar randomAvatar = new RandomAvatar();
		return randomAvatar.CreateAvatars(bodyMask, avatarsCount);
	}

	public void UpdateDependenciesWorker(object state)
	{
		AvatarManifestObjectState avatarManifestObjectState = state as AvatarManifestObjectState;
		Exception error = null;
		try
		{
			UpdateDependencies(avatarManifestObjectState.DataManager);
		}
		catch (Exception ex)
		{
			error = ex;
		}
		AsyncCompletedEventArgs arg = new AsyncCompletedEventArgs(error, cancelled: false, null);
		avatarManifestObjectState.AsyncOp.PostOperationCompleted(OnUpdateDependenciesCompleted, arg);
	}

	public void OnUpdateDependenciesCompleted(object operationState)
	{
		AsyncCompletedEventArgs e = operationState as AsyncCompletedEventArgs;
		if (this.UpdateDependenciesCompleted != null)
		{
			this.UpdateDependenciesCompleted(this, e);
		}
	}

	public void UpdateDependenciesAsync(IDataManager dataManager)
	{
		AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(null);
		AvatarManifestObjectState avatarManifestObjectState = new AvatarManifestObjectState(asyncOp);
		avatarManifestObjectState.DataManager = dataManager;
		ThreadPool.QueueUserWorkItem(UpdateDependenciesWorker, avatarManifestObjectState);
	}

	public AvatarManifest()
	{
	}
}
