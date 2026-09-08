using System;
using Microsoft.XboxLive.Avatars.Internal.Parsers;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class CachedBinaryAssetTexture : CachedBinaryAsset
{
	public AssetTextureParser m_TextureParser = new AssetTextureParser();

	public CachedBinaryAssetTexture(Guid assetId)
	{
		AssetId = assetId;
		m_AssetType = BinaryAssetParserType.Texture;
	}

	public IBaseTextureAnimated GetTexture()
	{
		return m_TextureParser.Texture;
	}

	public override bool ParseAsset(StructuredBinary structuredBinary, BinaryAssetParseContext context)
	{
		m_CoordinateSystem = CoordinateSystem.LeftHanded;
		BlockIterator iterator = structuredBinary.Iterator;
		if (iterator == null)
		{
			return false;
		}
		if (!iterator.FindFirst(StructuredBinaryBlockId.Texture))
		{
			return false;
		}
		if ((int)iterator.Length <= 0)
		{
			Logger.Log(new DebugLog(this, "block size <= 0; strb file is corrupted"));
			return false;
		}
		DecompressStream stream = new DecompressStream(iterator, (int)iterator.Length);
		m_TextureParser.Parse(stream, context.m_ResourceFactory);
		return true;
	}

	public override int GetMemoryUsageInternal()
	{
		return (m_TextureParser.Texture?.GetMemoryUsage() ?? 0) + base.GetMemoryUsageInternal();
	}
}
