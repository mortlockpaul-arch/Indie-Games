using System;
using System.IO;
using System.Runtime.InteropServices;
using DataCompressor;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace EGEngine;

public class HeightMapPhysics
{
	public static Texture2D texHeightMap;

	public static float[,] Heightmap;

	public static Texture2D texAlphaMap;

	public static Texture2D texAlphaMapPalette;

	public static uint[] AlphaMap;

	private static float HeightMapScale = 1f;

	private static float SeaLevel = 0f;

	private static bool m_IsSet = false;

	private static byte[,] TreeMap;

	private static float QuantizedResolution = 65535f;

	private static Vector3 tmpOffset = Vector3.Zero;

	private static Vector3 tmpv0 = Vector3.Zero;

	private static Vector3 tmpv1 = Vector3.Zero;

	private static Vector3 tmpv2 = Vector3.Zero;

	private static Vector3 tmpedge0 = Vector3.Zero;

	private static Vector3 tmpedge1 = Vector3.Zero;

	private static Vector3 tmpStart = Vector3.Zero;

	private static Vector3 tmpEnd = Vector3.Zero;

	private static Vector3 tmpPosSave = Vector3.Zero;

	private static Vector3 tmpPos = Vector3.Zero;

	public static void Initialize()
	{
	}

	public static bool GetQuantizedPosition(ref Vector3 pos, ref uint offset, ref byte quant)
	{
		if (!m_IsSet)
		{
			return false;
		}
		quant = (byte)((pos.X + 262144f) / QuantizedResolution);
		quant |= (byte)((byte)((pos.Z + 262144f) / QuantizedResolution) << 4);
		offset = (uint)((pos.X + 262144f) % QuantizedResolution);
		offset |= (uint)((pos.Z + 262144f) % QuantizedResolution) << 16;
		return true;
	}

	public static bool ExspandQuantizedPosition(ref Vector3 pos, ref uint offset, ref byte quant)
	{
		if (!m_IsSet)
		{
			return false;
		}
		pos.X = (float)(quant & 0xF) * QuantizedResolution - 262144f;
		pos.X += offset & 0xFFFF;
		pos.Z = (float)((quant & 0xF0) >> 4) * QuantizedResolution - 262144f;
		pos.Z += (offset & 0xFFFF0000u) >> 16;
		return true;
	}

	public static float GetHeight(ref Vector3 position, out Vector3 normal)
	{
		if (!m_IsSet)
		{
			normal = Vector3.UnitY;
			return 0f;
		}
		tmpOffset.Y = position.Y;
		tmpOffset.X = position.X + 65536f;
		tmpOffset.Z = position.Z + 65536f;
		int num = (int)tmpOffset.X / 512;
		int num2 = (int)tmpOffset.Z / 512;
		num = ((num >= 0) ? num : 0);
		num2 = ((num2 >= 0) ? num2 : 0);
		num = ((num + 1 >= 256) ? 254 : num);
		num2 = ((num2 + 1 >= 256) ? 254 : num2);
		float num3 = tmpOffset.X % 512f / 512f;
		float num4 = tmpOffset.Z % 512f / 512f;
		float value = MathHelper.Lerp(Heightmap[num, num2], Heightmap[num + 1, num2], num3);
		float value2 = MathHelper.Lerp(Heightmap[num, num2 + 1], Heightmap[num + 1, num2 + 1], num3);
		if (num3 + num4 >= 1f)
		{
			tmpv0.X = 512f;
			tmpv0.Y = Heightmap[num + 1, num2];
			tmpv0.Z = 0f;
			tmpv1.X = 0f;
			tmpv1.Y = Heightmap[num, num2 + 1];
			tmpv1.Z = 512f;
			tmpv2.X = 512f;
			tmpv2.Y = Heightmap[num + 1, num2 + 1];
			tmpv2.Z = 512f;
		}
		else
		{
			tmpv0.X = 0f;
			tmpv0.Y = Heightmap[num, num2];
			tmpv0.Z = 0f;
			tmpv1.X = 0f;
			tmpv1.Y = Heightmap[num, num2 + 1];
			tmpv1.Z = 512f;
			tmpv2.X = 512f;
			tmpv2.Y = Heightmap[num + 1, num2];
			tmpv2.Z = 0f;
		}
		tmpedge0 = tmpv1 - tmpv0;
		tmpedge1 = tmpv2 - tmpv0;
		Vector3.Cross(ref tmpedge0, ref tmpedge1, out normal);
		normal.Normalize();
		return MathHelper.Lerp(value, value2, num4);
	}

