using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class BinaryAssetParseContext
{
	public CoordinateSystem m_CoordinateSystem;

	public Avatar m_Target;

	public AvatarComponentMasks m_CombinedComponentMask;

	public AvatarGender m_BodyType;

	public IResourceFactory m_ResourceFactory;

	public AvatarAssetCacheManager m_AssetCache;

	public Skeleton.SkeletonVersion m_skeletonVersion;
}
