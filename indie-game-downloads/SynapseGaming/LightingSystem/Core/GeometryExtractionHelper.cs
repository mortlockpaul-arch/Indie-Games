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
	private static Dictionary<RenderableMesh.ComparisonIndex, GeometryData> _3A_0018 = new Dictionary<RenderableMesh.ComparisonIndex, GeometryData>(256);

	private static Dictionary<RenderableMesh.ComparisonIndex.BufferComparisonIndex, GeometryData> _3AL = new Dictionary<RenderableMesh.ComparisonIndex.BufferComparisonIndex, GeometryData>(256);

	private static byte[] _3A_0019 = new byte[1];

	private static ushort[] _3A3 = new ushort[1];

	private static int[] _3A6 = new int[1];

	private static Dictionary<int, int> _3AD = new Dictionary<int, int>(256);

	private static SystemStatistic _3A_0017 = SystemConsole.GetStatistic("GeometryExtraction_GetRawBufferData", SystemStatisticCategory.SceneGraph);

	private static SystemStatistic _3A_0003 = SystemConsole.GetStatistic("GeometryExtraction_GetMeshData", SystemStatisticCategory.SceneGraph);

	/// <summary>
	/// Clears all cached mesh data.
	/// </summary>
	public static void Clear()
	{
		_3A_0018.Clear();
		_3AL.Clear();
	}

	/// <summary>
	/// Extracts geometry data from the provided graphics resources (vertex and index buffers).
	/// If the data is already extracted the cached version is returned.
	/// </summary>
	/// <param name="buffercomparisonindex"></param>
	/// <returns></returns>
	public static GeometryData GetRawBufferData(RenderableMesh.ComparisonIndex.BufferComparisonIndex buffercomparisonindex)
	{
		if (_3AL.TryGetValue(buffercomparisonindex, out var value))
		{
			return value;
		}
		_3A_0017.AccumulationValue++;
		value = new GeometryData();
		_3AL.Add(buffercomparisonindex, value);
		VertexBuffer vertexBuffer = buffercomparisonindex._3AL;
		IndexBuffer indexBuffer = buffercomparisonindex._3A_0018;
		int vertexCount = vertexBuffer.VertexCount;
		int num = vertexCount * vertexBuffer.VertexDeclaration.VertexStride;
		int indexCount = indexBuffer.IndexCount;
		if (indexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
		{
			if (_3A3.Length < indexCount)
			{
				_3A3 = new ushort[indexCount];
			}
			indexBuffer.GetData(_3A3, 0, indexCount);
			for (int i = 0; i < indexCount; i++)
			{
				value.Indices.Add(_3A3[i]);
			}
		}
		else
		{
			if (_3A6.Length < indexCount)
			{
				_3A6 = new int[indexCount];
			}
			indexBuffer.GetData(_3A6, 0, indexCount);
			for (int j = 0; j < indexCount; j++)
			{
				value.Indices.Add(_3A6[j]);
			}
		}
		if (_3A_0019.Length < num)
		{
			_3A_0019 = new byte[num];
		}
		vertexBuffer.GetData(_3A_0019, 0, num);
		int vertexStride = vertexBuffer.VertexDeclaration.VertexStride;
		for (int k = 0; k < vertexCount; k++)
		{
			int num2 = k * vertexStride;
			int num3 = 4;
			Vector3 item = new Vector3
			{
				X = BitConverter.ToSingle(_3A_0019, num2),
				Y = BitConverter.ToSingle(_3A_0019, num2 + num3),
				Z = BitConverter.ToSingle(_3A_0019, num2 + num3 * 2)
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
		if (_3A_0018.TryGetValue(mesh.Index, out var value))
		{
			return value;
		}
		_3A_0003.AccumulationValue++;
		value = new GeometryData();
		_3A_0018.Add(mesh.Index, value);
		if (mesh.PrimitiveType != PrimitiveType.TriangleList)
		{
			throw new Exception("Collideable meshes must use TriangleList primitive type.");
		}
		if (mesh.VertexStreamOffset != 0)
		{
			throw new Exception("Collideable meshes cannot use stream offset.");
		}
		GeometryData rawBufferData = GetRawBufferData(mesh.Index.BufferIndex);
		_3AD.Clear();
		int num = mesh.PrimitiveCount * 3;
		int elementStart = mesh.ElementStart;
		int vertexBase = mesh.VertexBase;
		for (int i = 0; i < num; i++)
		{
			int num2 = rawBufferData.Indices[i + elementStart] + vertexBase;
			if (_3AD.ContainsKey(num2))
			{
				value.Indices.Add(_3AD[num2]);
				continue;
			}
			Vector3 item = rawBufferData.Vertices[num2];
			int count = value.Vertices.Count;
			_3AD.Add(num2, count);
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