	public static bool RayCast(ref Ray ray, ref Vector3 hitPos, ref Vector3 hitNorm, ref float hitDistance)
	{
		tmpStart = ray.Position;
		tmpStart.X += 65536f;
		tmpStart.Z += 65536f;
		int num = (int)Math.Floor(tmpStart.X / 512f);
		int num2 = (int)Math.Floor(tmpStart.Z / 512f);
		tmpEnd = ray.Position + ray.Direction * 15000f;
		tmpEnd.X += 65536f;
		tmpEnd.Z += 65536f;
		int num3 = (int)Math.Floor(tmpEnd.X / 512f);
		int num4 = (int)Math.Floor(tmpEnd.Z / 512f);
		int num5 = ((tmpStart.X < tmpEnd.X) ? 1 : ((tmpStart.X > tmpEnd.X) ? (-1) : 0));
		int num6 = ((tmpStart.Z < tmpEnd.Z) ? 1 : ((tmpStart.Z > tmpEnd.Z) ? (-1) : 0));
		float x = tmpStart.X;
		float num7 = 512f * (float)Math.Floor(x / 512f);
		float num8 = num7 + 512f;
		float num9 = ((tmpStart.X > tmpEnd.X) ? (x - num7) : (num8 - x)) / Math.Abs(tmpEnd.X - tmpStart.X);
		float z = tmpStart.Z;
		float num10 = 512f * (float)Math.Floor(z / 512f);
		float num11 = num10 + 512f;
		float num12 = ((tmpStart.Z > tmpEnd.Z) ? (z - num10) : (num11 - z)) / Math.Abs(tmpEnd.Z - tmpStart.Z);
		float num13 = 512f / Math.Abs(tmpEnd.X - tmpStart.X);
		float num14 = 512f / Math.Abs(tmpEnd.Z - tmpStart.Z);
		while (true)
		{
			if (num >= 0 && num < 256 && num2 >= 0 && num2 < 256)
			{
				int num15 = num;
				int num16 = num2;
				num15 = ((num15 >= 0) ? num15 : 0);
				num16 = ((num16 >= 0) ? num16 : 0);
				num15 = ((num15 + 1 >= 256) ? 254 : num15);
				num16 = ((num16 + 1 >= 256) ? 254 : num16);
				float num17 = tmpStart.X % 512f / 512f;
				float num18 = tmpStart.Z % 512f / 512f;
				if (num17 >= num18)
				{
					tmpv0.X = num15 * 512 + 512;
					tmpv0.Y = Heightmap[num15 + 1, num16];
					tmpv0.Z = num16 * 512;
					tmpv1.X = num15 * 512;
					tmpv1.Y = Heightmap[num15, num16 + 1];
					tmpv1.Z = num16 * 512 + 512;
					tmpv2.X = num15 * 512 + 512;
					tmpv2.Y = Heightmap[num15 + 1, num16 + 1];
					tmpv2.Z = num16 * 512 + 512;
				}
				else
				{
					tmpv0.X = num15 * 512;
					tmpv0.Y = Heightmap[num15, num16];
					tmpv0.Z = num16 * 512;
					tmpv1.X = num15 * 512;
					tmpv1.Y = Heightmap[num15, num16 + 1];
					tmpv1.Z = num16 * 512 + 512;
					tmpv2.X = num15 * 512 + 512;
					tmpv2.Y = Heightmap[num15 + 1, num16];
					tmpv2.Z = num16 * 512;
				}
				tmpv0.X -= 65536f;
				tmpv0.Z -= 65536f;
				tmpv1.X -= 65536f;
				tmpv1.Z -= 65536f;
				tmpv2.X -= 65536f;
				tmpv2.Z -= 65536f;
				float lineParameter = float.MaxValue;
				if (MyMath.IntersectRayTriangle(ref ray.Position, ref ray.Direction, ref tmpv0, ref tmpv1, ref tmpv2, ref lineParameter))
				{
					hitDistance = lineParameter;
					hitPos = ray.Position + ray.Direction * hitDistance;
					tmpv1 -= tmpv0;
					tmpv2 -= tmpv0;
					hitNorm = Vector3.Cross(tmpv2, tmpv1);
					return true;
				}
			}
			if (num9 <= num12)
			{
				if (num == num3)
				{
					break;
				}
				num9 += num13;
				num += num5;
			}
			else
			{
				if (num2 == num4)
				{
					break;
				}
				num12 += num14;
				num2 += num6;
			}
		}
		return false;
	}

