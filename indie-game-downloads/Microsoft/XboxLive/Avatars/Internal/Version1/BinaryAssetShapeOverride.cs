using System;
using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class BinaryAssetShapeOverride : BinaryAsset
{
	public BinaryAssetShapeOverride(Guid id, AvatarComponentMasks mask)
		: base(mask)
	{
		m_AssetId = id;
		m_AssetType = BinaryAssetParserType.ShapeOverride;
	}

	public override bool ProcessOverridesFromStream(BinaryAssetParseContext context)
	{
		StructuredBinary structuredBinary = new StructuredBinary();
		m_Stream.Seek(0L, SeekOrigin.Begin);
		if (!structuredBinary.Open(m_Stream))
		{
			return false;
		}
		if (!(structuredBinary.Namespace == BinaryAsset.AvatarAssetGuid))
		{
			return false;
		}
		BlockIterator iterator = structuredBinary.Iterator;
		while (iterator.Find(StructuredBinaryBlockId.ShapeOverrides))
		{
			if (!ProcessShapeOverride(context, iterator))
			{
				return false;
			}
			iterator.NextBlock();
		}
		return true;
	}

	public override bool ProcessOverridesFromCache(BinaryAssetParseContext context)
	{
		if (!(m_Cache is CachedBinaryAssetShapeOverride cachedBinaryAssetShapeOverride))
		{
			return false;
		}
		m_SkeletonVersion = m_Cache.m_Metadata.m_skeletonVersion;
		int count = cachedBinaryAssetShapeOverride.m_ShapeOverrides.Count;
		for (int i = 0; i < count; i++)
		{
			AssetShapeOverrideParser assetShapeOverrideParser = cachedBinaryAssetShapeOverride.m_ShapeOverrides[i];
			Guid targetAssetId = assetShapeOverrideParser.GetTargetAssetId();
			Avatar target = context.m_Target;
			int num = target.m_Models.Count;
			while (--num >= 0)
			{
				if (target.m_Models[num].AssetId == targetAssetId)
				{
					if (!assetShapeOverrideParser.Apply(target.m_Models[num]))
					{
						return false;
					}
					target.m_Models[num].m_AvatarComponentManifest.AddAsset(m_AssetId);
				}
			}
		}
		return true;
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
			Logger.Log(new DebugLog(this, $"Incompatible skeleton version. Required {context.m_skeletonVersion}, received {metadata.AssetSkeletonVersion} for component {m_AssetId}"));
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

	public bool ProcessShapeOverride(BinaryAssetParseContext context, Stream stream)
	{
		AssetShapeOverrideParser assetShapeOverrideParser = new AssetShapeOverrideParser(context.m_CoordinateSystem);
		assetShapeOverrideParser.Parse(stream);
		Guid targetAssetId = assetShapeOverrideParser.GetTargetAssetId();
		Avatar target = context.m_Target;
		int num = target.Models.Count;
		while (--num >= 0)
		{
			if (target.Models[num].AssetId == targetAssetId)
			{
				if (!assetShapeOverrideParser.Apply(target.Models[num]))
				{
					return false;
				}
				target.m_Models[num].m_AvatarComponentManifest.AddAsset(m_AssetId);
			}
		}
		return true;
	}

	public override CachedBinaryAsset CreateCacheItem()
	{
		return new CachedBinaryAssetShapeOverride(m_AssetId);
	}
}
