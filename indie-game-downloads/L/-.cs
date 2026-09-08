using System;
using Microsoft.Xna.Framework;
using N;

namespace l;

internal class _0006<T> where T : struct
{
	public T[] Elements;

	internal int a5h;

	public int Count => a5h;

	public int Capacity
	{
		get
		{
			return Elements.Length;
		}
		set
		{
			T[] array = new T[value];
			Array.Copy(Elements, array, a5h);
			Elements = array;
		}
	}

	public _0006()
	{
		Elements = new T[4];
	}

	public _0006(int initialCapacity)
	{
		if (initialCapacity <= 0)
		{
			throw new ArgumentException("Initial capacity must be positive.");
		}
		Elements = new T[initialCapacity];
	}

	public void RemoveAt(int index)
	{
		if (index >= a5h)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		a5h--;
		if (index < a5h)
		{
			Elements[index] = Elements[a5h];
		}
	}

	public void Add(ref T item)
	{
		if (a5h == Elements.Length)
		{
			Capacity = Elements.Length * 2;
		}
		Elements[a5h++] = item;
	}

	public void Clear()
	{
		a5h = 0;
	}

	public bool Remove(ref T item)
	{
		int num = IndexOf(ref item);
		if (num == -1)
		{
			return false;
		}
		RemoveAt(num);
		return true;
	}

	public int IndexOf(ref T item)
	{
		return Array.IndexOf(Elements, item);
	}
}
internal struct _0018<T> where T : IEquatable<T>
{
	private T a5h;

	private T a5b;

	private T a56;

	private T a5a;

	private T a57;

	private T a5_0006;

	private T a5v;

	private T a5B;

	internal int a5X;

	public int Count => a5X;

	public T this[int index]
	{
		get
		{
			if (index > a5X - 1 || index < 0)
			{
				return default(T);
			}
			return index switch
			{
				0 => a5h, 
				1 => a5b, 
				2 => a56, 
				3 => a5a, 
				4 => a57, 
				5 => a5_0006, 
				6 => a5v, 
				7 => a5B, 
				_ => default(T), 
			};
		}
		set
		{
			if (index <= a5X - 1)
			{
				switch (index)
				{
				case 0:
					a5h = value;
					break;
				case 1:
					a5b = value;
					break;
				case 2:
					a56 = value;
					break;
				case 3:
					a5a = value;
					break;
				case 4:
					a57 = value;
					break;
				case 5:
					a5_0006 = value;
					break;
				case 6:
					a5v = value;
					break;
				case 7:
					a5B = value;
					break;
				}
			}
		}
	}

	public override string ToString()
	{
		return string.Concat("TinyList<", typeof(T), ">, Count: ", a5X);
	}

	public bool Add(T item)
	{
		switch (a5X)
		{
		case 0:
			a5h = item;
			break;
		case 1:
			a5b = item;
			break;
		case 2:
			a56 = item;
			break;
		case 3:
			a5a = item;
			break;
		case 4:
			a57 = item;
			break;
		case 5:
			a5_0006 = item;
			break;
		case 6:
			a5v = item;
			break;
		case 7:
			a5B = item;
			break;
		default:
			return false;
		}
		a5X++;
		return true;
	}

	public void Clear()
	{
		a5X = 0;
		a5h = default(T);
		a5b = default(T);
		a56 = default(T);
		a5a = default(T);
		a57 = default(T);
		a5_0006 = default(T);
		a5v = default(T);
		a5B = default(T);
	}

	public int IndexOf(T item)
	{
		if (a5h.Equals(item))
		{
			return 0;
		}
		if (a5b.Equals(item))
		{
			return 1;
		}
		if (a56.Equals(item))
		{
			return 2;
		}
		if (a5a.Equals(item))
		{
			return 3;
		}
		if (a57.Equals(item))
		{
			return 4;
		}
		if (a5_0006.Equals(item))
		{
			return 5;
		}
		if (a5v.Equals(item))
		{
			return 6;
		}
		if (a5B.Equals(item))
		{
			return 7;
		}
		return -1;
	}

	public bool Remove(T item)
	{
		int num = IndexOf(item);
		if (num != -1)
		{
			RemoveAt(num);
			return true;
		}
		return false;
	}

	public bool RemoveAt(int index)
	{
		if (index > a5X - 1 || index < 0)
		{
			return false;
		}
		switch (index)
		{
		case 0:
			a5h = a5b;
			a5b = a56;
			a56 = a5a;
			a5a = a57;
			a57 = a5_0006;
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 1:
			a5b = a56;
			a56 = a5a;
			a5a = a57;
			a57 = a5_0006;
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 2:
			a56 = a5a;
			a5a = a57;
			a57 = a5_0006;
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 3:
			a5a = a57;
			a57 = a5_0006;
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 4:
			a57 = a5_0006;
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 5:
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 6:
			a5v = a5B;
			break;
		}
		a5X--;
		return true;
	}
}
internal class _0002 : _6
{
	internal new N.h a5h = N.h.Identity;

	public N.h WorldTransform
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

	public _0002(Vector3[] vertices, int[] indices)
	{
		base.Vertices = vertices;
		base.Indices = indices;
	}

	public _0002(Vector3[] vertices, int[] indices, N.h worldTransform)
	{
		a5h = worldTransform;
		base.Vertices = vertices;
		base.Indices = indices;
	}

	public override void GetTriangle(int triangleIndex, out Vector3 v1, out Vector3 v2, out Vector3 v3)
	{
		N.h.Transform(ref a5b[base.a5h[triangleIndex]], ref a5h, out v1);
		N.h.Transform(ref a5b[base.a5h[triangleIndex + 1]], ref a5h, out v2);
		N.h.Transform(ref a5b[base.a5h[triangleIndex + 2]], ref a5h, out v3);
	}

	public override void GetVertexPosition(int i, out Vector3 vertex)
	{
		N.h.Transform(ref a5b[i], ref a5h, out vertex);
	}
}
internal struct _000E<T1, T2>(T1 overlapA, T2 overlapB)
{
	public T1 OverlapA = overlapA;

	public T2 OverlapB = overlapB;
}
