using System;
using System.Collections.Generic;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class CachedBinaryAssetModel : CachedBinaryAssetShapeOverride
{
	public ComponentColorTable ColorTable;

	public List<AvatarComponent> Models = new List<AvatarComponent>();

	public CachedBinaryAssetModel(Guid assetId)
		: base(assetId)
	{
		m_AssetType = BinaryAssetParserType.Model;
	}

	public override bool ParseAsset(StructuredBinary structuredBinary, BinaryAssetParseContext context)
	{
		BlockIterator iterator = structuredBinary.Iterator;
		if (!iterator.FindFirst(StructuredBinaryBlockId.Model))
		{
			return false;
		}
		do
		{
			AvatarComponent avatarComponent = new AvatarComponent();
			Models.Add(avatarComponent);
			if (!ParseModel(iterator, avatarComponent, context.m_ResourceFactory, context.m_CoordinateSystem))
			{
				return false;
			}
		}
		while (iterator.NextBlock() && iterator.Find(StructuredBinaryBlockId.Model));
		if (!ParseColorTable(iterator))
		{
			return false;
		}
		return base.ParseAsset(structuredBinary, context);
	}

	public static bool ParseModel(BlockIterator iterator, AvatarComponent model, IResourceFactory resourceFactory, CoordinateSystem coordinateSystem)
	{
		AssetModelParser assetModelParser = new AssetModelParser(model, coordinateSystem);
		int num = (int)iterator.Length;
		if (num <= 0)
		{
			Logger.Log(new DebugLog(new object(), "block size <= 0; strb file is corrupted"));
			return false;
		}
		DecompressStream stream = new DecompressStream(iterator, num);
		assetModelParser.Parse(stream, resourceFactory);
		return true;
	}

	public bool ParseColorTable(BlockIterator iterator)
	{
		if (iterator.FindFirst(StructuredBinaryBlockId.CustomColorTable))
		{
			ColorTable = AssetCustomColorTableParser.Parse(iterator);
		}
		return true;
	}

	public override int GetMemoryUsageInternal()
	{
		int num = base.GetMemoryUsageInternal();
		int count = Models.Count;
		for (int i = 0; i < count; i++)
		{
			num += Models[i].GetMemoryUsage();
		}
		return num;
	}
}
