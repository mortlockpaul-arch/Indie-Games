using System;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class CachedBinaryAssetBlendShape : CachedBinaryAssetShapeOverride
{
	public CachedBinaryAssetBlendShape(Guid assetId)
		: base(assetId)
	{
		m_AssetType = BinaryAssetParserType.BlendShape;
	}
}
