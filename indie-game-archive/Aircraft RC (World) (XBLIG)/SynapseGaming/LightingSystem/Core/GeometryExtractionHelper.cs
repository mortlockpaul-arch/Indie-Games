using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Helper class that extracts geometry information from graphics resources (vertex and index buffers).
/// </summary>
public static class GeometryExtractionHelper
{
	private static Dictionary<int, GeometryData> HCB = new Dictionary<int, GeometryData>(256);

	private static Dictionary<int, GeometryData> HC_0002 = new Dictionary<int, GeometryData>(256);

	private static byte[] HC_0012 = new byte[1];

	private static ushort[] HCH = new ushort[1];

	private static int[] HC7 = new int[1];

	private static Dictionary<int, int> HC_0001 = new Dictionary<int, int>(256);

	private static SystemStatistic HCw = SystemConsole.GetStatistic("GeometryExtraction_GetRawBufferData", SystemStatisticCategory.SceneGraph);

	private static SystemStatistic HCZ = SystemConsole.GetStatistic("GeometryExtraction_GetMeshData", SystemStatisticCategory.SceneGraph);

	/// <summary>
	/// Clears all cached mesh data.
	/// </summary>
	public static void Clear()
	{
		HCB.Clear();
		HC_0002.Clear();
	}

	private static int _0002Z(RenderableMesh P_0)
	{
		return (P_0.ElementStart ^ 1) + (P_0.IndexBuffer.GetHashCode() ^ 2) + (P_0.PrimitiveCount ^ 3) + (P_0.VertexBase ^ 4) + (P_0.VertexBuffer.GetHashCode() ^ 5) + (P_0.VertexStreamOffset ^ 6);
	}

	/// <summary>
	/// Extracts geometry data from the provided graphics resources (vertex and index buffers).
	/// If the data is already extracted the cached version is returned.
	/// </summary>
	/// <param name="vertexbuffer"></param>
	/// <param name="indexbuffer"></param>
	/// <returns></returns>
	public static GeometryData GetRawBufferData(VertexBuffer vertexbuffer, IndexBuffer indexbuffer)
	{
		int key = (indexbuffer.GetHashCode() ^ 1) + (vertexbuffer.GetHashCode() ^ 2);
		if (HC_0002.TryGetValue(key, out var value))
		{
			return value;
		}
		HCw.AccumulationValue++;
		value = new GeometryData();
		HC_0002.Add(key, value);
		int vertexCount = vertexbuffer.VertexCount;
		int num = vertexCount * vertexbuffer.VertexDeclaration.VertexStride;
		int indexCount = indexbuffer.IndexCount;
		if (indexbuffer.IndexElementSize == IndexElementSize.SixteenBits)
		{
			if (HCH.Length < indexCount)
			{
				HCH = new ushort[indexCount];
			}
			indexbuffer.GetData(HCH, 0, indexCount);
			for (int i = 0; i < indexCount; i++)
			{
				value.Indices.Add(HCH[i]);
			}
		}
		else
		{
			if (HC7.Length < indexCount)
			{
				HC7 = new int[indexCount];
			}
			indexbuffer.GetData(HC7, 0, indexCount);
			for (int j = 0; j < indexCount; j++)
			{
				value.Indices.Add(HC7[j]);
			}
		}
		if (HC_0012.Length < num)
		{
			HC_0012 = new byte[num];
		}
		vertexbuffer.GetData(HC_0012, 0, num);
		int vertexStride = vertexbuffer.VertexDeclaration.VertexStride;
		for (int k = 0; k < vertexCount; k++)
		{
			int num2 = k * vertexStride;
			int num3 = 4;
			Vector3 item = new Vector3
			{
				X = BitConverter.ToSingle(HC_0012, num2),
				Y = BitConverter.ToSingle(HC_0012, num2 + num3),
				Z = BitConverter.ToSingle(HC_0012, num2 + num3 * 2)
			};
			value.Vertices.Add(item);
		}
		return value;
	}

	/// <summary>
	/// Extracts geometry data from the provided renderable
	/// mesh. If the data is already extracted the cached version is returned.
	/// </summary>
	/// <param name="mesh">Renderable mesh to generated the collision mesh data from.</param>
	/// <returns></returns>
	public static GeometryData GetMeshData(RenderableMesh mesh)
	{
		int key = _0002Z(mesh);
		if (HCB.TryGetValue(key, out var value))
		{
			return value;
		}
		HCZ.AccumulationValue++;
		value = new GeometryData();
		HCB.Add(key, value);
		if (mesh.PrimitiveType != PrimitiveType.TriangleList)
		{
			throw new Exception("Collideable meshes must use TriangleList primitive type.");
		}
		if (mesh.VertexStreamOffset != 0)
		{
			throw new Exception("Collideable meshes cannot use stream offset.");
		}
		GeometryData rawBufferData = GetRawBufferData(mesh.HC_0003, mesh.HCK);
		HC_0001.Clear();
		int num = mesh.PrimitiveCount * 3;
		int elementStart = mesh.ElementStart;
		int vertexBase = mesh.VertexBase;
		for (int i = 0; i < num; i++)
		{
			int num2 = rawBufferData.Indices[i + elementStart] + vertexBase;
			if (HC_0001.ContainsKey(num2))
			{
				value.Indices.Add(HC_0001[num2]);
				continue;
			}
			Vector3 item = rawBufferData.Vertices[num2];
			int count = value.Vertices.Count;
			HC_0001.Add(num2, count);
			value.Indices.Add(count);
			value.Vertices.Add(item);
		}
		int num3 = mesh.PrimitiveCount;
		for (int j = 0; j < num3; j++)
		{
			int num4 = j * 3;
			Vector3 a = value.Vertices[value.Indices[num4]];
			Vector3 b = value.Vertices[value.Indices[num4 + 1]];
			Vector3 c = value.Vertices[value.Indices[num4 + 2]];
			if (CoreHelper.IsDegenerate(ref a, ref b, ref c))
			{
				value.Indices.RemoveRange(num4, 3);
				j--;
				num3--;
			}
		}
		return value;
	}
}
