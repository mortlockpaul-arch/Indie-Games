using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace EGEngine;

public class TerrainRoads : PropModelBase
{
	public struct tmpVertStrcut
	{
		public Vector3 pos;

		public int tex;

		public int norm;

		public int tan;
	}

	private Vector3 tmpPos = Vector3.Zero;

	private Vector3 tmpNorm = Vector3.Zero;

	private Matrix matScale = Matrix.CreateScale(10f);

	private Matrix RoadLookAt = Matrix.CreateLookAt(Vector3.UnitY * 30000f, Vector3.UnitY * -1000f, Vector3.UnitZ);

	private Matrix RoadProj = Matrix.CreateOrthographicOffCenter(-512f, 512f, 512f, -512f, 0f, 40000f);

	public unsafe override void Load(string n)
	{
		base.Load(n);
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			ModelMesh modelMesh = propModel.Meshes[i];
			for (int j = 0; j < modelMesh.MeshParts.Count; j++)
			{
				ModelMeshPart modelMeshPart = modelMesh.MeshParts[j];
				int vertexStride = modelMeshPart.VertexBuffer.VertexDeclaration.VertexStride;
				int vertexCount = modelMeshPart.VertexBuffer.VertexCount;
				int vertexOffset = modelMeshPart.VertexOffset;
				int numVertices = modelMeshPart.NumVertices;
				Matrix matrix = propTransforms[modelMesh.ParentBone.Index];
				Matrix matrix2 = Matrix.Invert(matrix);
				Matrix matrix3 = matrix2;
				matrix3.Translation = Vector3.Zero;
				byte[] array = new byte[vertexCount * vertexStride];
				modelMeshPart.VertexBuffer.GetData(array);
				tmpVertStrcut* ptr = (tmpVertStrcut*)(void*)GCHandle.Alloc(array, GCHandleType.Pinned).AddrOfPinnedObject();
				Vector3 zero = Vector3.Zero;
				float num = 512f;
				Vector3 zero2 = Vector3.Zero;
				Vector3 zero3 = Vector3.Zero;
				for (int k = 0; k < 2; k++)
				{
					for (int l = vertexOffset; l < vertexOffset + numVertices; l++)
					{
						zero = Vector3.Transform(ptr[l].pos, matrix);
						zero = Vector3.Transform(zero, matScale);
						zero.Y = HeightMapPhysics.GetHeight(ref zero) + 4f;
						num = 512f;
						for (int m = -1; m < 2; m++)
						{
							for (int num2 = -1; num2 < 2; num2++)
							{
								zero3 = zero;
								zero3.X += (float)m * num;
								zero3.Z += (float)num2 * num;
								zero2 = zero3 - zero;
								zero2.Y = 0f;
								float num3 = 1f - zero2.Length() / (num * 3f);
								HeightMapPhysics.SetHeightMap(ref zero3, zero.Y, num3 * 0.25f);
							}
						}
					}
				}
			}
		}
	}

	public unsafe void Finalize()
	{
		try
		{
			for (int i = 0; i < propModel.Meshes.Count; i++)
			{
				ModelMesh modelMesh = propModel.Meshes[i];
				for (int j = 0; j < modelMesh.MeshParts.Count; j++)
				{
					ModelMeshPart modelMeshPart = modelMesh.MeshParts[j];
					int vertexStride = modelMeshPart.VertexBuffer.VertexDeclaration.VertexStride;
					int vertexCount = modelMeshPart.VertexBuffer.VertexCount;
					int vertexOffset = modelMeshPart.VertexOffset;
					int numVertices = modelMeshPart.NumVertices;
					Matrix matrix = propTransforms[modelMesh.ParentBone.Index];
					Matrix matrix2 = matrix;
					matrix2.Translation = Vector3.Zero;
					byte[] array = new byte[vertexCount * vertexStride];
					modelMeshPart.VertexBuffer.GetData(array);
					tmpVertStrcut* ptr = (tmpVertStrcut*)(void*)GCHandle.Alloc(array, GCHandleType.Pinned).AddrOfPinnedObject();
					Vector3[] array2 = new Vector3[vertexCount];
					int[] array3 = new int[vertexCount];
					float num = 64f;
					float num2 = 128f;
					Vector3 zero = Vector3.Zero;
					for (int k = vertexOffset; k < vertexOffset + numVertices; k++)
					{
						ref Vector3 reference = ref array2[k];
						reference = Vector3.Transform(ptr[k].pos, matrix);
						ref Vector3 reference2 = ref array2[k];
						reference2 = Vector3.Transform(array2[k], matScale);
						array3[k] = ptr[k].tex;
						array2[k].Y = HeightMapPhysics.GetHeight(ref array2[k]) + 8f;
						for (int l = -2; l < 3; l++)
						{
							for (int m = -2; m < 3; m++)
							{
								zero = array2[k];
								zero.X += (float)l * num;
								zero.Z += (float)m * num;
								zero = array2[k];
								zero.X += (float)l * num2;
								zero.Z += (float)m * num2;
								HeightMapPhysics.SetTreeMap(ref zero, 0);
							}
						}
						ptr[k].pos = array2[k];
					}
					short[] array4 = new short[modelMeshPart.IndexBuffer.IndexCount];
					modelMeshPart.IndexBuffer.GetData(array4);
					Vector3[] array5 = new Vector3[vertexCount];
					Vector3[] array6 = new Vector3[vertexCount];
					Vector3[] array7 = new Vector3[vertexCount];
					HalfVector2 halfVector = default(HalfVector2);
					HalfVector2 halfVector2 = default(HalfVector2);
					HalfVector2 halfVector3 = default(HalfVector2);
					Vector3 zero2 = Vector3.Zero;
					Vector3 zero3 = Vector3.Zero;
					Vector3 zero4 = Vector3.Zero;
					Vector2 zero5 = Vector2.Zero;
					Vector2 zero6 = Vector2.Zero;
					Vector2 zero7 = Vector2.Zero;
					int num3 = 0;
					for (int n = 0; n < modelMeshPart.PrimitiveCount; n++)
					{
						int num4 = vertexOffset + array4[modelMeshPart.StartIndex + num3++];
						int num5 = vertexOffset + array4[modelMeshPart.StartIndex + num3++];
						int num6 = vertexOffset + array4[modelMeshPart.StartIndex + num3++];
						zero2 = array2[num4];
						zero3 = array2[num5];
						zero4 = array2[num6];
						zero2.Y *= 2f;
						zero3.Y *= 2f;
						zero4.Y *= 2f;
						halfVector.PackedValue = (uint)array3[num4];
						halfVector2.PackedValue = (uint)array3[num5];
						halfVector3.PackedValue = (uint)array3[num6];
						zero5 = halfVector.ToVector2();
						zero6 = halfVector2.ToVector2();
						zero7 = halfVector3.ToVector2();
						float num7 = zero3.X - zero2.X;
						float num8 = zero4.X - zero2.X;
						float num9 = zero3.Y - zero2.Y;
						float num10 = zero4.Y - zero2.Y;
						float num11 = zero3.Z - zero2.Z;
						float num12 = zero4.Z - zero2.Z;
						float num13 = zero6.X - zero5.X;
						float num14 = zero7.X - zero5.X;
						float num15 = zero6.Y - zero5.Y;
						float num16 = zero7.Y - zero5.Y;
						float num17 = 1f / (num13 * num16 - num14 * num15);
						Vector3 vector = new Vector3((num16 * num7 - num15 * num8) * num17, (num16 * num9 - num15 * num10) * num17, (num16 * num11 - num15 * num12) * num17);
						Vector3 vector2 = new Vector3((num13 * num8 - num14 * num7) * num17, (num13 * num10 - num14 * num9) * num17, (num13 * num12 - num14 * num11) * num17);
						array5[num4] += vector;
						array5[num5] += vector;
						array5[num6] += vector;
						array6[num4] += vector2;
						array6[num5] += vector2;
						array6[num6] += vector2;
						Vector3 vector3 = new Vector3(num7, num9, num11);
						Vector3 vector4 = new Vector3(num8, num10, num12);
						Vector3 vector5 = Vector3.Cross(vector4, vector3);
						vector5.Normalize();
						array7[num4] += vector5;
						array7[num5] += vector5;
						array7[num6] += vector5;
					}
					for (int num18 = vertexOffset; num18 < vertexOffset + numVertices; num18++)
					{
						Vector3 vector6 = array7[num18];
						Vector3 vector7 = array5[num18];
						vector6.Normalize();
						ptr[num18].norm = (int)((vector6.X + 1f) * 127.5f);
						ptr[num18].norm |= (int)((vector6.Y + 1f) * 127.5f) << 8;
						ptr[num18].norm |= (int)((vector6.Z + 1f) * 127.5f) << 16;
						Vector3 vector8 = vector7 - vector6 * Vector3.Dot(vector6, vector7);
						vector8.Normalize();
						vector8 *= ((Vector3.Dot(Vector3.Cross(vector6, vector8), array6[num18]) < 0f) ? (-1f) : 1f);
						ptr[num18].tan = (int)((vector8.X + 1f) * 127.5f);
						ptr[num18].tan |= (int)((vector8.Y + 1f) * 127.5f) << 8;
						ptr[num18].tan |= (int)((vector8.Z + 1f) * 127.5f) << 16;
					}
					modelMeshPart.VertexBuffer.SetData(array);
				}
			}
		}
		catch (Exception threadExceptionArgument)
		{
			EndGameEngine.ThreadExceptionArgument = threadExceptionArgument;
		}
	}

	public void Update(float eTime, int qIndex, PlayerBase playerRef)
	{
		ref Matrix reference = ref matWorld[qIndex];
		reference = Matrix.Identity;
		tmpPos.X = 0f - playerRef.vecHeadPosition[qIndex].X;
		tmpPos.Z = 0f - playerRef.vecHeadPosition[qIndex].Z;
		matWorld[qIndex].Translation = tmpPos;
	}

	public override void Draw(PlayerBase viewer, int qIndex)
	{
	}
}
