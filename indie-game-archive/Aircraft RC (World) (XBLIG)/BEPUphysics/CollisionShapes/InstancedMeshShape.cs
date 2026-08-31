using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes;

/// <summary>
///  Local space data associated with an instanced mesh.
///  This contains a hierarchy and all the other heavy data needed
///  by an InstancedMesh.
/// </summary>
public class InstancedMeshShape : CollisionShape
{
	private TriangleMesh triangleMesh;

	/// <summary>
	///  Gets or sets the TriangleMesh data structure used by this shape.
	/// </summary>
	public TriangleMesh TriangleMesh
	{
		get
		{
			return triangleMesh;
		}
		set
		{
			triangleMesh = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Constructs a new instanced mesh shape.
	/// </summary>
	/// <param name="vertices">Vertices of the mesh.</param>
	/// <param name="indices">Indices of the mesh.</param>
	public InstancedMeshShape(Vector3[] vertices, int[] indices)
	{
		TriangleMesh = new TriangleMesh(new StaticMeshData(vertices, indices));
	}

	/// <summary>
	///  Computes the bounding box of the transformed mesh shape.
	/// </summary>
	/// <param name="transform">Transform to apply to the shape during the bounding box calculation.</param>
	/// <param name="boundingBox">Bounding box containing the transformed mesh shape.</param>
	public void ComputeBoundingBox(ref AffineTransform transform, out BoundingBox boundingBox)
	{
		boundingBox = default(BoundingBox);
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		float num3 = float.MaxValue;
		float num4 = float.MinValue;
		float num5 = float.MinValue;
		float num6 = float.MinValue;
		for (int i = 0; i < triangleMesh.Data.vertices.Length; i++)
		{
			triangleMesh.Data.GetVertexPosition(i, out var vertex);
			Matrix3X3.Transform(ref vertex, ref transform.LinearTransform, out vertex);
			if (vertex.X < num)
			{
				num = vertex.X;
			}
			if (vertex.X > num4)
			{
				num4 = vertex.X;
			}
			if (vertex.Y < num2)
			{
				num2 = vertex.Y;
			}
			if (vertex.Y > num5)
			{
				num5 = vertex.Y;
			}
			if (vertex.Z < num3)
			{
				num3 = vertex.Z;
			}
			if (vertex.Z > num6)
			{
				num6 = vertex.Z;
			}
		}
		boundingBox.Min.X = transform.Translation.X + num;
		boundingBox.Min.Y = transform.Translation.Y + num2;
		boundingBox.Min.Z = transform.Translation.Z + num3;
		boundingBox.Max.X = transform.Translation.X + num4;
		boundingBox.Max.Y = transform.Translation.Y + num5;
		boundingBox.Max.Z = transform.Translation.Z + num6;
	}
}