	public static float GetHeight(ref Vector3 position)
	{
		tmpOffset.Y = position.Y;
		tmpOffset.X = position.X + 65536f;
		tmpOffset.Z = position.Z + 65536f;
		int num = (int)tmpOffset.X / 512;
		int num2 = (int)tmpOffset.Z / 512;
		num = ((num >= 0) ? num : 0);
		num2 = ((num2 >= 0) ? num2 : 0);
		num = ((num + 1 >= 256) ? 254 : num);
		num2 = ((num2 + 1 >= 256) ? 254 : num2);
		float amount = tmpOffset.X % 512f / 512f;
		float amount2 = tmpOffset.Z % 512f / 512f;
		float value = MathHelper.Lerp(Heightmap[num, num2], Heightmap[num + 1, num2], amount);
		float value2 = MathHelper.Lerp(Heightmap[num, num2 + 1], Heightmap[num + 1, num2 + 1], amount);
		return MathHelper.Lerp(value, value2, amount2);
	}

	public static void SetTreeMap(ref Vector3 position, byte treeValue)
	{
		tmpOffset = position;
		tmpOffset.X *= 4f;
		tmpOffset.Z *= 4f;
		tmpOffset.X += 262144f;
		tmpOffset.Z += 262144f;
		int num = (int)tmpOffset.X / 512;
		int num2 = (int)tmpOffset.Z / 512;
		num = ((num >= 0) ? num : 0);
		num2 = ((num2 >= 0) ? num2 : 0);
		num = ((num + 1 >= 1024) ? 1022 : num);
		num2 = ((num2 + 1 >= 1024) ? 1022 : num2);
		TreeMap[num, num2] = treeValue;
	}

	public static byte ReadTreeMap(ref Vector3 position)
	{
		tmpOffset = position;
		tmpOffset.X *= 4f;
		tmpOffset.Z *= 4f;
		tmpOffset.X += 262144f;
		tmpOffset.Z += 262144f;
		int num = (int)tmpOffset.X / 512;
		int num2 = (int)tmpOffset.Z / 512;
		num = ((num >= 0) ? num : 0);
		num2 = ((num2 >= 0) ? num2 : 0);
		num = ((num + 1 >= 1024) ? 1022 : num);
		num2 = ((num2 + 1 >= 1024) ? 1022 : num2);
		return TreeMap[num, num2];
	}

