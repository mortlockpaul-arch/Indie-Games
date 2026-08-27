using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.PathingBase, DataContent")]
public class PathingBase
{
	public const int NumOfLayers = 30;

	private const float CELL_SIZE = 32f;

	private const float HALFCELL_SIZE = 16f;

	private const float HIEGHT_STEP = 32f;

	public uint Flg_Clear;

	public uint Flg_Walkable = 1u;

	public uint Flg_OpenList = 1073741824u;

	public uint Flg_ClosedList = 2147483648u;

	public float CellSize = 32f;

	public float HalfCellSize = 16f;

	public float HieghtStep = 32f;

	public float TestOffsetY = 60f;

	public float CapsulRadius = 18f;

	public float CapsulRadiusSqr = 256f;

	public uint[] PathingData;

	public int PathingSizeX;

	public int PathingSizeZ;

	public int PathingSizeY;

	public float PathingHeightSteps;

	public Vector3 Min;

	public Vector3 Max;

	public Vector3 ContainerSize;

	private static Vector3 tmpAxis = Vector3.Zero;

	private static Vector3[] AxisTest = new Vector3[4]
	{
		Vector3.UnitX,
		-Vector3.UnitX,
		Vector3.UnitZ,
		-Vector3.UnitZ
	};

	private static Vector2[] testDirection = new Vector2[8]
	{
		new Vector2(0f, 1f),
		new Vector2(-1f, 1f),
		new Vector2(-1f, 0f),
		new Vector2(-1f, -1f),
		new Vector2(0f, -1f),
		new Vector2(1f, -1f),
		new Vector2(1f, 0f),
		new Vector2(1f, 1f)
	};

	public void AllocateData(eOOBB m)
	{
		int num = (int)m.Min.X;
		int num2 = (int)m.Min.Y;
		int num3 = (int)m.Min.Z;
		Min.X = num;
		Min.Y = num2;
		Min.Z = num3;
		num = (int)m.Max.X;
		num2 = (int)m.Max.Y;
		num3 = (int)m.Max.Z;
		Max.X = num;
		Max.Y = num2;
		Max.Z = num3;
		ContainerSize = Max - Min;
		PathingSizeX = (int)(ContainerSize.X / CellSize);
		PathingSizeZ = (int)(ContainerSize.Z / CellSize);
		PathingSizeY = (int)(ContainerSize.Y / 960f);
		PathingSizeY = 30;
		PathingData = new uint[PathingSizeX * PathingSizeZ];
		for (int i = 0; i < PathingSizeX * PathingSizeZ; i++)
		{
			PathingData[i] = Flg_Clear;
		}
	}

	public void Initialize(eOOBB m, List<eOOBB> p, CollisionMesh e)
	{
		_ = Vector3.Zero;
		_ = Vector3.Zero;
		_ = Vector3.Zero;
		_ = Vector3.Zero;
		_ = Vector3.Zero;
		uint[] array = new uint[PathingSizeX * PathingSizeZ];
		uint[] array2 = new uint[PathingSizeX * PathingSizeZ];
		for (int i = 0; i < PathingSizeX * PathingSizeZ; i++)
		{
			array[i] = Flg_Clear;
			array2[i] = Flg_Clear;
		}
		SetWalkableBitBuffer(array, e);
	}

	public void SetWalkableBitBuffer(uint[] nodes, CollisionMesh e)
	{
		Vector3 zero = Vector3.Zero;
		IntersectSegmentParams segment = default(IntersectSegmentParams);
		for (int i = 0; i < PathingSizeX; i++)
		{
			for (int j = 0; j < PathingSizeZ; j++)
			{
				for (int k = 0; k < PathingSizeY; k++)
				{
					uint num = Flg_Walkable << k;
					if ((nodes[i * PathingSizeX + j] & (num >> 1)) != 0 || (nodes[i * PathingSizeX + j] & (num >> 2)) != 0 || (nodes[i * PathingSizeX + j] & (num >> 3)) != 0)
					{
						continue;
					}
					bool flag = true;
					bool flag2 = false;
					zero.X = Min.X + (float)i * CellSize;
					zero.Z = Min.Z + (float)j * CellSize;
					zero.Y = Min.Y + (float)k * HieghtStep + TestOffsetY;
					segment.SegmentDirection = -Vector3.UnitY;
					segment.SegmentLength = 500f;
					segment.SegmentStart = zero;
					segment.SegmentEnd = zero + Vector3.UnitY * (0f - segment.SegmentLength);
					segment.PreComputeParameters();
					int outX = 0;
					int outZ = 0;
					e.GetGridPosition(ref outX, ref outZ, zero.X, zero.Z);
					if (e.WalkableData[outX][outZ].Indices != null)
					{
						for (int l = 0; l < e.WalkableData[outX][outZ].Indices.Length; l++)
						{
							int num2 = e.WalkableData[outX][outZ].Indices[l];
							Vector3 aabbMin = zero;
							Vector3 aabbMax = zero;
							aabbMin.X -= CapsulRadius;
							aabbMin.Y -= CapsulRadius;
							aabbMin.Z -= CapsulRadius;
							aabbMax.X += CapsulRadius;
							aabbMax.Y += CapsulRadius + 42f;
							aabbMax.Z += CapsulRadius;
							if (MyMath.TestTriangleAABB(ref aabbMin, ref aabbMax, ref e.TriangleDataMesh[num2]))
							{
								flag = false;
								break;
							}
							if (MyMath.IntersectSegmentTriangle(ref segment, ref e.TriangleDataMesh[num2]))
							{
								float num3 = (segment.hitPosition - segment.SegmentStart).LengthSquared();
								if (num3 < (TestOffsetY + 8f) * (TestOffsetY + 8f))
								{
									flag2 = true;
								}
							}
						}
					}
					if (flag && flag2)
					{
						nodes[i * PathingSizeX + j] |= num;
					}
				}
			}
		}
	}
}
