using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using a;

namespace SynapseGaming.LightingSystem.Collision.Legacy;

/// <summary>
/// Contains the complete collision mesh for an object.
/// Generally the mesh is in world-space.
/// </summary>
public class CollisionMesh
{
	/// <summary>
	/// Creates a new CollisionSurface instance.
	/// </summary>
	public class CollisionSurface : IWorldBoundingBoxObject
	{
		/// <summary />
		public Plane Surface;

		/// <summary />
		public Plane Edge0;

		/// <summary />
		public Plane Edge1;

		/// <summary />
		public Plane Edge2;

		/// <summary />
		public int VertexIndex0;

		/// <summary />
		public int VertexIndex1;

		/// <summary />
		public int VertexIndex2;

		/// <summary />
		public ICollisionMaterial Material;

		/// <summary />
		public BoundingBox WorldBoundingBox => default(BoundingBox);

		internal bool bp(CollisionMesh P_0, ref BoundingBox P_1)
		{
			a57.AccumulationValue++;
			P_1.Intersects(ref Edge0, out var result);
			if (result == PlaneIntersectionType.Back)
			{
				return false;
			}
			P_1.Intersects(ref Edge1, out var result2);
			if (result2 == PlaneIntersectionType.Back)
			{
				return false;
			}
			P_1.Intersects(ref Edge2, out var result3);
			if (result3 == PlaneIntersectionType.Back)
			{
				return false;
			}
			bool flag = result == PlaneIntersectionType.Intersecting;
			bool flag2 = result2 == PlaneIntersectionType.Intersecting;
			bool flag3 = result3 == PlaneIntersectionType.Intersecting;
			if (flag || flag2 || flag3)
			{
				a5_0006.AccumulationValue++;
			}
			return true;
		}

		internal bool bp(CollisionMesh P_0, ref Vector3 P_1, ref BoundingSphere P_2, float P_3)
		{
			a57.AccumulationValue++;
			float num = P_1.X * Edge0.Normal.X + P_1.Y * Edge0.Normal.Y + P_1.Z * Edge0.Normal.Z + Edge0.D;
			if (num < P_3)
			{
				return false;
			}
			float num2 = P_1.X * Edge1.Normal.X + P_1.Y * Edge1.Normal.Y + P_1.Z * Edge1.Normal.Z + Edge1.D;
			if (num2 < P_3)
			{
				return false;
			}
			float num3 = P_1.X * Edge2.Normal.X + P_1.Y * Edge2.Normal.Y + P_1.Z * Edge2.Normal.Z + Edge2.D;
			if (num3 < P_3)
			{
				return false;
			}
			bool flag = num < 0f;
			bool flag2 = num2 < 0f;
			bool flag3 = num3 < 0f;
			if (flag || flag2 || flag3)
			{
				a5_0006.AccumulationValue++;
				Vector3 vector = P_0.a5b[VertexIndex0];
				Vector3 vector2 = P_0.a5b[VertexIndex1];
				if (flag && !b_0014(ref vector, ref vector2, ref P_2))
				{
					return false;
				}
				Vector3 vector3 = P_0.a5b[VertexIndex2];
				if (flag2 && !b_0014(ref vector2, ref vector3, ref P_2))
				{
					return false;
				}
				if (flag3 && !b_0014(ref vector3, ref vector, ref P_2))
				{
					return false;
				}
			}
			return true;
		}

		private bool b_0014(ref Vector3 P_0, ref Vector3 P_1, ref BoundingSphere P_2)
		{
			Vector3 vector = P_1 - P_0;
			Vector3 vector2 = P_2.Center - P_0;
			float value = Vector3.Dot(vector, vector2) / Vector3.Dot(vector, vector);
			value = MathHelper.Clamp(value, 0f, 1f);
			Vector3 value2 = vector * value + P_0;
			float num = Vector3.DistanceSquared(P_2.Center, value2);
			return num <= P_2.Radius * P_2.Radius;
		}
	}

	private int[] a5h;

	private Vector3[] a5b;

	private List<CollisionSurface> a56 = new List<CollisionSurface>();

	internal a.b<CollisionSurface> a5a = new a.b<CollisionSurface>();

	private static SystemStatistic a57 = SystemConsole.GetStatistic("Collision_PolyEdgeTestCount", SystemStatisticCategory.Collision);

	private static SystemStatistic a5_0006 = SystemConsole.GetStatistic("Collision_PolyEdgeRayCastTestCount", SystemStatisticCategory.Collision);

	/// <summary>
	/// List of the geometry's indices.
	/// </summary>
	public int[] Indices
	{
		get
		{
			return a5h;
		}
		set
		{
			a5h = value;
		}
	}

	/// <summary>
	/// List of the geometry's vertices.
	/// </summary>
	public Vector3[] Vertices
	{
		get
		{
			return a5b;
		}
		set
		{
			a5b = value;
		}
	}

	/// <summary>
	/// List of the geometry's surfaces.
	/// </summary>
	public List<CollisionSurface> Surfaces
	{
		get
		{
			return a56;
		}
		set
		{
			a56 = value;
		}
	}

	internal void bD()
	{
		if (a5b == null || a5b.Length <= 0)
		{
			return;
		}
		int num = Math.Max(Surfaces.Count / 100, 1);
		BoundingBox boundingBox = new BoundingBox(Vertices[0], Vertices[0]);
		Vector3[] array = a5b;
		foreach (Vector3 value in array)
		{
			boundingBox.Max = Vector3.Max(boundingBox.Max, value);
			boundingBox.Min = Vector3.Min(boundingBox.Min, value);
		}
		a5a.T();
		a5a.h(ref boundingBox, num);
		foreach (CollisionSurface item in a56)
		{
			Vector3 value2 = a5b[item.VertexIndex0];
			Vector3 value3 = a5b[item.VertexIndex1];
			Vector3 value4 = a5b[item.VertexIndex2];
			boundingBox.Max = Vector3.Max(Vector3.Max(value2, value3), value4);
			boundingBox.Min = Vector3.Min(Vector3.Min(value2, value3), value4);
			a5a.E(boundingBox, item);
		}
	}
}
