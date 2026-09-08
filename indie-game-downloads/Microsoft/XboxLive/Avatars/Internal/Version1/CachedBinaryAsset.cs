using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public abstract class CachedBinaryAsset : CachedAsset
{
	public enum AssetState
	{
		Default,
		Invalid,
		Downloaded,
		Parsed
	}

	public AssetMetadataParser m_Metadata = new AssetMetadataParser();

	public BinaryAssetParserType m_AssetType;

	public AssetState m_AssetState;

	public override AssetCacheKey AssetKey => new AssetCacheKey(AssetId, (int)m_AssetType, m_CoordinateSystem, 1, m_Metadata.AssetSkeletonVersion);

	public CachedBinaryAsset()
	{
		m_AssetType = BinaryAssetParserType.Base;
	}

	public bool Parse(Stream stream, BinaryAssetParseContext context)
	{
		m_CoordinateSystem = context.m_CoordinateSystem;
		StructuredBinary structuredBinary = new StructuredBinary();
		stream.Seek(0L, SeekOrigin.Begin);
		if (!structuredBinary.Open(stream))
		{
			return false;
		}
		if (!(structuredBinary.Namespace == BinaryAsset.AvatarAssetGuid))
		{
			return false;
		}
		BlockIterator iterator = structuredBinary.Iterator;
		if (iterator == null)
		{
			return false;
		}
		if (!m_Metadata.LoadFromStrb(iterator))
		{
			Logger.Log(new DebugLog(this, $"Asset {AssetId} does not contain metadata."));
			return false;
		}
		if (!ParseAsset(structuredBinary, context))
		{
			return false;
		}
		m_MemoryUsage = GetMemoryUsageInternal();
		return true;
	}

	public Skeleton.SkeletonVersion GetAssetSkeletonVersion()
	{
		return m_Metadata.AssetSkeletonVersion;
	}

	public abstract bool ParseAsset(StructuredBinary structuredBinary, BinaryAssetParseContext context);
}
