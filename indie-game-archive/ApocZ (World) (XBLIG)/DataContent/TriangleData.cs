using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.TriangleData, DataContent")]
public struct TriangleData
{
	public const uint FL_Clear = 0u;

	public const uint FL_Walkable = 1u;

	public const uint FL_NoWalk = 2u;

	public const uint FL_LOD = 4u;

	public const uint FL_WalkOnly = 8u;

	public const uint FL_Pathable = 16u;

	public const uint FL_Metal = 65536u;

	public const uint FL_Wood = 131072u;

	public const uint FL_Rock = 262144u;

	public const uint FL_Brick = 524288u;

	public const uint FL_Glass = 1048576u;

	public const uint FL_Concrete = 2097152u;

	public const uint FL_Clothe = 4194304u;

	public const uint FL_Terrian = 8388608u;

	public const uint FL_CollectableStar = 16777216u;

	public const uint FL_AcrobatHoop = 33554432u;

	public const uint FL_WaterBob = 1073741824u;

	public const uint FL_SurfaceFloat = 2147483648u;

	public uint Flags;

	public int p1;

	public int p2;

	public int p3;

	public float Distance;

	public float DistEdge1;

	public float DistEdge2;

	public Vector3 Normal;

	public Vector3 NormalEdge1;

	public Vector3 NormalEdge2;
}
