using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class AssetMetadataParser
{
	public AvatarGender m_bodyTypeMask;

	public BinaryAssetType m_AssetType;

	public uint m_assetTypeDetails;

	public AssetSubcategory m_assetSubCategory;

	public Skeleton.SkeletonVersion m_skeletonVersion;

	public AvatarGender BodyTypeMask => m_bodyTypeMask;

	public BinaryAssetType AssetType => m_AssetType;

	public uint AssetTypeDetails => m_assetTypeDetails;

	public AssetSubcategory AssetSubcategory => m_assetSubCategory;

	public Skeleton.SkeletonVersion AssetSkeletonVersion => m_skeletonVersion;

	public void ParseLegacyV1(BlockIterator blockIterator)
	{
		bool littleEndian = blockIterator.LittleEndian;
		blockIterator.LittleEndian = true;
		m_bodyTypeMask = (AvatarGender)blockIterator.ReadByte();
		m_AssetType = (BinaryAssetType)blockIterator.ReadInt();
		m_assetTypeDetails = blockIterator.ReadUInt();
		m_assetSubCategory = (AssetSubcategory)blockIterator.ReadInt();
		blockIterator.LittleEndian = littleEndian;
		m_skeletonVersion = Skeleton.SkeletonVersion.Nxe;
	}

	public void Parse(BlockIterator blockIterator)
	{
		switch (blockIterator.ReadByte())
		{
		case 1:
			ParseLegacyV1(blockIterator);
			break;
		case 2:
		{
			bool littleEndian = blockIterator.LittleEndian;
			blockIterator.LittleEndian = true;
			m_bodyTypeMask = (AvatarGender)blockIterator.ReadByte();
			m_AssetType = (BinaryAssetType)blockIterator.ReadInt();
			m_assetTypeDetails = blockIterator.ReadUInt();
			m_assetSubCategory = (AssetSubcategory)blockIterator.ReadInt();
			m_skeletonVersion = (Skeleton.SkeletonVersion)blockIterator.ReadByte();
			blockIterator.LittleEndian = littleEndian;
			break;
		}
		}
	}

	public bool LoadFromStrb(BlockIterator strb)
	{
		if (strb.FindFirst(StructuredBinaryBlockId.AssetMetadata))
		{
			ParseLegacyV1(strb);
			return true;
		}
		if (strb.FindFirst(StructuredBinaryBlockId.AssetMetadataVersioned))
		{
			Parse(strb);
			return true;
		}
		return false;
	}
}
