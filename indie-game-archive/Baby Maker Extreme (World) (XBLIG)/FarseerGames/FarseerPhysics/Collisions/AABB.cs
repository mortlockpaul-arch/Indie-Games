using System;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public struct AABB : IEquatable<AABB>
{
	private const float _epsilon = 1E-05f;

	private Vector2 _vector;

	internal Vector2 max;

	internal Vector2 min;

	public Vector2 Min
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return min;
		}
	}

	public Vector2 Max
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return max;
		}
	}

	public float Width => max.X - min.X;

	public float Height => max.Y - min.Y;

	public Vector2 Position
	{
		get
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2((min.X + max.X) / 2f, (min.Y + max.Y) / 2f);
		}
		set
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(value.X - Width / 2f, value.Y - Height / 2f);
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))._002Ector(value.X + Width / 2f, value.Y + Height / 2f);
			min = val;
			max = val2;
		}
	}

	public AABB(ref AABB aabb)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		min = aabb.Min;
		max = aabb.Max;
		_vector = Vector2.Zero;
	}

	public AABB(ref Vector2 min, ref Vector2 max)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		this.min = min;
		this.max = max;
		_vector = Vector2.Zero;
	}

	public Vertices GetVertices()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		vertices.Add(Min);
		vertices.Add(new Vector2(Min.X, Max.Y));
		vertices.Add(Max);
		vertices.Add(new Vector2(Max.X, Min.Y));
		return vertices;
	}

	public Vector2 GetCenter()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(Min.X + (Max.X - Min.X) / 2f, Min.Y + (Max.Y - Min.Y) / 2f);
	}

	public float GetShortestSide()
	{
		float val = max.X - min.X;
		float val2 = max.Y - min.Y;
		return Math.Min(val, val2);
	}

	public float GetDistance(ref Vector2 point)
	{
		GetDistance(ref point, out var distance);
		return distance;
	}

	public void GetDistance(ref Vector2 point, out float distance)
	{
		float num = Math.Abs(point.X - (max.X + min.X) * 0.5f) - (max.X - min.X) * 0.5f;
		float num2 = Math.Abs(point.Y - (max.Y + min.Y) * 0.5f) - (max.Y - min.Y) * 0.5f;
		if (num > 0f && num2 > 0f)
		{
			distance = (float)Math.Sqrt(num * num + num2 * num2);
		}
		else
		{
			distance = Math.Max(num, num2);
		}
	}

	internal void Update(ref Vertices vertices)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		min = vertices[0];
		max = min;
		for (int i = 0; i < vertices.Count; i++)
		{
			_vector = vertices[i];
			if (_vector.X < min.X)
			{
				min.X = _vector.X;
			}
			if (_vector.X > max.X)
			{
				max.X = _vector.X;
			}
			if (_vector.Y < min.Y)
			{
				min.Y = _vector.Y;
			}
			if (_vector.Y > max.Y)
			{
				max.Y = _vector.Y;
			}
		}
	}

	public bool Contains(Vector2 point)
	{
		if (point.X > min.X + 1E-05f && point.X < max.X - 1E-05f && point.Y > min.Y + 1E-05f && point.Y < max.Y - 1E-05f)
		{
			return true;
		}
		return false;
	}

	public bool Contains(ref Vector2 point)
	{
		if (point.X > min.X + 1E-05f && point.X < max.X - 1E-05f && point.Y > min.Y + 1E-05f && point.Y < max.Y - 1E-05f)
		{
			return true;
		}
		return false;
	}

	public static bool Intersect(ref AABB aabb1, ref AABB aabb2)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (aabb1.min.X > aabb2.max.X || aabb2.min.X > aabb1.max.X)
		{
			return false;
		}
		if (aabb1.min.Y > aabb2.Max.Y || aabb2.min.Y > aabb1.Max.Y)
		{
			return false;
		}
		return true;
	}

	public bool Equals(AABB other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (min == other.min)
		{
			return max == other.max;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is AABB)
		{
			return Equals((AABB)obj);
		}
		return false;
	}

	public bool Equals(ref AABB other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (min == other.min)
		{
			return max == other.max;
		}
		return false;
	}

	public override int GetHashCode()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return ((object)Min/*cast due to constrained. prefix*/).GetHashCode() + ((object)Max/*cast due to constrained. prefix*/).GetHashCode();
	}

	public static bool operator ==(AABB a, AABB b)
	{
		return a.Equals(ref b);
	}

	public static bool operator !=(AABB a, AABB b)
	{
		return !a.Equals(ref b);
	}
}
