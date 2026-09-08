using System;
using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class AssetShapeOverrideParser
{
	public AssetTriangleOverrideParser m_TriangleOverride;

	public AssetVertexOverrideParser m_VertexOverride;

	public AssetShapeOverrideParser(CoordinateSystem coordinateSystem)
	{
		m_VertexOverride = new AssetVertexOverrideParser(coordinateSystem);
		m_TriangleOverride = new AssetTriangleOverrideParser();
	}

	public void Parse(Stream stream)
	{
		m_TriangleOverride.Parse(stream);
		m_VertexOverride.Parse(stream);
	}

	public Guid GetTargetAssetId()
	{
		return m_TriangleOverride.GetOriginalAssetId();
	}

	public bool Apply(AvatarComponent model)
	{
		Guid assetId = model.AssetId;
		if (assetId != m_TriangleOverride.GetOriginalAssetId())
		{
			return false;
		}
		if (assetId != m_VertexOverride.GetOriginalAssetId())
		{
			return false;
		}
		int num = model.m_Batches.Length;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < num; i++)
		{
			num2 += model.m_Batches[i].Triangles.Length * 6;
			num3 += model.m_Batches[i].Vertices.Positions.Length * (28 + model.m_Batches[i].Vertices.TextureChannelCount * 4);
		}
		num3 = (num3 + 127) & -128;
		if (num2 != m_TriangleOverride.m_GlobalIndexBufferSize)
		{
			return false;
		}
		if (num3 != m_VertexOverride.m_GlobalVertexBufferSize)
		{
			return false;
		}
		m_TriangleOverride.Apply(model);
		m_VertexOverride.Apply(model);
		return true;
	}

	public int GetMemoryUsage()
	{
		return m_TriangleOverride.GetMemoryUsage() + m_VertexOverride.GetMemoryUsage() + 16;
	}
}
