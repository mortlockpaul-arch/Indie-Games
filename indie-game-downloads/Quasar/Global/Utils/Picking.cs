using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Global.Utils;

public static class Picking
{
	public static void ExtractTriangles(List<Vector3> triangles, Model model)
	{
		Matrix[] array = new Matrix[model.Bones.Count];
		model.CopyAbsoluteBoneTransformsTo(array);
		foreach (ModelMesh mesh in model.Meshes)
		{
			Matrix matrix = array[mesh.ParentBone.Index];
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				VertexDeclaration vertexDeclaration = meshPart.VertexBuffer.VertexDeclaration;
				VertexElement[] vertexElements = vertexDeclaration.GetVertexElements();
				VertexElement vertexElement = default(VertexElement);
				VertexElement[] array2 = vertexElements;
				for (int i = 0; i < array2.Length; i++)
				{
					VertexElement vertexElement2 = array2[i];
					if (vertexElement2.VertexElementUsage == VertexElementUsage.Position && vertexElement2.VertexElementFormat == VertexElementFormat.Vector3)
					{
						vertexElement = vertexElement2;
						break;
					}
				}
				if (vertexElement.VertexElementUsage != VertexElementUsage.Position || vertexElement.VertexElementFormat != VertexElementFormat.Vector3)
				{
					throw new Exception("Model uses unsupported vertex format!");
				}
				Vector3[] array3 = new Vector3[meshPart.NumVertices];
				meshPart.VertexBuffer.GetData(meshPart.VertexOffset * vertexDeclaration.VertexStride + vertexElement.Offset, array3, 0, meshPart.NumVertices, vertexDeclaration.VertexStride);
				for (int j = 0; j != array3.Length; j++)
				{
					Vector3.Transform(ref array3[j], ref matrix, out array3[j]);
				}
				int[] array4 = new int[meshPart.PrimitiveCount * 3];
				if (meshPart.IndexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
				{
					short[] array5 = new short[meshPart.PrimitiveCount * 3];
					meshPart.IndexBuffer.GetData(meshPart.StartIndex * 2, array5, 0, meshPart.PrimitiveCount * 3);
					for (int k = 0; k < array5.Length; k++)
					{
						array4[k] = array5[k];
					}
				}
				else
				{
					meshPart.IndexBuffer.GetData(meshPart.StartIndex * 2, array4, 0, meshPart.PrimitiveCount * 3);
				}
				for (int l = 0; l < 3 * meshPart.PrimitiveCount; l++)
				{
					triangles.Add(array3[array4[l]]);
				}
			}
		}
	}

	public static float? RayIntersectsModel(Ray ray, List<Vector3> triangles, ref Matrix modelTransform, out bool insideBoundingSphere, out Vector3 vertex1, out Vector3 vertex2, out Vector3 vertex3)
	{
		vertex1 = (vertex2 = (vertex3 = Vector3.Zero));
		Matrix matrix = Matrix.Invert(modelTransform);
		ray.Position = Vector3.Transform(ray.Position, matrix);
		ray.Direction = Vector3.TransformNormal(ray.Direction, matrix);
		insideBoundingSphere = true;
		float? num = null;
		for (int i = 0; i < triangles.Count; i += 3)
		{
			RayIntersectsTriangle(ray, triangles[i], triangles[i + 1], triangles[i + 2], out var result);
			if (result.HasValue && (!num.HasValue || result < num))
			{
				num = result;
				vertex1 = Vector3.Transform(triangles[i], modelTransform);
				vertex2 = Vector3.Transform(triangles[i + 1], modelTransform);
				vertex3 = Vector3.Transform(triangles[i + 2], modelTransform);
			}
		}
		return num;
	}

	public static float? RayIntersectsModel(Ray ray, List<Vector3> triangles)
	{
		float? num = null;
		for (int i = 0; i < triangles.Count; i += 3)
		{
			RayIntersectsTriangle(ray, triangles[i], triangles[i + 1], triangles[i + 2], out var result);
			if (result.HasValue && (!num.HasValue || result < num))
			{
				num = result;
			}
		}
		return num;
	}

	public static void RayIntersectsTriangle(Ray ray, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, out float? result)
	{
		Vector3.Subtract(ref vertex2, ref vertex1, out var result2);
		Vector3.Subtract(ref vertex3, ref vertex1, out var result3);
		Vector3.Cross(ref ray.Direction, ref result3, out var result4);
		Vector3.Dot(ref result2, ref result4, out var result5);
		if (result5 > -1E-45f && result5 < float.Epsilon)
		{
			result = null;
			return;
		}
		float num = 1f / result5;
		Vector3.Subtract(ref ray.Position, ref vertex1, out var result6);
		Vector3.Dot(ref result6, ref result4, out var result7);
		result7 *= num;
		if (result7 < 0f || result7 > 1f)
		{
			result = null;
			return;
		}
		Vector3.Cross(ref result6, ref result2, out var result8);
		Vector3.Dot(ref ray.Direction, ref result8, out var result9);
		result9 *= num;
		if (result9 < 0f || result7 + result9 > 1f)
		{
			result = null;
			return;
		}
		Vector3.Dot(ref result3, ref result8, out var result10);
		result10 *= num;
		if (result10 < 0f)
		{
			result = null;
		}
		else
		{
			result = result10;
		}
	}
}
