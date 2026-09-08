using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.CollisionMesh, DataContent")]
public struct CollisionMesh
{
	public TriangleData[] TriangleDataMesh;

	public Vector3[] PositionListData;

	public GridElement[][] NoWalkData;

	public GridElement[][] WalkableData;

	public int GridCellSize;

	public int NumberGridX;

	public int NumberGridY;

	public int NumberGridZ;

	public Vector3 GridSize;

	public Vector3 GridMin;

	public Vector3 GridMax;

	public void SetParameters()
	{
		MyMath.PositionList = PositionListData;
	}

	public void Initialize(List<TriangleData> triangledata, Vector3[] positionData, int positionCount)
	{
		TriangleDataMesh = new TriangleData[triangledata.Count];
		triangledata.CopyTo(TriangleDataMesh);
		PositionListData = new Vector3[positionCount];
		for (int i = 0; i < positionCount; i++)
		{
			ref Vector3 reference = ref PositionListData[i];
			reference = positionData[i];
		}
		MyMath.PositionList = PositionListData;
		Vector3 zero = Vector3.Zero;
		Vector3 zero2 = Vector3.Zero;
		Vector3 zero3 = Vector3.Zero;
		GridMin = new Vector3(1000000f, 1000000f, 1000000f);
		GridMax = new Vector3(-1000000f, -1000000f, -1000000f);
		for (int j = 0; j < TriangleDataMesh.Length; j++)
		{
			if ((TriangleDataMesh[j].Flags & 1) != 0)
			{
				zero = PositionListData[TriangleDataMesh[j].p1];
				zero2 = PositionListData[TriangleDataMesh[j].p2];
				zero3 = PositionListData[TriangleDataMesh[j].p3];
				GridMin.X = ((GridMin.X > zero.X) ? zero.X : GridMin.X);
				GridMin.Y = ((GridMin.Y > zero.Y) ? zero.Y : GridMin.Y);
				GridMin.Z = ((GridMin.Z > zero.Z) ? zero.Z : GridMin.Z);
				GridMin.X = ((GridMin.X > zero2.X) ? zero2.X : GridMin.X);
				GridMin.Y = ((GridMin.Y > zero2.Y) ? zero2.Y : GridMin.Y);
				GridMin.Z = ((GridMin.Z > zero2.Z) ? zero2.Z : GridMin.Z);
				GridMin.X = ((GridMin.X > zero3.X) ? zero3.X : GridMin.X);
				GridMin.Y = ((GridMin.Y > zero3.Y) ? zero3.Y : GridMin.Y);
				GridMin.Z = ((GridMin.Z > zero3.Z) ? zero3.Z : GridMin.Z);
				GridMax.X = ((GridMax.X < zero.X) ? zero.X : GridMax.X);
				GridMax.Y = ((GridMax.Y < zero.Y) ? zero.Y : GridMax.Y);
				GridMax.Z = ((GridMax.Z < zero.Z) ? zero.Z : GridMax.Z);
				GridMax.X = ((GridMax.X < zero2.X) ? zero2.X : GridMax.X);
				GridMax.Y = ((GridMax.Y < zero2.Y) ? zero2.Y : GridMax.Y);
				GridMax.Z = ((GridMax.Z < zero2.Z) ? zero2.Z : GridMax.Z);
				GridMax.X = ((GridMax.X < zero3.X) ? zero3.X : GridMax.X);
				GridMax.Y = ((GridMax.Y < zero3.Y) ? zero3.Y : GridMax.Y);
				GridMax.Z = ((GridMax.Z < zero3.Z) ? zero3.Z : GridMax.Z);
			}
		}
		GridSize.X = (int)(Math.Abs(GridMin.X) + GridMax.X);
		GridSize.Z = (int)(Math.Abs(GridMin.Z) + GridMax.Z);
		GridSize.Y = 0f;
		GridCellSize = 256;
		NumberGridY = 0;
		NumberGridX = (int)GridSize.X / GridCellSize + 1;
		NumberGridZ = (int)GridSize.Z / GridCellSize + 1;
		NoWalkData = new GridElement[NumberGridX][];
		WalkableData = new GridElement[NumberGridX][];
		for (int k = 0; k < NumberGridX; k++)
		{
			NoWalkData[k] = new GridElement[NumberGridZ];
			WalkableData[k] = new GridElement[NumberGridZ];
		}
		int num = 0;
		int num2 = 0;
		int[] array = new int[8192];
		int[] array2 = new int[8192];
		Vector3 min = Vector3.Zero;
		Vector3 max = Vector3.Zero;
		float num3 = 36f;
		float num4 = GridMin.X;
		while (num4 < GridMax.X)
		{
			int num5 = 0;
			float num6 = GridMin.Z;
			while (num6 < GridMax.Z)
			{
				min.X = num4;
				min.Z = num6;
				min.Y = GridMin.Y;
				max.X = num4 + (float)GridCellSize;
				max.Z = num6 + (float)GridCellSize;
				max.Y = GridMax.Y;
				ref GridElement reference2 = ref NoWalkData[num2][num5];
				reference2 = new GridElement(ref min, ref max);
				ref GridElement reference3 = ref WalkableData[num2][num5];
				reference3 = new GridElement(ref min, ref max);
				min.X -= num3;
				min.Z -= num3;
				max.X += num3;
				max.Z += num3;
				int num7 = 0;
				for (int l = 0; l < TriangleDataMesh.Length; l++)
				{
					if ((TriangleDataMesh[l].Flags & 1) != 0 && (TriangleDataMesh[l].Flags & 4) == 0 && MyMath.TestTriangleAABB(ref min, ref max, ref TriangleDataMesh[l]))
					{
						array[num7] = l;
						num7++;
						if (num7 >= 8192)
						{
							break;
						}
					}
				}
				int num8 = 0;
				for (int m = 0; m < TriangleDataMesh.Length; m++)
				{
					if ((TriangleDataMesh[m].Flags & 1) == 0 && (TriangleDataMesh[m].Flags & 8) == 0 && (TriangleDataMesh[m].Flags & 4) == 0 && MyMath.TestTriangleAABB(ref min, ref max, ref TriangleDataMesh[m]))
					{
						array2[num8] = m;
						num8++;
						if (num8 >= 8192)
						{
							break;
						}
					}
				}
				NoWalkData[num2][num5].SetIndices(array2, num8);
				WalkableData[num2][num5].SetIndices(array, num7);
				num6 += (float)GridCellSize;
				num5++;
			}
			num4 += (float)GridCellSize;
			num2++;
		}
		if (num > 0)
		{
			num = 0;
		}
	}

	public void GetGridPosition(ref int outX, ref int outZ, float inX, float inZ)
	{
		outX = (int)(Math.Abs(GridMin.X) + inX) / GridCellSize;
		outZ = (int)(Math.Abs(GridMin.Z) + inZ) / GridCellSize;
		outX = ((outX >= 0) ? outX : 0);
		outZ = ((outZ >= 0) ? outZ : 0);
		outX = ((outX < NumberGridX) ? outX : (NumberGridX - 1));
		outZ = ((outZ < NumberGridZ) ? outZ : (NumberGridZ - 1));
	}
}
