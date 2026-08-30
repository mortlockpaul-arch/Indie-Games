using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public static class Collision
{
	private enum EPAxisType
	{
		Unknown,
		EdgeA,
		EdgeB
	}

	private struct EPAxis
	{
		public int Index;

		public float Separation;

		public EPAxisType Type;
	}

	private static PolygonShape s_polygonA = new PolygonShape();

	private static PolygonShape s_polygonB = new PolygonShape();

	public static void GetPointStates(out FixedArray2<PointState> state1, out FixedArray2<PointState> state2, ref Manifold manifold1, ref Manifold manifold2)
	{
		state1 = default(FixedArray2<PointState>);
		state2 = default(FixedArray2<PointState>);
		for (int i = 0; i < manifold1.PointCount; i++)
		{
			ContactID id = manifold1.Points[i].Id;
			state1[i] = PointState.Remove;
			for (int j = 0; j < manifold2.PointCount; j++)
			{
				if (manifold2.Points[j].Id.Key == id.Key)
				{
					state1[i] = PointState.Persist;
					break;
				}
			}
		}
		for (int k = 0; k < manifold2.PointCount; k++)
		{
			ContactID id2 = manifold2.Points[k].Id;
			state2[k] = PointState.Add;
			for (int l = 0; l < manifold1.PointCount; l++)
			{
				if (manifold1.Points[l].Id.Key == id2.Key)
				{
					state2[k] = PointState.Persist;
					break;
				}
			}
		}
	}

	public static void CollideCircles(ref Manifold manifold, CircleShape circleA, ref Transform xfA, CircleShape circleB, ref Transform xfB)
	{
		manifold.PointCount = 0;
		Vector2 vector = MathUtils.Multiply(ref xfA, circleA.Position);
		Vector2 vector2 = MathUtils.Multiply(ref xfB, circleB.Position);
		Vector2 vector3 = vector2 - vector;
		float num = Vector2.Dot(vector3, vector3);
		float radius = circleA.Radius;
		float radius2 = circleB.Radius;
		float num2 = radius + radius2;
		if (!(num > num2 * num2))
		{
			manifold.Type = ManifoldType.Circles;
			manifold.LocalPoint = circleA.Position;
			manifold.LocalNormal = Vector2.Zero;
			manifold.PointCount = 1;
			ManifoldPoint value = manifold.Points[0];
			value.LocalPoint = circleB.Position;
			value.Id.Key = 0u;
			manifold.Points[0] = value;
		}
	}

	public static void CollidePolygonAndCircle(ref Manifold manifold, PolygonShape polygonA, ref Transform transformA, CircleShape circleB, ref Transform transformB)
	{
		manifold.PointCount = 0;
		Vector2 v = MathUtils.Multiply(ref transformB, circleB.Position);
		Vector2 vector = MathUtils.MultiplyT(ref transformA, v);
		int num = 0;
		float num2 = float.MinValue;
		float num3 = polygonA.Radius + circleB.Radius;
		int count = polygonA.Vertices.Count;
		for (int i = 0; i < count; i++)
		{
			float num4 = Vector2.Dot(polygonA.Normals[i], vector - polygonA.Vertices[i]);
			if (num4 > num3)
			{
				return;
			}
			if (num4 > num2)
			{
				num2 = num4;
				num = i;
			}
		}
		int num5 = num;
		int index = ((num5 + 1 < count) ? (num5 + 1) : 0);
		Vector2 vector2 = polygonA.Vertices[num5];
		Vector2 vector3 = polygonA.Vertices[index];
		if (num2 < 1.1920929E-07f)
		{
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.FaceA;
			manifold.LocalNormal = polygonA.Normals[num];
			manifold.LocalPoint = 0.5f * (vector2 + vector3);
			ManifoldPoint value = manifold.Points[0];
			value.LocalPoint = circleB.Position;
			value.Id.Key = 0u;
			manifold.Points[0] = value;
			return;
		}
		float num6 = Vector2.Dot(vector - vector2, vector3 - vector2);
		float num7 = Vector2.Dot(vector - vector3, vector2 - vector3);
		if (num6 <= 0f)
		{
			if (!(Vector2.DistanceSquared(vector, vector2) > num3 * num3))
			{
				manifold.PointCount = 1;
				manifold.Type = ManifoldType.FaceA;
				manifold.LocalNormal = vector - vector2;
				manifold.LocalNormal.Normalize();
				manifold.LocalPoint = vector2;
				ManifoldPoint value2 = manifold.Points[0];
				value2.LocalPoint = circleB.Position;
				value2.Id.Key = 0u;
				manifold.Points[0] = value2;
			}
			return;
		}
		if (num7 <= 0f)
		{
			if (!(Vector2.DistanceSquared(vector, vector3) > num3 * num3))
			{
				manifold.PointCount = 1;
				manifold.Type = ManifoldType.FaceA;
				manifold.LocalNormal = vector - vector3;
				manifold.LocalNormal.Normalize();
				manifold.LocalPoint = vector3;
				ManifoldPoint value3 = manifold.Points[0];
				value3.LocalPoint = circleB.Position;
				value3.Id.Key = 0u;
				manifold.Points[0] = value3;
			}
			return;
		}
		Vector2 vector4 = 0.5f * (vector2 + vector3);
		float num8 = Vector2.Dot(vector - vector4, polygonA.Normals[num5]);
		if (!(num8 > num3))
		{
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.FaceA;
			manifold.LocalNormal = polygonA.Normals[num5];
			manifold.LocalPoint = vector4;
			ManifoldPoint value4 = manifold.Points[0];
			value4.LocalPoint = circleB.Position;
			value4.Id.Key = 0u;
			manifold.Points[0] = value4;
		}
	}

	public static void CollidePolygons(ref Manifold manifold, PolygonShape polyA, ref Transform transformA, PolygonShape polyB, ref Transform transformB)
	{
		manifold.PointCount = 0;
		float num = polyA.Radius + polyB.Radius;
		int edgeIndex = 0;
		float num2 = FindMaxSeparation(out edgeIndex, polyA, ref transformA, polyB, ref transformB);
		if (num2 > num)
		{
			return;
		}
		int edgeIndex2 = 0;
		float num3 = FindMaxSeparation(out edgeIndex2, polyB, ref transformB, polyA, ref transformA);
		if (num3 > num)
		{
			return;
		}
		PolygonShape polygonShape;
		PolygonShape poly;
		Transform xf;
		Transform xf2;
		int num4;
		bool flag;
		if (num3 > 0.98f * num2 + 0.001f)
		{
			polygonShape = polyB;
			poly = polyA;
			xf = transformB;
			xf2 = transformA;
			num4 = edgeIndex2;
			manifold.Type = ManifoldType.FaceB;
			flag = true;
		}
		else
		{
			polygonShape = polyA;
			poly = polyB;
			xf = transformA;
			xf2 = transformB;
			num4 = edgeIndex;
			manifold.Type = ManifoldType.FaceA;
			flag = false;
		}
		FindIncidentEdge(out var c, polygonShape, ref xf, num4, poly, ref xf2);
		int count = polygonShape.Vertices.Count;
		int num5 = num4;
		int num6 = ((num4 + 1 < count) ? (num4 + 1) : 0);
		Vector2 vector = polygonShape.Vertices[num5];
		Vector2 vector2 = polygonShape.Vertices[num6];
		Vector2 vector3 = vector2 - vector;
		vector3.Normalize();
		Vector2 localNormal = MathUtils.Cross(vector3, 1f);
		Vector2 localPoint = 0.5f * (vector + vector2);
		Vector2 vector4 = MathUtils.Multiply(ref xf.R, vector3);
		Vector2 value = MathUtils.Cross(vector4, 1f);
		vector = MathUtils.Multiply(ref xf, vector);
		vector2 = MathUtils.Multiply(ref xf, vector2);
		float num7 = Vector2.Dot(value, vector);
		float offset = 0f - Vector2.Dot(vector4, vector) + num;
		float offset2 = Vector2.Dot(vector4, vector2) + num;
		int num8 = ClipSegmentToLine(out var vOut, ref c, -vector4, offset, num5);
		if (num8 < 2)
		{
			return;
		}
		num8 = ClipSegmentToLine(out var vOut2, ref vOut, vector4, offset2, num6);
		if (num8 < 2)
		{
			return;
		}
		manifold.LocalNormal = localNormal;
		manifold.LocalPoint = localPoint;
		int num9 = 0;
		for (int i = 0; i < 2; i++)
		{
			float num10 = Vector2.Dot(value, vOut2[i].V) - num7;
			if (num10 <= num)
			{
				ManifoldPoint value2 = manifold.Points[num9];
				value2.LocalPoint = MathUtils.MultiplyT(ref xf2, vOut2[i].V);
				value2.Id = vOut2[i].ID;
				if (flag)
				{
					ContactFeature features = value2.Id.Features;
					value2.Id.Features.IndexA = features.IndexB;
					value2.Id.Features.IndexB = features.IndexA;
					value2.Id.Features.TypeA = features.TypeB;
					value2.Id.Features.TypeB = features.TypeA;
				}
				manifold.Points[num9] = value2;
				num9++;
			}
		}
		manifold.PointCount = num9;
	}

	public static void CollideEdgeAndCircle(ref Manifold manifold, EdgeShape edgeA, ref Transform transformA, CircleShape circleB, ref Transform transformB)
	{
		manifold.PointCount = 0;
		Vector2 vector = MathUtils.MultiplyT(ref transformA, MathUtils.Multiply(ref transformB, circleB.Position));
		Vector2 vertex = edgeA.Vertex1;
		Vector2 vertex2 = edgeA.Vertex2;
		Vector2 vector2 = vertex2 - vertex;
		float num = Vector2.Dot(vector2, vertex2 - vector);
		float num2 = Vector2.Dot(vector2, vector - vertex);
		float num3 = edgeA.Radius + circleB.Radius;
		ContactFeature features = default(ContactFeature);
		features.IndexB = 0;
		features.TypeB = 0;
		Vector2 vector3;
		Vector2 vector4;
		if (num2 <= 0f)
		{
			vector3 = vertex;
			vector4 = vector - vector3;
			float num4 = Vector2.Dot(vector4, vector4);
			if (num4 > num3 * num3)
			{
				return;
			}
			if (edgeA.HasVertex0)
			{
				Vector2 vertex3 = edgeA.Vertex0;
				Vector2 vector5 = vertex;
				Vector2 value = vector5 - vertex3;
				float num5 = Vector2.Dot(value, vector5 - vector);
				if (num5 > 0f)
				{
					return;
				}
			}
			features.IndexA = 0;
			features.TypeA = 0;
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.Circles;
			manifold.LocalNormal = Vector2.Zero;
			manifold.LocalPoint = vector3;
			ManifoldPoint value2 = new ManifoldPoint
			{
				Id = 
				{
					Key = 0u,
					Features = features
				},
				LocalPoint = circleB.Position
			};
			manifold.Points[0] = value2;
			return;
		}
		if (num <= 0f)
		{
			vector3 = vertex2;
			vector4 = vector - vector3;
			float num6 = Vector2.Dot(vector4, vector4);
			if (num6 > num3 * num3)
			{
				return;
			}
			if (edgeA.HasVertex3)
			{
				Vector2 vertex4 = edgeA.Vertex3;
				Vector2 vector6 = vertex2;
				Vector2 value3 = vertex4 - vector6;
				float num7 = Vector2.Dot(value3, vector - vector6);
				if (num7 > 0f)
				{
					return;
				}
			}
			features.IndexA = 1;
			features.TypeA = 0;
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.Circles;
			manifold.LocalNormal = Vector2.Zero;
			manifold.LocalPoint = vector3;
			ManifoldPoint value4 = new ManifoldPoint
			{
				Id = 
				{
					Key = 0u,
					Features = features
				},
				LocalPoint = circleB.Position
			};
			manifold.Points[0] = value4;
			return;
		}
		float num8 = Vector2.Dot(vector2, vector2);
		vector3 = 1f / num8 * (num * vertex + num2 * vertex2);
		vector4 = vector - vector3;
		float num9 = Vector2.Dot(vector4, vector4);
		if (!(num9 > num3 * num3))
		{
			Vector2 vector7 = new Vector2(0f - vector2.Y, vector2.X);
			if (Vector2.Dot(vector7, vector - vertex) < 0f)
			{
				vector7 = new Vector2(0f - vector7.X, 0f - vector7.Y);
			}
			vector7.Normalize();
			features.IndexA = 0;
			features.TypeA = 1;
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.FaceA;
			manifold.LocalNormal = vector7;
			manifold.LocalPoint = vertex;
			ManifoldPoint value5 = new ManifoldPoint
			{
				Id = 
				{
					Key = 0u,
					Features = features
				},
				LocalPoint = circleB.Position
			};
			manifold.Points[0] = value5;
		}
	}

	private static void ComputeEdgeSeperation(ref Vector2 v1, ref Vector2 n, PolygonShape polygonB, out EPAxis axis)
	{
		axis.Type = EPAxisType.EdgeA;
		axis.Index = 0;
		axis.Separation = Vector2.Dot(n, polygonB.Vertices[0] - v1);
		for (int i = 1; i < polygonB.Vertices.Count; i++)
		{
			float num = Vector2.Dot(n, polygonB.Vertices[i] - v1);
			if (num < axis.Separation)
			{
				axis.Separation = num;
			}
		}
	}

	private static void ComputePolygonSeperation(ref Vector2 v1, ref Vector2 v2, PolygonShape polygonB, float radius, out EPAxis axis)
	{
		axis.Type = EPAxisType.EdgeB;
		axis.Index = 0;
		axis.Separation = float.MinValue;
		for (int i = 0; i < polygonB.Vertices.Count; i++)
		{
			float val = Vector2.Dot(polygonB.Normals[i], v1 - polygonB.Vertices[i]);
			float val2 = Vector2.Dot(polygonB.Normals[i], v2 - polygonB.Vertices[i]);
			float num = Math.Min(val, val2);
			if (num > axis.Separation)
			{
				axis.Index = i;
				axis.Separation = num;
				if (num > radius)
				{
					break;
				}
			}
		}
	}

	private static void FindIncidentEdge(ref FixedArray2<ClipVertex> c, PolygonShape poly1, int edge1, PolygonShape poly2)
	{
		_ = poly1.Vertices.Count;
		int count = poly2.Vertices.Count;
		Vector2 value = poly1.Normals[edge1];
		int num = 0;
		float num2 = float.MaxValue;
		for (int i = 0; i < count; i++)
		{
			float num3 = Vector2.Dot(value, poly2.Normals[i]);
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
			}
		}
		int num4 = num;
		int num5 = ((num4 + 1 < count) ? (num4 + 1) : 0);
		ClipVertex value2 = (c[0] = new ClipVertex
		{
			V = poly2.Vertices[num4],
			ID = 
			{
				Features = 
				{
					IndexA = (byte)edge1,
					IndexB = (byte)num4,
					TypeA = 1,
					TypeB = 0
				}
			}
		});
		value2.V = poly2.Vertices[num5];
		value2.ID.Features.IndexA = (byte)edge1;
		value2.ID.Features.IndexB = (byte)num5;
		value2.ID.Features.TypeA = 1;
		value2.ID.Features.TypeB = 0;
		c[1] = value2;
	}

	public static void CollideEdgeAndPolygon(ref Manifold manifold, EdgeShape edgeA, ref Transform xfA, PolygonShape polygonB_in, ref Transform xfB)
	{
		manifold.PointCount = 0;
		MathUtils.MultiplyT(ref xfA, ref xfB, out var C);
		s_polygonA.SetAsEdge(edgeA.Vertex1, edgeA.Vertex2);
		s_polygonB.Radius = polygonB_in.Radius;
		s_polygonB.Centroid = MathUtils.Multiply(ref C, polygonB_in.Centroid);
		s_polygonB.Vertices = new Vertices(polygonB_in.Vertices.Count);
		s_polygonB.Normals = new Vertices(polygonB_in.Vertices.Count);
		for (int i = 0; i < polygonB_in.Vertices.Count; i++)
		{
			s_polygonB.Vertices.Add(MathUtils.Multiply(ref C, polygonB_in.Vertices[i]));
			s_polygonB.Normals.Add(MathUtils.Multiply(ref C.R, polygonB_in.Normals[i]));
		}
		float num = s_polygonA.Radius + s_polygonB.Radius;
		Vector2 v = edgeA.Vertex1;
		Vector2 v2 = edgeA.Vertex2;
		Vector2 vector = v2 - v;
		Vector2 n = new Vector2(vector.Y, 0f - vector.X);
		n.Normalize();
		bool flag = Vector2.Dot(n, s_polygonB.Centroid - v) >= 0f;
		if (!flag)
		{
			n = -n;
		}
		ComputeEdgeSeperation(ref v, ref n, s_polygonB, out var axis);
		if (axis.Separation > num)
		{
			return;
		}
		FixedArray2<EdgeType> fixedArray = default(FixedArray2<EdgeType>);
		if (edgeA.HasVertex0)
		{
			Vector2 vertex = edgeA.Vertex0;
			float num2 = Vector2.Dot(n, vertex - v);
			if (num2 > 0.0005f)
			{
				fixedArray[0] = EdgeType.Concave;
			}
			else if (num2 >= -0.0005f)
			{
				fixedArray[0] = EdgeType.Flat;
			}
			else
			{
				fixedArray[0] = EdgeType.Convex;
			}
		}
		if (edgeA.HasVertex3)
		{
			Vector2 vertex2 = edgeA.Vertex3;
			float num3 = Vector2.Dot(n, vertex2 - v2);
			if (num3 > 0.0005f)
			{
				fixedArray[1] = EdgeType.Concave;
			}
			else if (num3 >= -0.0005f)
			{
				fixedArray[1] = EdgeType.Flat;
			}
			else
			{
				fixedArray[1] = EdgeType.Convex;
			}
		}
		if (fixedArray[0] == EdgeType.Convex)
		{
			Vector2 v3 = edgeA.Vertex0;
			Vector2 vector2 = v - v3;
			Vector2 n2 = new Vector2(vector2.Y, 0f - vector2.X);
			n2.Normalize();
			if (!flag)
			{
				n2 = -n2;
			}
			ComputeEdgeSeperation(ref v3, ref n2, s_polygonB, out var axis2);
			if (axis2.Separation > axis.Separation)
			{
				return;
			}
		}
		if (fixedArray[1] == EdgeType.Convex)
		{
			Vector2 vertex3 = edgeA.Vertex3;
			Vector2 vector3 = vertex3 - v2;
			Vector2 n3 = new Vector2(vector3.Y, 0f - vector3.X);
			n3.Normalize();
			if (!flag)
			{
				n3 = -n3;
			}
			ComputeEdgeSeperation(ref v2, ref n3, s_polygonB, out var axis3);
			if (axis3.Separation > axis.Separation)
			{
				return;
			}
		}
		ComputePolygonSeperation(ref v, ref v2, s_polygonB, num, out var axis4);
		if (axis4.Separation > num)
		{
			return;
		}
		EPAxis ePAxis = ((!(axis4.Separation > 0.98f * axis.Separation + 0.001f)) ? axis : axis4);
		PolygonShape polygonShape;
		PolygonShape poly;
		if (ePAxis.Type == EPAxisType.EdgeA)
		{
			polygonShape = s_polygonA;
			poly = s_polygonB;
			if (!flag)
			{
				ePAxis.Index = 1;
			}
			manifold.Type = ManifoldType.FaceA;
		}
		else
		{
			polygonShape = s_polygonB;
			poly = s_polygonA;
			manifold.Type = ManifoldType.FaceB;
		}
		int index = ePAxis.Index;
		FixedArray2<ClipVertex> c = default(FixedArray2<ClipVertex>);
		FindIncidentEdge(ref c, polygonShape, ePAxis.Index, poly);
		int count = polygonShape.Vertices.Count;
		int num4 = index;
		int num5 = ((index + 1 < count) ? (index + 1) : 0);
		Vector2 vector4 = polygonShape.Vertices[num4];
		Vector2 vector5 = polygonShape.Vertices[num5];
		Vector2 vector6 = vector5 - vector4;
		vector6.Normalize();
		Vector2 vector7 = MathUtils.Cross(vector6, 1f);
		Vector2 vector8 = 0.5f * (vector4 + vector5);
		float num6 = Vector2.Dot(vector7, vector4);
		float offset = 0f - Vector2.Dot(vector6, vector4) + num;
		float offset2 = Vector2.Dot(vector6, vector5) + num;
		int num7 = ClipSegmentToLine(out var vOut, ref c, -vector6, offset, num4);
		if (num7 < 2)
		{
			return;
		}
		num7 = ClipSegmentToLine(out var vOut2, ref vOut, vector6, offset2, num5);
		if (num7 < 2)
		{
			return;
		}
		if (ePAxis.Type == EPAxisType.EdgeA)
		{
			manifold.LocalNormal = vector7;
			manifold.LocalPoint = vector8;
		}
		else
		{
			manifold.LocalNormal = MathUtils.MultiplyT(ref C.R, vector7);
			manifold.LocalPoint = MathUtils.MultiplyT(ref C, vector8);
		}
		int num8 = 0;
		for (int j = 0; j < 2; j++)
		{
			float num9 = Vector2.Dot(vector7, vOut2[j].V) - num6;
			if (num9 <= num)
			{
				ManifoldPoint value = manifold.Points[num8];
				if (ePAxis.Type == EPAxisType.EdgeA)
				{
					value.LocalPoint = MathUtils.MultiplyT(ref C, vOut2[j].V);
					value.Id = vOut2[j].ID;
				}
				else
				{
					value.LocalPoint = vOut2[j].V;
					value.Id.Features.TypeA = vOut2[j].ID.Features.TypeB;
					value.Id.Features.TypeB = vOut2[j].ID.Features.TypeA;
					value.Id.Features.IndexA = vOut2[j].ID.Features.IndexB;
					value.Id.Features.IndexB = vOut2[j].ID.Features.IndexA;
				}
				manifold.Points[num8] = value;
				if (value.Id.Features.TypeA != 0 || fixedArray[value.Id.Features.IndexA] != EdgeType.Flat)
				{
					num8++;
				}
			}
		}
		manifold.PointCount = num8;
	}

	public static int ClipSegmentToLine(out FixedArray2<ClipVertex> vOut, ref FixedArray2<ClipVertex> vIn, Vector2 normal, float offset, int vertexIndexA)
	{
		vOut = default(FixedArray2<ClipVertex>);
		int num = 0;
		float num2 = Vector2.Dot(normal, vIn[0].V) - offset;
		float num3 = Vector2.Dot(normal, vIn[1].V) - offset;
		if (num2 <= 0f)
		{
			vOut[num++] = vIn[0];
		}
		if (num3 <= 0f)
		{
			vOut[num++] = vIn[1];
		}
		if (num2 * num3 < 0f)
		{
			float num4 = num2 / (num2 - num3);
			ClipVertex value = vOut[num];
			value.V = vIn[0].V + num4 * (vIn[1].V - vIn[0].V);
			value.ID.Features.IndexA = (byte)vertexIndexA;
			value.ID.Features.IndexB = vIn[0].ID.Features.IndexB;
			value.ID.Features.TypeA = 0;
			value.ID.Features.TypeB = 1;
			vOut[num] = value;
			num++;
		}
		return num;
	}

	private static float EdgeSeparation(PolygonShape poly1, ref Transform xf1, int edge1, PolygonShape poly2, ref Transform xf2)
	{
		_ = poly1.Vertices.Count;
		int count = poly2.Vertices.Count;
		Vector2 vector = poly1.Normals[edge1];
		Vector2 value = new Vector2(xf1.R.col1.X * vector.X + xf1.R.col2.X * vector.Y, xf1.R.col1.Y * vector.X + xf1.R.col2.Y * vector.Y);
		Vector2 value2 = new Vector2(value.X * xf2.R.col1.X + value.Y * xf2.R.col1.Y, value.X * xf2.R.col2.X + value.Y * xf2.R.col2.Y);
		int index = 0;
		float num = float.MaxValue;
		for (int i = 0; i < count; i++)
		{
			float num2 = Vector2.Dot(poly2.Vertices[i], value2);
			if (num2 < num)
			{
				num = num2;
				index = i;
			}
		}
		Vector2 vector2 = poly1.Vertices[edge1];
		Vector2 vector3 = poly2.Vertices[index];
		Vector2 vector4 = new Vector2(xf1.Position.X + xf1.R.col1.X * vector2.X + xf1.R.col2.X * vector2.Y, xf1.Position.Y + xf1.R.col1.Y * vector2.X + xf1.R.col2.Y * vector2.Y);
		Vector2 vector5 = new Vector2(xf2.Position.X + xf2.R.col1.X * vector3.X + xf2.R.col2.X * vector3.Y, xf2.Position.Y + xf2.R.col1.Y * vector3.X + xf2.R.col2.Y * vector3.Y);
		return Vector2.Dot(vector5 - vector4, value);
	}

	private static float FindMaxSeparation(out int edgeIndex, PolygonShape poly1, ref Transform xf1, PolygonShape poly2, ref Transform xf2)
	{
		edgeIndex = -1;
		int count = poly1.Vertices.Count;
		Vector2 v = MathUtils.Multiply(ref xf2, poly2.Centroid) - MathUtils.Multiply(ref xf1, poly1.Centroid);
		Vector2 value = MathUtils.MultiplyT(ref xf1.R, v);
		int num = 0;
		float num2 = float.MinValue;
		for (int i = 0; i < count; i++)
		{
			float num3 = Vector2.Dot(poly1.Normals[i], value);
			if (num3 > num2)
			{
				num2 = num3;
				num = i;
			}
		}
		float num4 = EdgeSeparation(poly1, ref xf1, num, poly2, ref xf2);
		int num5 = ((num - 1 >= 0) ? (num - 1) : (count - 1));
		float num6 = EdgeSeparation(poly1, ref xf1, num5, poly2, ref xf2);
		int num7 = ((num + 1 < count) ? (num + 1) : 0);
		float num8 = EdgeSeparation(poly1, ref xf1, num7, poly2, ref xf2);
		int num9;
		int num10;
		float num11;
		if (num6 > num4 && num6 > num8)
		{
			num9 = -1;
			num10 = num5;
			num11 = num6;
		}
		else
		{
			if (!(num8 > num4))
			{
				edgeIndex = num;
				return num4;
			}
			num9 = 1;
			num10 = num7;
			num11 = num8;
		}
		while (true)
		{
			num = ((num9 != -1) ? ((num10 + 1 < count) ? (num10 + 1) : 0) : ((num10 - 1 >= 0) ? (num10 - 1) : (count - 1)));
			num4 = EdgeSeparation(poly1, ref xf1, num, poly2, ref xf2);
			if (!(num4 > num11))
			{
				break;
			}
			num10 = num;
			num11 = num4;
		}
		edgeIndex = num10;
		return num11;
	}

	private static void FindIncidentEdge(out FixedArray2<ClipVertex> c, PolygonShape poly1, ref Transform xf1, int edge1, PolygonShape poly2, ref Transform xf2)
	{
		c = default(FixedArray2<ClipVertex>);
		_ = poly1.Vertices.Count;
		int count = poly2.Vertices.Count;
		Vector2 value = MathUtils.MultiplyT(ref xf2.R, MathUtils.Multiply(ref xf1.R, poly1.Normals[edge1]));
		int num = 0;
		float num2 = float.MaxValue;
		for (int i = 0; i < count; i++)
		{
			float num3 = Vector2.Dot(value, poly2.Normals[i]);
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
			}
		}
		int num4 = num;
		int num5 = ((num4 + 1 < count) ? (num4 + 1) : 0);
		ClipVertex value2 = c[0];
		value2.V = MathUtils.Multiply(ref xf2, poly2.Vertices[num4]);
		value2.ID.Features.IndexA = (byte)edge1;
		value2.ID.Features.IndexB = (byte)num4;
		value2.ID.Features.TypeA = 1;
		value2.ID.Features.TypeB = 0;
		c[0] = value2;
		ClipVertex value3 = c[1];
		value3.V = MathUtils.Multiply(ref xf2, poly2.Vertices[num5]);
		value3.ID.Features.IndexA = (byte)edge1;
		value3.ID.Features.IndexB = (byte)num5;
		value3.ID.Features.TypeA = 1;
		value3.ID.Features.TypeB = 0;
		c[1] = value3;
	}
}
