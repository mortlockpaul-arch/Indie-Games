using System;
using System.Collections.Generic;
using System.Text;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Mathematics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class Vertices : List<Vector2>
{
	private struct Vertex
	{
		public readonly Vector2 Position;

		public readonly short Index;

		public Vertex(Vector2 position, short index)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Position = position;
			Index = index;
		}

		public override bool Equals(object obj)
		{
			if ((object)obj.GetType() != typeof(Vertex))
			{
				return false;
			}
			return Equals((Vertex)obj);
		}

		public bool Equals(Vertex obj)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Vector2 position = obj.Position;
			if (((Vector2)(ref position)).Equals(Position))
			{
				return obj.Index == Index;
			}
			return false;
		}

		public override int GetHashCode()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return (((object)Position/*cast due to constrained. prefix*/).GetHashCode() * 397) ^ Index;
		}

		public override string ToString()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return $"{Position} ({Index})";
		}
	}

	private struct LineSegment(Vertex a, Vertex b)
	{
		public Vertex A = a;

		public Vertex B = b;

		public float? IntersectsWithRay(Vector2 origin, Vector2 direction)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			float num = MathHelper.Max(A.Position.X - origin.X, B.Position.X - origin.X) * 2f;
			Vector2? val = FindIntersection(b: new LineSegment(new Vertex(origin, 0), new Vertex(origin + direction * num, 0)), a: this);
			float? result = null;
			if (val.HasValue)
			{
				result = Vector2.Distance(origin, val.Value);
			}
			return result;
		}

		public static Vector2? FindIntersection(LineSegment a, LineSegment b)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			float x = a.A.Position.X;
			float y = a.A.Position.Y;
			float x2 = a.B.Position.X;
			float y2 = a.B.Position.Y;
			float x3 = b.A.Position.X;
			float y3 = b.A.Position.Y;
			float x4 = b.B.Position.X;
			float y4 = b.B.Position.Y;
			float num = (y4 - y3) * (x2 - x) - (x4 - x3) * (y2 - y);
			float num2 = (x4 - x3) * (y - y3) - (y4 - y3) * (x - x3);
			float num3 = (x2 - x) * (y - y3) - (y2 - y) * (x - x3);
			float num4 = num2 / num;
			float num5 = num3 / num;
			if (MathHelper.Clamp(num4, 0f, 1f) != num4 || MathHelper.Clamp(num5, 0f, 1f) != num5)
			{
				return null;
			}
			return a.A.Position + (a.B.Position - a.A.Position) * num4;
		}
	}

	private struct Triangle(Vertex a, Vertex b, Vertex c)
	{
		public readonly Vertex A = a;

		public readonly Vertex B = b;

		public readonly Vertex C = c;

		public bool ContainsPoint(Vertex point)
		{
			if (point.Equals(A) || point.Equals(B) || point.Equals(C))
			{
				return true;
			}
			bool flag = false;
			if (checkPointToSegment(C, A, point))
			{
				flag = !flag;
			}
			if (checkPointToSegment(A, B, point))
			{
				flag = !flag;
			}
			if (checkPointToSegment(B, C, point))
			{
				flag = !flag;
			}
			return flag;
		}

		public static bool ContainsPoint(Vertex a, Vertex b, Vertex c, Vertex point)
		{
			return new Triangle(a, b, c).ContainsPoint(point);
		}

		private static bool checkPointToSegment(Vertex sA, Vertex sB, Vertex point)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			if ((sA.Position.Y < point.Position.Y && sB.Position.Y >= point.Position.Y) || (sB.Position.Y < point.Position.Y && sA.Position.Y >= point.Position.Y))
			{
				float num = sA.Position.X + (point.Position.Y - sA.Position.Y) / (sB.Position.Y - sA.Position.Y) * (sB.Position.X - sA.Position.X);
				if (num < point.Position.X)
				{
					return true;
				}
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if ((object)obj.GetType() != typeof(Triangle))
			{
				return false;
			}
			return Equals((Triangle)obj);
		}

		public bool Equals(Triangle obj)
		{
			if (obj.A.Equals(A) && obj.B.Equals(B))
			{
				return obj.C.Equals(C);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = A.GetHashCode();
			hashCode = (hashCode * 397) ^ B.GetHashCode();
			return (hashCode * 397) ^ C.GetHashCode();
		}
	}

	private class CyclicalList<T> : List<T>
	{
		public new T this[int index]
		{
			get
			{
				while (index < 0)
				{
					index = base.Count + index;
				}
				if (index >= base.Count)
				{
					index %= base.Count;
				}
				return base[index];
			}
		}
	}

	private class IndexableCyclicalLinkedList<T> : LinkedList<T>
	{
		public LinkedListNode<T> this[int index]
		{
			get
			{
				while (index < 0)
				{
					index = base.Count + index;
				}
				if (index >= base.Count)
				{
					index %= base.Count;
				}
				LinkedListNode<T> linkedListNode = base.First;
				for (int i = 0; i < index; i++)
				{
					linkedListNode = linkedListNode.Next;
				}
				return linkedListNode;
			}
		}

		public void RemoveAt(int index)
		{
			Remove(this[index]);
		}

		public int IndexOf(T item)
		{
			for (int i = 0; i < base.Count; i++)
			{
				if (this[i].Value.Equals(item))
				{
					return i;
				}
			}
			return -1;
		}
	}

	public enum WindingOrder
	{
		Clockwise,
		CounterClockwise
	}

	private Vector2 _res;

	private Vector2 _vectorTemp1;

	private Vector2 _vectorTemp2;

	private Vector2 _vectorTemp3;

	private Vector2 _vectorTemp4;

	private Vector2 _vectorTemp5;

	private static readonly int[,] _closePixels = new int[8, 2]
	{
		{ -1, -1 },
		{ 0, -1 },
		{ 1, -1 },
		{ 1, 0 },
		{ 1, 1 },
		{ 0, 1 },
		{ -1, 1 },
		{ -1, 0 }
	};

	private static readonly IndexableCyclicalLinkedList<Vertex> polygonVertices = new IndexableCyclicalLinkedList<Vertex>();

	private static readonly IndexableCyclicalLinkedList<Vertex> earVertices = new IndexableCyclicalLinkedList<Vertex>();

	private static readonly CyclicalList<Vertex> convexVertices = new CyclicalList<Vertex>();

	private static readonly CyclicalList<Vertex> reflexVertices = new CyclicalList<Vertex>();

	public Vertices()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		_vectorTemp1 = Vector2.Zero;
		_vectorTemp2 = Vector2.Zero;
		_vectorTemp3 = Vector2.Zero;
		_vectorTemp4 = Vector2.Zero;
		_vectorTemp5 = Vector2.Zero;
		base._002Ector();
	}

	public Vertices(int capacity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		_vectorTemp1 = Vector2.Zero;
		_vectorTemp2 = Vector2.Zero;
		_vectorTemp3 = Vector2.Zero;
		_vectorTemp4 = Vector2.Zero;
		_vectorTemp5 = Vector2.Zero;
		base._002Ector();
		base.Capacity = capacity;
	}

	public Vertices(ref Vector2[] vector2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		_vectorTemp1 = Vector2.Zero;
		_vectorTemp2 = Vector2.Zero;
		_vectorTemp3 = Vector2.Zero;
		_vectorTemp4 = Vector2.Zero;
		_vectorTemp5 = Vector2.Zero;
		base._002Ector();
		for (int i = 0; i < vector2.Length; i++)
		{
			Add(vector2[i]);
		}
	}

	public Vertices(IList<Vector2> vertices)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		_vectorTemp1 = Vector2.Zero;
		_vectorTemp2 = Vector2.Zero;
		_vectorTemp3 = Vector2.Zero;
		_vectorTemp4 = Vector2.Zero;
		_vectorTemp5 = Vector2.Zero;
		base._002Ector();
		for (int i = 0; i < vertices.Count; i++)
		{
			Add(vertices[i]);
		}
	}

	public Vector2[] GetVerticesArray()
	{
		return ToArray();
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

	public Vector2 GetEdge(int index)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		int index2 = NextIndex(index);
		_vectorTemp2 = base[index2];
		_vectorTemp3 = base[index];
		Vector2.Subtract(ref _vectorTemp2, ref _vectorTemp3, ref _vectorTemp1);
		return _vectorTemp1;
	}

	public void GetEdge(int index, out Vector2 edge)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		int index2 = NextIndex(index);
		_vectorTemp2 = base[index2];
		_vectorTemp3 = base[index];
		Vector2.Subtract(ref _vectorTemp2, ref _vectorTemp3, ref edge);
	}

	public Vector2 GetEdgeMidPoint(int index)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		GetEdge(index, out _vectorTemp1);
		Vector2.Multiply(ref _vectorTemp1, 0.5f, ref _vectorTemp2);
		_vectorTemp3 = base[index];
		Vector2.Add(ref _vectorTemp3, ref _vectorTemp2, ref _vectorTemp1);
		return _vectorTemp1;
	}

	public void GetEdgeMidPoint(int index, out Vector2 midPoint)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		GetEdge(index, out _vectorTemp1);
		Vector2.Multiply(ref _vectorTemp1, 0.5f, ref _vectorTemp2);
		_vectorTemp3 = base[index];
		Vector2.Add(ref _vectorTemp3, ref _vectorTemp2, ref midPoint);
	}

	public Vector2 GetEdgeNormal(int index)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		GetEdge(index, out _vectorTemp1);
		_vectorTemp2.X = 0f - _vectorTemp1.Y;
		_vectorTemp2.Y = _vectorTemp1.X;
		Vector2.Normalize(ref _vectorTemp2, ref _vectorTemp3);
		return _vectorTemp3;
	}

	public void GetEdgeNormal(int index, out Vector2 edgeNormal)
	{
		GetEdge(index, out _vectorTemp4);
		_vectorTemp5.X = 0f - _vectorTemp4.Y;
		_vectorTemp5.Y = _vectorTemp4.X;
		Vector2.Normalize(ref _vectorTemp5, ref edgeNormal);
	}

	public Vector2 GetVertexNormal(int index)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		GetEdgeNormal(index, out _vectorTemp1);
		int index2 = PreviousIndex(index);
		GetEdgeNormal(index2, out _vectorTemp2);
		Vector2.Add(ref _vectorTemp1, ref _vectorTemp2, ref _vectorTemp3);
		Vector2.Normalize(ref _vectorTemp3, ref _vectorTemp1);
		return _vectorTemp1;
	}

	public void GetVertexNormal(int index, out Vector2 vertexNormal)
	{
		GetEdgeNormal(index, out _vectorTemp1);
		int index2 = PreviousIndex(index);
		GetEdgeNormal(index2, out _vectorTemp2);
		Vector2.Add(ref _vectorTemp1, ref _vectorTemp2, ref _vectorTemp3);
		Vector2.Normalize(ref _vectorTemp3, ref vertexNormal);
	}

	public float GetShortestEdge()
	{
		float num = float.MaxValue;
		for (int i = 0; i < base.Count; i++)
		{
			GetEdge(i, out _vectorTemp1);
			float num2 = ((Vector2)(ref _vectorTemp1)).Length();
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	public void SubDivideEdges(float maxEdgeLength)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		Vector2 val3 = default(Vector2);
		for (int i = 0; i < base.Count; i++)
		{
			Vector2 val = base[i];
			Vector2 val2 = base[NextIndex(i)];
			Vector2.Subtract(ref val, ref val2, ref val3);
			float num = ((Vector2)(ref val3)).Length();
			vertices.Add(val);
			if (num > maxEdgeLength)
			{
				double num2 = Math.Ceiling((double)num / (double)maxEdgeLength);
				for (int j = 0; (double)j < num2 - 1.0; j++)
				{
					Vector2 item = Vector2.Lerp(val, val2, (float)(j + 1) / (float)num2);
					vertices.Add(item);
				}
			}
		}
		Clear();
		for (int k = 0; k < vertices.Count; k++)
		{
			Add(vertices[k]);
		}
	}

	public void ForceCounterClockWiseOrder()
	{
		float signedArea = GetSignedArea();
		if (signedArea > 0f)
		{
			Reverse();
		}
	}

	private float GetSignedArea()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		float area = GetArea();
		return GetCentroid(area);
	}

	public Vector2 GetCentroid(float area)
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = 0f;
		float signedArea = GetSignedArea();
		float num5;
		if (signedArea > 0f)
		{
			for (int num3 = base.Count - 1; num3 >= 0; num3--)
			{
				int num4 = (num3 - 1) % base.Count;
				if (num4 < 0)
				{
					num4 += base.Count;
				}
				num5 = 0f - (this[num3].X * this[num4].Y - this[num4].X * this[num3].Y);
				num += (this[num3].X + this[num4].X) * num5;
				num2 += (this[num3].Y + this[num4].Y) * num5;
			}
		}
		else
		{
			for (int num3 = 0; num3 < base.Count; num3++)
			{
				int index = (num3 + 1) % base.Count;
				num5 = 0f - (this[num3].X * this[index].Y - this[index].X * this[num3].Y);
				num += (this[num3].X + this[index].X) * num5;
				num2 += (this[num3].Y + this[index].Y) * num5;
			}
		}
		area *= 6f;
		num5 = 1f / area;
		num *= num5;
		num2 *= num5;
		_res.X = num;
		_res.Y = num2;
		return _res;
	}

	public float GetMomentOfInertia()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices(this);
		vertices.ForceCounterClockWiseOrder();
		Vector2 vector = vertices.GetCentroid();
		Vector2.Multiply(ref vector, -1f, ref vector);
		vertices.Translate(ref vector);
		if (vertices.Count == 0)
		{
			throw new ArgumentException("Can't calculate MOI on zero vertices");
		}
		if (vertices.Count == 1)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = 0f;
		Vector2 value = vertices[vertices.Count - 1];
		int num3 = 0;
		float num4 = default(float);
		float num5 = default(float);
		float num6 = default(float);
		while (num3 < vertices.Count)
		{
			Vector2 value2 = vertices[num3];
			Vector2.Dot(ref value2, ref value2, ref num4);
			Vector2.Dot(ref value2, ref value, ref num5);
			Vector2.Dot(ref value, ref value, ref num6);
			Calculator.Cross(ref value, ref value2, out var ret);
			ret = Math.Abs(ret);
			num2 += ret;
			num += (num4 + num5 + num6) * ret;
			num3++;
			value = value2;
		}
		return num / (num2 * 6f);
	}

	public void ProjectToAxis(ref Vector2 axis, out float min, out float max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
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

	public void Translate(ref Vector2 vector)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < base.Count; i++)
		{
			base[i] = Vector2.Add(base[i], vector);
		}
	}

	public void Scale(ref Vector2 value)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < base.Count; i++)
		{
			base[i] = Vector2.Multiply(base[i], value);
		}
	}

	public void Rotate(float value)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = default(Matrix);
		Matrix.CreateRotationZ(value, ref val);
		for (int i = 0; i < base.Count; i++)
		{
			base[i] = Vector2.Transform(base[i], val);
		}
	}

	public bool IsConvex()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
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

	public static Vertices CreateRectangle(float width, float height)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		vertices.Add(new Vector2((0f - width) * 0.5f, (0f - height) * 0.5f));
		vertices.Add(new Vector2((0f - width) * 0.5f, (0f - height) * 0.25f));
		vertices.Add(new Vector2((0f - width) * 0.5f, 0f));
		vertices.Add(new Vector2((0f - width) * 0.5f, height * 0.25f));
		vertices.Add(new Vector2((0f - width) * 0.5f, height * 0.5f));
		vertices.Add(new Vector2((0f - width) * 0.25f, height * 0.5f));
		vertices.Add(new Vector2(0f, height * 0.5f));
		vertices.Add(new Vector2(width * 0.25f, height * 0.5f));
		vertices.Add(new Vector2(width * 0.5f, height * 0.5f));
		vertices.Add(new Vector2(width * 0.5f, height * 0.25f));
		vertices.Add(new Vector2(width * 0.5f, 0f));
		vertices.Add(new Vector2(width * 0.5f, (0f - height) * 0.25f));
		vertices.Add(new Vector2(width * 0.5f, (0f - height) * 0.5f));
		vertices.Add(new Vector2(width * 0.25f, (0f - height) * 0.5f));
		vertices.Add(new Vector2(0f, (0f - height) * 0.5f));
		vertices.Add(new Vector2((0f - width) * 0.25f, (0f - height) * 0.5f));
		return vertices;
	}

	public static Vertices CreateSimpleRectangle(float width, float height)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		vertices.Add(new Vector2((0f - width) * 0.5f, (0f - height) * 0.5f));
		vertices.Add(new Vector2((0f - width) * 0.5f, height * 0.5f));
		vertices.Add(new Vector2(width * 0.5f, height * 0.5f));
		vertices.Add(new Vector2(width * 0.5f, (0f - height) * 0.5f));
		return vertices;
	}

	public static Vertices CreateCircle(float radius, int numberOfEdges)
	{
		return CreateEllipse(radius, radius, numberOfEdges);
	}

	public static Vertices CreateEllipse(float xRadius, float yRadius, int numberOfEdges)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		float num = (float)Math.PI * 2f / (float)numberOfEdges;
		vertices.Add(new Vector2(xRadius, 0f));
		for (int i = 1; i < numberOfEdges; i++)
		{
			vertices.Add(new Vector2(xRadius * Calculator.Cos(num * (float)i), (0f - yRadius) * Calculator.Sin(num * (float)i)));
		}
		return vertices;
	}

	public static Vertices CreateGear(float radius, int numberOfTeeth, float tipPercentage, float toothHeight)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		float num = (float)Math.PI * 2f / (float)numberOfTeeth;
		float num2 = num / 2f * tipPercentage;
		float num3 = (num - num2 * 2f) / 2f;
		for (int i = 0; i < numberOfTeeth; i++)
		{
			vertices.Add(new Vector2(radius * Calculator.Cos(num * (float)i), (0f - radius) * Calculator.Sin(num * (float)i)));
			vertices.Add(new Vector2((radius + toothHeight) * Calculator.Cos(num * (float)i + num3), (0f - (radius + toothHeight)) * Calculator.Sin(num * (float)i + num3)));
			vertices.Add(new Vector2((radius + toothHeight) * Calculator.Cos(num * (float)i + num3 + num2), (0f - (radius + toothHeight)) * Calculator.Sin(num * (float)i + num3 + num2)));
			vertices.Add(new Vector2(radius * Calculator.Cos(num * (float)i + num3 * 2f + num2), (0f - radius) * Calculator.Sin(num * (float)i + num3 * 2f + num2)));
		}
		return vertices;
	}

	public override string ToString()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < base.Count; i++)
		{
			stringBuilder.Append(((object)base[i]/*cast due to constrained. prefix*/).ToString());
			if (i < base.Count - 1)
			{
				stringBuilder.Append(" ");
			}
		}
		return stringBuilder.ToString();
	}

	public static Vector2 FindMidpoint(Vector2 firstVector, Vector2 secondVector)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		float num = ((!(firstVector.X < secondVector.X)) ? ((secondVector.X - firstVector.X) * 0.5f) : Math.Abs((firstVector.X - secondVector.X) * 0.5f));
		float num2 = ((!(firstVector.Y < secondVector.Y)) ? ((secondVector.Y - firstVector.Y) * 0.5f) : Math.Abs((firstVector.Y - secondVector.Y) * 0.5f));
		return new Vector2(firstVector.X + num, firstVector.Y + num2);
	}

	public static Vector2 FindEdgeNormal(Vector2 first, Vector2 second)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 zero = Vector2.Zero;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(first.X - second.X, first.Y - second.Y);
		zero.X = 0f - val.Y;
		zero.Y = val.X;
		((Vector2)(ref zero)).Normalize();
		return zero;
	}

	public static Vector2 FindVertexNormal(Vector2 first, Vector2 second, Vector2 c)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = FindEdgeNormal(first, second);
		Vector2 val2 = FindEdgeNormal(second, c);
		Vector2 result = default(Vector2);
		Vector2.Add(ref val, ref val2, ref result);
		return result;
	}

	public static float FindNormalAngle(Vector2 n)
	{
		if (n.Y > 0f && n.X > 0f)
		{
			return (float)Math.Atan(n.X / (0f - n.Y));
		}
		if (n.Y < 0f && n.X > 0f)
		{
			return (float)Math.Atan(n.X / (0f - n.Y));
		}
		if (n.Y > 0f && n.X < 0f)
		{
			return (float)Math.Atan((0f - n.X) / n.Y);
		}
		if (n.Y < 0f && n.X < 0f)
		{
			return (float)Math.Atan((0f - n.X) / n.Y);
		}
		return 0f;
	}

	public static Vertices CreatePolygon(uint[] data, int width, int height)
	{
		PolygonCreationAssistance pca = new PolygonCreationAssistance(data, width, height);
		List<Vertices> list = CreatePolygon(ref pca);
		return list[0];
	}

	public static List<Vertices> CreatePolygon(uint[] data, int width, int height, float hullTolerance, byte alphaTolerance, bool multiPartDetection, bool holeDetection)
	{
		PolygonCreationAssistance pca = new PolygonCreationAssistance(data, width, height);
		pca.HullTolerance = hullTolerance;
		pca.AlphaTolerance = alphaTolerance;
		pca.MultipartDetection = multiPartDetection;
		pca.HoleDetection = holeDetection;
		return CreatePolygon(ref pca);
	}

	public static List<Vertices> CreatePolygon(ref PolygonCreationAssistance pca)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		List<Vertices> list = new List<Vertices>();
		Vector2? startVertex = null;
		Vector2? entrance = null;
		List<Vector2> list2 = new List<Vector2>();
		if (pca.IsValid())
		{
			bool flag;
			do
			{
				Vertices vertices;
				if (list.Count == 0)
				{
					vertices = CreateSimplePolygon(ref pca, Vector2.Zero, Vector2.Zero);
					if (vertices != null && vertices.Count > 2)
					{
						entrance = GetTopMostVertex(ref vertices);
					}
				}
				else
				{
					if (!entrance.HasValue)
					{
						break;
					}
					vertices = CreateSimplePolygon(ref pca, entrance.Value, new Vector2(entrance.Value.X - 1f, entrance.Value.Y));
				}
				flag = false;
				if (vertices == null || vertices.Count <= 2)
				{
					continue;
				}
				if (pca.HoleDetection)
				{
					while (true)
					{
						startVertex = GetHoleHullEntrance(ref pca, ref vertices, startVertex);
						if (!startVertex.HasValue || list2.Contains(startVertex.Value))
						{
							break;
						}
						list2.Add(startVertex.Value);
						Vertices vertices2 = CreateSimplePolygon(ref pca, startVertex.Value, new Vector2(startVertex.Value.X + 1f, startVertex.Value.Y));
						if (vertices2 != null && vertices2.Count > 2)
						{
							vertices2.Add(vertices2[0]);
							if (SplitPolygonEdge(ref vertices, EdgeAlignment.Vertical, startVertex.Value, out var _, out var vertex2Index))
							{
								vertices.InsertRange(vertex2Index, vertices2);
							}
						}
					}
				}
				list.Add(vertices);
				if (!pca.MultipartDetection)
				{
					continue;
				}
				while (GetNextHullEntrance(ref pca, entrance.Value, out entrance))
				{
					bool flag2 = false;
					for (int i = 0; i < list.Count; i++)
					{
						vertices = list[i];
						if (InPolygon(ref pca, ref vertices, entrance.Value))
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						flag = true;
						break;
					}
				}
			}
			while (flag);
			return list;
		}
		throw new Exception("Sizes don't match: Color array must contain texture width * texture height elements.");
	}

	private static Vector2? GetHoleHullEntrance(ref PolygonCreationAssistance pca, ref Vertices polygon, Vector2? startVertex)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		List<CrossingEdgeInfo> list = new List<CrossingEdgeInfo>();
		int num = 0;
		if (polygon != null && polygon.Count > 0)
		{
			int num2 = ((!startVertex.HasValue) ? ((int)GetTopMostCoord(ref polygon)) : ((int)startVertex.Value.Y));
			int num3 = (int)GetBottomMostCoord(ref polygon);
			if (num2 > 0 && num2 < pca.Height && num3 > 0 && num3 < pca.Height)
			{
				for (int i = num2; i <= num3; i += pca.HoleDetectionLineStepSize)
				{
					list = GetCrossingEdges(ref polygon, EdgeAlignment.Vertical, i);
					if (list.Count <= 1 || list.Count % 2 != 0)
					{
						continue;
					}
					for (int j = 0; j < list.Count; j += 2)
					{
						bool flag = false;
						bool flag2 = false;
						for (int k = (int)list[j].CrossingPoint.X; k <= (int)list[j + 1].CrossingPoint.X; k++)
						{
							if (pca.IsSolid(k, i))
							{
								if (!flag2)
								{
									flag = true;
									num = k;
								}
								if (flag && flag2)
								{
									Vector2? result = new Vector2((float)num, (float)i);
									if (DistanceToHullAcceptable(ref pca, ref polygon, result.Value, higherDetail: true))
									{
										return result;
									}
									result = null;
									break;
								}
							}
							else if (flag)
							{
								flag2 = true;
							}
						}
					}
				}
			}
		}
		return null;
	}

	private static bool DistanceToHullAcceptable(ref PolygonCreationAssistance pca, ref Vertices polygon, Vector2 point, bool higherDetail)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (polygon != null && polygon.Count > 2)
		{
			Vector2 lineEndPoint = polygon[polygon.Count - 1];
			if (higherDetail)
			{
				for (int i = 0; i < polygon.Count; i++)
				{
					Vector2 lineEndPoint2 = polygon[i];
					if (Calculator.DistanceBetweenPointAndLineSegment(ref point, ref lineEndPoint2, ref lineEndPoint) <= pca.HullTolerance || Calculator.DistanceBetweenPointAndPoint(ref point, ref lineEndPoint2) <= pca.HullTolerance)
					{
						return false;
					}
					lineEndPoint = polygon[i];
				}
				return true;
			}
			for (int j = 0; j < polygon.Count; j++)
			{
				Vector2 lineEndPoint2 = polygon[j];
				if (Calculator.DistanceBetweenPointAndLineSegment(ref point, ref lineEndPoint2, ref lineEndPoint) <= pca.HullTolerance)
				{
					return false;
				}
				lineEndPoint = polygon[j];
			}
			return true;
		}
		return false;
	}

	private static bool InPolygon(ref PolygonCreationAssistance pca, ref Vertices polygon, Vector2 point)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !DistanceToHullAcceptable(ref pca, ref polygon, point, higherDetail: true);
		if (!flag)
		{
			List<CrossingEdgeInfo> crossingEdges = GetCrossingEdges(ref polygon, EdgeAlignment.Vertical, (int)point.Y);
			if (crossingEdges.Count > 0 && crossingEdges.Count % 2 == 0)
			{
				for (int i = 0; i < crossingEdges.Count; i += 2)
				{
					if (crossingEdges[i].CrossingPoint.X <= point.X && crossingEdges[i + 1].CrossingPoint.X >= point.X)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
		return flag;
	}

	private static Vector2? GetTopMostVertex(ref Vertices vertices)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		Vector2? result = null;
		for (int i = 0; i < vertices.Count; i++)
		{
			if (num > vertices[i].Y)
			{
				num = vertices[i].Y;
				result = vertices[i];
			}
		}
		return result;
	}

	private static float GetTopMostCoord(ref Vertices vertices)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		for (int i = 0; i < vertices.Count; i++)
		{
			if (num > vertices[i].Y)
			{
				num = vertices[i].Y;
			}
		}
		return num;
	}

	private static float GetBottomMostCoord(ref Vertices vertices)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MinValue;
		for (int i = 0; i < vertices.Count; i++)
		{
			if (num < vertices[i].Y)
			{
				num = vertices[i].Y;
			}
		}
		return num;
	}

	private static List<CrossingEdgeInfo> GetCrossingEdges(ref Vertices polygon, EdgeAlignment edgeAlign, int checkLine)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		List<CrossingEdgeInfo> list = new List<CrossingEdgeInfo>();
		if (polygon.Count > 1)
		{
			Vector2 val = polygon[polygon.Count - 1];
			switch (edgeAlign)
			{
			case EdgeAlignment.Vertical:
			{
				Vector2 crossingPoint = default(Vector2);
				for (int i = 0; i < polygon.Count; i++)
				{
					Vector2 val2 = polygon[i];
					if (((val2.Y >= (float)checkLine && val.Y <= (float)checkLine) || (val2.Y <= (float)checkLine && val.Y >= (float)checkLine)) && val2.Y != val.Y)
					{
						bool flag = true;
						Vector2 val3 = val - val2;
						if (val2.Y == (float)checkLine)
						{
							Vector2 val4 = polygon[(i + 1) % polygon.Count];
							Vector2 val5 = val2 - val4;
							flag = ((!(val3.Y > 0f)) ? (val5.Y >= 0f) : (val5.Y <= 0f));
						}
						if (flag)
						{
							((Vector2)(ref crossingPoint))._002Ector(((float)checkLine - val2.Y) / val3.Y * val3.X + val2.X, (float)checkLine);
							list.Add(new CrossingEdgeInfo(val2, val, crossingPoint, edgeAlign));
						}
					}
					val = val2;
				}
				break;
			}
			case EdgeAlignment.Horizontal:
				throw new Exception("EdgeAlignment.Horizontal isn't implemented yet. Sorry.");
			}
		}
		list.Sort();
		return list;
	}

	private static bool SplitPolygonEdge(ref Vertices polygon, EdgeAlignment edgeAlign, Vector2 coordInsideThePolygon, out int vertex1Index, out int vertex2Index)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		List<CrossingEdgeInfo> list = new List<CrossingEdgeInfo>();
		int num = 0;
		int index = 0;
		bool flag = false;
		float num2 = float.MaxValue;
		bool flag2 = false;
		Vector2 point = Vector2.Zero;
		vertex1Index = 0;
		vertex2Index = 0;
		switch (edgeAlign)
		{
		case EdgeAlignment.Vertical:
		{
			list = GetCrossingEdges(ref polygon, EdgeAlignment.Vertical, (int)coordInsideThePolygon.Y);
			point.Y = coordInsideThePolygon.Y;
			if (list == null || list.Count <= 1 || list.Count % 2 != 0)
			{
				break;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].CrossingPoint.X < coordInsideThePolygon.X)
				{
					float num3 = coordInsideThePolygon.X - list[i].CrossingPoint.X;
					if (num3 < num2)
					{
						num2 = num3;
						point.X = list[i].CrossingPoint.X;
						flag2 = true;
					}
				}
			}
			if (!flag2)
			{
				break;
			}
			num2 = float.MaxValue;
			int num4 = polygon.Count - 1;
			for (int j = 0; j < polygon.Count; j++)
			{
				Vector2 lineEndPoint = polygon[j];
				Vector2 lineEndPoint2 = polygon[num4];
				float num3 = Calculator.DistanceBetweenPointAndLineSegment(ref point, ref lineEndPoint, ref lineEndPoint2);
				if (num3 < num2)
				{
					num2 = num3;
					num = j;
					index = num4;
					flag = true;
				}
				num4 = j;
			}
			if (flag)
			{
				Vector2 val = polygon[index] - polygon[num];
				((Vector2)(ref val)).Normalize();
				Vector2 point2 = polygon[num];
				float num3 = Calculator.DistanceBetweenPointAndPoint(ref point2, ref point);
				vertex1Index = num;
				vertex2Index = num + 1;
				polygon.Insert(num, num3 * val + polygon[vertex1Index]);
				polygon.Insert(num, num3 * val + polygon[vertex2Index]);
				return true;
			}
			break;
		}
		case EdgeAlignment.Horizontal:
			throw new Exception("EdgeAlignment.Horizontal isn't implemented yet. Sorry.");
		}
		return false;
	}

	private static Vertices CreateSimplePolygon(ref PolygonCreationAssistance pca, Vector2 entrance, Vector2 last)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Vertices vertices = new Vertices();
		Vertices hullArea = new Vertices();
		Vector2 current = Vector2.Zero;
		if (entrance == Vector2.Zero || !pca.InBounds(entrance))
		{
			flag = GetHullEntrance(ref pca, out entrance);
			if (flag)
			{
				current = new Vector2(entrance.X - 1f, entrance.Y);
			}
		}
		else if (pca.IsSolid(entrance))
		{
			Vector2 foundPixel;
			if (IsNearPixel(ref pca, entrance, last))
			{
				current = last;
				flag = true;
			}
			else if (SearchNearPixels(ref pca, searchingForSolidPixel: false, entrance, out foundPixel))
			{
				current = foundPixel;
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			Vector2 next = entrance;
			vertices.Add(entrance);
			do
			{
				hullArea.Add(next);
				if (SearchForOutstandingVertex(ref hullArea, pca.HullTolerance, out var outstanding))
				{
					vertices.Add(outstanding);
					hullArea.RemoveRange(0, hullArea.IndexOf(outstanding));
				}
				last = current;
				current = next;
				if (!GetNextHullPoint(ref pca, ref last, ref current, out next))
				{
					next = entrance;
				}
			}
			while (next != entrance);
		}
		return vertices;
	}

	private static bool SearchNearPixels(ref PolygonCreationAssistance pca, bool searchingForSolidPixel, Vector2 current, out Vector2 foundPixel)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 8; i++)
		{
			int num = (int)current.X + _closePixels[i, 0];
			int num2 = (int)current.Y + _closePixels[i, 1];
			if (!searchingForSolidPixel ^ pca.IsSolid(num, num2))
			{
				foundPixel = new Vector2((float)num, (float)num2);
				return true;
			}
		}
		foundPixel = Vector2.Zero;
		return false;
	}

	private static bool IsNearPixel(ref PolygonCreationAssistance pca, Vector2 current, Vector2 near)
	{
		for (int i = 0; i < 8; i++)
		{
			int num = (int)current.X + _closePixels[i, 0];
			int num2 = (int)current.Y + _closePixels[i, 1];
			if (num >= 0 && num <= pca.Width && num2 >= 0 && num2 <= pca.Height && num == (int)near.X && num2 == (int)near.Y)
			{
				return true;
			}
		}
		return false;
	}

	private static bool GetHullEntrance(ref PolygonCreationAssistance pca, out Vector2 entrance)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i <= pca.Height; i++)
		{
			for (int j = 0; j <= pca.Width; j++)
			{
				if (pca.IsSolid(j, i))
				{
					entrance = new Vector2((float)j, (float)i);
					return true;
				}
			}
		}
		entrance = Vector2.Zero;
		return false;
	}

	private static bool GetNextHullEntrance(ref PolygonCreationAssistance pca, Vector2 start, out Vector2? entrance)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		int num = pca.Height * pca.Width;
		bool flag = false;
		for (int i = (int)start.X + (int)start.Y * pca.Width; i <= num; i++)
		{
			if (pca.IsSolid(i))
			{
				if (flag)
				{
					int num2 = i % pca.Width;
					entrance = new Vector2((float)num2, (float)((i - num2) / pca.Width));
					return true;
				}
			}
			else
			{
				flag = true;
			}
		}
		entrance = null;
		return false;
	}

	private static bool GetNextHullPoint(ref PolygonCreationAssistance pca, ref Vector2 last, ref Vector2 current, out Vector2 next)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		int indexOfFirstPixelToCheck = GetIndexOfFirstPixelToCheck(last, current);
		for (int i = 0; i < 8; i++)
		{
			int num = (indexOfFirstPixelToCheck + i) % 8;
			int num2 = (int)current.X + _closePixels[num, 0];
			int num3 = (int)current.Y + _closePixels[num, 1];
			if (num2 >= 0 && num2 < pca.Width && num3 >= 0 && num3 <= pca.Height && pca.IsSolid(num2, num3))
			{
				next = new Vector2((float)num2, (float)num3);
				return true;
			}
		}
		next = Vector2.Zero;
		return false;
	}

	private static bool SearchForOutstandingVertex(ref Vertices hullArea, float hullTolerance, out Vector2 outstanding)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		int num = hullArea.Count - 1;
		Vector2 val = Vector2.Zero;
		bool result = false;
		for (int i = 1; i < num; i++)
		{
			Vector2 point = hullArea[i];
			Vector2 lineEndPoint = hullArea[0];
			Vector2 lineEndPoint2 = hullArea[num];
			if (Calculator.DistanceBetweenPointAndLineSegment(ref point, ref lineEndPoint, ref lineEndPoint2) >= hullTolerance)
			{
				val = hullArea[i];
				result = true;
				break;
			}
		}
		outstanding = val;
		return result;
	}

	private static int GetIndexOfFirstPixelToCheck(Vector2 last, Vector2 current)
	{
		switch ((int)(current.X - last.X))
		{
		case 1:
			switch ((int)(current.Y - last.Y))
			{
			case 1:
				return 1;
			case 0:
				return 0;
			case -1:
				return 7;
			}
			break;
		case 0:
			switch ((int)(current.Y - last.Y))
			{
			case 1:
				return 2;
			case -1:
				return 6;
			}
			break;
		case -1:
			switch ((int)(current.Y - last.Y))
			{
			case 1:
				return 3;
			case 0:
				return 4;
			case -1:
				return 5;
			}
			break;
		}
		return 0;
	}

	public static Vertices Union(Vertices polygon1, Vertices polygon2, out PolyUnionError error)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		int num = PreparePolygons(polygon1, polygon2, out var poly, out var poly2, out var intersections, out error);
		if (num == -1)
		{
			switch (error)
			{
			case PolyUnionError.NoIntersections:
				return null;
			case PolyUnionError.Poly1InsidePoly2:
				return polygon2;
			}
		}
		Vertices vertices = new Vertices();
		Vertices vertices2 = poly;
		Vertices vertices3 = poly2;
		Vector2 val = poly[num];
		int index = num;
		do
		{
			vertices.Add(vertices2[index]);
			foreach (EdgeIntersectInfo item in intersections)
			{
				if (!(vertices2[index] == item.IntersectionPoint))
				{
					continue;
				}
				int num2 = vertices3.IndexOf(item.IntersectionPoint);
				if (!PointInPolygonAngle(vertices3[vertices3.NextIndex(num2)], vertices2))
				{
					if (vertices2 == poly)
					{
						vertices2 = poly2;
						vertices3 = poly;
					}
					else
					{
						vertices2 = poly;
						vertices3 = poly2;
					}
					index = num2;
					break;
				}
			}
			index = vertices2.NextIndex(index);
		}
		while (vertices2[index] != val && vertices.Count <= poly.Count + poly2.Count);
		if (vertices.Count > poly.Count + poly2.Count)
		{
			error = PolyUnionError.InfiniteLoop;
		}
		return vertices;
	}

	public static Vertices Subtract(Vertices polygon1, Vertices polygon2, out PolyUnionError error)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		int num = PreparePolygons(polygon1, polygon2, out var poly, out var poly2, out var intersections, out error);
		if (num == -1)
		{
			switch (error)
			{
			case PolyUnionError.NoIntersections:
				return null;
			case PolyUnionError.Poly1InsidePoly2:
				return null;
			}
		}
		Vertices vertices = new Vertices();
		Vertices vertices2 = poly;
		Vertices vertices3 = poly2;
		Vector2 val = poly[num];
		int index = num;
		bool flag = true;
		do
		{
			vertices.Add(vertices2[index]);
			foreach (EdgeIntersectInfo item in intersections)
			{
				if (!(vertices2[index] == item.IntersectionPoint))
				{
					continue;
				}
				int num2 = vertices3.IndexOf(item.IntersectionPoint);
				Vector2 point;
				if (flag)
				{
					point = vertices3[vertices3.PreviousIndex(num2)];
					if (PointInPolygonAngle(point, vertices2))
					{
						if (vertices2 == poly)
						{
							vertices2 = poly2;
							vertices3 = poly;
						}
						else
						{
							vertices2 = poly;
							vertices3 = poly2;
						}
						index = num2;
						flag = !flag;
						break;
					}
					continue;
				}
				point = vertices3[vertices3.NextIndex(num2)];
				if (!PointInPolygonAngle(point, vertices2))
				{
					if (vertices2 == poly)
					{
						vertices2 = poly2;
						vertices3 = poly;
					}
					else
					{
						vertices2 = poly;
						vertices3 = poly2;
					}
					index = num2;
					flag = !flag;
					break;
				}
			}
			index = ((!flag) ? vertices2.PreviousIndex(index) : vertices2.NextIndex(index));
		}
		while (vertices2[index] != val && vertices.Count <= poly.Count + poly2.Count);
		if (vertices.Count > poly.Count + poly2.Count)
		{
			error = PolyUnionError.InfiniteLoop;
		}
		return vertices;
	}

	public static Vertices Intersect(Vertices polygon1, Vertices polygon2, out PolyUnionError error)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		error = PolyUnionError.None;
		int num = PreparePolygons(polygon1, polygon2, out var poly, out var poly2, out var intersections, out var error2);
		if (num == -1)
		{
			switch (error2)
			{
			case PolyUnionError.NoIntersections:
				return null;
			case PolyUnionError.Poly1InsidePoly2:
				return polygon2;
			}
		}
		Vertices vertices = new Vertices();
		Vertices vertices2 = poly;
		Vertices vertices3 = poly2;
		int index = poly.IndexOf(intersections[0].IntersectionPoint);
		Vector2 val = poly[index];
		do
		{
			vertices.Add(vertices2[index]);
			foreach (EdgeIntersectInfo item in intersections)
			{
				if (!(vertices2[index] == item.IntersectionPoint))
				{
					continue;
				}
				int num2 = vertices3.IndexOf(item.IntersectionPoint);
				if (PointInPolygonAngle(vertices3[vertices3.NextIndex(num2)], vertices2))
				{
					if (vertices2 == poly)
					{
						vertices2 = poly2;
						vertices3 = poly;
					}
					else
					{
						vertices2 = poly;
						vertices3 = poly2;
					}
					index = num2;
					break;
				}
			}
			index = vertices2.NextIndex(index);
		}
		while (vertices2[index] != val && vertices.Count <= poly.Count + poly2.Count);
		if (vertices.Count > poly.Count + poly2.Count)
		{
			error = PolyUnionError.InfiniteLoop;
		}
		return vertices;
	}

	private static int PreparePolygons(Vertices polygon1, Vertices polygon2, out Vertices poly1, out Vertices poly2, out List<EdgeIntersectInfo> intersections, out PolyUnionError error)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		error = PolyUnionError.None;
		poly1 = Round(polygon1);
		poly2 = Round(polygon2);
		intersections = new List<EdgeIntersectInfo>();
		if (!VerticesIntersect(poly1, poly2, ref intersections))
		{
			error = PolyUnionError.NoIntersections;
			return -1;
		}
		foreach (EdgeIntersectInfo intersection in intersections)
		{
			if (!poly1.Contains(intersection.IntersectionPoint))
			{
				poly1.Insert(poly1.IndexOf(intersection.EdgeOne.EdgeStart) + 1, intersection.IntersectionPoint);
			}
			if (!poly2.Contains(intersection.IntersectionPoint))
			{
				poly2.Insert(poly2.IndexOf(intersection.EdgeTwo.EdgeStart) + 1, intersection.IntersectionPoint);
			}
		}
		int num = -1;
		int num2 = 0;
		do
		{
			if (!PointInPolygonAngle(poly1[num2], poly2))
			{
				num = num2;
				break;
			}
			num2 = poly1.NextIndex(num2);
		}
		while (num2 != 0);
		if (num == -1)
		{
			error = PolyUnionError.Poly1InsidePoly2;
		}
		return num;
	}

	private static bool VerticesIntersect(Vertices polygon1, Vertices polygon2, ref List<EdgeIntersectInfo> intersections)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		intersections.Clear();
		for (int i = 0; i < polygon1.Count; i++)
		{
			Vector2 val = polygon1[i];
			Vector2 val2 = polygon1[polygon1.NextIndex(i)];
			for (int j = 0; j < polygon2.Count; j++)
			{
				Vector2 val3 = polygon2[j];
				Vector2 val4 = polygon2[polygon2.NextIndex(j)];
				if (RayHelper.LineIntersect(val, val2, val3, val4, firstIsSegment: true, secondIsSegment: true, 1E-05f, out var intersectionPoint))
				{
					intersectionPoint = new Vector2((float)Math.Round(intersectionPoint.X, 0), (float)Math.Round(intersectionPoint.Y, 0));
					intersections.Add(new EdgeIntersectInfo(new Edge(val, val2), new Edge(val3, val4), intersectionPoint));
				}
			}
		}
		return intersections.Count > 0;
	}

	private static bool PointInPolygonAngle(Vector2 point, Vertices polygon)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		double num = 0.0;
		for (int i = 0; i < polygon.Count; i++)
		{
			Vector2 p = polygon[i] - point;
			Vector2 p2 = polygon[polygon.NextIndex(i)] - point;
			num += VectorAngle(p, p2);
		}
		if (Math.Abs(num) < Math.PI)
		{
			return false;
		}
		return true;
	}

	private static double VectorAngle(Vector2 p1, Vector2 p2)
	{
		double num = Math.Atan2(p1.Y, p1.X);
		double num2 = Math.Atan2(p2.Y, p2.X);
		double num3;
		for (num3 = num2 - num; num3 > Math.PI; num3 -= Math.PI * 2.0)
		{
		}
		for (; num3 < -Math.PI; num3 += Math.PI * 2.0)
		{
		}
		return num3;
	}

	public static Vertices Round(Vertices polygon)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		for (int i = 0; i < polygon.Count; i++)
		{
			vertices.Add(new Vector2((float)Math.Round(polygon[i].X, 0), (float)Math.Round(polygon[i].Y, 0)));
		}
		return vertices;
	}

	private static bool VerticesAreCollinear(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		double num = (p3.X - p1.X) * (p2.Y - p1.Y) + (p3.Y - p1.Y) * (p1.X - p2.X);
		return num == 0.0;
	}

	public static Vertices Simplify(Vertices polygon, int bias)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (polygon.Count < 3)
		{
			return polygon;
		}
		Vertices vertices = new Vertices();
		Vertices vertices2 = Round(polygon);
		for (int i = 0; i < vertices2.Count; i++)
		{
			int index = vertices2.PreviousIndex(i);
			int index2 = vertices2.NextIndex(i);
			Vector2 val = vertices2[index] - vertices2[i];
			if (!(((Vector2)(ref val)).Length() <= (float)bias) && !VerticesAreCollinear(vertices2[index], vertices2[i], vertices2[index2]))
			{
				vertices.Add(vertices2[i]);
			}
		}
		return vertices;
	}

	public static Vertices Simplify(Vertices polygon)
	{
		return Simplify(polygon, 0);
	}

	public static Vertices CreateCapsule(float height, float endRadius, int edges)
	{
		if (endRadius >= height / 2f)
		{
			throw new ArgumentException("The radius must be lower than height / 2. Higher values of radius would create a circle, and not a half circle.", "endRadius");
		}
		return CreateCapsule(height, endRadius, edges, endRadius, edges);
	}

	public static Vertices CreateCapsule(float height, float topRadius, int topEdges, float bottomRadius, int bottomEdges)
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		if (height <= 0f)
		{
			throw new ArgumentException("Height must be longer than 0", "height");
		}
		if (topRadius <= 0f)
		{
			throw new ArgumentException("The top radius must be more than 0", "topRadius");
		}
		if (topEdges <= 0)
		{
			throw new ArgumentException("Top edges must be more than 0", "topEdges");
		}
		if (bottomRadius <= 0f)
		{
			throw new ArgumentException("The bottom radius must be more than 0", "bottomRadius");
		}
		if (bottomEdges <= 0)
		{
			throw new ArgumentException("Bottom edges must be more than 0", "bottomEdges");
		}
		if (topRadius >= height / 2f)
		{
			throw new ArgumentException("The top radius must be lower than height / 2. Higher values of top radius would create a circle, and not a half circle.", "topRadius");
		}
		if (bottomRadius >= height / 2f)
		{
			throw new ArgumentException("The bottom radius must be lower than height / 2. Higher values of bottom radius would create a circle, and not a half circle.", "bottomRadius");
		}
		Vertices vertices = new Vertices();
		float num = (height - topRadius - bottomRadius) * 0.5f;
		vertices.Add(new Vector2(topRadius, num));
		float num2 = (float)Math.PI / (float)topEdges;
		for (int i = 1; i < topEdges; i++)
		{
			vertices.Add(new Vector2(topRadius * Calculator.Cos(num2 * (float)i), topRadius * Calculator.Sin(num2 * (float)i) + num));
		}
		vertices.Add(new Vector2(0f - topRadius, num));
		vertices.Add(new Vector2(0f - bottomRadius, 0f - num));
		num2 = (float)Math.PI / (float)bottomEdges;
		for (int j = 1; j < bottomEdges; j++)
		{
			vertices.Add(new Vector2((0f - bottomRadius) * Calculator.Cos(num2 * (float)j), (0f - bottomRadius) * Calculator.Sin(num2 * (float)j) - num));
		}
		vertices.Add(new Vector2(bottomRadius, 0f - num));
		return vertices;
	}

	public static void Triangulate(Vector2[] inputVertices, WindingOrder desiredWindingOrder, out Vector2[] outputVertices, out short[] indices)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<Triangle> list = new List<Triangle>();
		if (DetermineWindingOrder(inputVertices) == WindingOrder.Clockwise)
		{
			outputVertices = ReverseWindingOrder(inputVertices);
		}
		else
		{
			outputVertices = (Vector2[])inputVertices.Clone();
		}
		polygonVertices.Clear();
		earVertices.Clear();
		convexVertices.Clear();
		reflexVertices.Clear();
		for (int i = 0; i < outputVertices.Length; i++)
		{
			polygonVertices.AddLast(new Vertex(outputVertices[i], (short)i));
		}
		FindConvexAndReflexVertices();
		FindEarVertices();
		while (polygonVertices.Count > 3 && earVertices.Count > 0)
		{
			ClipNextEar(list);
		}
		if (polygonVertices.Count == 3)
		{
			list.Add(new Triangle(polygonVertices[0].Value, polygonVertices[1].Value, polygonVertices[2].Value));
		}
		indices = new short[list.Count * 3];
		if (desiredWindingOrder == WindingOrder.CounterClockwise)
		{
			for (int j = 0; j < list.Count; j++)
			{
				indices[j * 3] = list[j].A.Index;
				indices[j * 3 + 1] = list[j].B.Index;
				indices[j * 3 + 2] = list[j].C.Index;
			}
		}
		else
		{
			for (int k = 0; k < list.Count; k++)
			{
				indices[k * 3] = list[k].C.Index;
				indices[k * 3 + 1] = list[k].B.Index;
				indices[k * 3 + 2] = list[k].A.Index;
			}
		}
	}

	public static Vector2[] CutHoleInShape(Vector2[] shapeVerts, Vector2[] holeVerts)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		Log("\nCutting hole into shape...");
		shapeVerts = EnsureWindingOrder(shapeVerts, WindingOrder.CounterClockwise);
		holeVerts = EnsureWindingOrder(holeVerts, WindingOrder.Clockwise);
		polygonVertices.Clear();
		earVertices.Clear();
		convexVertices.Clear();
		reflexVertices.Clear();
		for (int i = 0; i < shapeVerts.Length; i++)
		{
			polygonVertices.AddLast(new Vertex(shapeVerts[i], (short)i));
		}
		CyclicalList<Vertex> cyclicalList = new CyclicalList<Vertex>();
		for (int j = 0; j < holeVerts.Length; j++)
		{
			cyclicalList.Add(new Vertex(holeVerts[j], (short)(j + polygonVertices.Count)));
		}
		FindConvexAndReflexVertices();
		FindEarVertices();
		Vertex vertex = cyclicalList[0];
		foreach (Vertex item in cyclicalList)
		{
			if (item.Position.X > vertex.Position.X)
			{
				vertex = item;
			}
		}
		List<LineSegment> list = new List<LineSegment>();
		for (int k = 0; k < polygonVertices.Count; k++)
		{
			Vertex value = polygonVertices[k].Value;
			Vertex value2 = polygonVertices[k + 1].Value;
			if ((value.Position.X > vertex.Position.X || value2.Position.X > vertex.Position.X) && ((value.Position.Y >= vertex.Position.Y && value2.Position.Y <= vertex.Position.Y) || (value.Position.Y <= vertex.Position.Y && value2.Position.Y >= vertex.Position.Y)))
			{
				list.Add(new LineSegment(value, value2));
			}
		}
		float? num = null;
		LineSegment lineSegment = default(LineSegment);
		foreach (LineSegment item2 in list)
		{
			float? num2 = item2.IntersectsWithRay(vertex.Position, Vector2.UnitX);
			if (num2.HasValue && (!num.HasValue || num.Value > num2.Value))
			{
				num = num2;
				lineSegment = item2;
			}
		}
		if (!num.HasValue)
		{
			return shapeVerts;
		}
		Vector2 position = vertex.Position + Vector2.UnitX * num.Value;
		Vertex vertex2 = ((lineSegment.A.Position.X > lineSegment.B.Position.X) ? lineSegment.A : lineSegment.B);
		Triangle triangle = new Triangle(vertex, new Vertex(position, 1), vertex2);
		List<Vertex> list2 = new List<Vertex>();
		foreach (Vertex reflexVertex in reflexVertices)
		{
			if (triangle.ContainsPoint(reflexVertex))
			{
				list2.Add(reflexVertex);
			}
		}
		if (list2.Count > 0)
		{
			float num3 = -1f;
			foreach (Vertex item3 in list2)
			{
				Vector2 val = Vector2.Normalize(item3.Position - vertex.Position);
				float num4 = Vector2.Dot(Vector2.UnitX, val);
				if (num4 > num3)
				{
					num3 = num4;
					vertex2 = item3;
				}
			}
		}
		int num5 = cyclicalList.IndexOf(vertex);
		int index = polygonVertices.IndexOf(vertex2);
		Log("Inserting hole at injection point {0} starting at hole vertex {1}.", vertex2, vertex);
		for (int l = num5; l <= num5 + cyclicalList.Count; l++)
		{
			Log("Inserting vertex {0} after vertex {1}.", cyclicalList[l], polygonVertices[index].Value);
			polygonVertices.AddAfter(polygonVertices[index++], cyclicalList[l]);
		}
		polygonVertices.AddAfter(polygonVertices[index], vertex2);
		Vector2[] array = (Vector2[])(object)new Vector2[polygonVertices.Count];
		for (int m = 0; m < polygonVertices.Count; m++)
		{
			ref Vector2 reference = ref array[m];
			reference = polygonVertices[m].Value.Position;
		}
		return array;
	}

	public static Vector2[] EnsureWindingOrder(Vector2[] vertices, WindingOrder windingOrder)
	{
		if (DetermineWindingOrder(vertices) != windingOrder)
		{
			return ReverseWindingOrder(vertices);
		}
		return vertices;
	}

	public static Vector2[] ReverseWindingOrder(Vector2[] vertices)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Vector2[] array = (Vector2[])(object)new Vector2[vertices.Length];
		ref Vector2 reference = ref array[0];
		reference = vertices[0];
		for (int i = 1; i < array.Length; i++)
		{
			ref Vector2 reference2 = ref array[i];
			reference2 = vertices[vertices.Length - i];
		}
		return array;
	}

	public static WindingOrder DetermineWindingOrder(Vector2[] vertices)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		Vector2 val = vertices[0];
		for (int i = 1; i < vertices.Length; i++)
		{
			Vector2 val2 = vertices[i];
			Vector2 val3 = vertices[(i + 1) % vertices.Length];
			Vector2 val4 = val - val2;
			Vector2 val5 = val3 - val2;
			if (val4.X * val5.Y - val4.Y * val5.X >= 0f)
			{
				num++;
			}
			else
			{
				num2++;
			}
			val = val2;
		}
		if (num <= num2)
		{
			return WindingOrder.CounterClockwise;
		}
		return WindingOrder.Clockwise;
	}

	private static void ClipNextEar(ICollection<Triangle> triangles)
	{
		Vertex value = earVertices[0].Value;
		Vertex value2 = polygonVertices[polygonVertices.IndexOf(value) - 1].Value;
		Vertex value3 = polygonVertices[polygonVertices.IndexOf(value) + 1].Value;
		triangles.Add(new Triangle(value, value3, value2));
		earVertices.RemoveAt(0);
		polygonVertices.RemoveAt(polygonVertices.IndexOf(value));
		ValidateAdjacentVertex(value2);
		ValidateAdjacentVertex(value3);
	}

	private static void ValidateAdjacentVertex(Vertex vertex)
	{
		if (reflexVertices.Contains(vertex) && IsConvex(vertex))
		{
			reflexVertices.Remove(vertex);
			convexVertices.Add(vertex);
		}
		if (convexVertices.Contains(vertex))
		{
			bool flag = earVertices.Contains(vertex);
			bool flag2 = IsEar(vertex);
			if (flag && !flag2)
			{
				earVertices.Remove(vertex);
			}
			else if (!flag && flag2)
			{
				earVertices.AddFirst(vertex);
			}
		}
	}

	private static void FindConvexAndReflexVertices()
	{
		for (int i = 0; i < polygonVertices.Count; i++)
		{
			Vertex value = polygonVertices[i].Value;
			if (IsConvex(value))
			{
				convexVertices.Add(value);
			}
			else
			{
				reflexVertices.Add(value);
			}
		}
	}

	private static void FindEarVertices()
	{
		for (int i = 0; i < convexVertices.Count; i++)
		{
			Vertex vertex = convexVertices[i];
			if (IsEar(vertex))
			{
				earVertices.AddLast(vertex);
			}
		}
	}

	private static bool IsEar(Vertex c)
	{
		Vertex value = polygonVertices[polygonVertices.IndexOf(c) - 1].Value;
		Vertex value2 = polygonVertices[polygonVertices.IndexOf(c) + 1].Value;
		foreach (Vertex reflexVertex in reflexVertices)
		{
			if (!reflexVertex.Equals(value) && !reflexVertex.Equals(c) && !reflexVertex.Equals(value2) && Triangle.ContainsPoint(value, c, value2, reflexVertex))
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsConvex(Vertex c)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		Vertex value = polygonVertices[polygonVertices.IndexOf(c) - 1].Value;
		Vertex value2 = polygonVertices[polygonVertices.IndexOf(c) + 1].Value;
		Vector2 val = Vector2.Normalize(c.Position - value.Position);
		Vector2 val2 = Vector2.Normalize(value2.Position - c.Position);
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(0f - val2.Y, val2.X);
		return Vector2.Dot(val, val3) <= 0f;
	}

	private static bool IsReflex(Vertex c)
	{
		return !IsConvex(c);
	}

	private static void Log(string format, params object[] parameters)
	{
		Console.WriteLine(format, parameters);
	}

	public static List<Geom> DecomposeGeom(Vertices vertices, Body body, int maxPolysToFind)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Vertices[] array = Polygon.DecomposeVertices(vertices, maxPolysToFind);
		List<Geom> list = new List<Geom>();
		Vector2 centroid = vertices.GetCentroid();
		Vertices[] array2 = array;
		foreach (Vertices vertices2 in array2)
		{
			list.Add(new Geom(body, vertices2, -centroid, 0f, 1f));
		}
		return list;
	}

	public Vertices GetConvexHull()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		if (base.Count < 3)
		{
			return this;
		}
		Vector2[] array = (Vector2[])(object)new Vector2[base.Count + 1];
		int num = 3;
		int num2 = 0;
		int i = 3;
		float num3 = IsLeft(base[0], base[1], base[2]);
		if (num3 == 0f)
		{
			ref Vector2 reference = ref array[0];
			reference = base[0];
			ref Vector2 reference2 = ref array[1];
			reference2 = base[2];
			ref Vector2 reference3 = ref array[2];
			reference3 = base[0];
			num = 2;
			for (i = 3; i < base.Count && IsLeft(array[0], array[1], base[i]) == 0f; i++)
			{
				ref Vector2 reference4 = ref array[1];
				reference4 = base[i];
			}
		}
		else
		{
			ref Vector2 reference5 = ref array[0];
			ref Vector2 reference6 = ref array[3];
			reference5 = (reference6 = base[2]);
			if (num3 > 0f)
			{
				ref Vector2 reference7 = ref array[1];
				reference7 = base[0];
				ref Vector2 reference8 = ref array[2];
				reference8 = base[1];
			}
			else
			{
				ref Vector2 reference9 = ref array[1];
				reference9 = base[1];
				ref Vector2 reference10 = ref array[2];
				reference10 = base[0];
			}
		}
		int num4 = ((num == 0) ? (array.Length - 1) : (num - 1));
		int num5 = ((num2 != array.Length - 1) ? (num2 + 1) : 0);
		for (int j = i; j < base.Count; j++)
		{
			Vector2 val = base[j];
			if (!(IsLeft(array[num4], array[num], val) > 0f) || !(IsLeft(array[num2], array[num5], val) > 0f))
			{
				while (!(IsLeft(array[num4], array[num], val) > 0f))
				{
					num = num4;
					num4 = ((num == 0) ? (array.Length - 1) : (num - 1));
				}
				num = ((num != array.Length - 1) ? (num + 1) : 0);
				num4 = ((num == 0) ? (array.Length - 1) : (num - 1));
				array[num] = val;
				while (!(IsLeft(array[num2], array[num5], val) > 0f))
				{
					num2 = num5;
					num5 = ((num2 != array.Length - 1) ? (num2 + 1) : 0);
				}
				num2 = ((num2 == 0) ? (array.Length - 1) : (num2 - 1));
				num5 = ((num2 != array.Length - 1) ? (num2 + 1) : 0);
				array[num2] = val;
			}
		}
		Vertices vertices = new Vertices(base.Count + 1);
		if (num2 < num)
		{
			for (int k = num2; k < num; k++)
			{
				vertices.Add(array[k]);
			}
		}
		else
		{
			for (int l = 0; l < num; l++)
			{
				vertices.Add(array[l]);
			}
			for (int m = num2; m < array.Length; m++)
			{
				vertices.Add(array[m]);
			}
		}
		return vertices;
	}

	private float IsLeft(Vector2 a, Vector2 b, Vector2 c)
	{
		return (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
	}
}
