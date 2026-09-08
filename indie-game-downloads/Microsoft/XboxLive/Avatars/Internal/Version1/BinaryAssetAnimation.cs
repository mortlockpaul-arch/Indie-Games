using System;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class BinaryAssetAnimation : BinaryAsset
{
	public BinaryAssetAnimation(Guid animationAssetId)
		: base(AvatarComponentMasks.None)
	{
		m_AssetId = animationAssetId;
		m_AssetType = BinaryAssetParserType.Animation;
	}

	public override bool Validate(BinaryAssetParseContext context)
	{
		AssetMetadataParser metadata = GetMetadata();
		if (metadata == null)
		{
			return false;
		}
		if (context.m_skeletonVersion != metadata.AssetSkeletonVersion)
		{
			Logger.Log(new DebugLog(this, $"Incompatible skeleton version in animation asset. Required {context.m_skeletonVersion}, received {metadata.AssetSkeletonVersion} for component {m_AssetId}"));
			return false;
		}
		return true;
	}

	public override bool ValidateFromCache(BinaryAssetParseContext context)
	{
		if (m_Cache == null)
		{
			return false;
		}
		if (m_Cache.m_Metadata == null)
		{
			return false;
		}
		if (context.m_skeletonVersion != m_Cache.m_Metadata.m_skeletonVersion)
		{
			Logger.Log(new DebugLog(this, $"Incompatible skeleton version in animation asset. Required {context.m_skeletonVersion}, received {m_Cache.m_Metadata.m_skeletonVersion} for component {m_AssetId}"));
			return false;
		}
		return true;
	}

	public override CachedBinaryAsset CreateCacheItem()
	{
		return new CachedBinaryAssetAnimation(m_AssetId);
	}
}
