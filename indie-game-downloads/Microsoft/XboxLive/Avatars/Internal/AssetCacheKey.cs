using System;
using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.XboxLive.Avatars.Internal;

public struct AssetCacheKey(Guid assetId, int assetType, CoordinateSystem assetCoordinates, int version, Skeleton.SkeletonVersion skeletonVersion)
{
	public int version = version;

	public int assetType = assetType;

	public Guid assetId = assetId;

	public CoordinateSystem assetCoordinates = assetCoordinates;

	public Skeleton.SkeletonVersion skeletonVersion = Skeleton.SkeletonVersion.Invalid;
}
