using System;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class CachedBinaryAssetColorTable : CachedBinaryAsset
{
	public ComponentColorTable m_ColorTable;

	public ComponentColorTable ColorTable => m_ColorTable;

	public CachedBinaryAssetColorTable(Guid assetId)
	{
		AssetId = assetId;
		m_AssetType = BinaryAssetParserType.ColorTable;
	}

	public override bool ParseAsset(StructuredBinary structuredBinary, BinaryAssetParseContext context)
	{
		m_CoordinateSystem = CoordinateSystem.LeftHanded;
		BlockIterator iterator = structuredBinary.Iterator;
		if (iterator == null)
		{
			return false;
		}
		if (!iterator.FindFirst(StructuredBinaryBlockId.Model))
		{
			return false;
		}
		if (iterator.FindFirst(StructuredBinaryBlockId.CustomColorTable))
		{
			m_ColorTable = AssetCustomColorTableParser.Parse(iterator);
		}
		return true;
	}

	public override int GetMemoryUsageInternal()
	{
		return ((m_ColorTable != null) ? m_ColorTable.GetMemoryUsage() : 0) + base.GetMemoryUsageInternal();
	}
}
