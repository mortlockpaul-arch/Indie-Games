using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class BinaryAssetCarryable : BinaryAssetModel
{
	public BinaryAssetCarryable(ComponentInfo description, int shaderOverridesCount, AvatarComponentMasks mask)
		: base(description, shaderOverridesCount, mask)
	{
		m_AssetType = BinaryAssetParserType.Carryable;
	}

	public override bool ProcessComponentsFromStream(BinaryAssetParseContext context)
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
		if (!iterator.FindFirst(StructuredBinaryBlockId.Model))
		{
			return false;
		}
		Avatar target = context.m_Target;
		target.m_Carryable = new AvatarCarryable();
		target.m_Carryable.m_ComponentModel.m_AvatarComponentManifest = new ComponentManifest(m_ComponentDescription, m_ShaderConstantOverrides, 0);
		ProcessModel(iterator, ref target.m_Carryable.m_ComponentModel, context);
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
		target.m_Carryable.m_Skeleton = assetSkeletonParser.Parse(stream);
		if (iterator.FindFirst(StructuredBinaryBlockId.Animation))
		{
			AssetAnimationParser assetAnimationParser = new AssetAnimationParser(context.m_CoordinateSystem, AvatarGender.Both);
			target.m_Carryable.m_Animation = assetAnimationParser.Parse(iterator);
		}
		m_ContainShapeOverrides = iterator.FindFirst(StructuredBinaryBlockId.ShapeOverrides);
		return true;
	}

	public override bool ProcessComponentsFromCache(BinaryAssetParseContext context)
	{
		if (!(m_Cache is CachedBinaryAssetCarryable cachedBinaryAssetCarryable))
		{
			return false;
		}
		m_SkeletonVersion = m_Cache.m_Metadata.m_skeletonVersion;
		Avatar target = context.m_Target;
		AvatarCarryable avatarCarryable = new AvatarCarryable();
		avatarCarryable.m_Animation = cachedBinaryAssetCarryable.m_CarryableAnimation;
		avatarCarryable.m_Skeleton = new Skeleton();
		avatarCarryable.m_Skeleton.Joints = new Joint[cachedBinaryAssetCarryable.m_CarryableSkeleton.Joints.Length];
		cachedBinaryAssetCarryable.m_CarryableSkeleton.Joints.CopyTo(avatarCarryable.m_Skeleton.Joints, 0);
		avatarCarryable.m_ComponentModel.m_AvatarComponentManifest = new ComponentManifest(m_ComponentDescription, m_ShaderConstantOverrides, 0);
		target.m_Carryable = avatarCarryable;
		if (!ProcessModel(cachedBinaryAssetCarryable.Models[0], cachedBinaryAssetCarryable.ColorTable, ref avatarCarryable.m_ComponentModel))
		{
			return false;
		}
		m_ContainShapeOverrides = cachedBinaryAssetCarryable.m_ShapeOverrides.Count != 0;
		return true;
	}

	public override CachedBinaryAsset CreateCacheItem()
	{
		return new CachedBinaryAssetCarryable(m_AssetId);
	}
}