	public static void Set(int heightScale, int seaDepth)
	{
		HeightMapScale = heightScale;
		SeaLevel = seaDepth;
		Texture2D texture2D = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\treeMap");
		Color[] array = new Color[texture2D.Width * texture2D.Height];
		texture2D.GetData(array);
		TreeMap = new byte[texture2D.Width, texture2D.Height];
		for (int i = 0; i < 1024; i++)
		{
			for (int j = 0; j < 1024; j++)
			{
				int num = i + j * 1024;
				TreeMap[i, j] = array[num].R;
			}
		}
		Model model = EndGameEngine.GameAssetMgr.Load<Model>("level\\TerrainMesh");
		Heightmap = new float[256, 256];
		for (int k = 0; k < 256; k++)
		{
			for (int l = 0; l < 256; l++)
			{
				int num2 = k + l * 256;
				Heightmap[k, l] = ((float[])model.Tag)[num2];
			}
		}
		FileStream fileStream = File.OpenRead(EndGameEngine.GameAssetMgr.RootDirectory + "\\terrain\\AlphaMask.idx");
		int num3 = fileStream.ReadByte();
		num3 |= fileStream.ReadByte() << 8;
		num3 |= fileStream.ReadByte() << 16;
		num3 |= fileStream.ReadByte() << 24;
		int num4 = fileStream.ReadByte();
		num4 |= fileStream.ReadByte() << 8;
		num4 |= fileStream.ReadByte() << 16;
		num4 |= fileStream.ReadByte() << 24;
		byte[] array2 = new byte[num3 * num4];
		for (int m = 0; m < num4; m++)
		{
			int dstPos = m * num3;
			EncodeData.DecodeRow(array2, fileStream, dstPos, num3);
		}
		Color[] array3 = new Color[256];
		for (int n = 0; n < 256; n++)
		{
			ref Color reference = ref array3[n];
			reference = Color.White;
			int num5 = fileStream.ReadByte();
			int num6 = fileStream.ReadByte();
			int num7 = fileStream.ReadByte();
			int num8 = fileStream.ReadByte();
			array3[n].R = (byte)num5;
			array3[n].G = (byte)num6;
			array3[n].B = (byte)num7;
			array3[n].A = (byte)num8;
		}
		fileStream.Close();
		fileStream.Dispose();
		texAlphaMap = new Texture2D(EndGameEngine.GraphicMgr.GraphicsDevice, num3, num4, mipMap: false, SurfaceFormat.Alpha8);
		texAlphaMap.SetData(array2);
		texAlphaMapPalette = new Texture2D(EndGameEngine.GraphicMgr.GraphicsDevice, 256, 1, mipMap: false, SurfaceFormat.Color);
		texAlphaMapPalette.SetData(array3);
		AlphaMap = new uint[1048576];
		float num9 = (float)num3 / 1024f;
		float num10 = (float)num4 / 1024f;
		float num11 = 0f;
		for (int num12 = 0; num12 < 1024; num12++)
		{
			float num13 = 0f;
			for (int num14 = 0; num14 < 1024; num14++)
			{
				int num15 = (int)num11 + (int)num13 * num3;
				int num16 = num12 + num14 * 1024;
				uint r = array3[array2[num15]].R;
				uint g = array3[array2[num15]].G;
				uint b = array3[array2[num15]].B;
				AlphaMap[num16] = 4278190080u;
				if (r < 32 && g < 32 && b < 32)
				{
					AlphaMap[num16] = 4278190080u;
				}
				else if (r > g && r > b)
				{
					AlphaMap[num16] = 16711680u;
				}
				else if (g > r && g > b)
				{
					AlphaMap[num16] = 65280u;
				}
				else if (b > r && b > g)
				{
					AlphaMap[num16] = 255u;
				}
				num13 += num10;
			}
			num11 += num9;
		}
		m_IsSet = true;
	}

	public static void SetAlphaMap(ref Vector3 pos, uint alphaValue)
	{
		tmpOffset = pos;
		tmpOffset.X *= 4f;
		tmpOffset.Z *= 4f;
		tmpOffset.X += 261888f;
		tmpOffset.Z += 261888f;
		int num = (int)tmpOffset.X / 512;
		int num2 = (int)tmpOffset.Z / 512;
		num = ((num >= 0) ? num : 0);
		num2 = ((num2 >= 0) ? num2 : 0);
		num = ((num + 1 >= 1024) ? 1022 : num);
		num2 = ((num2 + 1 >= 1024) ? 1022 : num2);
		int num3 = num + num2 * 1024;
		int num4 = num + 1 + num2 * 1024;
		int num5 = num + (num2 + 1) * 1024;
		int num6 = num + 1 + (num2 + 1) * 1024;
		_ = AlphaMap[num3];
		_ = AlphaMap[num4];
		_ = AlphaMap[num5];
		_ = AlphaMap[num6];
		AlphaMap[num3] = 65280u;
		AlphaMap[num4] = 65280u;
		AlphaMap[num5] = 65280u;
		AlphaMap[num6] = 65280u;
	}

	public static uint GetAlphaMap(ref Vector3 position)
	{
		tmpOffset = position;
		tmpOffset.X *= 4f;
		tmpOffset.Z *= 4f;
		tmpOffset.X += 261888f;
		tmpOffset.Z += 261888f;
		int num = (int)tmpOffset.X / 512;
		int num2 = (int)tmpOffset.Z / 512;
		num = ((num >= 0) ? num : 0);
		num2 = ((num2 >= 0) ? num2 : 0);
		num = ((num + 1 >= 1024) ? 1022 : num);
		num2 = ((num2 + 1 >= 1024) ? 1022 : num2);
		int num3 = num + num2 * 1024;
		return AlphaMap[num3];
	}

