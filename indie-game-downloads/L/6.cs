using Microsoft.Xna.Framework;

namespace l;

internal abstract class _6
{
	internal int[] a5h;

	internal Vector3[] a5b;

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

	public void GetBoundingBox(int triangleIndex, out BoundingBox boundingBox)
	{
		GetTriangle(triangleIndex, out var v2, out var v3, out var v4);
		Vector3.Min(ref v2, ref v3, out boundingBox.Min);
		Vector3.Min(ref boundingBox.Min, ref v4, out boundingBox.Min);
		Vector3.Max(ref v2, ref v3, out boundingBox.Max);
		Vector3.Max(ref boundingBox.Max, ref v4, out boundingBox.Max);
	}

	public abstract void GetTriangle(int triangleIndex, out Vector3 v1, out Vector3 v2, out Vector3 v3);

	public abstract void GetVertexPosition(int i, out Vector3 vertex);
}
