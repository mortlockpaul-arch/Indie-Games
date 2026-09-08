using System;
using Microsoft.XboxLive.Avatars.Internal.Animations;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class CachedBinaryAssetAnimation : CachedBinaryAsset
{
	public AvatarAnimation m_Animation;

	public AvatarAnimation Animation => m_Animation;

	public CachedBinaryAssetAnimation(Guid assetId)
	{
		AssetId = assetId;
		m_AssetType = BinaryAssetParserType.Animation;
	}

	public override bool ParseAsset(StructuredBinary structuredBinary, BinaryAssetParseContext context)
	{
		BlockIterator iterator = structuredBinary.Iterator;
		if (!iterator.FindFirst(StructuredBinaryBlockId.Animation))
		{
			return false;
		}
		AssetAnimationParser assetAnimationParser = new AssetAnimationParser(context.m_CoordinateSystem, m_Metadata.BodyTypeMask);
		m_Animation = assetAnimationParser.Parse(iterator);
		return m_Animation != null;
	}

	public override int GetMemoryUsageInternal()
	{
		if (m_Animation == null)
		{
			return base.GetMemoryUsageInternal();
		}
		return m_Animation.GetMemoryUsage() + base.GetMemoryUsageInternal();
	}
}