	public static void SetHeightMap(ref Vector3 pos, float heightValue, float weight)
	{
		tmpOffset.Y = pos.Y;
		tmpOffset.X = pos.X + 65536f;
		tmpOffset.Z = pos.Z + 65536f;
		int num = (int)tmpOffset.X / 512;
		int num2 = (int)tmpOffset.Z / 512;
		num = ((num >= 0) ? num : 0);
		num2 = ((num2 >= 0) ? num2 : 0);
		num = ((num + 1 >= 256) ? 254 : num);
		num2 = ((num2 + 1 >= 256) ? 254 : num2);
		if (Heightmap[num, num2] < heightValue)
		{
			float num3 = (heightValue - Heightmap[num, num2]) * weight;
			Heightmap[num, num2] += num3;
		}
		else
		{
			float num4 = (Heightmap[num, num2] - heightValue) * weight;
			Heightmap[num, num2] -= num4;
		}
	}

	public static void SetHeightMap(ref Vector3 pos)
	{
		tmpOffset.Y = pos.Y;
		tmpOffset.X = pos.X + 65536f;
		tmpOffset.Z = pos.Z + 65536f;
		int num = (int)tmpOffset.X / 512;
		int num2 = (int)tmpOffset.Z / 512;
		num = ((num >= 0) ? num : 0);
		num2 = ((num2 >= 0) ? num2 : 0);
		num = ((num + 1 >= 256) ? 254 : num);
		num2 = ((num2 + 1 >= 256) ? 254 : num2);
		Heightmap[num, num2] = tmpOffset.Y;
	}

