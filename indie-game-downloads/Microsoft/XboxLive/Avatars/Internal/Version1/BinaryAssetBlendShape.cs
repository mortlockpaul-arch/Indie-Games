using System;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class BinaryAssetBlendShape : BinaryAssetShapeOverride
{
	public BlendShapeType m_Shape;

	public BinaryAssetBlendShape(BlendShapeType shapeId, Guid id)
		: base(id, AvatarComponentMasks.None)
	{
		m_Shape = shapeId;
		m_AssetType = BinaryAssetParserType.BlendShape;
	}

	public override bool Validate(BinaryAssetParseContext context)
	{
		AssetMetadataParser metadata = GetMetadata();
		if (metadata == null)
		{
			return false;
		}
		if ((metadata.BodyTypeMask & context.m_BodyType) == 0)
		{
			return false;
		}
		if (metadata.AssetType != BinaryAssetType.ShapeOverride)
		{
			return false;
		}
		uint assetTypeDetails = metadata.AssetTypeDetails;
		BlendShapeType blendShapeType = (BlendShapeType)assetTypeDetails;
		if (blendShapeType != m_Shape)
		{
			return false;
		}
		if (context.m_skeletonVersion != metadata.AssetSkeletonVersion)
		{
			Logger.Log(new DebugLog(this, $"Incompatible skeleton version. Required {context.m_skeletonVersion}, received {metadata.AssetSkeletonVersion} for component {m_AssetId}"));
			return false;
		}
		return true;
	}

	public override bool ProcessOverridesFromStream(BinaryAssetParseContext context)
	{
		return true;
	}

	public override bool ProcessOverridesFromCache(BinaryAssetParseContext context)
	{
		return true;
	}

	public override bool ProcessAssetsFromStream(BinaryAssetParseContext context)
	{
		return base.ProcessOverridesFromStream(context);
	}

	public override bool ProcessAssetsFromCache(BinaryAssetParseContext context)
	{
		return base.ProcessOverridesFromCache(context);
	}

	public override CachedBinaryAsset CreateCacheItem()
	{
		return new CachedBinaryAssetBlendShape(m_AssetId);
	}
}
