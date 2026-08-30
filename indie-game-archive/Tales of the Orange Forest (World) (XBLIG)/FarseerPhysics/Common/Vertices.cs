using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public class Vertices : List<Vector2>
{
	private class PolyNode
	{
		private const int MaxConnected = 32;

		public PolyNode[] Connected = new PolyNode[32];

		public int NConnected;

		public Vector2 Position;

		public PolyNode(Vector2 pos)
		{
			Position = pos;
			NConnected = 0;
		}

		private bool IsRighter(float sinA, float cosA, float sinB, float cosB)
		{
			if (sinA < 0f)
			{
				if (sinB > 0f || cosA <= cosB)
				{
					return true;
				}
				return false;
			}
			if (sinB < 0f || cosA <= cosB)
			{
				return false;
			}
			return true;
		}

		public void AddConnection(PolyNode toMe)
		{
			for (int i = 0; i < NConnected; i++)
			{
				if (Connected[i] == toMe)
				{
					return;
				}
			}
			Connected[NConnected] = toMe;
			NConnected++;
		}

		public void RemoveConnection(PolyNode fromMe)
		{
			int num = -1;
			for (int i = 0; i < NConnected; i++)
			{
				if (fromMe == Connected[i])
				{
					num = i;
					break;
				}
			}
			NConnected--;
			for (int j = num; j < NConnected; j++)
			{
				Connected[j] = Connected[j + 1];
			}
		}

		public PolyNode GetRightestConnection(PolyNode incoming)
		{
			_ = NConnected;
			if (NConnected == 1)
			{
				return incoming;
			}
			Vector2 vector = Position - incoming.Position;
			vector.Length();
			vector.Normalize();
			PolyNode polyNode = null;
			for (int i = 0; i < NConnected; i++)
			{
				if (Connected[i] == incoming)
				{
					continue;
				}
				Vector2 vector2 = Connected[i].Position - Position;
				vector2.LengthSquared();
				vector2.Normalize();
				float cosA = Vector2.Dot(vector, vector2);
				float sinA = MathUtils.Cross(vector, vector2);
				if (polyNode != null)
				{
					Vector2 vector3 = polyNode.Position - Position;
					vector3.Normalize();
					float cosB = Vector2.Dot(vector, vector3);
					float sinB = MathUtils.Cross(vector, vector3);
					if (IsRighter(sinA, cosA, sinB, cosB))
					{
						polyNode = Connected[i];
					}
				}
				else
				{
					polyNode = Connected[i];
				}
			}
			return polyNode;
		}

		public PolyNode GetRightestConnection(Vector2 incomingDir)
		{
			Vector2 pos = Position - incomingDir;
			PolyNode incoming = new PolyNode(pos);
			return GetRightestConnection(incoming);
		}
	}

	public Vertices()
	{
	}

	public Vertices(int capacity)
	{
		base.Capacity = capacity;
	}

	public Vertices(Vector2[] vector2)
	{
		for (int i = 0; i < vector2.Length; i++)
		{
			Add(vector2[i]);
		}
	}

	public Vertices(IList<Vector2> vertices)
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			Add(vertices[i]);
		}
	}

	public int NextIndex(int index)
	{
		if (index == base.Count - 1)
		{
			return 0;
		}
		return index + 1;
	}

	public int PreviousIndex(int index)
	{
		if (index == 0)
		{
			return base.Count - 1;
		}
		return index - 1;
	}

	public float GetSignedArea()
	{
		float num = 0f;
		for (int i = 0; i < base.Count; i++)
		{
			int index = (i + 1) % base.Count;
			num += base[i].X * base[index].Y;
			num -= base[i].Y * base[index].X;
		}
		return num / 2f;
	}

	public float GetArea()
	{
		float num = 0f;
		for (int i = 0; i < base.Count; i++)
		{
			int index = (i + 1) % base.Count;
			num += base[i].X * base[index].Y;
			num -= base[i].Y * base[index].X;
		}
		num /= 2f;
		if (!(num < 0f))
		{
			return num;
		}
		return 0f - num;
	}

	public Vector2 GetCentroid()
	{
		Vector2 zero = Vector2.Zero;
		float num = 0f;
		Vector2 vector = new Vector2(0f, 0f);
		for (int i = 0; i < base.Count; i++)
		{
			Vector2 vector2 = vector;
			Vector2 vector3 = base[i];
			Vector2 vector4 = ((i + 1 < base.Count) ? base[i + 1] : base[0]);
			Vector2 a = vector3 - vector2;
			Vector2 b = vector4 - vector2;
			float num2 = MathUtils.Cross(a, b);
			float num3 = 0.5f * num2;
			num += num3;
			zero += num3 * (1f / 3f) * (vector2 + vector3 + vector4);
		}
		return zero * (1f / num);
	}

	public float GetRadius()
	{
		float signedArea = GetSignedArea();
		double num = (double)signedArea / 3.1415927410125732;
		if (num < 0.0)
		{
			num *= -1.0;
		}
		return (float)Math.Sqrt(num);
	}

	public AABB GetCollisionBox()
	{
		Vector2 lowerBound = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 upperBound = new Vector2(float.MinValue, float.MinValue);
		for (int i = 0; i < base.Count; i++)
		{
			if (base[i].X < lowerBound.X)
			{
				lowerBound.X = base[i].X;
			}
			else if (base[i].X > upperBound.X)
			{
				upperBound.X = base[i].X;
			}
			if (base[i].Y < lowerBound.Y)
			{
				lowerBound.Y = base[i].Y;
			}
			else if (base[i].Y > upperBound.Y)
			{
				upperBound.Y = base[i].Y;
			}
		}
		AABB result = default(AABB);
		result.LowerBound = lowerBound;
		result.UpperBound = upperBound;
		return result;
	}

	public void Translate(ref Vector2 vector)
	{
		for (int i = 0; i < base.Count; i++)
		{
			base[i] = Vector2.Add(base[i], vector);
		}
	}

	public void Scale(ref Vector2 value)
	{
		for (int i = 0; i < base.Count; i++)
		{
			base[i] = Vector2.Multiply(base[i], value);
		}
	}

	public void Rotate(float value)
	{
		Matrix.CreateRotationZ(value, out var result);
		for (int i = 0; i < base.Count; i++)
		{
			base[i] = Vector2.Transform(base[i], result);
		}
	}

	public bool IsConvex()
	{
		bool flag = false;
		for (int i = 0; i < base.Count; i++)
		{
			int index = ((i == 0) ? (base.Count - 1) : (i - 1));
			int index2 = i;
			int index3 = ((i != base.Count - 1) ? (i + 1) : 0);
			float num = base[index2].X - base[index].X;
			float num2 = base[index2].Y - base[index].Y;
			float num3 = base[index3].X - base[index2].X;
			float num4 = base[index3].Y - base[index2].Y;
			float num5 = num * num4 - num3 * num2;
			bool flag2 = num5 >= 0f;
			if (i == 0)
			{
				flag = flag2;
			}
			else if (flag != flag2)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsCounterClockWise()
	{
		if (base.Count < 3)
		{
			return true;
		}
		return GetSignedArea() > 0f;
	}

	public void ForceCounterClockWise()
	{
		if (!IsCounterClockWise())
		{
			Reverse();
		}
	}

	public bool IsSimple()
	{
		for (int i = 0; i < base.Count; i++)
		{
			int index = ((i + 1 <= base.Count - 1) ? (i + 1) : 0);
			Vector2 a = new Vector2(base[i].X, base[i].Y);
			Vector2 a2 = new Vector2(base[index].X, base[index].Y);
			for (int j = i + 1; j < base.Count; j++)
			{
				int index2 = ((j + 1 <= base.Count - 1) ? (j + 1) : 0);
				Vector2 b = new Vector2(base[j].X, base[j].Y);
				Vector2 b2 = new Vector2(base[index2].X, base[index2].Y);
				if (LineTools.LineIntersect2(a, a2, b, b2, out var _))
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool CheckPolygon()
	{
		int num = -1;
		if (base.Count < 3 || base.Count > 8)
		{
			num = 0;
		}
		if (!IsConvex())
		{
			num = 1;
		}
		if (!IsSimple())
		{
			num = 2;
		}
		if (GetArea() < 1.1920929E-07f)
		{
			num = 3;
		}
		Vector2[] array = new Vector2[base.Count];
		Vertices vertices = new Vertices(base.Count);
		for (int i = 0; i < base.Count; i++)
		{
			vertices.Add(new Vector2(base[i].X, base[i].Y));
			int index = i;
			int index2 = ((i + 1 < base.Count) ? (i + 1) : 0);
			Vector2 a = new Vector2(base[index2].X - base[index].X, base[index2].Y - base[index].Y);
			ref Vector2 reference = ref array[i];
			reference = MathUtils.Cross(a, 1f);
			array[i].Normalize();
		}
		for (int j = 0; j < base.Count; j++)
		{
			int num2 = ((j == 0) ? (base.Count - 1) : (j - 1));
			float a2 = MathUtils.Cross(array[num2], array[j]);
			a2 = MathUtils.Clamp(a2, -1f, 1f);
			float num3 = (float)Math.Asin(a2);
			if (num3 <= (float)Math.PI / 90f)
			{
				num = 4;
				break;
			}
			for (int k = 0; k < base.Count; k++)
			{
				if (k != j && k != (j + 1) % base.Count)
				{
					float num4 = Vector2.Dot(array[j], vertices[k] - vertices[j]);
					if (num4 >= -0.005f)
					{
						num = 5;
					}
				}
			}
			Vector2 centroid = vertices.GetCentroid();
			Vector2 value = array[num2];
			Vector2 value2 = array[j];
			Vector2 value3 = vertices[j] - centroid;
			Vector2 vector = new Vector2
			{
				X = Vector2.Dot(value, value3),
				Y = Vector2.Dot(value2, value3)
			};
			if (vector.X < 0f || vector.Y < 0f)
			{
				num = 6;
			}
		}
		return num switch
		{
			_ => num != -1, 
		};
	}

	public Vertices TraceEdge(Vertices verts)
	{
		PolyNode[] array = new PolyNode[verts.Count * verts.Count];
		int num = 0;
		for (int i = 0; i < verts.Count; i++)
		{
			Vector2 position = new Vector2(verts[i].X, verts[i].Y);
			array[i].Position = position;
			num++;
			int num2 = ((i != verts.Count - 1) ? (i + 1) : 0);
			int num3 = ((i == 0) ? (verts.Count - 1) : (i - 1));
			array[i].AddConnection(array[num2]);
			array[i].AddConnection(array[num3]);
		}
		bool flag = true;
		int num4 = 0;
		while (flag)
		{
			flag = false;
			for (int j = 0; j < num; j++)
			{
				int k;
				int l;
				int num5;
				Vector2 intersectionPoint;
				for (k = 0; k < array[j].NConnected; k++)
				{
					for (l = 0; l < num; l++)
					{
						if (l == j || array[l] == array[j].Connected[k])
						{
							continue;
						}
						num5 = 0;
						while (num5 < array[l].NConnected)
						{
							if (array[l].Connected[num5] == array[j].Connected[k] || array[l].Connected[num5] == array[j] || !LineTools.LineIntersect(array[j].Position, array[j].Connected[k].Position, array[l].Position, array[l].Connected[num5].Position, out intersectionPoint))
							{
								num5++;
								continue;
							}
							goto IL_0151;
						}
					}
				}
				continue;
				IL_0151:
				flag = true;
				PolyNode polyNode = array[j].Connected[k];
				PolyNode polyNode2 = array[l].Connected[num5];
				array[j].Connected[k].RemoveConnection(array[j]);
				array[j].RemoveConnection(polyNode);
				array[l].Connected[num5].RemoveConnection(array[l]);
				array[l].RemoveConnection(polyNode2);
				array[num] = new PolyNode(intersectionPoint);
				array[num].AddConnection(array[j]);
				array[j].AddConnection(array[num]);
				array[num].AddConnection(array[l]);
				array[l].AddConnection(array[num]);
				array[num].AddConnection(polyNode);
				polyNode.AddConnection(array[num]);
				array[num].AddConnection(polyNode2);
				polyNode2.AddConnection(array[num]);
				num++;
				break;
			}
			num4++;
		}
		bool flag2 = true;
		int num6 = num;
		while (flag2)
		{
			flag2 = false;
			for (int m = 0; m < num; m++)
			{
				if (array[m].NConnected == 0)
				{
					continue;
				}
				for (int n = m + 1; n < num; n++)
				{
					if (array[n].NConnected == 0 || !((array[m].Position - array[n].Position).LengthSquared() <= 1.4210855E-14f))
					{
						continue;
					}
					if (num6 <= 3)
					{
						return new Vertices();
					}
					num6--;
					flag2 = true;
					PolyNode polyNode3 = array[m];
					PolyNode polyNode4 = array[n];
					int nConnected = polyNode4.NConnected;
					for (int num7 = 0; num7 < nConnected; num7++)
					{
						PolyNode polyNode5 = polyNode4.Connected[num7];
						if (polyNode5 != polyNode3)
						{
							polyNode3.AddConnection(polyNode5);
							polyNode5.AddConnection(polyNode3);
						}
						polyNode5.RemoveConnection(polyNode4);
					}
					polyNode4.NConnected = 0;
				}
			}
		}
		float num8 = float.MaxValue;
		float num9 = float.MinValue;
		int num10 = -1;
		for (int num11 = 0; num11 < num; num11++)
		{
			if (array[num11].Position.Y < num8 && array[num11].NConnected > 1)
			{
				num8 = array[num11].Position.Y;
				num10 = num11;
				num9 = array[num11].Position.X;
			}
			else if (array[num11].Position.Y == num8 && array[num11].Position.X > num9 && array[num11].NConnected > 1)
			{
				num10 = num11;
				num9 = array[num11].Position.X;
			}
		}
		Vector2 incomingDir = new Vector2(1f, 0f);
		Vector2[] array2 = new Vector2[4 * num];
		int num12 = 0;
		PolyNode polyNode6 = array[num10];
		PolyNode polyNode7 = polyNode6;
		PolyNode rightestConnection = polyNode6.GetRightestConnection(incomingDir);
		if (rightestConnection == null)
		{
			Vertices vertices = new Vertices(num12);
			for (int num13 = 0; num13 < num12; num13++)
			{
				vertices.Add(array2[num13]);
			}
			return vertices;
		}
		ref Vector2 reference = ref array2[0];
		reference = polyNode7.Position;
		num12++;
		while (rightestConnection != polyNode7)
		{
			_ = 4 * num;
			ref Vector2 reference2 = ref array2[num12++];
			reference2 = rightestConnection.Position;
			PolyNode incoming = polyNode6;
			polyNode6 = rightestConnection;
			rightestConnection = polyNode6.GetRightestConnection(incoming);
			if (rightestConnection == null)
			{
				Vertices vertices2 = new Vertices(num12);
				for (int num14 = 0; num14 < num12; num14++)
				{
					vertices2.Add(array2[num14]);
				}
				return vertices2;
			}
		}
		return new Vertices();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < base.Count; i++)
		{
			stringBuilder.Append(base[i].ToString());
			if (i < base.Count - 1)
			{
				stringBuilder.Append(" ");
			}
		}
		return stringBuilder.ToString();
	}

	public void ProjectToAxis(ref Vector2 axis, out float min, out float max)
	{
		max = (min = Vector2.Dot(axis, base[0]));
		for (int i = 0; i < base.Count; i++)
		{
			float num = Vector2.Dot(base[i], axis);
			if (num < min)
			{
				min = num;
			}
			else if (num > max)
			{
				max = num;
			}
		}
	}

	public bool PointInPolygon(ref Vector2 point)
	{
		int num = 0;
		for (int i = 0; i < base.Count; i++)
		{
			Vector2 a = base[i];
			Vector2 b = base[NextIndex(i)];
			if (a.Y <= point.Y)
			{
				if (b.Y > point.Y && MathUtils.Area(ref a, ref b, ref point) > 0f)
				{
					num++;
				}
			}
			else if (b.Y <= point.Y && MathUtils.Area(ref a, ref b, ref point) < 0f)
			{
				num--;
			}
		}
		return num != 0;
	}

	public bool PointInPolygonAngle(ref Vector2 point)
	{
		double num = 0.0;
		for (int i = 0; i < base.Count; i++)
		{
			Vector2 p = base[i] - point;
			Vector2 p2 = base[NextIndex(i)] - point;
			num += MathUtils.VectorAngle(ref p, ref p2);
		}
		if (Math.Abs(num) < Math.PI)
		{
			return false;
		}
		return true;
	}
}