	public static void FinalizeHeightMap()
	{
		float[] array = new float[65536];
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				int num = i + j * 256;
				array[num] = (Heightmap[i, j] + SeaLevel) / HeightMapScale;
			}
		}
		texHeightMap = new Texture2D(EndGameEngine.GraphicMgr.GraphicsDevice, 256, 256, mipMap: false, SurfaceFormat.Single);
		texHeightMap.SetData(array);
	}

	public static void FinalizeMaps()
	{
	}

	public void Update(float eTime)
	{
	}

	public unsafe static void SetVerticeToHeightData(ModelMeshPart meshPart, Matrix matObjectSpace, Matrix matWorld, Matrix matScale, bool transformToWorldSpace, bool resetNormals, bool noTrees, bool noGrass, ref BoundingBox bbox)
	{
		Matrix matrix = matObjectSpace * matWorld;
		Matrix matrix2 = matrix;
		matrix2.Translation = Vector3.Zero;
		Matrix matrix3 = Matrix.Invert(matrix);
		Matrix matrix4 = matrix3;
		matrix4.Translation = Vector3.Zero;
		int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
		int vertexCount = meshPart.VertexBuffer.VertexCount;
		int vertexOffset = meshPart.VertexOffset;
		int numVertices = meshPart.NumVertices;
		byte[] array = new byte[vertexCount * vertexStride];
		meshPart.VertexBuffer.GetData(array);
		Vector3[] array2 = new Vector3[vertexCount];
		uint[] array3 = new uint[vertexCount];
		short[] array4 = new short[meshPart.IndexBuffer.IndexCount];
		meshPart.IndexBuffer.GetData(array4);
		Vector3[] array5 = new Vector3[vertexCount];
		Vector3[] array6 = new Vector3[vertexCount];
		Vector3[] array7 = new Vector3[vertexCount];
		PropModelVertStruct* ptr = (PropModelVertStruct*)(void*)GCHandle.Alloc(array, GCHandleType.Pinned).AddrOfPinnedObject();
		float num = 64f;
		float num2 = 64f;
		Vector3 zero = Vector3.Zero;
		for (int i = vertexOffset; i < vertexOffset + numVertices; i++)
		{
			ref Vector3 reference = ref array2[i];
			reference = Vector3.Transform(ptr[i].pos, matrix);
			ref Vector3 reference2 = ref array2[i];
			reference2 = Vector3.Transform(array2[i], matScale);
			array3[i] = ptr[i].tex;
			array2[i].Y += GetHeight(ref array2[i]) + 1f;
			bbox.Min.X = ((bbox.Min.X > array2[i].X) ? array2[i].X : bbox.Min.X);
			bbox.Min.Y = ((bbox.Min.Y > array2[i].Y) ? array2[i].Y : bbox.Min.Y);
			bbox.Min.Z = ((bbox.Min.Z > array2[i].Z) ? array2[i].Z : bbox.Min.Z);
			bbox.Max.X = ((bbox.Max.X < array2[i].X) ? array2[i].X : bbox.Max.X);
			bbox.Max.Y = ((bbox.Max.Y < array2[i].Y) ? array2[i].Y : bbox.Max.Y);
			bbox.Max.Z = ((bbox.Max.Z < array2[i].Z) ? array2[i].Z : bbox.Max.Z);
			if (noTrees || noGrass)
			{
				for (int j = -2; j < 3; j++)
				{
					for (int k = -2; k < 3; k++)
					{
						if (noGrass)
						{
							zero = array2[i];
							zero.X += (float)j * num;
							zero.Z += (float)k * num;
							SetAlphaMap(ref zero, 4294967040u);
						}
						if (noTrees)
						{
							zero = array2[i];
							zero.X += (float)j * num2;
							zero.Z += (float)k * num2;
							SetTreeMap(ref zero, 0);
						}
					}
				}
			}
			if (transformToWorldSpace)
			{
				ptr[i].pos = array2[i];
			}
			else
			{
				ptr[i].pos = Vector3.Transform(array2[i], matrix3);
			}
		}
		if (resetNormals)
		{
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
			for (int l = 0; l < meshPart.PrimitiveCount; l++)
			{
				int num4 = vertexOffset + array4[meshPart.StartIndex + num3++];
				int num5 = vertexOffset + array4[meshPart.StartIndex + num3++];
				int num6 = vertexOffset + array4[meshPart.StartIndex + num3++];
				zero2 = array2[num4];
				zero3 = array2[num5];
				zero4 = array2[num6];
				zero2.Y *= 2f;
				zero3.Y *= 2f;
				zero4.Y *= 2f;
				halfVector.PackedValue = array3[num4];
				halfVector2.PackedValue = array3[num5];
				halfVector3.PackedValue = array3[num6];
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
			for (int m = vertexOffset; m < vertexOffset + numVertices; m++)
			{
				Vector3 vector6 = array7[m];
				Vector3 vector7 = array5[m];
				if (!transformToWorldSpace)
				{
					vector6 = Vector3.Transform(vector6, matrix4);
				}
				vector6.Normalize();
				ptr[m].norm = (uint)((vector6.X + 1f) * 127.5f);
				ptr[m].norm |= (uint)((vector6.Y + 1f) * 127.5f) << 8;
				ptr[m].norm |= (uint)((vector6.Z + 1f) * 127.5f) << 16;
				Vector3 vector8 = vector7 - vector6 * Vector3.Dot(vector6, vector7);
				if (!transformToWorldSpace)
				{
					vector8 = Vector3.Transform(vector8, matrix4);
				}
				vector8.Normalize();
				vector8 *= ((Vector3.Dot(Vector3.Cross(vector6, vector8), array6[m]) < 0f) ? (-1f) : 1f);
				ptr[m].tan = (uint)((vector8.X + 1f) * 127.5f);
				ptr[m].tan |= (uint)((vector8.Y + 1f) * 127.5f) << 8;
				ptr[m].tan |= (uint)((vector8.Z + 1f) * 127.5f) << 16;
			}
		}
		meshPart.VertexBuffer.SetData(array);
	}

	public unsafe static void RelaxHeightMapAtVertice(ModelMeshPart meshPart, Matrix matObjectSpace, Matrix matWorld, bool noTrees, bool noGrass, Matrix matScale)
	{
		int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
		int vertexCount = meshPart.VertexBuffer.VertexCount;
		int vertexOffset = meshPart.VertexOffset;
		int numVertices = meshPart.NumVertices;
		Matrix matrix = matObjectSpace * matWorld;
		Matrix matrix2 = Matrix.Invert(matrix);
		Matrix matrix3 = matrix2;
		matrix3.Translation = Vector3.Zero;
		byte[] array = new byte[vertexCount * vertexStride];
		meshPart.VertexBuffer.GetData(array);
		PropModelVertStruct* ptr = (PropModelVertStruct*)(void*)GCHandle.Alloc(array, GCHandleType.Pinned).AddrOfPinnedObject();
		Vector3 zero = Vector3.Zero;
		_ = Vector3.Zero;
		Vector3 zero2 = Vector3.Zero;
		Vector3 zero3 = Vector3.Zero;
		for (int i = 0; i < 2; i++)
		{
			for (int j = vertexOffset; j < vertexOffset + numVertices; j++)
			{
				zero = Vector3.Transform(ptr[j].pos, matrix);
				zero = Vector3.Transform(zero, matScale);
				zero.Y = GetHeight(ref zero);
				float num = 256f;
				for (int k = -2; k < 3; k++)
				{
					for (int l = -2; l < 3; l++)
					{
						zero2 = zero;
						zero2.X += (float)k * num;
						zero2.Z += (float)l * num;
						zero3 = zero2 - zero;
						zero3.Y = 0f;
						float num2 = 1f - zero3.Length() / (num * 3f);
						SetHeightMap(ref zero2, zero.Y, num2 * 0.25f);
						if ((double)num2 > 0.4)
						{
							if (noTrees)
							{
								SetTreeMap(ref zero2, 0);
							}
							if (noGrass)
							{
								SetAlphaMap(ref zero2, 4294967040u);
							}
						}
					}
				}
			}
		}
	}

	public unsafe static void ForceHeightDataToVertice(ModelMeshPart meshPart, Matrix matObjectSpace, Matrix matWorld, Matrix matScale, bool noTrees, bool noGrass, bool iterate, float influenceRad)
	{
		int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
		int vertexCount = meshPart.VertexBuffer.VertexCount;
		int vertexOffset = meshPart.VertexOffset;
		int numVertices = meshPart.NumVertices;
		Matrix matrix = matObjectSpace * matWorld;
		Matrix matrix2 = Matrix.Invert(matrix);
		Matrix matrix3 = matrix2;
		matrix3.Translation = Vector3.Zero;
		byte[] array = new byte[vertexCount * vertexStride];
		meshPart.VertexBuffer.GetData(array);
		PropModelVertStruct* ptr = (PropModelVertStruct*)(void*)GCHandle.Alloc(array, GCHandleType.Pinned).AddrOfPinnedObject();
		Vector3 zero = Vector3.Zero;
		_ = Vector3.Zero;
		if (iterate)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = vertexOffset; j < vertexOffset + numVertices; j++)
				{
					zero = Vector3.Transform(ptr[j].pos, matrix);
					zero = Vector3.Transform(zero, matScale);
					Vector3 zero2 = Vector3.Zero;
					Vector3 zero3 = Vector3.Zero;
					for (int k = -2; k < 3; k++)
					{
						for (int l = -2; l < 3; l++)
						{
							zero3 = zero;
							zero3.X += (float)k * influenceRad;
							zero3.Z += (float)l * influenceRad;
							zero2 = zero3 - zero;
							zero2.Y = 0f;
							float num = zero2.Length();
							num /= influenceRad * 3f;
							float num2 = 1f - num;
							SetHeightMap(ref zero3, zero.Y, num2);
							if (noGrass && num2 > 0.75f)
							{
								SetAlphaMap(ref zero3, 4294967040u);
							}
							if (noTrees && num2 > 0.6f)
							{
								SetTreeMap(ref zero3, 0);
							}
						}
					}
				}
			}
			return;
		}
		for (int m = vertexOffset; m < vertexOffset + numVertices; m++)
		{
			zero = Vector3.Transform(ptr[m].pos, matrix);
			zero = Vector3.Transform(zero, matScale);
			SetHeightMap(ref zero, zero.Y, 1f);
			if (noGrass)
			{
				SetAlphaMap(ref zero, 4294967040u);
			}
			if (noTrees)
			{
				SetTreeMap(ref zero, 0);
			}
		}
	}
}
