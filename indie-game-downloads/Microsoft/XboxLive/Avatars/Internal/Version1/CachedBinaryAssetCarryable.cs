using System;
using Microsoft.XboxLive.Avatars.Internal.Animations;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class CachedBinaryAssetCarryable : CachedBinaryAssetModel
{
	public Skeleton m_CarryableSkeleton;

	public AvatarAnimation m_CarryableAnimation;

	public CachedBinaryAssetCarryable(Guid assetId)
		: base(assetId)
	{
		m_AssetType = BinaryAssetParserType.Carryable;
	}

	public override bool ParseAsset(StructuredBinary structuredBinary, BinaryAssetParseContext context)
	{
		BlockIterator iterator = structuredBinary.Iterator;
		if (iterator == null)
		{
			return false;
		}
		if (!iterator.FindFirst(StructuredBinaryBlockId.Model))
		{
			return false;
		}
		AvatarComponent avatarComponent = new AvatarComponent();
		Models.Add(avatarComponent);
		if (!CachedBinaryAssetModel.ParseModel(iterator, avatarComponent, context.m_ResourceFactory, context.m_CoordinateSystem))
		{
			return false;
		}
		if (!ParseColorTable(iterator))
		{
			return false;
		}
		if (!iterator.FindFirst(StructuredBinaryBlockId.Skeleton))
		{
			return false;
		}
		if ((int)iterator.Length <= 0)
		{
			Logger.Log(new DebugLog(this, "block size <= 0; strb file is corrupted"));
			return false;
		}
		DecompressStream stream = new DecompressStream(iterator, (int)iterator.Length);
		AssetSkeletonParser assetSkeletonParser = new AssetSkeletonParser(context.m_CoordinateSystem);
		m_CarryableSkeleton = assetSkeletonParser.Parse(stream);
		if (iterator.FindFirst(StructuredBinaryBlockId.Animation))
		{
			AssetAnimationParser assetAnimationParser = new AssetAnimationParser(context.m_CoordinateSystem, AvatarGender.Both);
			m_CarryableAnimation = assetAnimationParser.Parse(iterator);
		}
		return base.ParseAsset(structuredBinary, context);
	}
}
