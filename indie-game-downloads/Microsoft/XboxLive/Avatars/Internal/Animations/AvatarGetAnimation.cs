using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Parsers;
using Microsoft.XboxLive.Avatars.Internal.Version1;

namespace Microsoft.XboxLive.Avatars.Internal.Animations;

public class AvatarGetAnimation
{
	public CoordinateSystem m_CoordSystem;

	public AvatarGetAnimation(CoordinateSystem flags)
	{
		m_CoordSystem = flags;
	}

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

	public AvatarAnimation Load(Guid animationAssetId, AssetLoader assetLoader, IDataManager dataManager)
	{
		AvatarAssetCacheManager assetCacheManager = assetLoader.GetAssetCacheManager();
		if (assetCacheManager != null)
		{
			AvatarAssetCacheV1 avatarAssetCacheV = assetCacheManager.GetAssetCache(1) as AvatarAssetCacheV1;
			BinaryAssetAnimation binaryAssetAnimation = new BinaryAssetAnimation(animationAssetId);
			BinaryAssetParseContext binaryAssetParseContext = new BinaryAssetParseContext();
			binaryAssetParseContext.m_CoordinateSystem = m_CoordSystem;
			binaryAssetParseContext.m_AssetCache = assetCacheManager;
			List<BinaryAsset> list = new List<BinaryAsset>();
			list.Add(binaryAssetAnimation);
			avatarAssetCacheV.LoadAssets(list, binaryAssetParseContext, dataManager);
			if (!(binaryAssetAnimation.m_Cache is CachedBinaryAssetAnimation { m_AssetState: var assetState } cachedBinaryAssetAnimation))
			{
				throw new AvatarException(Resources.InvalidAnimationAssetFileText);
			}
			return assetState switch
			{
				CachedBinaryAsset.AssetState.Invalid => throw new AvatarException(Resources.FailedToDownloadAnimationAssetText), 
				CachedBinaryAsset.AssetState.Downloaded => throw new AvatarException(Resources.FailedToParseAnimationAssetFileText), 
				_ => cachedBinaryAssetAnimation.Animation, 
			};
		}
		using SyncDownloadContext syncDownloadContext = new SyncDownloadContext();
		dataManager.GetAssetAsync(animationAssetId, DownloadAssetCompleted, syncDownloadContext);
		syncDownloadContext.syncEvent.WaitOne();
		Stream stream = syncDownloadContext.stream;
		if (stream == null)
		{
			throw new AvatarException(Resources.FailedToDownloadAnimationAssetText);
		}
		StructuredBinary structuredBinary = new StructuredBinary();
		if (!structuredBinary.Open(stream))
		{
			throw new AvatarException(Resources.InvalidAnimationAssetFileText);
		}
		BlockIterator iterator = structuredBinary.Iterator;
		if (!iterator.FindFirst(StructuredBinaryBlockId.Animation))
		{
			throw new AvatarException(Resources.InvalidAnimationAssetFileText);
		}
		AssetAnimationParser assetAnimationParser = new AssetAnimationParser(m_CoordSystem, AssetLoader.GetAssetBodyType(animationAssetId));
		return assetAnimationParser.Parse(iterator);
	}

	public AvatarAnimation Load(Stream animationStream)
	{
		animationStream.Seek(0L, SeekOrigin.Begin);
		StructuredBinary structuredBinary = new StructuredBinary();
		if (!structuredBinary.Open(animationStream))
		{
			throw new AvatarException(Resources.InvalidAnimationAssetFileText);
		}
		BlockIterator iterator = structuredBinary.Iterator;
		AssetMetadataParser assetMetadataParser = new AssetMetadataParser();
		if (!assetMetadataParser.LoadFromStrb(iterator))
		{
			throw new AvatarException(Resources.InvalidAnimationAssetFileText);
		}
		if (!iterator.FindFirst(StructuredBinaryBlockId.Animation))
		{
			throw new AvatarException(Resources.InvalidAnimationAssetFileText);
		}
		AssetAnimationParser assetAnimationParser = new AssetAnimationParser(m_CoordSystem, assetMetadataParser.m_bodyTypeMask);
		return assetAnimationParser.Parse(iterator);
	}
}
