using System;
using System.Collections.Generic;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class CachedBinaryAssetShapeOverride : CachedBinaryAsset
{
	public List<AssetShapeOverrideParser> m_ShapeOverrides = new List<AssetShapeOverrideParser>();

	public CachedBinaryAssetShapeOverride(Guid assetId)
	{
		AssetId = assetId;
		m_AssetType = BinaryAssetParserType.ShapeOverride;
	}

	public override bool ParseAsset(StructuredBinary structuredBinary, BinaryAssetParseContext context)
	{
		BlockIterator iterator = structuredBinary.Iterator;
		if (iterator == null)
		{
			return false;
		}
		if (!iterator.FirstBlock())
		{
			return false;
		}
		while (iterator.Find(StructuredBinaryBlockId.ShapeOverrides))
		{
			AssetShapeOverrideParser assetShapeOverrideParser = new AssetShapeOverrideParser(context.m_CoordinateSystem);
			m_ShapeOverrides.Add(assetShapeOverrideParser);
			assetShapeOverrideParser.Parse(iterator);
			iterator.NextBlock();
		}
		return true;
	}

	public override int GetMemoryUsageInternal()
	{
		int num = base.GetMemoryUsageInternal();
		int count = m_ShapeOverrides.Count;
		for (int i = 0; i < count; i++)
		{
			num += m_ShapeOverrides[i].GetMemoryUsage();
		}
		return num;
	}
}
