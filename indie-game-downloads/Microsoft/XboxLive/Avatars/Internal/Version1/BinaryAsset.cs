using System;
using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public abstract class BinaryAsset
{
	public const long AvatarAssetVersion = 265023668400864L;

	public static readonly Guid AvatarAssetGuid = new Guid(1505616011, 18697, 20196, 185, 145, 173, 168, 123, 124, 11, 107);

	public Stream m_Stream;

	public Guid m_AssetId;

	public BinaryAssetParserType m_AssetType;

	public AvatarComponentMasks m_ComponentMask;

	public Skeleton.SkeletonVersion m_SkeletonVersion;

	public CachedBinaryAsset m_Cache;

	public Stream Stream
	{
		get
		{
			return m_Stream;
		}
		set
		{
			m_Stream = value;
		}
	}

	public bool IsPreloaded => m_Stream != null;

	public Guid AssetId => m_AssetId;

	public virtual bool IsCoordinateSystemIndependent => false;

	public BinaryAsset(AvatarComponentMasks mask)
	{
		m_ComponentMask = mask;
		m_AssetType = BinaryAssetParserType.Base;
		m_SkeletonVersion = Skeleton.SkeletonVersion.Invalid;
	}

	public virtual bool ProcessComponentsFromStream(BinaryAssetParseContext context)
	{
		return true;
	}

	public virtual bool ProcessComponentsFromCache(BinaryAssetParseContext context)
	{
		return true;
	}

	public virtual bool ProcessAssetsFromStream(BinaryAssetParseContext context)
	{
		return true;
	}

	public virtual bool ProcessAssetsFromCache(BinaryAssetParseContext context)
	{
		return true;
	}

	public virtual bool ProcessOverridesFromStream(BinaryAssetParseContext context)
	{
		return true;
	}

	public virtual bool ProcessOverridesFromCache(BinaryAssetParseContext context)
	{
		return true;
	}

	public abstract bool Validate(BinaryAssetParseContext context);

	public abstract bool ValidateFromCache(BinaryAssetParseContext context);

	public AssetMetadataParser GetMetadata()
	{
		if (m_Stream == null)
		{
			return null;
		}
		StructuredBinary structuredBinary = new StructuredBinary();
		m_Stream.Seek(0L, SeekOrigin.Begin);
		if (!structuredBinary.Open(m_Stream))
		{
			return null;
		}
		if (!(structuredBinary.Namespace == AvatarAssetGuid))
		{
			return null;
		}
		BlockIterator iterator = structuredBinary.Iterator;
		AssetMetadataParser assetMetadataParser = new AssetMetadataParser();
		if (!assetMetadataParser.LoadFromStrb(iterator))
		{
			Logger.Log(new DebugLog(this, $"Asset {AssetId} does not contain metadata."));
			return null;
		}
		m_SkeletonVersion = assetMetadataParser.AssetSkeletonVersion;
		return assetMetadataParser;
	}

	public abstract CachedBinaryAsset CreateCacheItem();

	public AssetCacheKey GetAssetKey(Guid assetId, CoordinateSystem coordinateSystem, Skeleton.SkeletonVersion skeletonVersion)
	{
		if (IsCoordinateSystemIndependent)
		{
			return new AssetCacheKey(assetId, (int)m_AssetType, CoordinateSystem.LeftHanded, 1, skeletonVersion);
		}
		return new AssetCacheKey(assetId, (int)m_AssetType, coordinateSystem, 1, skeletonVersion);
	}
}
