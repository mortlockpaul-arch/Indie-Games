using System;
using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class BinaryAssetColorTable : BinaryAsset
{
	public override bool IsCoordinateSystemIndependent => true;

	public BinaryAssetColorTable(Guid id, AvatarComponentMasks mask)
		: base(mask)
	{
		m_AssetId = id;
		m_AssetType = BinaryAssetParserType.ColorTable;
	}

	public ComponentColorTable GetPrimaryColorTable()
	{
		StructuredBinary structuredBinary = new StructuredBinary();
		m_Stream.Seek(0L, SeekOrigin.Begin);
		if (!structuredBinary.Open(m_Stream))
		{
			return null;
		}
		if (!(structuredBinary.Namespace == BinaryAsset.AvatarAssetGuid))
		{
			return null;
		}
		BlockIterator iterator = structuredBinary.Iterator;
		if (!iterator.FindFirst(StructuredBinaryBlockId.Model))
		{
			return null;
		}
		if (!iterator.FindFirst(StructuredBinaryBlockId.CustomColorTable))
		{
			return null;
		}
		return AssetCustomColorTableParser.Parse(iterator);
	}

	public override bool Validate(BinaryAssetParseContext context)
	{
		return true;
	}

	public override bool ValidateFromCache(BinaryAssetParseContext context)
	{
		return true;
	}

	public override CachedBinaryAsset CreateCacheItem()
	{
		return new CachedBinaryAssetColorTable(m_AssetId);
	}
}
