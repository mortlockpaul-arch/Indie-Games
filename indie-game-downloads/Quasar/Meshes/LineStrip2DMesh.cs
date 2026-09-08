using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global.Utils;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class LineStrip2DMesh : UserVertexMesh<VertexPositionColorUV>, IList<Vector2>, ICollection<Vector2>, IEnumerable<Vector2>, IEnumerable
{
	public struct LinePoint
	{
		public Vector2 position;

		public Vector3 diffuse;

		public float alpha;
	}

	private const int MAX_VERTICES = 1024;

	private LinePoint[] points;

	private int pointCount;

	private float width;

	private float UVLength;

	private float uvOffset;

	private float zValue;

	private bool useXZCoords;

	private bool autoUV = true;

	private List<Vector2> finalVertices;

	public LinePoint[] Points => points;

	public int PointCount
	{
		get
		{
			return pointCount;
		}
		set
		{
			pointCount = value;
		}
	}

	public float UVOffset
	{
		get
		{
			return uvOffset;
		}
		set
		{
			uvOffset = value;
		}
	}

	public float ZValue
	{
		get
		{
			return zValue;
		}
		set
		{
			zValue = value;
		}
	}

	public bool UseXZCoords
	{
		get
		{
			return useXZCoords;
		}
		set
		{
			useXZCoords = value;
		}
	}

	public bool AutoUV
	{
		get
		{
			return autoUV;
		}
		set
		{
			autoUV = value;
		}
	}

	Vector2 IList<Vector2>.this[int index]
	{
		get
		{
			return points[index].position;
		}
		set
		{
		}
	}

	int ICollection<Vector2>.Count => pointCount;

	bool ICollection<Vector2>.IsReadOnly => false;

	private void initializeBuffer(int maxPoints)
	{
		initMesh(2 * maxPoints);
		points = new LinePoint[maxPoints];
		finalVertices = new List<Vector2>(2 * maxPoints);
		base.PrimitiveCount = 0;
		base.PrimitiveType = PrimitiveType.TriangleStrip;
	}

	public void SetPoints(List<Vector2> points)
	{
		int num = 0;
		foreach (Vector2 point in points)
		{
			this.points[num].alpha = 1f;
			this.points[num].diffuse = Vector3.One;
			this.points[num].position = point;
			num++;
			if (num >= this.points.Length)
			{
				break;
			}
		}
		pointCount = num;
	}

	public void updateMesh()
	{
		LineStrip.Process(finalVertices, this, width, useXZCoords);
		int num = Math.Min(finalVertices.Count, verticesBuffer.Length);
		for (int i = 0; i < num; i++)
		{
			LinePoint linePoint = points[i / 2];
			if (!useXZCoords)
			{
				verticesBuffer[i].Position = new Vector3(finalVertices[i], zValue);
			}
			else
			{
				verticesBuffer[i].Position = new Vector3(finalVertices[i].X, zValue, finalVertices[i].Y);
			}
			verticesBuffer[i].Color = new Color(linePoint.diffuse.X, linePoint.diffuse.Y, linePoint.diffuse.Z, linePoint.alpha);
		}
		float num2 = 0f;
		for (int j = 0; j < Math.Min(pointCount, verticesBuffer.Length / 2); j++)
		{
			if (autoUV)
			{
				verticesBuffer[2 * j].UV = new Vector2(num2 + uvOffset, 0f);
				verticesBuffer[2 * j + 1].UV = new Vector2(num2 + uvOffset, 1f);
				if (j < pointCount - 1)
				{
					num2 += Vector2.Distance(points[j].position, points[j + 1].position) / UVLength;
				}
			}
			else
			{
				verticesBuffer[2 * j].UV = new Vector2(num2 + uvOffset, 0f);
				verticesBuffer[2 * j + 1].UV = new Vector2(num2 + uvOffset, 1f);
				if (j < pointCount - 1)
				{
					num2 += UVLength;
				}
			}
		}
		updateBoundingSphere(num);
		base.PrimitiveCount = num - 2;
	}

	public LineStrip2DMesh(float width, float UVLength, int maxPoints)
	{
		this.width = width;
		this.UVLength = UVLength;
		initializeBuffer(maxPoints);
		Material item = new Material();
		materials.Add(item);
		shader = ShaderManager.Shaders["GUIColor"];
	}

	public LineStrip2DMesh(ReadOnlyCollection<Vector2> points, float width, float UVLength, int maxVertices)
		: this(width, UVLength, maxVertices)
	{
		int num = 0;
		foreach (Vector2 point in points)
		{
			this.points[num].alpha = 1f;
			this.points[num].diffuse = Vector3.One;
			this.points[num].position = point;
			num++;
			if (num >= maxVertices)
			{
				break;
			}
		}
		pointCount = num;
		updateMesh();
	}

	public LineStrip2DMesh(ReadOnlyCollection<Vector2> points, float width, float UVLength)
		: this(points, width, UVLength, 1024)
	{
	}

	int IList<Vector2>.IndexOf(Vector2 item)
	{
		return -1;
	}

	void IList<Vector2>.Insert(int index, Vector2 item)
	{
	}

	void IList<Vector2>.RemoveAt(int index)
	{
	}

	void ICollection<Vector2>.Add(Vector2 item)
	{
	}

	void ICollection<Vector2>.Clear()
	{
	}

	bool ICollection<Vector2>.Contains(Vector2 item)
	{
		return false;
	}

	void ICollection<Vector2>.CopyTo(Vector2[] array, int arrayIndex)
	{
	}

	bool ICollection<Vector2>.Remove(Vector2 item)
	{
		return false;
	}

	IEnumerator<Vector2> IEnumerable<Vector2>.GetEnumerator()
	{
		return null;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return null;
	}
}
